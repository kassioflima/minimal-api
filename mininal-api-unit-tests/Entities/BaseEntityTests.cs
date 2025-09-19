using mininal_api.Dominio.Entidades;

namespace mininal_api_unit_tests.Entities;

public class BaseEntityTests
{
    [Fact]
    public void Construtor_DeveInicializarPropriedadesCorretamente()
    {
        // Act
        var baseEntity = new TestEntity();

        // Assert
        baseEntity.Id.Should().Be(0);
        baseEntity.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        baseEntity.DataAtualizacao.Should().BeNull();
        baseEntity.CriadoPor.Should().BeNull();
        baseEntity.AtualizadoPor.Should().BeNull();
        baseEntity.Ativo.Should().BeTrue();
    }

    [Fact]
    public void SetPropriedades_DeveDefinirValoresCorretamente()
    {
        // Arrange
        var baseEntity = new TestEntity();
        var dataTeste = DateTime.UtcNow.AddDays(-1);

        // Act
        baseEntity.Id = 1;
        baseEntity.DataCriacao = dataTeste;
        baseEntity.DataAtualizacao = dataTeste;
        baseEntity.CriadoPor = "sistema";
        baseEntity.AtualizadoPor = "admin";
        baseEntity.Ativo = false;

        // Assert
        baseEntity.Id.Should().Be(1);
        baseEntity.DataCriacao.Should().Be(dataTeste);
        baseEntity.DataAtualizacao.Should().Be(dataTeste);
        baseEntity.CriadoPor.Should().Be("sistema");
        baseEntity.AtualizadoPor.Should().Be("admin");
        baseEntity.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Id_DevePermitirAlteracao()
    {
        // Arrange
        var baseEntity = new TestEntity();

        // Act & Assert
        baseEntity.Id = 1;
        baseEntity.Id.Should().Be(1);

        baseEntity.Id = 999;
        baseEntity.Id.Should().Be(999);

        baseEntity.Id = 0;
        baseEntity.Id.Should().Be(0);
    }

    [Fact]
    public void DataCriacao_DevePermitirAlteracao()
    {
        // Arrange
        var baseEntity = new TestEntity();
        var dataOriginal = baseEntity.DataCriacao;
        var novaData = DateTime.UtcNow.AddDays(-5);

        // Act
        baseEntity.DataCriacao = novaData;

        // Assert
        baseEntity.DataCriacao.Should().Be(novaData);
        baseEntity.DataCriacao.Should().NotBe(dataOriginal);
    }

    [Fact]
    public void DataAtualizacao_DevePermitirAlteracao()
    {
        // Arrange
        var baseEntity = new TestEntity();
        var dataAtualizacao = DateTime.UtcNow.AddDays(-2);

        // Act & Assert - Deve começar null
        baseEntity.DataAtualizacao.Should().BeNull();

        // Act
        baseEntity.DataAtualizacao = dataAtualizacao;

        // Assert
        baseEntity.DataAtualizacao.Should().Be(dataAtualizacao);

        // Act - Deve permitir voltar para null
        baseEntity.DataAtualizacao = null;

        // Assert
        baseEntity.DataAtualizacao.Should().BeNull();
    }

    [Theory]
    [InlineData("sistema")]
    [InlineData("admin")]
    [InlineData("usuario@teste.com")]
    [InlineData("")]
    [InlineData(null)]
    public void CriadoPor_DeveAceitarDiferentesValores(string? criadoPor)
    {
        // Arrange
        var baseEntity = new TestEntity();

        // Act
        baseEntity.CriadoPor = criadoPor;

        // Assert
        baseEntity.CriadoPor.Should().Be(criadoPor);
    }

    [Theory]
    [InlineData("sistema")]
    [InlineData("admin")]
    [InlineData("usuario@teste.com")]
    [InlineData("")]
    [InlineData(null)]
    public void AtualizadoPor_DeveAceitarDiferentesValores(string? atualizadoPor)
    {
        // Arrange
        var baseEntity = new TestEntity();

        // Act
        baseEntity.AtualizadoPor = atualizadoPor;

        // Assert
        baseEntity.AtualizadoPor.Should().Be(atualizadoPor);
    }

    [Fact]
    public void Ativo_DevePermitirAlteracao()
    {
        // Arrange
        var baseEntity = new TestEntity();

        // Act & Assert - Deve começar ativo
        baseEntity.Ativo.Should().BeTrue();

        // Act & Assert - Deve permitir desativar
        baseEntity.Ativo = false;
        baseEntity.Ativo.Should().BeFalse();

        // Act & Assert - Deve permitir reativar
        baseEntity.Ativo = true;
        baseEntity.Ativo.Should().BeTrue();
    }

    [Fact]
    public void DataCriacao_DeveSerDefinidaAutomaticamente()
    {
        // Arrange
        var dataAntes = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var baseEntity = new TestEntity();

        // Assert
        baseEntity.DataCriacao.Should().BeAfter(dataAntes);
        baseEntity.DataCriacao.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }
}

// Classe de teste para testar BaseEntity
public class TestEntity : BaseEntity
{
    // Propriedades adicionais para teste se necessário
}
