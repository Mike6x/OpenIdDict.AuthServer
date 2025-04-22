using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.Core.OpenApi;

/// <summary>
/// Fixed error enum name generation with _0, _1, _2 ...
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
        {
            return;
        }

        var array = new OpenApiArray();
        array.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
        // NSwag
        schema.Extensions.Add("x-enumNames", array);
        // Openapi-generator
        schema.Extensions.Add("x-enum-varnames", array);
    }
}