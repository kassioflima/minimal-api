using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.ModelViews;

namespace mininal_api.Dominio.Interfaces;

public interface IVeiculoServico
{
    PaginacaoModelView<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);
    Veiculo? BuscaPorId(int id);
    void Incluir(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    void Apagar(Veiculo veiculo);
}