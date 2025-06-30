
using FluentValidation;
using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.DTOs.Product;

namespace OdontFlow.Domain.BusinessRules.Product;
public class CraeateProductValidator : AbstractValidator<CreateProductDTO>
{
    public CraeateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre de producto es requerido");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("El precio de producto debe ser mayor a $0.00");
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductDTO>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre de producto es requerido");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("El precio de producto debe ser mayor a $0.00");
    }
}
