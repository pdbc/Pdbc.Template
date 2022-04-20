using Aertssen.Framework.Infra.Validation;
using Aertssen.Framework.Infra.Validation.Extensions;
using FluentValidation;
using MyTemplate.Domain.Validations;
using MyTemplate.Dto.MyEntity;
using MyTemplate.I18N;

namespace MyTemplate.Core.Validators
{
    public class StoreMyEntityDtoValidator : FluentValidationValidator<IStoreMyEntityDto>
    {

        public StoreMyEntityDtoValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(ValidationConstants.MyEntityNameMaximumLength)
                .WithErrorMessage(nameof(Errors.MyEntityNameMaximumLengthValidationFailed));
        }
    }
}
