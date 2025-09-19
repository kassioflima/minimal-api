using System.Security.Claims;

namespace mininal_api.Dominio.Servicos;

public class UsuarioContextoServico : IUsuarioContextoServico
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioContextoServico(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? ObterUsuarioId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    public string? ObterUsuarioEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public string? ObterUsuarioPerfil()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }

    public bool EstaAutenticado()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public ClaimsPrincipal? ObterUsuarioAtual()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}
