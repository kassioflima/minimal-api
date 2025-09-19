namespace mininal_api.Dominio.ModelViews;

public class UsuarioLogadoModelView
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public bool EstaAutenticado { get; set; }
}
