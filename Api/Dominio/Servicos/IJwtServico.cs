using mininal_api.Dominio.Entidades;

namespace mininal_api.Dominio.Servicos;

public interface IJwtServico
{
    string GerarAccessToken(Administrador administrador);
    string GerarRefreshToken();
    Task<bool> ValidarRefreshToken(string refreshToken);
    string RenovarAccessToken(string refreshToken);
}
