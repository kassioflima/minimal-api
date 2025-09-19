using System.Security.Claims;

namespace mininal_api.Dominio.Servicos;

public interface IUsuarioContextoServico
{
    int? ObterUsuarioId();
    string? ObterUsuarioEmail();
    string? ObterUsuarioPerfil();
    bool EstaAutenticado();
    ClaimsPrincipal? ObterUsuarioAtual();
}
