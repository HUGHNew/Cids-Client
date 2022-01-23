using Newtonsoft.Json;
namespace Client {
    namespace Json {
        public abstract class JsonExtensionBase {
            public string GetJsonString() { return JsonConvert.SerializeObject(this); }
        }
        class Issue : JsonExtensionBase {
            public string message { get; set; }
            public string time { get; set; }
        }
    }
}
