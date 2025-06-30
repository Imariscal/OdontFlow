using FluentValidation;
using OdontFlow.Domain.DTOs.Supplier;

namespace OdontFlow.Domain.BusinessRules.Supplier;

public class CreateSupplierValidator : AbstractValidator<CreateSupplierDTO>
{
    public CreateSupplierValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre de proveedor es requerido");
        RuleFor(x => x.Email).NotEmpty().WithMessage("El correo de proveedor es requerido").EmailAddress().WithMessage("El correo electronico es invalido");
        RuleFor(x => x.Contact).NotEmpty().WithMessage("El nombre de contacto es requerido");
    }
}

public class UpdateSupplierValidator : AbstractValidator<UpdateSupplierDTO>
{
    public UpdateSupplierValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre de proveedor es requerido");
        RuleFor(x => x.Email).NotEmpty().WithMessage("El correo de proveedor es requerido").EmailAddress().WithMessage("El correo electronico es invalido");
        RuleFor(x => x.Contact).NotEmpty().WithMessage("El nombre de contacto es requerido");
    }
}
