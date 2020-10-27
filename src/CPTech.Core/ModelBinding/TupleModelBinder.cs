using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace CPTech.ModelBinding
{
    public class TupleModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");

            string body = await new StreamReader(bindingContext.HttpContext.Request.Body).ReadToEndAsync();
            var tupleAttribute = (bindingContext.ModelMetadata.GetType().GetProperty("Attributes").GetValue(bindingContext.ModelMetadata) as ModelAttributes).Attributes.OfType<TupleElementNamesAttribute>().FirstOrDefault();
            if (tupleAttribute == null)
                bindingContext.Result = ModelBindingResult.Failed();
            else
                bindingContext.Result = ModelBindingResult.Success(ParseTupleFromModelAttributes(body, tupleAttribute, bindingContext.ModelType));
        }

        public static object ParseTupleFromModelAttributes(string body, TupleElementNamesAttribute tupleAttribute, Type tupleType)
        {
            object obj = Activator.CreateInstance(tupleType);

            int num = 1;
            var jObject = JsonDocument.Parse(body).RootElement;
            foreach (string transformName in tupleAttribute.TransformNames)
            {
                FieldInfo field = tupleType.GetField("Item" + num++.ToString());

                JsonProperty property = jObject.EnumerateObject().FirstOrDefault(c => c.Name.Equals(transformName, StringComparison.OrdinalIgnoreCase));
                if (!property.Equals(null) && property.Value.ToString() != "")
                {
                    if (field.FieldType.IsPrimitive || field.FieldType == typeof(string) || field.FieldType == typeof(decimal))
                        field.SetValue(obj, Convert.ChangeType(property.Value.ToString(), field.FieldType));
                    else
                        field.SetValue(obj, JsonSerializer.Deserialize(property.Value.ToString(), field.FieldType));
                }
                else
                    field.SetValue(obj, null);
            }

            return obj;
        }
    }
}
