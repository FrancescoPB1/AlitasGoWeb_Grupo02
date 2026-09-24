namespace AlitasGoWeb.Data.Repositories
{
    public class UnidadDeTrabajo : IUnidadDeTrabajo
    {
        private readonly AlitasGoDbContext _ctx;

        public UnidadDeTrabajo(AlitasGoDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<int> GuardarCambiosAsync() => _ctx.SaveChangesAsync();

        public async Task EjecutarEnTransaccionAsync(Func<Task> operacion)
        {
            await using var transaccion = await _ctx.Database.BeginTransactionAsync();
            try
            {
                await operacion();
                await _ctx.SaveChangesAsync();
                await transaccion.CommitAsync();
            }
            catch
            {
                await transaccion.RollbackAsync();
                _ctx.ChangeTracker.Clear();
                throw;
            }
        }
    }
}
