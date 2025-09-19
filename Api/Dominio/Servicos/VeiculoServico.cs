using Microsoft.EntityFrameworkCore;
using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.Interfaces;
using mininal_api.Dominio.ModelViews;
using mininal_api.Infraestrutura.Db;

namespace mininal_api.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;
    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public void Apagar(Veiculo veiculo)
    {
        // Soft delete
        veiculo.Ativo = false;
        veiculo.DataAtualizacao = DateTime.UtcNow;
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        veiculo.DataAtualizacao = DateTime.UtcNow;
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _contexto.Veiculos.FirstOrDefault(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public PaginacaoModelView<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.Veiculos.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome.ToLower()}%"));
        }

        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(v => EF.Functions.Like(v.Marca.ToLower(), $"%{marca.ToLower()}%"));
        }

        int itensPorPagina = 10;
        int paginaAtual = pagina ?? 1;
        int totalItens = query.Count();
        int totalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina);

        var itens = query
            .Skip((paginaAtual - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToList();

        return new PaginacaoModelView<Veiculo>
        {
            Itens = itens,
            PaginaAtual = paginaAtual,
            ItensPorPagina = itensPorPagina,
            TotalItens = totalItens,
            TotalPaginas = totalPaginas
        };
    }
}