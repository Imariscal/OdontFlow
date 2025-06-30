using FluentValidation;
using OdontFlow.Domain.DTOs.PriceList;
namespace OdontFlow.Domain.BusinessRules.PriceList;
public class CreatePriceListValidator : AbstractValidator<CreatePriceListDTO>
{
    public CreatePriceListValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre de la lista es requerido");
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).WithMessage("El descuento debe ser mayor igual a 0.");
        RuleFor(x => x.Discount).LessThanOrEqualTo(100).WithMessage("El descuento debe ser menor igual a 100.");
    }
}

public class UpdatePriceListValidator : AbstractValidator<UpdatePriceListDTO>
{
    public UpdatePriceListValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre de la lista es requerido");
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).WithMessage("El descuento debe ser mayor igual a 0.");
        RuleFor(x => x.Discount).LessThanOrEqualTo(100).WithMessage("El descuento debe ser menor igual a 100.");
    }
}
