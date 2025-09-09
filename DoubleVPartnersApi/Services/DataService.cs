using DoubleVPartnersApi.Interfaces;
using LibraryDBApi.Core;
using LibraryDBApi.Models;
using LibStoredProcedureParameters = LibraryDBApi.Models.StoredProcedureParameters;
using LibModelPaginacion = LibraryDBApi.Models.ModelPaginacion;

namespace DoubleVPartnersApi.Services
{
	public class DataService : BaseDataService, DoubleVPartnersApi.Interfaces.IDataService
	{
		private readonly IConfiguration _configuration;
		private readonly IRequestService _requestService;
		private readonly string connectionString;

		public DataService(IConfiguration configuration, IRequestService requestService)
		{
			_configuration = configuration;
			connectionString = _configuration.GetConnectionString("DoubleVPartnersDBConnection")!;
			_requestService = requestService;
		}

		public async Task<StoredProcedureResult<IEnumerable<TResult>>> EjecutarProcedimientoAsync<TResult>(string nombreProcedimiento, object parametros) where TResult : new()
		{
			try
			{
				var modelPaginacion = _requestService.GetPaginationParameters();
				var libParameters = new LibStoredProcedureParameters
				{
					ConnectionString = this.connectionString,
					ProcedureName = nombreProcedimiento,
					Model = parametros,
					ModelPaginacion = modelPaginacion != null ? new LibModelPaginacion
					{
						PageNumber = modelPaginacion.PageNumber,
						PageSize = modelPaginacion.PageSize,
						Filter = modelPaginacion.Filter
					} : null
				};

				return await base.EjecutarProcedimientoAsync<TResult>(libParameters);
			}
			catch (Exception ex)
			{
				return StoredProcedureResult<IEnumerable<TResult>>.Failure(ex);
			}
		}

		public async Task<StoredProcedureResult<IEnumerable<TResult>>> EjecutarProcedimientoAsync<TResult>(string procedureName) where TResult : new()
		{
			try
			{
				var modelPaginacion = _requestService.GetPaginationParameters();
				var libModelPaginacion = modelPaginacion != null ? new LibModelPaginacion
				{
					PageNumber = modelPaginacion.PageNumber,
					PageSize = modelPaginacion.PageSize,
					Filter = modelPaginacion.Filter
				} : null;

				return await base.EjecutarProcedimientoAsync<TResult>(connectionString, procedureName, libModelPaginacion);
			}
			catch (Exception ex)
			{
				return StoredProcedureResult<IEnumerable<TResult>>.Failure(ex);
			}
		}
	}
}