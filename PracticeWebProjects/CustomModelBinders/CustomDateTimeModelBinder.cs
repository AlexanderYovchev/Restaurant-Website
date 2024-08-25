using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace PracticeWebProjects.CustomModelBinders
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == ValueProviderResult.None || string.IsNullOrEmpty(value.FirstValue))
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Date must not be null or empty value");
                return Task.CompletedTask;
            }

            string valueAsString = value.FirstValue;

            if (DateTime.TryParseExact
                (valueAsString, 
                DataValidatingClass.saleDateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
            {
                bindingContext.Result = ModelBindingResult.Success(parsedDate);
            }
            
            else
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    String.Format("Date must be in {0} format", DataValidatingClass.saleDateFormat));
            }

            return Task.CompletedTask;

        }
    }
}
