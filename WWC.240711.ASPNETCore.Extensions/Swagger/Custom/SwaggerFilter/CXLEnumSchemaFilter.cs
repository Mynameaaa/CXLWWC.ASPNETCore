using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class CXLEnumSchemaFilter : ISchemaFilter
    {
        static readonly ConcurrentDictionary<Type, Tuple<string, object>[]> dict = new ConcurrentDictionary<Type, Tuple<string, object>[]>();

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var items = GetTextValueItems(context.Type);
                if (items.Length > 0)
                {
                    string decription = string.Join(",", items.Select(f => $"{f.Item1}={f.Item2}"));
                    schema.Description = string.IsNullOrEmpty(schema.Description) ? decription : $"{schema.Description}:{decription}";
                }
            }
            else if (context.Type.IsClass && context.Type != typeof(string))
            {
                UpdateSchemaDescription(schema, context);
            }
        }

        private void UpdateSchemaDescription(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Reference != null)
            {
                var s = context.SchemaRepository.Schemas[schema.Reference.Id];
                if (s != null && s.Enum != null && s.Enum.Count > 0)
                {
                    string description = $"【{s.Description}】";
                    if (string.IsNullOrEmpty(schema.Description) || !schema.Description.EndsWith(description))
                        schema.Description += description;
                }
            }

            foreach (var key in schema.Properties.Keys)
            {
                var s = schema.Properties[key];
                UpdateSchemaDescription(s, context);
            }

        }

        /// <summary>
        /// 获取枚举值+描述  
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        private Tuple<string, object>[] GetTextValueItems(Type enumType)
        {
            Tuple<string, object>[] tuples;
            if (dict.TryGetValue(enumType, out tuples) && tuples != null)
            {
                return tuples;
            }

            FieldInfo[] fields = enumType.GetFields();
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    var attribute = field.GetCustomAttribute<EnumDescriptionAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }
                    string key = attribute?.Description ?? field.Name;
                    int value = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null));
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    list.Add(new KeyValuePair<string, int>(key, value));
                }
            }
            tuples = list.OrderBy(f => f.Value).Select(f => new Tuple<string, object>(f.Key, f.Value.ToString())).ToArray();
            dict.TryAdd(enumType, tuples);
            return tuples;
        }

    }

}
