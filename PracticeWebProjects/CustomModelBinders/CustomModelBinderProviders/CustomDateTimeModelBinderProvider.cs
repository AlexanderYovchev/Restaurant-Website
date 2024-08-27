using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace PracticeWebProjects.CustomModelBinders.CustomModelBinderProviders
{
    public class CustomDateTimeModelBinderProvider : IModelBinderProvider
    {
        private readonly string _dateFormat;

        public CustomDateTimeModelBinderProvider(string dateFormat)
        {
            _dateFormat = dateFormat;
        }
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
            {
                return new BinderTypeModelBinder(typeof(CustomDateTimeModelBinder));
            }

            return null;
        }
    }
}
