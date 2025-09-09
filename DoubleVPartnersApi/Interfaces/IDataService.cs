using LibraryDBApi.Models;

namespace DoubleVPartnersApi.Interfaces
{
	public interface IDataService
	{
		Task<StoredProcedureResult<IEnumerable<TResult>>> EjecutarProcedimientoAsync<TResult>(string procedureName) where TResult : new();
		Task<StoredProcedureResult<IEnumerable<TResult>>> EjecutarProcedimientoAsync<TResult>(string procedureName, object parametros) where TResult : new();
	}
}
