using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.Enuns;

namespace mininal_api_unit_tests.Entities;

public class AdministradorTests
{
    [Fact]
    public void Construtor_DeveInicializarPropriedadesCorretamente()
    {
        // Act
        var administrador = new Administrador();

        // Assert
        administrador.Id.Should().Be(0);
        administrador.Email.Should().BeNullOrEmpty();
        administrador.Senha.Should().BeNullOrEmpty();
        administrador.Perfil.Should().BeNullOrEmpty();
        administrador.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        administrador.DataAtualizacao.Should().BeNull();
        administrador.CriadoPor.Should().BeNull();
        administrador.AtualizadoPor.Should().BeNull();
        administrador.Ativo.Should().BeTrue();
    }

    [Fact]
    public void SetPropriedades_DeveDefinirValoresCorretamente()
    {
        // Arrange
        var administrador = new Administrador();
        var dataTeste = DateTime.UtcNow.AddDays(-1);

        // Act
        administrador.Id = 1;
        administrador.Email = "teste@teste.com";
        administrador.Senha = "senha123";
        administrador.Perfil = Perfil.Adm.ToString();
        administrador.DataCriacao = dataTeste;
        administrador.DataAtualizacao = dataTeste;
        administrador.CriadoPor = "sistema";
        administrador.AtualizadoPor = "admin";
        administrador.Ativo = false;

        // Assert
        administrador.Id.Should().Be(1);
        administrador.Email.Should().Be("teste@teste.com");
        administrador.Senha.Should().Be("senha123");
        administrador.Perfil.Should().Be("Adm");
        administrador.DataCriacao.Should().Be(dataTeste);
        administrador.DataAtualizacao.Should().Be(dataTeste);
        administrador.CriadoPor.Should().Be("sistema");
        administrador.AtualizadoPor.Should().Be("admin");
        administrador.Ativo.Should().BeFalse();
    }

    [Theory]
    [InlineData("adm@teste.com", "Adm")]
    [InlineData("editor@teste.com", "Editor")]
    [InlineData("user@teste.com", "User")]
    public void Propriedades_DeveAceitarDiferentesValores(string email, string perfil)
    {
        // Arrange
        var administrador = new Administrador();

        // Act
        administrador.Email = email;
        administrador.Perfil = perfil;

        // Assert
        administrador.Email.Should().Be(email);
        administrador.Perfil.Should().Be(perfil);
    }

    [Theory]
    [InlineData("teste@exemplo.com")]
    [InlineData("usuario123@dominio.com.br")]
    [InlineData("admin@empresa.co.uk")]
    [InlineData("test.email@subdominio.exemplo.com")]
    public void Email_DeveAceitarEmailsValidos(string email)
    {
        // Arrange
        var administrador = new Administrador();

        // Act
        administrador.Email = email;

        // Assert
        administrador.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("Adm")]
    [InlineData("Editor")]
    [InlineData("User")]
    public void Perfil_DeveAceitarPerfisValidos(string perfil)
    {
        // Arrange
        var administrador = new Administrador();

        // Act
        administrador.Perfil = perfil;

        // Assert
        administrador.Perfil.Should().Be(perfil);
    }

    [Fact]
    public void Senha_DeveAceitarSenhasDiferentes()
    {
        // Arrange
        var administrador = new Administrador();
        var senhas = new[] { "123456", "senha123", "MinhaSenh@123", "a1b2c3d4" };

        foreach (var senha in senhas)
        {
            // Act
            administrador.Senha = senha;

            // Assert
            administrador.Senha.Should().Be(senha);
        }
    }

    [Fact]
    public void Ativo_DevePermitirAlteracao()
    {
        // Arrange
        var administrador = new Administrador();

        // Act & Assert - Deve começar ativo
        administrador.Ativo.Should().BeTrue();

        // Act & Assert - Deve permitir desativar
        administrador.Ativo = false;
        administrador.Ativo.Should().BeFalse();

        // Act & Assert - Deve permitir reativar
        administrador.Ativo = true;
        administrador.Ativo.Should().BeTrue();
    }
}
