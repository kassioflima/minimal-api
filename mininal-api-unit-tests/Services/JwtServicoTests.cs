using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.Servicos;
using mininal_api.Infraestrutura.Db;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace mininal_api_unit_tests.Services;

public class JwtServicoTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly DbContexto _contexto;
    private readonly JwtServico _jwtServico;
    private const string ChaveSecreta = "minimal-api-alunos-vamos_lá-super-secreta-e-segura-2024";

    public JwtServicoTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x["Jwt:ChaveSecreta"]).Returns(ChaveSecreta);

        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _contexto = new DbContexto(options);
        _jwtServico = new JwtServico(_configurationMock.Object, _contexto);
    }

    [Fact]
    public void GerarAccessToken_DeveRetornarTokenValido()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();

        // Act
        var token = _jwtServico.GerarAccessToken(administrador);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
        token.Split('.').Should().HaveCount(3); // JWT tem 3 partes separadas por ponto
    }

    [Fact]
    public void GerarAccessToken_DeveConterClaimsCorretos()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();

        // Act
        var token = _jwtServico.GerarAccessToken(administrador);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        // Decodificar o token para verificar claims
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == administrador.Id.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == administrador.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == administrador.Perfil);
        jsonToken.Claims.Should().Contain(c => c.Type == "jti");
    }

    [Fact]
    public void GerarAccessToken_DiferentesAdministradores_DeveGerarTokensDiferentes()
    {
        // Arrange
        var admin1 = new Administrador { Id = 1, Email = "admin1@teste.com", Perfil = "Adm" };
        var admin2 = new Administrador { Id = 2, Email = "admin2@teste.com", Perfil = "Editor" };

        // Act
        var token1 = _jwtServico.GerarAccessToken(admin1);
        var token2 = _jwtServico.GerarAccessToken(admin2);

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GerarAccessToken_MesmoAdministrador_DeveGerarTokensDiferentes()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();

        // Act
        var token1 = _jwtServico.GerarAccessToken(administrador);
        var token2 = _jwtServico.GerarAccessToken(administrador);

        // Assert
        token1.Should().NotBe(token2); // JTI diferente a cada geração
    }

    [Fact]
    public void GerarRefreshToken_DeveRetornarTokenValido()
    {
        // Act
        var refreshToken = _jwtServico.GerarRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(50); // Base64 de 64 bytes
    }

    [Fact]
    public void GerarRefreshToken_MultiplasChamadas_DeveRetornarTokensDiferentes()
    {
        // Act
        var token1 = _jwtServico.GerarRefreshToken();
        var token2 = _jwtServico.GerarRefreshToken();
        var token3 = _jwtServico.GerarRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
        token1.Should().NotBe(token3);
        token2.Should().NotBe(token3);
    }

    [Fact]
    public async Task ValidarRefreshToken_TokenValido_DeveRetornarTrue()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var refreshToken = _jwtServico.GerarRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            JwtId = Guid.NewGuid().ToString(),
            DataCriacao = DateTime.UtcNow,
            DataExpiracao = DateTime.UtcNow.AddDays(7),
            AdministradorId = administrador.Id,
            Usado = false,
            Invalido = false
        };

        _contexto.RefreshTokens.Add(refreshTokenEntity);
        await _contexto.SaveChangesAsync();

        // Act
        var resultado = await _jwtServico.ValidarRefreshToken(refreshToken);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarRefreshToken_TokenExpirado_DeveRetornarFalse()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var refreshToken = _jwtServico.GerarRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            JwtId = Guid.NewGuid().ToString(),
            DataCriacao = DateTime.UtcNow.AddDays(-8),
            DataExpiracao = DateTime.UtcNow.AddDays(-1), // Expirado
            AdministradorId = administrador.Id,
            Usado = false,
            Invalido = false
        };

        _contexto.RefreshTokens.Add(refreshTokenEntity);
        await _contexto.SaveChangesAsync();

        // Act
        var resultado = await _jwtServico.ValidarRefreshToken(refreshToken);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ValidarRefreshToken_TokenUsado_DeveRetornarFalse()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var refreshToken = _jwtServico.GerarRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            JwtId = Guid.NewGuid().ToString(),
            DataCriacao = DateTime.UtcNow,
            DataExpiracao = DateTime.UtcNow.AddDays(7),
            AdministradorId = administrador.Id,
            Usado = true, // Já usado
            Invalido = false
        };

        _contexto.RefreshTokens.Add(refreshTokenEntity);
        await _contexto.SaveChangesAsync();

        // Act
        var resultado = await _jwtServico.ValidarRefreshToken(refreshToken);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ValidarRefreshToken_TokenInvalido_DeveRetornarFalse()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var refreshToken = _jwtServico.GerarRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            JwtId = Guid.NewGuid().ToString(),
            DataCriacao = DateTime.UtcNow,
            DataExpiracao = DateTime.UtcNow.AddDays(7),
            AdministradorId = administrador.Id,
            Usado = false,
            Invalido = true // Invalidado
        };

        _contexto.RefreshTokens.Add(refreshTokenEntity);
        await _contexto.SaveChangesAsync();

        // Act
        var resultado = await _jwtServico.ValidarRefreshToken(refreshToken);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ValidarRefreshToken_TokenInexistente_DeveRetornarFalse()
    {
        // Arrange
        var tokenInexistente = "token_inexistente";

        // Act
        var resultado = await _jwtServico.ValidarRefreshToken(tokenInexistente);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void RenovarAccessToken_TokenValido_DeveRetornarNovoToken()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var refreshToken = _jwtServico.GerarRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            JwtId = Guid.NewGuid().ToString(),
            DataCriacao = DateTime.UtcNow,
            DataExpiracao = DateTime.UtcNow.AddDays(7),
            AdministradorId = administrador.Id,
            Usado = false,
            Invalido = false
        };

        _contexto.RefreshTokens.Add(refreshTokenEntity);
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        // Act
        var novoToken = _jwtServico.RenovarAccessToken(refreshToken);

        // Assert
        novoToken.Should().NotBeNullOrEmpty();
        novoToken.Should().Contain(".");
        novoToken.Split('.').Should().HaveCount(3);

        // Verificar se o refresh token foi marcado como usado
        var tokenAtualizado = _contexto.RefreshTokens.First(t => t.Token == refreshToken);
        tokenAtualizado.Usado.Should().BeTrue();
    }

    [Fact]
    public void RenovarAccessToken_TokenInvalido_DeveLancarExcecao()
    {
        // Arrange
        var tokenInvalido = "token_invalido";

        // Act & Assert
        var action = () => _jwtServico.RenovarAccessToken(tokenInvalido);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Refresh token inválido ou expirado");
    }

    [Fact]
    public void RenovarAccessToken_AdministradorInexistente_DeveLancarExcecao()
    {
        // Arrange
        var refreshToken = _jwtServico.GerarRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            JwtId = Guid.NewGuid().ToString(),
            DataCriacao = DateTime.UtcNow,
            DataExpiracao = DateTime.UtcNow.AddDays(7),
            AdministradorId = 999, // ID inexistente
            Usado = false,
            Invalido = false
        };

        _contexto.RefreshTokens.Add(refreshTokenEntity);
        _contexto.SaveChanges();

        // Act & Assert
        var action = () => _jwtServico.RenovarAccessToken(refreshToken);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Administrador não encontrado");
    }

    [Fact]
    public void Construtor_ChaveSecretaNula_DeveLancarExcecao()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["Jwt:ChaveSecreta"]).Returns((string?)null);

        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var contexto = new DbContexto(options);

        // Act & Assert
        var action = () => new JwtServico(configurationMock.Object, contexto);
        action.Should().Throw<ArgumentNullException>();
    }

    private Administrador CriarAdministradorTeste()
    {
        return new Administrador
        {
            Id = 1,
            Email = "teste@teste.com",
            Perfil = "Adm",
            Senha = "senha123"
        };
    }
}
