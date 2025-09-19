using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mininal_api.Dominio.Entidades;
using mininal_api.Infraestrutura.Db;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mininal_api.Dominio.Servicos;

public class JwtServico : IJwtServico
{
    private readonly string _chaveSecreta;
    private readonly DbContexto _contexto;

    public JwtServico(IConfiguration configuration, DbContexto contexto)
    {
        _chaveSecreta = configuration["Jwt:ChaveSecreta"] ?? throw new ArgumentNullException("Jwt:ChaveSecreta");
        _contexto = contexto;
    }

    public string GerarAccessToken(Administrador administrador)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var chave = Encoding.UTF8.GetBytes(_chaveSecreta); // Usar UTF8 ao invés de ASCII

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, administrador.Id.ToString()),
            new Claim(ClaimTypes.Email, administrador.Email),
            new Claim(ClaimTypes.Role, administrador.Perfil),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(50), // Token expira em 15 minutos
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GerarRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> ValidarRefreshToken(string refreshToken)
    {
        var token = await _contexto.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        return token != null && !token.Usado && !token.Invalido && token.DataExpiracao > DateTime.UtcNow;
    }

    public string RenovarAccessToken(string refreshToken)
    {
        var token = _contexto.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
        if (token == null || token.Usado || token.Invalido || token.DataExpiracao <= DateTime.UtcNow)
            throw new InvalidOperationException("Refresh token inválido ou expirado");

        var administrador = _contexto.Administradores.Find(token.AdministradorId);
        if (administrador == null)
            throw new InvalidOperationException("Administrador não encontrado");

        // Marca o refresh token como usado
        token.Usado = true;
        _contexto.SaveChanges();

        return GerarAccessToken(administrador);
    }
}
