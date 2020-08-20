using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace SaneWebNetCore
{
    public class Web
    {
        public string Get(string url)
        {
            ServicePointManager.DefaultConnectionLimit = 16;
            ServicePointManager.Expect100Continue = false;
            WebRequest request = WebRequest.Create(url);
            request.Proxy = GlobalProxySelection.GetEmptyWebProxy();
            return new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
        }
        public List<Node> Parse(string html, string element = "", string sort = "")
        {
            //string attribution = "(?<=" + get + "=\")[^\"]*(?=\")";
            List<Node> list = new List<Node>();
            foreach (Match singlematch in Regex.Matches(html, $"<" + element + "[^>]* (" + sort + ")[^>]*(\\/>|>[^<]*<\\/" + element + ">)"))
            {
                if (singlematch.Success)
                {
                    list.Add(new Node(singlematch.Value));
                }

            }
            return list;
        }
    }
    public class Node
    {
        public Node(string html)
        {
            InnerHtml = html;
        }

        public string GetAttributeValue(string attributename)
        {
            return Regex.Match(InnerHtml, $"(?<={attributename}=\")[^\"]*(?=\")").Value;
        }
        public void SetAttributeValue(string attributename, string attributevalue)
        {
            Resolve(attributename);
            InnerHtml = Regex.Replace(InnerHtml, $"(?<={attributename}=\")[^\"]*(?=\")", attributevalue);
        }
        public void DeleteAttribute(string attributename)
        {
            InnerHtml = Regex.Replace(InnerHtml, $" {attributename}=\"[^\"]*\"", string.Empty);
        }
        public string GetValue()
        {
            return Regex.Match(InnerHtml, @"(?<=<[^>]*>)[^<]*(?=</[^>]*>)").Value;
        }
        public string InnerHtml { get; set; }

        //private methods
        private void Resolve(string attributename)
        {
            if (!Regex.Match(InnerHtml, $"{attributename}=\"[^\"]*\"").Success)
            {
                InnerHtml = InnerHtml.Insert(Regex.Match(InnerHtml, "[^A-zA-Za-z0-9<>\\/\"\\s]*(?=\\/?>)").Index, $" {attributename}=\" \"");
            }
        }
    }
}
