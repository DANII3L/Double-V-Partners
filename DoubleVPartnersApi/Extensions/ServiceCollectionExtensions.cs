using DoubleVPartnersApi.Services;
using DoubleVPartnersApi.Interfaces;

namespace DoubleVPartnersApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {   
            // Registrar HttpContextAccessor para acceder al contexto HTTP
            services.AddHttpContextAccessor();

            // Registrar el servicio de manejo de peticiones
            services.AddScoped<IRequestService, RequestService>();

            services.AddTransient<ICliente, ClienteService>();
            services.AddTransient<IProducto, ProductoService>();
            services.AddTransient<IFactura, FacturaService>();
            services.AddScoped<IDataService, DataService>();

            return services;
        }
    }
}
