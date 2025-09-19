using mininal_api.Dominio.Enuns;

namespace mininal_api.Dominio.DTOs;
public class AdministradorDTO
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public Perfil? Perfil { get; set; } = default!;
}