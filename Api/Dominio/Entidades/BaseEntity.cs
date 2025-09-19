using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mininal_api.Dominio.Entidades
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [DataType(DataType.DateTime)]
        public DateTime DataCriacao { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DataAtualizacao { get; set; }

        [StringLength(255)]
        public string? CriadoPor { get; set; }

        [StringLength(255)]
        public string? AtualizadoPor { get; set; }

        public bool Ativo { get; set; } = true;

        public BaseEntity()
        {
            DataCriacao = DateTime.UtcNow;
            Ativo = true;
        }
    }
}
