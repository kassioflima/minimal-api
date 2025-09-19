using mininal_api.Dominio.Interfaces;
using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.DTOs;
using mininal_api.Infraestrutura.Db;

namespace mininal_api.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    private readonly IHashServico _hashServico;

    public AdministradorServico(DbContexto contexto, IHashServico hashServico)
    {
        _contexto = contexto;
        _hashServico = hashServico;
    }

    public Administrador? BuscaPorId(int id)
    {
        return _contexto.Administradores.FirstOrDefault(v => v.Id == id);
    }

    public Administrador Incluir(Administrador administrador)
    {
        // Hash da senha antes de salvar
        administrador.Senha = _hashServico.HashSenha(administrador.Senha);

        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.FirstOrDefault(a => a.Email == loginDTO.Email);
        if (adm != null && _hashServico.VerificarSenha(loginDTO.Senha, adm.Senha))
        {
            return adm;
        }
        return null;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }

    public void Excluir(int id, int usuarioId)
    {
        var administrador = BuscaPorId(id);
        if (administrador != null)
        {
            administrador.Ativo = false;
            administrador.DataAtualizacao = DateTime.UtcNow;
            administrador.AtualizadoPor = usuarioId.ToString();
            _contexto.SaveChanges();
        }
    }

    public void Atualizar(Administrador administrador)
    {
        var existente = BuscaPorId(administrador.Id);
        if (existente != null)
        {
            existente.Email = administrador.Email;
            existente.Perfil = administrador.Perfil;

            // Só atualiza a senha se foi fornecida uma nova
            if (!string.IsNullOrEmpty(administrador.Senha))
            {
                existente.Senha = _hashServico.HashSenha(administrador.Senha);
            }

            existente.DataAtualizacao = DateTime.UtcNow;

            _contexto.SaveChanges();
        }
    }
}