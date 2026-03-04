using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Primitives;
using Web.ModelBinders;

namespace UnitTest.ModelBinders;

public class FlexibleDecimalModelBinderTests
{
    [Theory]
    [InlineData("1234.56", 1234.56)]
    [InlineData("1234,56", 1234.56)]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("1,234.56", 1234.56)]
    [InlineData(" 1 234,56 ", 1234.56)]
    public async Task BindModelAsync_BindsDecimalFormats(string input, decimal expected)
    {
        var binder = new FlexibleDecimalModelBinder();
        var ctx = CreateContext(typeof(decimal), "value", input);

        await binder.BindModelAsync(ctx);

        Assert.True(ctx.Result.IsModelSet);
        Assert.Equal(expected, Assert.IsType<decimal>(ctx.Result.Model));
    }

    [Fact]
    public async Task BindModelAsync_BindsNull_ForNullableDecimalAndEmptyInput()
    {
        var binder = new FlexibleDecimalModelBinder();
        var ctx = CreateContext(typeof(decimal?), "value", "   ");

        await binder.BindModelAsync(ctx);

        Assert.True(ctx.Result.IsModelSet);
        Assert.Null(ctx.Result.Model);
    }

    [Fact]
    public async Task BindModelAsync_Fails_ForNonNullableDecimalAndEmptyInput()
    {
        var binder = new FlexibleDecimalModelBinder();
        var ctx = CreateContext(typeof(decimal), "value", "");

        await binder.BindModelAsync(ctx);

        Assert.False(ctx.Result.IsModelSet);
    }

    [Fact]
    public async Task BindModelAsync_AddsModelError_ForInvalidValue()
    {
        var binder = new FlexibleDecimalModelBinder();
        var ctx = CreateContext(typeof(decimal), "value", "nope");

        await binder.BindModelAsync(ctx);

        Assert.False(ctx.Result.IsModelSet);
        Assert.True(ctx.ModelState.ContainsKey("value"));
        var errors = ctx.ModelState["value"]!.Errors;
        Assert.Single(errors);
        Assert.Contains("Valor decimal invalid", errors[0].ErrorMessage);
    }

    [Fact]
    public async Task BindModelAsync_DoesNothing_WhenValueIsMissing()
    {
        var binder = new FlexibleDecimalModelBinder();
        var metadataProvider = new EmptyModelMetadataProvider();
        var modelMetadata = metadataProvider.GetMetadataForType(typeof(decimal));
        var bindingContext = new DefaultModelBindingContext
        {
            ModelMetadata = modelMetadata,
            ModelName = "value",
            FieldName = "value",
            ModelState = new ModelStateDictionary(),
            ValueProvider = new TestValueProvider(new Dictionary<string, string>())
        };

        await binder.BindModelAsync(bindingContext);

        Assert.False(bindingContext.Result.IsModelSet);
        Assert.Empty(bindingContext.ModelState);
    }

    [Fact]
    public void Provider_ReturnsBinder_ForDecimalTypes()
    {
        var provider = new FlexibleDecimalModelBinderProvider();

        var decimalContext = CreateProviderContext(typeof(decimal));
        var nullableContext = CreateProviderContext(typeof(decimal?));

        Assert.IsType<FlexibleDecimalModelBinder>(provider.GetBinder(decimalContext));
        Assert.IsType<FlexibleDecimalModelBinder>(provider.GetBinder(nullableContext));
    }

    [Fact]
    public void Provider_ReturnsNull_ForOtherTypes()
    {
        var provider = new FlexibleDecimalModelBinderProvider();
        var context = CreateProviderContext(typeof(string));

        var binder = provider.GetBinder(context);

        Assert.Null(binder);
    }

    private static DefaultModelBindingContext CreateContext(Type modelType, string modelName, string value)
    {
        var metadataProvider = new EmptyModelMetadataProvider();
        var modelMetadata = metadataProvider.GetMetadataForType(modelType);

        return new DefaultModelBindingContext
        {
            ModelMetadata = modelMetadata,
            ModelName = modelName,
            FieldName = modelName,
            ModelState = new ModelStateDictionary(),
            ValueProvider = new TestValueProvider(new Dictionary<string, string>
            {
                [modelName] = value
            })
        };
    }

    private static ModelBinderProviderContext CreateProviderContext(Type modelType)
    {
        var metadataProvider = new EmptyModelMetadataProvider();
        var metadata = metadataProvider.GetMetadataForType(modelType);
        return new TestModelBinderProviderContext(metadata);
    }

    private sealed class TestModelBinderProviderContext : ModelBinderProviderContext
    {
        public TestModelBinderProviderContext(ModelMetadata metadata)
        {
            Metadata = metadata;
            MetadataProvider = new EmptyModelMetadataProvider();
        }

        public override BindingInfo BindingInfo => new();

        public override ModelMetadata Metadata { get; }

        public override IModelMetadataProvider MetadataProvider { get; }

        public override IModelBinder CreateBinder(ModelMetadata metadata)
            => throw new NotSupportedException();
    }

    private sealed class TestValueProvider : IValueProvider
    {
        private readonly Dictionary<string, string> _values;

        public TestValueProvider(Dictionary<string, string> values)
        {
            _values = values;
        }

        public bool ContainsPrefix(string prefix) => _values.Keys.Any(k => k.StartsWith(prefix, StringComparison.Ordinal));

        public ValueProviderResult GetValue(string key)
        {
            if (_values.TryGetValue(key, out var value))
            {
                return new ValueProviderResult(new StringValues(value), CultureInfo.InvariantCulture);
            }

            return ValueProviderResult.None;
        }
    }
}
