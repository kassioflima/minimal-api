using FluentValidation;
using mininal_api.Dominio.DTOs;

namespace mininal_api.Dominio.Validadores;

public class VeiculoDTOValidator : AbstractValidator<VeiculoDTO>
{
    public VeiculoDTOValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres");

        RuleFor(x => x.Marca)
            .NotEmpty().WithMessage("Marca é obrigatória")
            .MaximumLength(100).WithMessage("Marca deve ter no máximo 100 caracteres");

        RuleFor(x => x.Ano)
            .GreaterThanOrEqualTo(1950).WithMessage("Ano deve ser maior ou igual a 1950")
            .LessThanOrEqualTo(2100).WithMessage("Ano deve ser menor ou igual a 2100");
    }
}
