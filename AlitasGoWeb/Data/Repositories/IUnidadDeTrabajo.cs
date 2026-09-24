namespace AlitasGoWeb.Data.Repositories
{
    // Unit of Work: todos los repositorios comparten el mismo AlitasGoDbContext (AddScoped).
    public interface IUnidadDeTrabajo
    {
        Task<int> GuardarCambiosAsync();

        // Maestro-detalle, stock y bitácora se confirman juntos o no se confirma nada.
        Task EjecutarEnTransaccionAsync(Func<Task> operacion);
    }
}
