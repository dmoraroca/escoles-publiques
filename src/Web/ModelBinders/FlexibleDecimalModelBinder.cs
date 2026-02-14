using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.ModelBinders;

/// <summary>
/// Parses decimals from form input accepting both ',' and '.' as decimal separators,
/// and handling common thousand-separator formats (e.g. 1.234,56 or 1,234.56).
/// </summary>
public sealed class FlexibleDecimalModelBinder : IModelBinder
{
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

public sealed class FlexibleDecimalModelBinderProvider : IModelBinderProvider
{
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

