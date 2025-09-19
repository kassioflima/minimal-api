using mininal_api.Dominio.Entidades;

namespace mininal_api_unit_tests.Entities;

public class VeiculoTests
{
    [Fact]
    public void Construtor_DeveInicializarPropriedadesCorretamente()
    {
        // Act
        var veiculo = new Veiculo();

        // Assert
        veiculo.Id.Should().Be(0);
        veiculo.Nome.Should().BeNullOrEmpty();
        veiculo.Marca.Should().BeNullOrEmpty();
        veiculo.Ano.Should().Be(0);
        veiculo.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        veiculo.DataAtualizacao.Should().BeNull();
        veiculo.CriadoPor.Should().BeNull();
        veiculo.AtualizadoPor.Should().BeNull();
        veiculo.Ativo.Should().BeTrue();
    }

    [Fact]
    public void SetPropriedades_DeveDefinirValoresCorretamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var dataTeste = DateTime.UtcNow.AddDays(-1);

        // Act
        veiculo.Id = 1;
        veiculo.Nome = "Civic";
        veiculo.Marca = "Honda";
        veiculo.Ano = 2023;
        veiculo.DataCriacao = dataTeste;
        veiculo.DataAtualizacao = dataTeste;
        veiculo.CriadoPor = "sistema";
        veiculo.AtualizadoPor = "admin";
        veiculo.Ativo = false;

        // Assert
        veiculo.Id.Should().Be(1);
        veiculo.Nome.Should().Be("Civic");
        veiculo.Marca.Should().Be("Honda");
        veiculo.Ano.Should().Be(2023);
        veiculo.DataCriacao.Should().Be(dataTeste);
        veiculo.DataAtualizacao.Should().Be(dataTeste);
        veiculo.CriadoPor.Should().Be("sistema");
        veiculo.AtualizadoPor.Should().Be("admin");
        veiculo.Ativo.Should().BeFalse();
    }

    [Theory]
    [InlineData("Civic", "Honda", 2023)]
    [InlineData("Corolla", "Toyota", 2022)]
    [InlineData("Gol", "Volkswagen", 2021)]
    [InlineData("HB20", "Hyundai", 2024)]
    public void Propriedades_DeveAceitarDiferentesVeiculos(string nome, string marca, int ano)
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act
        veiculo.Nome = nome;
        veiculo.Marca = marca;
        veiculo.Ano = ano;

        // Assert
        veiculo.Nome.Should().Be(nome);
        veiculo.Marca.Should().Be(marca);
        veiculo.Ano.Should().Be(ano);
    }

    [Theory]
    [InlineData(1950)]
    [InlineData(2000)]
    [InlineData(2023)]
    [InlineData(2100)]
    public void Ano_DeveAceitarAnosValidos(int ano)
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act
        veiculo.Ano = ano;

        // Assert
        veiculo.Ano.Should().Be(ano);
    }

    [Theory]
    [InlineData("Civic")]
    [InlineData("Corolla")]
    [InlineData("HB20")]
    [InlineData("Onix")]
    [InlineData("Argo")]
    public void Nome_DeveAceitarNomesValidos(string nome)
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act
        veiculo.Nome = nome;

        // Assert
        veiculo.Nome.Should().Be(nome);
    }

    [Theory]
    [InlineData("Honda")]
    [InlineData("Toyota")]
    [InlineData("Volkswagen")]
    [InlineData("Hyundai")]
    [InlineData("Chevrolet")]
    public void Marca_DeveAceitarMarcasValidas(string marca)
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act
        veiculo.Marca = marca;

        // Assert
        veiculo.Marca.Should().Be(marca);
    }

    [Fact]
    public void Ativo_DevePermitirAlteracao()
    {
        // Arrange
        var veiculo = new Veiculo();

        // Act & Assert - Deve começar ativo
        veiculo.Ativo.Should().BeTrue();

        // Act & Assert - Deve permitir desativar
        veiculo.Ativo = false;
        veiculo.Ativo.Should().BeFalse();

        // Act & Assert - Deve permitir reativar
        veiculo.Ativo = true;
        veiculo.Ativo.Should().BeTrue();
    }

    [Fact]
    public void DataCriacao_DeveSerDefinidaAutomaticamente()
    {
        // Arrange
        var veiculo = new Veiculo();
        var dataAntes = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var veiculoCriado = new Veiculo();

        // Assert
        veiculoCriado.DataCriacao.Should().BeAfter(dataAntes);
        veiculoCriado.DataCriacao.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }
}
