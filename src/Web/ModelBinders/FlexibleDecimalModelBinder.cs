using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.ModelBinders;
/// <summary>
/// Encapsulates the functional responsibility of flexible decimal model binder within the application architecture.
/// </summary>
public sealed class FlexibleDecimalModelBinder : IModelBinder
{
            /// <summary>
            /// Executes the bind model async operation as part of this component.
            /// </summary>
            public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
        var raw = valueProviderResult.FirstValue;

        if (string.IsNullOrWhiteSpace(raw))
        {
            // Let [Required] handle it if needed
            bindingContext.Result = bindingContext.ModelType == typeof(decimal?)
                ? ModelBindingResult.Success(null)
                : ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var normalized = Normalize(raw);

        if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            bindingContext.Result = ModelBindingResult.Success(value);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.TryAddModelError(
            bindingContext.ModelName,
            $"Valor decimal invalid: '{raw}'.");
        return Task.CompletedTask;
    }
            /// <summary>
            /// Executes the normalize operation as part of this component.
            /// </summary>
            private static string Normalize(string input)
    {
        var s = input.Trim();
        s = s.Replace(" ", "").Replace("\u00A0", ""); // spaces + non-breaking spaces

        var lastDot = s.LastIndexOf('.');
        var lastComma = s.LastIndexOf(',');

        // Both present: whichever appears last is the decimal separator.
        if (lastDot >= 0 && lastComma >= 0)
        {
            if (lastComma > lastDot)
            {
                // 1.234,56 -> remove thousand dots, comma -> dot
                s = s.Replace(".", "");
                s = s.Replace(',', '.');
            }
            else
            {
                // 1,234.56 -> remove thousand commas
                s = s.Replace(",", "");
            }
            return s;
        }

        // Only comma: treat as decimal separator
        if (lastComma >= 0)
        {
            return s.Replace(',', '.');
        }

        // Only dot or neither: invariant parse will handle it
        return s;
    }
}
/// <summary>
/// Encapsulates the functional responsibility of flexible decimal model binder provider within the application architecture.
/// </summary>
public sealed class FlexibleDecimalModelBinderProvider : IModelBinderProvider
{
            /// <summary>
            /// Retrieves binder and returns it to the caller.
            /// </summary>
            public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
        {
            return new FlexibleDecimalModelBinder();
        }

        return null;
    }
}

