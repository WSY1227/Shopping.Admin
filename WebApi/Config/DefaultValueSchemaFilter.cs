using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Config;

public class DefaultValueSchemaFilter: ISchemaFilter
{
    public void Apply(OpenApiSchema? schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || context.Type == null) return;
        foreach (var property in schema.Properties)
        {
            property.Value.Default = property.Value.Type switch
            {
                // 按照数据类型设置默认值
                "string" when property.Value.Default == null => new OpenApiString(""),
                "integer" when property.Value.Default == null => new OpenApiInteger(0),
                "number" when property.Value.Default == null => new OpenApiDouble(0),
                _ => property.Key switch
                {
                    "pageIndex" => new OpenApiInteger(1),
                    "pageSize" => new OpenApiInteger(10),
                    _ => property.Value.Default
                }
            };
            
            // 通过特性来实现
            var defaultAttribute = context.ParameterInfo?.GetCustomAttribute<DefaultValueAttribute>();
            
            if (defaultAttribute != null)
            {
                property.Value.Example = (IOpenApiAny)defaultAttribute.Value;
                
            }
            
        }
    }
}