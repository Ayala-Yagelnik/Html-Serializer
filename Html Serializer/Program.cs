using Html_Serializer;
using System;
using System.Text.RegularExpressions;

async Task<string> Load(string url)
{
    using (HttpClient client = new HttpClient())
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // מוודא שהבקשה הצליחה
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
        catch (Exception e)
        {
            Console.WriteLine("error in loading:" + e.Message);
            return "";
        }

    }
}

 static HtmlElement buildTree(List<string> htmlLines)
{
    //htmlLines = htmlLines.SkipWhile(s => !s.StartsWith("html")).ToList();

    var root = new HtmlElement { Children = new List<HtmlElement>() };
    var current = root;
    foreach (var line in htmlLines)
    {
        //הסתיימו התגיות
        if (line == "/html")
            return root;
        //סגירת תגית
        if (line.StartsWith("/"))
        {
            if (current.Parent != null)
                current = current.Parent;
            continue;
        }
        string name = line.Split(' ')[0];
        name = name.EndsWith("/") ? name.Substring(0, name.Length - 1) : name;

        //פתיחת תגית
        if (HtmlHelper.Instance.Tags.Contains(name))
        {
            var newElement = new HtmlElement
            {
                Attributes = new Dictionary<string, string>(),
                Children = new List<HtmlElement>(),
                Classes = new List<string>(),
                Name = name
            };

            current.Children.Add(newElement);
            newElement.Parent = current;
            var rest = line.Substring(name.Length).Trim();
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(rest);

            foreach (Match match in attributes)
            {
                if (match.Groups[1].Value == "id")
                    newElement.Id = match.Groups[2].Value;

                else if (match.Groups[1].Value == "class")
                    newElement.Classes = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                else
                {
                    //string[] key_value = match.Groups[1].Value.Split('=');
                    //string key = key_value[0];
                    //string value = string.Join(" ", key_value.Skip(1));
                    //newElement.Attributes.Add(key,value);
                    //newElement.Attributes.Add(match.Groups[2].Value);
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    newElement.Attributes.Add(key, value);
                }
            }
            if (!line.EndsWith("/") && !HtmlHelper.Instance.VoidTags.Contains(name))
                current = newElement;
        }
        else
            current.InnerHtml = line;
    }

    return root;
}


string html = await Load("http://netfree.link");
var cleanHtml = new Regex("\\s").Replace(html, " ");
cleanHtml = Regex.Replace(cleanHtml, @"[ ]{2,}", " ");
cleanHtml = Regex.Replace(cleanHtml, @"&nbsp;", " ");
var tagMatches = Regex.Matches(cleanHtml, @"<\/?([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>|([^<]+)")
                      .Where(l => !String.IsNullOrWhiteSpace(l.Value));
var htmlLines = new List<string>();
foreach (Match item in tagMatches)
{
    string tag = item.Value.Trim();
    if (tag.StartsWith('<'))
        tag = tag.Trim('<', '>');
    htmlLines.Add(tag);
}
htmlLines.RemoveAt(0);
var root = buildTree(htmlLines);
Selector selector = Selector.ConvertQuery("ul.nav.navbar-nav");
var elements = root.Query(selector);
elements.ToList().ForEach(e => { Console.WriteLine(e.ToString()); });