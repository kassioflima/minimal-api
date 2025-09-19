using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mininal_api.Dominio.Entidades;

public class Veiculo : BaseEntity
{
    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Marca { get; set; } = default!;

    [Required]
    [Range(1950, 2100)]
    public int Ano { get; set; } = default!;
}