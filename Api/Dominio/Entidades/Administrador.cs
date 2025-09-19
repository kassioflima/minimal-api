using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mininal_api.Dominio.Entidades;

public class Administrador : BaseEntity
{
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Senha { get; set; } = default!;

    [Required]
    [StringLength(10)]
    public string Perfil { get; set; } = default!;
}