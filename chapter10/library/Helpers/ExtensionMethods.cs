using System;
using System.Linq;
using System.Text;

using HtmlAgilityPack;

namespace chapter10.lib.Helpers
{
    public static class ExtensionMethods
    {
        public static string[] ToPropertyList<T>(this Type objType, string labelName) => 
            objType.GetProperties().Where(a => a.Name != labelName).Select(a => a.Name).ToArray();

        public static string ToWebContentString(this string url)
        {
            var web = new HtmlWeb();

            var htmlDoc = web.Load(url);
            
            var sb = new StringBuilder();

            htmlDoc.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Name == "style").ToList().ForEach(n => n.Remove());

            foreach (var node in htmlDoc.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']"))
            {
                sb.Append(node.InnerText.Trim().Replace(" ", ""));
            }

            return sb.ToString();
        }
    }
}