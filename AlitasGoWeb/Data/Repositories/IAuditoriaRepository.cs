using AlitasGoWeb.Models;

namespace AlitasGoWeb.Data.Repositories
{
    public interface IAuditoriaRepository
    {
        Task AgregarAsync(Auditoria registro);
        Task<List<Auditoria>> ListarAsync(DateTime desde, DateTime hastaExclusivo, int maximo);
    }
}
