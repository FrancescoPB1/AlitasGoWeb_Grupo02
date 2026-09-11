using AlitasGoWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AlitasGoWeb.Data.Repositories
{
    public class TipoProductoRepository
    {
        private readonly AlitasGoDbContext _context;
        public TipoProductoRepository(AlitasGoDbContext context) => _context = context;

        
    }
}
