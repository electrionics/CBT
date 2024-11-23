using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using FluentValidation;

namespace CBT.SharedComponents.Blazor.Common
{
    public class FluentValidationValidator<TValidator> : ComponentBase where TValidator : IValidator, new()
    {
        private readonly static char[] separators = ['.', '['];
        private TValidator validator;

        [CascadingParameter]
        private EditContext EditContext { get; set; }

        protected override void OnInitialized()
        {
            validator = new TValidator();
            var messages = new ValidationMessageStore(EditContext);

            /* Re-validate when any field changes or when the entire form   requests validation.*/
            EditContext.OnFieldChanged += (sender, eventArgs)
                => ValidateModel((EditContext)sender!, messages, false);

            EditContext.OnValidationRequested += (sender, eventArgs)
                => ValidateModel((EditContext)sender!, messages, true);
        }

        private void ValidateModel(EditContext editContext, ValidationMessageStore messages, bool submit)
        {
            if (submit)
                editContext.Properties["submitted"] = true;

            if (!editContext.Properties.TryGetValue("submitted", out _))
                return;

            var context = new ValidationContext<object>(editContext.Model);
            var validationResult = validator.Validate(context);
            messages.Clear();
            foreach (var error in validationResult.Errors)
            {
                var fieldIdentifier = ToFieldIdentifier(editContext, error.PropertyName);
                messages.Add(fieldIdentifier, error.ErrorMessage);
            }
            editContext.NotifyValidationStateChanged();
        }

        private static FieldIdentifier ToFieldIdentifier(EditContext editContext, string propertyPath)
        {
            var obj = editContext.Model;

            while (true)
            {
                var nextTokenEnd = propertyPath.IndexOfAny(separators);
                if (nextTokenEnd < 0)
                {
                    return new FieldIdentifier(obj, propertyPath);
                }

                var nextToken = propertyPath[..nextTokenEnd];
                propertyPath = propertyPath[(nextTokenEnd + 1)..];

                object? newObj;
                if (nextToken.EndsWith(']'))
                {
                    nextToken = nextToken[..^1];
                    var prop = obj.GetType().GetProperty("Item");
                    var indexerType = prop!.GetIndexParameters()[0].ParameterType;
                    var indexerValue = Convert.ChangeType(nextToken, indexerType);
                    newObj = prop.GetValue(obj, [indexerValue]);
                }
                else
                {
                    var prop = obj.GetType().GetProperty(nextToken) ?? throw new InvalidOperationException($"Could not find property named {nextToken} in object of type {obj.GetType().FullName}.");
                    newObj = prop.GetValue(obj);
                }

                if (newObj == null)
                {
                    return new FieldIdentifier(obj, nextToken);
                }

                obj = newObj;
            }
        }
    }
}
