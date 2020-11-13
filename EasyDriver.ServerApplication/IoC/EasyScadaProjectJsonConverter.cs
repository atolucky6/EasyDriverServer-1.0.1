using EasyDriver.Core;
using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace EasyScada.ServerApplication
{
    public class EasyScadaProjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IEasyScadaProject project = null;
            if (serializer.Deserialize(reader) is JObject jObject)
            {
                project = new EasyScadaProject();
                project.ProjectPath = jObject["ProjectPath"].Value<string>();
                project.Username = jObject["Username"].Value<string>();
                project.Password = jObject["Password"].Value<string>();
                var parameterContainer = JsonConvert.DeserializeObject<ParameterContainer>(jObject["ParameterContainer"].ToString());
                if (parameterContainer != null)
                    project.ParameterContainer = parameterContainer;

                CoreItemJsonConverter converter = new CoreItemJsonConverter();
                var childItems = jObject["Childs"];
                if (childItems != null)
                {
                    foreach (var item in childItems)
                    {
                        var child = JsonConvert.DeserializeObject(item.ToString(), typeof(ICoreItem), converter);
                        if (child != null)
                            project.Childs.Add(child);
                    }
                }

                if (project.Childs.Where(x => x is LocalStation).Count() > 1)
                    project.Childs.RemoveAt(0);

                project.UpdateParent(false);
            }
            return project;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value is IEasyScadaProject project)
            {
                writer.WritePropertyName("ProjectPath");
                writer.WriteValue(project.ProjectPath);

                writer.WritePropertyName("Username");
                writer.WriteValue(project.Username);

                writer.WritePropertyName("Password");
                writer.WriteValue(project.Password);

                writer.WritePropertyName("ParameterContainer");
                writer.WriteRawValue(JsonConvert.SerializeObject(project.ParameterContainer, Formatting.Indented));

                writer.WritePropertyName("Childs");
                writer.WriteStartArray();
                CoreItemJsonConverter converter = new CoreItemJsonConverter();
                foreach (var item in project.Childs)
                {
                    writer.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.Indented, converter));
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}
