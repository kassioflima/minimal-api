namespace mininal_api.Dominio.ModelViews;

public class PaginacaoModelView<T>
{
    public List<T> Itens { get; set; } = new();
    public int PaginaAtual { get; set; }
    public int ItensPorPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas { get; set; }
    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;
}
