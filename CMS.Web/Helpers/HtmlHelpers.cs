using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
namespace CMS.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlContent RequiredSign(this IHtmlHelper helper)
        {
            TagBuilder tb = new TagBuilder("span");
            tb.Attributes.Add("class", "text-danger");
            tb.Attributes.Add("title", "هذا الحقل مطلوب");
            tb.InnerHtml.AppendHtml("*");
            return tb;
        }
        public static IHtmlContent DropdownBtnGroup(this IHtmlHelper helper, params string[] links)
        {
            TagBuilder tb = new TagBuilder("div");
            TagBuilder tbUl = new TagBuilder("ul");
            TagBuilder tbButtonWrapper = new TagBuilder("div");
            tbButtonWrapper.AddCssClass("btn-group");
            TagBuilder tbButton = new TagBuilder("button");
            tbButton.Attributes.Add("type", "button");
            tbButton.AddCssClass("btn");
            tbButton.AddCssClass("btn-info");
            tbButton.AddCssClass("dropdown-toggle");
            tbButton.Attributes.Add("data-toggle", "dropdown");

            tbUl.AddCssClass("dropdown-menu");
            tbUl.AddCssClass("split-button");
            int index = 0;
            if (links.Count() > 1)
            {

                tb.Attributes.Add("class", "btn-group");
                tb.Attributes.Add("style", "flex");


                foreach (var item in links)
                {
                    if (index == 0)
                    {
                        tbButton.InnerHtml.AppendHtml("<span class=\"caret\"></span>");
                        tbButtonWrapper.InnerHtml.AppendHtml(tbButton);
                        tb.InnerHtml.AppendHtml(links.FirstOrDefault());
                        index = 1;
                    }
                    else
                    {
                        var tbli = new TagBuilder("li");
                        tbli.InnerHtml.AppendHtml(item);
                        tbUl.InnerHtml.AppendHtml(tbli);
                    }

                }

                tbButtonWrapper.InnerHtml.AppendHtml(tbUl);
                tb.InnerHtml.AppendHtml(tbButtonWrapper);

            }
            else
            {
                tb.InnerHtml.AppendHtml(links.FirstOrDefault());
            }

            return tb;
        }
    }
    public static class MultilevelMenuHelper
    {
        public static IHtmlContent GetMultilevelMenu(this IHtmlHelper helper, List<MenuItem> MenuItems, int? MenuItemId, bool? bootsrap, string LiClasses, string UlClasses, string AnchorClasses)
        {
            //<ul class="navbar-nav mr-auto">
            TagBuilder tb = new TagBuilder("ul");
            tb.AddCssClass("navbar-nav");
            tb.AddCssClass("mr-auto");
            var html = "";
            if (bootsrap == true)
                html = GetMultilevelMenuString(MenuItems, MenuItemId);
            else
                html = GetMultilevelMenuString(MenuItems, MenuItemId, LiClasses, UlClasses, AnchorClasses);
            //remove first ul and its ending tag.
            html = html.Remove(html.Length - 5, 5).Remove(0, html.IndexOf(">") + 1).Replace("<ul></ul>", "");
            tb.InnerHtml.AppendHtml(html);
            return tb;
        }

        private static string GetMultilevelMenuString(List<MenuItem> MenuItems, int? MenuItemId)
        {
            MenuItemId = MenuItemId ?? 0;
            var s = $"<ul aria-labelledby='dropdownMenu{MenuItemId}' class='dropdown-menu border-0 shadow'>";
            foreach (var item in MenuItems)
            {
                if (item.ParentId == MenuItemId)
                {
                    s += $@"<li class='nav-item {(item.HasChilds ? "dropdown-submenu" : "")}'>
                                <a id='dropdownMenu{item.Id}' href='{item.Url}' data-toggle='{(item.HasChilds ? "dropdown" : "")}' aria-haspopup='{(item.HasChilds ? "false" : "")}' aria-expanded='{(item.HasChilds ? "false" : "")}' class='nav-link {(item.HasChilds ? "dropdown-toggle" : "")}'> {item.Name}</a>";
                    s += GetMultilevelMenuString(MenuItems, item.Id);
                    s += "</li>";
                }

            }
            s += "</ul>";
            return s;
        }
        private static string GetMultilevelMenuString(List<MenuItem> MenuItems, int? MenuItemId, string LiClasses, string UlClasses, string AnchorClasses)
        {
            MenuItemId = MenuItemId ?? 0;
            var s = $"<ul class='{UlClasses}'>";
            foreach (var item in MenuItems)
            {
                if (item.ParentId == MenuItemId)
                {
                    s += $@"<li class='{LiClasses}'>
                                <a class='{AnchorClasses}' href='{item.Url}'> {item.Name}</a>";
                    s += GetMultilevelMenuString(MenuItems, item.Id, LiClasses, UlClasses, AnchorClasses);
                    s += "</li>";
                }

            }
            s += "</ul>";
            return s;
        }
        public class MenuItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ParentId { get; set; }
            public string Url { get; set; }
            public bool HasChilds { get; set; }
            public int? Order { get; set; }
            public bool IsExtended { get; set; } = false;

        }

    }

    public static class HtmlToText
    {

        public static string Convert(string path)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);
            return ConvertDoc(doc);
        }

        public static string ConvertHtml(string html)
        {
            if (html == null)
                return string.Empty;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return ConvertDoc(doc);
        }

        public static string ConvertDoc(HtmlDocument doc)
        {
            using (StringWriter sw = new StringWriter())
            {
                ConvertTo(doc.DocumentNode, sw);
                sw.Flush();
                return sw.ToString();
            }
        }

        internal static void ConvertContentTo(HtmlNode node, TextWriter outText, PreceedingDomTextInfo textInfo)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText, textInfo);
            }
        }
        public static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            ConvertTo(node, outText, new PreceedingDomTextInfo(false));
        }
        internal static void ConvertTo(HtmlNode node, TextWriter outText, PreceedingDomTextInfo textInfo)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;
                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText, textInfo);
                    break;
                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                    {
                        break;
                    }
                    // get text
                    html = ((HtmlTextNode)node).Text;
                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                    {
                        break;
                    }
                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Length == 0)
                    {
                        break;
                    }
                    if (!textInfo.WritePrecedingWhiteSpace || textInfo.LastCharWasSpace)
                    {
                        html = html.TrimStart();
                        if (html.Length == 0) { break; }
                        textInfo.IsFirstTextOfDocWritten.Value = textInfo.WritePrecedingWhiteSpace = true;
                    }
                    outText.Write(HtmlEntity.DeEntitize(Regex.Replace(html.TrimEnd(), @"\s{2,}", " ")));
                    if (textInfo.LastCharWasSpace = char.IsWhiteSpace(html[html.Length - 1]))
                    {
                        outText.Write(' ');
                    }
                    break;
                case HtmlNodeType.Element:
                    string endElementString = null;
                    bool isInline;
                    bool skip = false;
                    int listIndex = 0;
                    switch (node.Name)
                    {
                        case "nav":
                            skip = true;
                            isInline = false;
                            break;
                        case "body":
                        case "section":
                        case "article":
                        case "aside":
                        case "h1":
                        case "h2":
                        case "header":
                        case "footer":
                        case "address":
                        case "main":
                        case "div":
                        case "p": // stylistic - adjust as you tend to use
                            if (textInfo.IsFirstTextOfDocWritten)
                            {
                                outText.Write("\r\n");
                            }
                            endElementString = "\r\n";
                            isInline = false;
                            break;
                        case "br":
                            outText.Write("\r\n");
                            skip = true;
                            textInfo.WritePrecedingWhiteSpace = false;
                            isInline = true;
                            break;
                        case "a":
                            if (node.Attributes.Contains("href"))
                            {
                                string href = node.Attributes["href"].Value.Trim();
                                if (node.InnerText.IndexOf(href, StringComparison.InvariantCultureIgnoreCase) == -1)
                                {
                                    endElementString = "<" + href + ">";
                                }
                            }
                            isInline = true;
                            break;
                        case "li":
                            if (textInfo.ListIndex > 0)
                            {
                                outText.Write("\r\n{0}.\t", textInfo.ListIndex++);
                            }
                            else
                            {
                                outText.Write("\r\n*\t"); //using '*' as bullet char, with tab after, but whatever you want eg "\t->", if utf-8 0x2022
                            }
                            isInline = false;
                            break;
                        case "ol":
                            listIndex = 1;
                            goto case "ul";
                        case "ul": //not handling nested lists any differently at this stage - that is getting close to rendering problems
                            endElementString = "\r\n";
                            isInline = false;
                            break;
                        case "img": //inline-block in reality
                            if (node.Attributes.Contains("alt"))
                            {
                                outText.Write('[' + node.Attributes["alt"].Value);
                                endElementString = "]";
                            }
                            if (node.Attributes.Contains("src"))
                            {
                                outText.Write('<' + node.Attributes["src"].Value + '>');
                            }
                            isInline = true;
                            break;
                        default:
                            isInline = true;
                            break;
                    }
                    if (!skip && node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText, isInline ? textInfo : new PreceedingDomTextInfo(textInfo.IsFirstTextOfDocWritten) { ListIndex = listIndex });
                    }
                    if (endElementString != null)
                    {
                        outText.Write(endElementString);
                    }
                    break;
            }
        }
    }
    internal class PreceedingDomTextInfo
    {
        public PreceedingDomTextInfo(BoolWrapper isFirstTextOfDocWritten)
        {
            IsFirstTextOfDocWritten = isFirstTextOfDocWritten;
        }
        public bool WritePrecedingWhiteSpace { get; set; }
        public bool LastCharWasSpace { get; set; }
        public readonly BoolWrapper IsFirstTextOfDocWritten;
        public int ListIndex { get; set; }
    }
    internal class BoolWrapper
    {
        public BoolWrapper() { }
        public bool Value { get; set; }
        public static implicit operator bool(BoolWrapper boolWrapper)
        {
            return boolWrapper.Value;
        }
        public static implicit operator BoolWrapper(bool boolWrapper)
        {
            return new BoolWrapper { Value = boolWrapper };
        }
    }
}
