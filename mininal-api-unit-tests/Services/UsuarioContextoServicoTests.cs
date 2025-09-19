using mininal_api.Dominio.Servicos;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace mininal_api_unit_tests.Services;

public class UsuarioContextoServicoTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<HttpContext> _httpContextMock;
    private readonly UsuarioContextoServico _usuarioContextoServico;

    public UsuarioContextoServicoTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextMock = new Mock<HttpContext>();
        _usuarioContextoServico = new UsuarioContextoServico(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void ObterUsuarioId_UsuarioAutenticado_DeveRetornarId()
    {
        // Arrange
        var userId = 123;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, "teste@teste.com"),
            new Claim(ClaimTypes.Role, "Adm")
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioId();

        // Assert
        resultado.Should().Be(userId);
    }

    [Fact]
    public void ObterUsuarioId_UsuarioNaoAutenticado_DeveRetornarNull()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioId();

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void ObterUsuarioId_HttpContextNull_DeveRetornarNull()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioId();

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void ObterUsuarioId_IdInvalido_DeveRetornarNull()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "id_invalido"),
            new Claim(ClaimTypes.Email, "teste@teste.com"),
            new Claim(ClaimTypes.Role, "Adm")
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioId();

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void ObterUsuarioEmail_UsuarioAutenticado_DeveRetornarEmail()
    {
        // Arrange
        var email = "teste@teste.com";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "Adm")
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioEmail();

        // Assert
        resultado.Should().Be(email);
    }

    [Fact]
    public void ObterUsuarioEmail_UsuarioNaoAutenticado_DeveRetornarNull()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioEmail();

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void ObterUsuarioPerfil_UsuarioAutenticado_DeveRetornarPerfil()
    {
        // Arrange
        var perfil = "Adm";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "teste@teste.com"),
            new Claim(ClaimTypes.Role, perfil)
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioPerfil();

        // Assert
        resultado.Should().Be(perfil);
    }

    [Fact]
    public void ObterUsuarioPerfil_UsuarioNaoAutenticado_DeveRetornarNull()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioPerfil();

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void EstaAutenticado_UsuarioAutenticado_DeveRetornarTrue()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "teste@teste.com"),
            new Claim(ClaimTypes.Role, "Adm")
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextMock.Setup(x => x.User.Identity).Returns(identity);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.EstaAutenticado();

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void EstaAutenticado_UsuarioNaoAutenticado_DeveRetornarFalse()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextMock.Setup(x => x.User.Identity).Returns(identity);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.EstaAutenticado();

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void EstaAutenticado_HttpContextNull_DeveRetornarFalse()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var resultado = _usuarioContextoServico.EstaAutenticado();

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void ObterUsuarioAtual_UsuarioAutenticado_DeveRetornarPrincipal()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "teste@teste.com"),
            new Claim(ClaimTypes.Role, "Adm")
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioAtual();

        // Assert
        resultado.Should().Be(principal);
    }

    [Fact]
    public void ObterUsuarioAtual_HttpContextNull_DeveRetornarNull()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioAtual();

        // Assert
        resultado.Should().BeNull();
    }

    [Theory]
    [InlineData("Adm")]
    [InlineData("Editor")]
    [InlineData("User")]
    public void ObterUsuarioPerfil_DiferentesPerfis_DeveRetornarPerfilCorreto(string perfil)
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "teste@teste.com"),
            new Claim(ClaimTypes.Role, perfil)
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioPerfil();

        // Assert
        resultado.Should().Be(perfil);
    }

    [Theory]
    [InlineData("teste@teste.com")]
    [InlineData("admin@empresa.com.br")]
    [InlineData("user@dominio.co.uk")]
    public void ObterUsuarioEmail_DiferentesEmails_DeveRetornarEmailCorreto(string email)
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "Adm")
        };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _httpContextMock.Setup(x => x.User).Returns(principal);
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);

        // Act
        var resultado = _usuarioContextoServico.ObterUsuarioEmail();

        // Assert
        resultado.Should().Be(email);
    }
}
