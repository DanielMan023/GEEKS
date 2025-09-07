using FluentValidation;
using GEEKS.Dto;
using GEEKS.Utils;

namespace GEEKS.Validators
{
    /// <summary>
    /// Validador para CreateProductDTO
    /// </summary>
    public class CreateProductValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .MaximumLength(Constants.Validation.NameMaxLength)
                .WithMessage($"El nombre no puede exceder {Constants.Validation.NameMaxLength} caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .MaximumLength(Constants.Validation.DescriptionMaxLength)
                .WithMessage($"La descripción no puede exceder {Constants.Validation.DescriptionMaxLength} caracteres");

            RuleFor(x => x.ShortDescription)
                .MaximumLength(Constants.Validation.ShortDescriptionMaxLength)
                .WithMessage($"La descripción corta no puede exceder {Constants.Validation.ShortDescriptionMaxLength} caracteres");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

            RuleFor(x => x.DiscountPrice)
                .GreaterThanOrEqualTo(0).WithMessage("El precio de descuento debe ser mayor o igual a 0")
                .LessThan(x => x.Price).When(x => x.DiscountPrice.HasValue && x.Price > 0)
                .WithMessage("El precio de descuento debe ser menor al precio original");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock debe ser mayor o igual a 0");

            RuleFor(x => x.MinStock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo debe ser mayor o igual a 0");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .MaximumLength(Constants.Validation.SkuMaxLength)
                .WithMessage($"El SKU no puede exceder {Constants.Validation.SkuMaxLength} caracteres")
                .Matches(Constants.Patterns.SkuPattern)
                .WithMessage(Constants.ErrorMessages.InvalidSku);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("La categoría es requerida");

            RuleFor(x => x.Brand)
                .MaximumLength(Constants.Validation.BrandMaxLength)
                .WithMessage($"La marca no puede exceder {Constants.Validation.BrandMaxLength} caracteres");

            RuleFor(x => x.Weight)
                .GreaterThan(0).When(x => x.Weight > 0)
                .WithMessage("El peso debe ser mayor a 0");

            RuleFor(x => x.Length)
                .GreaterThan(0).When(x => x.Length > 0)
                .WithMessage("La longitud debe ser mayor a 0");

            RuleFor(x => x.Width)
                .GreaterThan(0).When(x => x.Width > 0)
                .WithMessage("El ancho debe ser mayor a 0");

            RuleFor(x => x.Height)
                .GreaterThan(0).When(x => x.Height > 0)
                .WithMessage("La altura debe ser mayor a 0");
        }
    }

    /// <summary>
    /// Validador para UpdateProductDTO
    /// </summary>
    public class UpdateProductValidator : AbstractValidator<UpdateProductDTO>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(Constants.Validation.NameMaxLength)
                .WithMessage($"El nombre no puede exceder {Constants.Validation.NameMaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(Constants.Validation.DescriptionMaxLength)
                .WithMessage($"La descripción no puede exceder {Constants.Validation.DescriptionMaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ShortDescription)
                .MaximumLength(Constants.Validation.ShortDescriptionMaxLength)
                .WithMessage($"La descripción corta no puede exceder {Constants.Validation.ShortDescriptionMaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.ShortDescription));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.DiscountPrice)
                .GreaterThanOrEqualTo(0).WithMessage("El precio de descuento debe ser mayor o igual a 0")
                .When(x => x.DiscountPrice.HasValue);

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock debe ser mayor o igual a 0")
                .When(x => x.Stock.HasValue);

            RuleFor(x => x.MinStock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo debe ser mayor o igual a 0")
                .When(x => x.MinStock.HasValue);

            RuleFor(x => x.SKU)
                .MaximumLength(Constants.Validation.SkuMaxLength)
                .WithMessage($"El SKU no puede exceder {Constants.Validation.SkuMaxLength} caracteres")
                .Matches(Constants.Patterns.SkuPattern)
                .WithMessage(Constants.ErrorMessages.InvalidSku)
                .When(x => !string.IsNullOrEmpty(x.SKU));

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("La categoría debe ser válida")
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.Brand)
                .MaximumLength(Constants.Validation.BrandMaxLength)
                .WithMessage($"La marca no puede exceder {Constants.Validation.BrandMaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.Brand));

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("El peso debe ser mayor a 0")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Length)
                .GreaterThan(0).WithMessage("La longitud debe ser mayor a 0")
                .When(x => x.Length.HasValue);

            RuleFor(x => x.Width)
                .GreaterThan(0).WithMessage("El ancho debe ser mayor a 0")
                .When(x => x.Width.HasValue);

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("La altura debe ser mayor a 0")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.State)
                .Must(state => state == null || state == Constants.States.Active || state == Constants.States.Inactive || state == Constants.States.Deleted)
                .WithMessage("El estado debe ser Active, Inactive o Deleted")
                .When(x => !string.IsNullOrEmpty(x.State));
        }
    }

    /// <summary>
    /// Validador para ProductFilterDTO
    /// </summary>
    public class ProductFilterValidator : AbstractValidator<ProductFilterDTO>
    {
        public ProductFilterValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(Constants.Pagination.MinPage)
                .WithMessage($"La página debe ser mayor o igual a {Constants.Pagination.MinPage}");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("El tamaño de página debe ser mayor a 0")
                .LessThanOrEqualTo(Constants.Pagination.MaxPageSize)
                .WithMessage($"El tamaño de página no puede exceder {Constants.Pagination.MaxPageSize}");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("El precio mínimo debe ser mayor o igual a 0")
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("El precio máximo debe ser mayor o igual a 0")
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThan(x => x.MinPrice).When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue)
                .WithMessage("El precio máximo debe ser mayor al precio mínimo");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("El ID de categoría debe ser válido")
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.SortBy)
                .Must(sortBy => sortBy == null || 
                    sortBy.ToLower() == "name" || 
                    sortBy.ToLower() == "price" || 
                    sortBy.ToLower() == "stock" || 
                    sortBy.ToLower() == "createdat")
                .WithMessage("El campo de ordenamiento debe ser: name, price, stock o createdat")
                .When(x => !string.IsNullOrEmpty(x.SortBy));

            RuleFor(x => x.SortOrder)
                .Must(sortOrder => sortOrder == null || 
                    sortOrder.ToLower() == "asc" || 
                    sortOrder.ToLower() == "desc")
                .WithMessage("El orden debe ser: asc o desc")
                .When(x => !string.IsNullOrEmpty(x.SortOrder));
        }
    }
}
