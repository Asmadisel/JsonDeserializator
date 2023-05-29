using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace JsonDeserializator.Models
{
    class PropertyBag
    {
        public Dictionary<string, string> Properties { get; set; }

        public PropertyBag()
        {
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    class Message : PropertyBag
    {
        public long Id { get; set; }
        public double Value { get; set; }
        public string Status { get; set; }
    }

    class PropertyBagConverter : JsonConverter
    {
        public override bool CanConvert(Type T)
        {
            return T.IsSubclassOf(typeof(PropertyBag));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            existingValue = existingValue ?? Activator.CreateInstance(objectType, true);

            var jObject = JObject.Load(reader);
            var properties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var props = new Dictionary<string, PropertyInfo>(properties.ToDictionary(k => k.Name, v => v), StringComparer.OrdinalIgnoreCase);

            var missingProps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var token in jObject)
            {
                PropertyInfo prop;
                if (props.TryGetValue(token.Key, out prop))
                {
                    dynamic val = token.Value.ToObject<object>();
                    prop.SetValue(existingValue, val, null);
                }
                else
                {
                    missingProps.Add(token.Key.ToUpperInvariant(), token.Value.ToString());
                }
            }

            var pb = existingValue as PropertyBag;
            if (pb != null)
            {
                pb.Properties = missingProps;
            }

            return existingValue;
        }
    }
}
