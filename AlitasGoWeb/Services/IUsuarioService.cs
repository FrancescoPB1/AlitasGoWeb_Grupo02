namespace AlitasGoWeb.Services
{
    public record UsuarioResumen(string Id, string Email, string Rol, bool DosFactores, bool Bloqueado);

    public interface IUsuarioService
    {
        Task<List<UsuarioResumen>> ListarAsync();
        Task<Resultado> CrearAsync(string email, string password, string rol, string usuarioActual);
        Task<Resultado> CambiarRolAsync(string idUsuario, string rol, string usuarioActual);
    }
}
