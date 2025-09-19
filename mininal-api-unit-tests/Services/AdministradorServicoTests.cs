using mininal_api.Dominio.DTOs;
using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.Interfaces;
using mininal_api.Dominio.Servicos;
using mininal_api.Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;

namespace mininal_api_unit_tests.Services;

public class AdministradorServicoTests
{
    private readonly Mock<IHashServico> _hashServicoMock;
    private readonly DbContexto _contexto;
    private readonly AdministradorServico _administradorServico;

    public AdministradorServicoTests()
    {
        _hashServicoMock = new Mock<IHashServico>();
        
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _contexto = new DbContexto(options);
        _administradorServico = new AdministradorServico(_contexto, _hashServicoMock.Object);
    }

    [Fact]
    public void Incluir_DeveSalvarAdministradorComSenhaHashada()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var senhaHashada = "senha_hashada";
        
        _hashServicoMock.Setup(x => x.HashSenha(administrador.Senha))
                       .Returns(senhaHashada);

        // Act
        var resultado = _administradorServico.Incluir(administrador);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().BeGreaterThan(0);
        resultado.Senha.Should().Be(senhaHashada);
        
        _hashServicoMock.Verify(x => x.HashSenha("123456"), Times.Once);
        
        var administradorSalvo = _contexto.Administradores.First();
        administradorSalvo.Email.Should().Be(administrador.Email);
        administradorSalvo.Perfil.Should().Be(administrador.Perfil);
        administradorSalvo.Senha.Should().Be(senhaHashada);
    }

    [Fact]
    public void BuscaPorId_AdministradorExistente_DeveRetornarAdministrador()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        // Act
        var resultado = _administradorServico.BuscaPorId(administrador.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(administrador.Id);
        resultado.Email.Should().Be(administrador.Email);
        resultado.Perfil.Should().Be(administrador.Perfil);
    }

    [Fact]
    public void BuscaPorId_AdministradorInexistente_DeveRetornarNull()
    {
        // Act
        var resultado = _administradorServico.BuscaPorId(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void Login_CredenciaisCorretas_DeveRetornarAdministrador()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var senhaHashada = "senha_hashada";
        administrador.Senha = senhaHashada;
        
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        var loginDTO = new LoginDTO { Email = "teste@teste.com", Senha = "123456" };
        
        _hashServicoMock.Setup(x => x.VerificarSenha(loginDTO.Senha, senhaHashada))
                       .Returns(true);

        // Act
        var resultado = _administradorServico.Login(loginDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(administrador.Id);
        resultado.Email.Should().Be(administrador.Email);
        
        _hashServicoMock.Verify(x => x.VerificarSenha(loginDTO.Senha, senhaHashada), Times.Once);
    }

    [Fact]
    public void Login_CredenciaisIncorretas_DeveRetornarNull()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var senhaHashada = "senha_hashada";
        administrador.Senha = senhaHashada;
        
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        var loginDTO = new LoginDTO { Email = "teste@teste.com", Senha = "senha_errada" };
        
        _hashServicoMock.Setup(x => x.VerificarSenha(loginDTO.Senha, senhaHashada))
                       .Returns(false);

        // Act
        var resultado = _administradorServico.Login(loginDTO);

        // Assert
        resultado.Should().BeNull();
        
        _hashServicoMock.Verify(x => x.VerificarSenha(loginDTO.Senha, senhaHashada), Times.Once);
    }

    [Fact]
    public void Login_EmailInexistente_DeveRetornarNull()
    {
        // Arrange
        var loginDTO = new LoginDTO { Email = "inexistente@teste.com", Senha = "123456" };

        // Act
        var resultado = _administradorServico.Login(loginDTO);

        // Assert
        resultado.Should().BeNull();
        
        _hashServicoMock.Verify(x => x.VerificarSenha(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Todos_SemPagina_DeveRetornarTodosAdministradores()
    {
        // Arrange
        var administradores = CriarListaAdministradores();
        _contexto.Administradores.AddRange(administradores);
        _contexto.SaveChanges();

        // Act
        var resultado = _administradorServico.Todos(null);

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Should().Contain(a => a.Email == "admin1@teste.com");
        resultado.Should().Contain(a => a.Email == "admin2@teste.com");
        resultado.Should().Contain(a => a.Email == "admin3@teste.com");
    }

    [Fact]
    public void Todos_ComPagina_DeveRetornarPaginaCorreta()
    {
        // Arrange
        var administradores = CriarListaAdministradores();
        _contexto.Administradores.AddRange(administradores);
        _contexto.SaveChanges();

        // Act
        var resultado = _administradorServico.Todos(1);

        // Assert
        resultado.Should().HaveCount(3); // Primeira página com 3 itens
    }

    [Fact]
    public void Excluir_AdministradorExistente_DeveMarcarComoInativo()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        
        var usuarioId = 1;

        // Act
        _administradorServico.Excluir(administrador.Id, usuarioId);

        // Assert
        var administradorExcluido = _contexto.Administradores.IgnoreQueryFilters().FirstOrDefault(a => a.Id == administrador.Id);
        administradorExcluido.Should().NotBeNull();
        administradorExcluido!.Ativo.Should().BeFalse();
        administradorExcluido.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        administradorExcluido.AtualizadoPor.Should().Be("Sistema");
    }

    [Fact]
    public void Excluir_AdministradorInexistente_NaoDeveLancarExcecao()
    {
        // Arrange
        var usuarioId = 1;

        // Act & Assert
        var action = () => _administradorServico.Excluir(999, usuarioId);
        action.Should().NotThrow();
    }

    [Fact]
    public void Atualizar_AdministradorExistente_DeveAtualizarPropriedades()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        var administradorAtualizado = new Administrador
        {
            Id = administrador.Id,
            Email = "novo@teste.com",
            Perfil = "Editor",
            Senha = "nova_senha"
        };

        var senhaHashada = "nova_senha_hashada";
        _hashServicoMock.Setup(x => x.HashSenha("nova_senha"))
                       .Returns(senhaHashada);

        // Act
        _administradorServico.Atualizar(administradorAtualizado);

        // Assert
        var resultado = _contexto.Administradores.First();
        resultado.Email.Should().Be("novo@teste.com");
        resultado.Perfil.Should().Be("Editor");
        resultado.Senha.Should().Be(senhaHashada);
        resultado.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        _hashServicoMock.Verify(x => x.HashSenha("nova_senha"), Times.Once);
    }

    [Fact]
    public void Atualizar_AdministradorExistenteSemNovaSenha_NaoDeveAtualizarSenha()
    {
        // Arrange
        var administrador = CriarAdministradorTeste();
        var senhaOriginal = "senha_original";
        administrador.Senha = senhaOriginal;
        
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        var administradorAtualizado = new Administrador
        {
            Id = administrador.Id,
            Email = "novo@teste.com",
            Perfil = "Editor",
            Senha = "" // Senha vazia
        };

        // Act
        _administradorServico.Atualizar(administradorAtualizado);

        // Assert
        var resultado = _contexto.Administradores.First();
        resultado.Email.Should().Be("novo@teste.com");
        resultado.Perfil.Should().Be("Editor");
        resultado.Senha.Should().Be(senhaOriginal); // Senha não deve ter mudado
        
        _hashServicoMock.Verify(x => x.HashSenha(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Atualizar_AdministradorInexistente_NaoDeveLancarExcecao()
    {
        // Arrange
        var administradorInexistente = new Administrador
        {
            Id = 999,
            Email = "inexistente@teste.com",
            Perfil = "Adm"
        };

        // Act & Assert
        var action = () => _administradorServico.Atualizar(administradorInexistente);
        action.Should().NotThrow();
    }

    private Administrador CriarAdministradorTeste()
    {
        return new Administrador
        {
            Email = "teste@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        };
    }

    private List<Administrador> CriarListaAdministradores()
    {
        return new List<Administrador>
        {
            new Administrador { Email = "admin1@teste.com", Senha = "senha1", Perfil = "Adm" },
            new Administrador { Email = "admin2@teste.com", Senha = "senha2", Perfil = "Editor" },
            new Administrador { Email = "admin3@teste.com", Senha = "senha3", Perfil = "User" }
        };
    }
}
