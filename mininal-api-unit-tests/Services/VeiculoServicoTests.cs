using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.Servicos;
using mininal_api.Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;

namespace mininal_api_unit_tests.Services;

public class VeiculoServicoTests
{
    private readonly DbContexto _contexto;
    private readonly VeiculoServico _veiculoServico;

    public VeiculoServicoTests()
    {
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _contexto = new DbContexto(options);
        _veiculoServico = new VeiculoServico(_contexto);
    }

    [Fact]
    public void Incluir_DeveSalvarVeiculo()
    {
        // Arrange
        var veiculo = CriarVeiculoTeste();

        // Act
        _veiculoServico.Incluir(veiculo);

        // Assert
        var veiculoSalvo = _contexto.Veiculos.First();
        veiculoSalvo.Id.Should().BeGreaterThan(0);
        veiculoSalvo.Nome.Should().Be(veiculo.Nome);
        veiculoSalvo.Marca.Should().Be(veiculo.Marca);
        veiculoSalvo.Ano.Should().Be(veiculo.Ano);
    }

    [Fact]
    public void BuscaPorId_VeiculoExistente_DeveRetornarVeiculo()
    {
        // Arrange
        var veiculo = CriarVeiculoTeste();
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(veiculo.Id);
        resultado.Nome.Should().Be(veiculo.Nome);
        resultado.Marca.Should().Be(veiculo.Marca);
        resultado.Ano.Should().Be(veiculo.Ano);
    }

    [Fact]
    public void BuscaPorId_VeiculoInexistente_DeveRetornarNull()
    {
        // Act
        var resultado = _veiculoServico.BuscaPorId(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void Atualizar_VeiculoExistente_DeveAtualizarPropriedades()
    {
        // Arrange
        var veiculo = CriarVeiculoTeste();
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();

        veiculo.Nome = "Civic Atualizado";
        veiculo.Marca = "Honda Atualizada";
        veiculo.Ano = 2024;

        // Act
        _veiculoServico.Atualizar(veiculo);

        // Assert
        var resultado = _contexto.Veiculos.First();
        resultado.Nome.Should().Be("Civic Atualizado");
        resultado.Marca.Should().Be("Honda Atualizada");
        resultado.Ano.Should().Be(2024);
        resultado.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Apagar_VeiculoExistente_DeveMarcarComoInativo()
    {
        // Arrange
        var veiculo = CriarVeiculoTeste();
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();

        // Recarregar o veículo do contexto para garantir que está sendo rastreado
        var veiculoDoContexto = _contexto.Veiculos.First(v => v.Id == veiculo.Id);

        // Act
        _veiculoServico.Apagar(veiculoDoContexto);

        // Assert
        var veiculoApagado = _contexto.Veiculos.IgnoreQueryFilters().FirstOrDefault(v => v.Id == veiculo.Id);
        veiculoApagado.Should().NotBeNull();
        veiculoApagado!.Ativo.Should().BeFalse();
        veiculoApagado.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Todos_SemFiltros_DeveRetornarTodosVeiculos()
    {
        // Arrange
        var veiculos = CriarListaVeiculos();
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos();

        // Assert
        resultado.Itens.Should().HaveCount(3);
        resultado.PaginaAtual.Should().Be(1);
        resultado.ItensPorPagina.Should().Be(10);
        resultado.TotalItens.Should().Be(3);
        resultado.TotalPaginas.Should().Be(1);
    }

    [Fact]
    public void Todos_ComFiltroNome_DeveRetornarVeiculosFiltrados()
    {
        // Arrange
        var veiculos = CriarListaVeiculos();
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(nome: "Civic");

        // Assert
        resultado.Itens.Should().HaveCount(1);
        resultado.Itens.First().Nome.Should().Be("Civic");
    }

    [Fact]
    public void Todos_ComFiltroMarca_DeveRetornarVeiculosFiltrados()
    {
        // Arrange
        var veiculos = CriarListaVeiculos();
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(marca: "Honda");

        // Assert
        resultado.Itens.Should().HaveCount(1);
        resultado.Itens.First().Marca.Should().Be("Honda");
    }

    [Fact]
    public void Todos_ComFiltrosNomeEMarca_DeveRetornarVeiculosFiltrados()
    {
        // Arrange
        var veiculos = CriarListaVeiculos();
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(nome: "Civic", marca: "Honda");

        // Assert
        resultado.Itens.Should().HaveCount(1);
        resultado.Itens.First().Nome.Should().Be("Civic");
        resultado.Itens.First().Marca.Should().Be("Honda");
    }

    [Fact]
    public void Todos_ComFiltrosInexistentes_DeveRetornarListaVazia()
    {
        // Arrange
        var veiculos = CriarListaVeiculos();
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(nome: "Inexistente");

        // Assert
        resultado.Itens.Should().BeEmpty();
        resultado.TotalItens.Should().Be(0);
    }

    [Fact]
    public void Todos_ComPagina_DeveRetornarPaginaCorreta()
    {
        // Arrange
        var veiculos = CriarListaVeiculos();
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(pagina: 1);

        // Assert
        resultado.PaginaAtual.Should().Be(1);
        resultado.Itens.Should().HaveCount(3);
    }

    [Fact]
    public void Todos_ComFiltroCaseInsensitive_DeveFuncionar()
    {
        // Arrange
        var veiculos = CriarListaVeiculos();
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(nome: "civic"); // lowercase

        // Assert
        resultado.Itens.Should().HaveCount(1);
        resultado.Itens.First().Nome.Should().Be("Civic");
    }

    [Fact]
    public void Todos_ComFiltroParcial_DeveFuncionar()
    {
        // Arrange
        var veiculos = new List<Veiculo>
        {
            new Veiculo { Nome = "Honda Civic", Marca = "Honda", Ano = 2023 },
            new Veiculo { Nome = "Toyota Corolla", Marca = "Toyota", Ano = 2022 },
            new Veiculo { Nome = "Honda Fit", Marca = "Honda", Ano = 2021 }
        };
        
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(nome: "Honda");

        // Assert
        resultado.Itens.Should().HaveCount(2);
        resultado.Itens.Should().OnlyContain(v => v.Nome.Contains("Honda"));
    }

    [Fact]
    public void Todos_ComFiltroMarcaParcial_DeveFuncionar()
    {
        // Arrange
        var veiculos = new List<Veiculo>
        {
            new Veiculo { Nome = "Civic", Marca = "Honda Motors", Ano = 2023 },
            new Veiculo { Nome = "Corolla", Marca = "Toyota", Ano = 2022 },
            new Veiculo { Nome = "Fit", Marca = "Honda Motors", Ano = 2021 }
        };
        
        _contexto.Veiculos.AddRange(veiculos);
        _contexto.SaveChanges();

        // Act
        var resultado = _veiculoServico.Todos(marca: "Honda");

        // Assert
        resultado.Itens.Should().HaveCount(2);
        resultado.Itens.Should().OnlyContain(v => v.Marca.Contains("Honda"));
    }

    private Veiculo CriarVeiculoTeste()
    {
        return new Veiculo
        {
            Nome = "Civic",
            Marca = "Honda",
            Ano = 2023
        };
    }

    private List<Veiculo> CriarListaVeiculos()
    {
        return new List<Veiculo>
        {
            new Veiculo { Nome = "Civic", Marca = "Honda", Ano = 2023 },
            new Veiculo { Nome = "Corolla", Marca = "Toyota", Ano = 2022 },
            new Veiculo { Nome = "Gol", Marca = "Volkswagen", Ano = 2021 }
        };
    }
}
