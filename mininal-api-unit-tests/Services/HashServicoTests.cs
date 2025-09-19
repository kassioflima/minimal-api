using mininal_api.Dominio.Servicos;

namespace mininal_api_unit_tests.Services;

public class HashServicoTests
{
    private readonly HashServico _hashServico;

    public HashServicoTests()
    {
        _hashServico = new HashServico();
    }

    [Fact]
    public void HashSenha_DeveRetornarHashDiferenteDaSenhaOriginal()
    {
        // Arrange
        var senha = "123456";

        // Act
        var hash = _hashServico.HashSenha(senha);

        // Assert
        hash.Should().NotBe(senha);
        hash.Should().NotBeNullOrEmpty();
        hash.Length.Should().BeGreaterThan(senha.Length);
    }

    [Fact]
    public void HashSenha_SenhasIguais_DeveRetornarHashesDiferentes()
    {
        // Arrange
        var senha = "123456";

        // Act
        var hash1 = _hashServico.HashSenha(senha);
        var hash2 = _hashServico.HashSenha(senha);

        // Assert
        hash1.Should().NotBe(hash2); // BCrypt gera salt diferente a cada hash
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("senha123")]
    [InlineData("MinhaSenh@123")]
    [InlineData("a1b2c3d4")]
    [InlineData("a")]
    [InlineData("senha muito longa com muitos caracteres especiais !@#$%^&*()")]
    public void HashSenha_DeveAceitarDiferentesSenhas(string senha)
    {
        // Act
        var hash = _hashServico.HashSenha(senha);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(senha);
    }

    [Fact]
    public void HashSenha_SenhaVazia_DeveLancarExcecao()
    {
        // Act & Assert
        var action = () => _hashServico.HashSenha("");
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerificarSenha_SenhaCorreta_DeveRetornarTrue()
    {
        // Arrange
        var senha = "123456";
        var hash = _hashServico.HashSenha(senha);

        // Act
        var resultado = _hashServico.VerificarSenha(senha, hash);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void VerificarSenha_SenhaIncorreta_DeveRetornarFalse()
    {
        // Arrange
        var senhaCorreta = "123456";
        var senhaIncorreta = "654321";
        var hash = _hashServico.HashSenha(senhaCorreta);

        // Act
        var resultado = _hashServico.VerificarSenha(senhaIncorreta, hash);

        // Assert
        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData("123456", "123456", true)]
    [InlineData("senha123", "senha123", true)]
    [InlineData("MinhaSenh@123", "MinhaSenh@123", true)]
    [InlineData("123456", "654321", false)]
    [InlineData("senha123", "123senha", false)]
    [InlineData("MinhaSenh@123", "MinhaSenha123", false)]
    public void VerificarSenha_DiferentesSenhas_DeveRetornarResultadoCorreto(string senhaOriginal, string senhaVerificar, bool esperado)
    {
        // Arrange
        var hash = _hashServico.HashSenha(senhaOriginal);

        // Act
        var resultado = _hashServico.VerificarSenha(senhaVerificar, hash);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void VerificarSenha_HashInvalido_DeveRetornarFalse()
    {
        // Arrange
        var senha = "123456";
        var hashInvalido = "hash_invalido";

        // Act
        var resultado = _hashServico.VerificarSenha(senha, hashInvalido);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void VerificarSenha_HashVazio_DeveRetornarFalse()
    {
        // Arrange
        var senha = "123456";
        var hashVazio = "";

        // Act
        var resultado = _hashServico.VerificarSenha(senha, hashVazio);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void VerificarSenha_SenhaNula_DeveRetornarFalse()
    {
        // Arrange
        var hash = _hashServico.HashSenha("123456");

        // Act
        var resultado = _hashServico.VerificarSenha(null!, hash);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void HashSenha_SenhaNula_DeveLancarExcecao()
    {
        // Act & Assert
        var action = () => _hashServico.HashSenha(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerificarSenha_HashNulo_DeveRetornarFalse()
    {
        // Arrange
        var senha = "123456";

        // Act
        var resultado = _hashServico.VerificarSenha(senha, null!);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void HashSenha_DeveGerarHashConsistente()
    {
        // Arrange
        var senha = "teste123";
        var hashes = new List<string>();

        // Act - Gerar múltiplos hashes da mesma senha
        for (int i = 0; i < 10; i++)
        {
            hashes.Add(_hashServico.HashSenha(senha));
        }

        // Assert - Todos os hashes devem ser diferentes (devido ao salt)
        hashes.Should().OnlyHaveUniqueItems();
        
        // Mas todos devem verificar corretamente
        foreach (var hash in hashes)
        {
            _hashServico.VerificarSenha(senha, hash).Should().BeTrue();
        }
    }

    [Fact]
    public void HashSenha_DeveSerSeguro()
    {
        // Arrange
        var senhasComuns = new[] { "123456", "password", "admin", "12345", "qwerty" };

        // Act & Assert
        foreach (var senha in senhasComuns)
        {
            var hash = _hashServico.HashSenha(senha);
            
            // Hash deve ter tamanho adequado para BCrypt (60 caracteres)
            hash.Length.Should().Be(60);
            
            // Hash deve começar com $2a$ (versão do BCrypt)
            hash.Should().StartWith("$2a$");
            
            // Deve ser verificável
            _hashServico.VerificarSenha(senha, hash).Should().BeTrue();
        }
    }
}
