using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] Tags { get; set; }
        public string[] VoidTags { get; set; }
        private HtmlHelper()
        {


            var jsonTags = File.ReadAllText("json\\HtmlTags.json");
            var jsonVoidTags = File.ReadAllText("json\\HtmlVoidTags.json");
            Tags = JsonSerializer.Deserialize<string[]>(jsonTags);
            VoidTags = JsonSerializer.Deserialize<string[]>(jsonVoidTags);

        }
    }
}
