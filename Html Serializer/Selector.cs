using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public static Selector PerSelector(string query)
        {
            Selector selector = new Selector();
            selector.Classes = new List<string>();
            string[] queries = Regex.Split(query, @"(#|\.)");

            for (int i = 0; i < queries.Length; i++)
            {
                if (queries[i].StartsWith('#') && i < queries.Length - 1)
                {
                    selector.Id = queries[++i];
                }
                if (queries[i].StartsWith('.') && i < queries.Length - 1)
                {
                    selector.Classes.Add(queries[++i]);
                }
                else if (HtmlHelper.Instance.Tags.Contains(queries[i]) || HtmlHelper.Instance.VoidTags.Contains(queries[i]))
                    selector.TagName = queries[i];
            }

            return selector;
        }


        public static Selector ConvertQuery(string query)
        {
            List<string> queries = query.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            Selector root = PerSelector(queries[0]);
            queries.RemoveAt(0);
            Selector current = root;
            foreach (string q in queries)
            {
                current.Child = PerSelector(q);
                current.Child.Parent = current;
                current = current.Child;
            }
            return root;
        }
        public override string ToString()
        {
            var classes = Classes.Count > 0 ? string.Join(" ", Classes) : "No Classes";
            return $"TagName: {TagName}, Id: {Id}, Classes: {classes}, Parent: {Parent?.TagName}, Child: {Child?.TagName}";
        }
    }
}
