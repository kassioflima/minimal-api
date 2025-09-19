using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mininal_api.Dominio.Entidades;

public class RefreshToken
{
    [Key]
    [StringLength(256)]
    public string Token { get; set; } = string.Empty;

    [StringLength(256)]
    public string JwtId { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime DataCriacao { get; set; }

    public DateTime DataExpiracao { get; set; }

    public bool Usado { get; set; }

    public bool Invalido { get; set; }

    public int AdministradorId { get; set; }

    [ForeignKey("AdministradorId")]
    public Administrador Administrador { get; set; } = default!;

    public RefreshToken()
    {
        DataCriacao = DateTime.UtcNow;
    }
}
