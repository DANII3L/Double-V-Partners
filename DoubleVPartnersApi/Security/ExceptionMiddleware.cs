using System.Net;
using System.Text.Json;
using System.Collections.Concurrent;

namespace DoubleVPartnersApi.Security
{
    /// <summary>
    /// Middleware global para el manejo de excepciones en la API.
    /// Captura cualquier excepción no manejada y devuelve una respuesta JSON estándar.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor del middleware de excepciones.
        /// </summary>
        /// <param name="next">El siguiente middleware en la cadena.</param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método principal que intercepta la petición HTTP y maneja excepciones globalmente.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Maneja la excepción y devuelve una respuesta JSON con el código de estado adecuado.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        /// <param name="exception">Excepción capturada.</param>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            // Personaliza el código de estado según el tipo de excepción
            if (exception is KeyNotFoundException)
                code = HttpStatusCode.NotFound;
            else if (exception is ArgumentException || exception is ArgumentNullException)
                code = HttpStatusCode.BadRequest;

            var result = JsonSerializer.Serialize(new
            {
                error = exception.Message,
                status = (int)code
            });

            // Log simple a consola (puedes usar un logger real)
            Console.WriteLine($"[ERROR] {exception.GetType().Name}: {exception.Message}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Middleware para limitar la cantidad de peticiones concurrentes que puede procesar el servidor.
    /// Si se supera el límite, responde con 429 Too Many Requests.
    /// </summary>
    public class ConcurrencyLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private static int _current = 0;
        private readonly int _maxConcurrent = 3; // Cambia el límite aquí

        /// <summary>
        /// Constructor del middleware de concurrencia.
        /// </summary>
        /// <param name="next">El siguiente middleware en la cadena.</param>
        public ConcurrencyLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Intercepta la petición y limita el número de peticiones concurrentes.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            if (Interlocked.Increment(ref _current) > _maxConcurrent)
            {
                Interlocked.Decrement(ref _current);
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too many concurrent requests");
                return;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                Interlocked.Decrement(ref _current);
            }
        }
    }

    /// <summary>
    /// Middleware que implementa el algoritmo Token Bucket para limitar la tasa de peticiones.
    /// Permite ráfagas de peticiones y recarga tokens a un ritmo fijo.
    /// </summary>
    public class TokenBucketRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, (int Tokens, DateTime LastRefill)> _buckets = new();

        private readonly int _capacity = 10;
        private readonly int _refillRate = 1; // tokens por segundo

        /// <summary>
        /// Constructor del middleware Token Bucket.
        /// </summary>
        /// <param name="next">El siguiente middleware en la cadena.</param>
        public TokenBucketRateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Intercepta la petición y aplica el algoritmo Token Bucket.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var key = context.Connection.RemoteIpAddress?.ToString() ?? "global";
            var now = DateTime.UtcNow;

            var (tokens, lastRefill) = _buckets.GetOrAdd(key, (_capacity, now));
            var secondsSinceLast = (now - lastRefill).TotalSeconds;
            var tokensToAdd = (int)(secondsSinceLast * _refillRate);

            tokens = Math.Min(_capacity, tokens + tokensToAdd);
            lastRefill = tokensToAdd > 0 ? now : lastRefill;

            if (tokens > 0)
            {
                tokens--;
                _buckets[key] = (tokens, lastRefill);
                await _next(context);
            }
            else
            {
                _buckets[key] = (tokens, lastRefill);
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded (Token Bucket)");
            }
        }
    }

    /// <summary>
    /// Middleware que implementa el algoritmo Fixed Window para limitar la tasa de peticiones.
    /// Permite un número fijo de peticiones por ventana de tiempo.
    /// </summary>
    public class FixedWindowRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _requests = new();

        private readonly int _limit = 5; // Cambia el límite aquí
        private readonly TimeSpan _window = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Constructor del middleware Fixed Window.
        /// </summary>
        /// <param name="next">El siguiente middleware en la cadena.</param>
        public FixedWindowRateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Intercepta la petición y aplica el algoritmo Fixed Window.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var key = context.Connection.RemoteIpAddress?.ToString() ?? "global";
            var now = DateTime.UtcNow;

            var (count, windowStart) = _requests.GetOrAdd(key, (0, now));

            if (now - windowStart > _window)
            {
                count = 0;
                windowStart = now;
            }

            count++;

            _requests[key] = (count, windowStart);

            if (count > _limit)
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded (Fixed Window)");
                return;
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Middleware que implementa el algoritmo Sliding Window para limitar la tasa de peticiones.
    /// Permite un número fijo de peticiones en una ventana móvil de tiempo.
    /// </summary>
    public class SlidingWindowRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

        private readonly int _limit = 5;
        private readonly TimeSpan _window = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Constructor del middleware Sliding Window.
        /// </summary>
        /// <param name="next">El siguiente middleware en la cadena.</param>
        public SlidingWindowRateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Intercepta la petición y aplica el algoritmo Sliding Window.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var key = context.Connection.RemoteIpAddress?.ToString() ?? "global";
            var now = DateTime.UtcNow;

            var timestamps = _requests.GetOrAdd(key, new List<DateTime>());
            lock (timestamps)
            {
                timestamps.RemoveAll(t => now - t > _window);
                if (timestamps.Count >= _limit)
                {
                    context.Response.StatusCode = 429;
                    context.Response.WriteAsync("Rate limit exceeded (Sliding Window)").Wait();
                    return;
                }
                timestamps.Add(now);
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Middleware para capturar la IP del usuario y guardarla en HttpContext.Items["IpAddress"].
    /// </summary>
    public class IpAddressMiddleware
    {
        private readonly RequestDelegate _next;

        public IpAddressMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }
            context.Items["IpAddress"] = ipAddress;
            await _next(context);
        }
    }
}
