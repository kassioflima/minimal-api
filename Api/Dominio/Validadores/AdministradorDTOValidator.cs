using FluentValidation;
using mininal_api.Dominio.DTOs;

namespace mininal_api.Dominio.Validadores;

public class AdministradorDTOValidator : AbstractValidator<AdministradorDTO>
{
    public AdministradorDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter um formato válido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
            .MaximumLength(255).WithMessage("Senha deve ter no máximo 255 caracteres");

        RuleFor(x => x.Perfil)
            .NotNull().WithMessage("Perfil é obrigatório")
            .IsInEnum().WithMessage("Perfil deve ser um valor válido");
    }
}
