using Raven.Imports.Newtonsoft.Json;

namespace RavenTrafficGeneratingTool.Model
{
    public class RandomEntity
    {
        public string Id { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
