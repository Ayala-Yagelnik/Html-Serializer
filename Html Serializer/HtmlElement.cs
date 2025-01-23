using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html_Serializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement()
        {
                Classes = new List<String>();
            Children = new List<HtmlElement>();
            Attributes = new Dictionary<string, string>();  
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (!(queue.Count == 0))
            {
                HtmlElement current = queue.Dequeue();
                yield return current;
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public IEnumerable<HtmlElement> Query(Selector selector)
        {
            var set = new HashSet<HtmlElement>();
            FindElementBySelector(selector, set, this.Descendants());
            return set;
        }
        private void FindElementBySelector(Selector selector, HashSet<HtmlElement> list, IEnumerable<HtmlElement> elements)
        {
            if (selector == null || elements == null || !elements.Any())
                return;

            foreach (var item in elements)
            {
                if (CheckSelector(item, selector))
                {
                    if (selector.Child == null)
                        list.Add(item);
                    FindElementBySelector(selector.Child, list, item.Descendants());
                }
            }
        }
        public bool CheckSelector(HtmlElement element, Selector selector)
        {
            if (selector.Id != null && !selector.Id.Equals(element.Id))
                return false;
            if (selector.TagName != null && selector.TagName != element.Name)
                return false;
            if (selector.Classes.Count > 0 && !selector.Classes.All(c => element.Classes.Contains(c)))
                return false;
            return true;
        }
     
        public override string ToString()
        {
            var attributes = Attributes.Count > 0 ? string.Join(", ", Attributes.Select(attr => $"{attr.Key}=\"{attr.Value}\"")) : "No Attributes";
            var classes = Classes.Count > 0 ? string.Join(" ", Classes) : "No Classes";
            var childrenCount = Children.Count;

            return $"Id: {Id}, Name: {Name}, Attributes: [{attributes}], Classes: [{classes}], InnerHtml: {InnerHtml}, Parent: {Parent?.Name}, Children Count: {childrenCount}";
        }
    
    }


}
