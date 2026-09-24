using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly AlitasGoDbContext _ctx;

        public AuditoriaRepository(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AgregarAsync(Auditoria registro) => await _ctx.Auditorias.AddAsync(registro);

        public Task<List<Auditoria>> ListarAsync(DateTime desde, DateTime hastaExclusivo, int maximo) =>
            _ctx.Auditorias.AsNoTracking()
                .Where(a => a.Fecha >= desde && a.Fecha < hastaExclusivo)
                .OrderByDescending(a => a.Fecha)
                .Take(maximo)
                .ToListAsync();
    }
}
