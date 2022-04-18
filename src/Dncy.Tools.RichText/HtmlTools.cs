using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if NETCOREAPP || (NET46_OR_GREATER&&!NET462)
using AngleSharp;
using AngleSharp.Dom;
using Ganss.XSS;
#endif

#if NET462
using HtmlAgilityPack;
#endif

namespace Dncy.Tools.RichText
{
    public static partial class HtmlTools
    {

#if NETCOREAPP || (NET46_OR_GREATER&&!NET462)

        private static readonly HtmlSanitizer Sanitizer = new HtmlSanitizer();

        static HtmlTools()
        {
            Sanitizer.AllowedAttributes.Remove("id");
            Sanitizer.AllowedAttributes.Remove("alt");
            Sanitizer.AllowedCssProperties.Remove("font-family");
            Sanitizer.AllowedCssProperties.Remove("background-color");
            Sanitizer.KeepChildNodes = true;
            Sanitizer.AllowedTags.Remove("input");
            Sanitizer.AllowedTags.Remove("button");
            Sanitizer.AllowedTags.Remove("iframe");
            Sanitizer.AllowedTags.Remove("frame");
            Sanitizer.AllowedTags.Remove("textarea");
            Sanitizer.AllowedTags.Remove("select");
            Sanitizer.AllowedTags.Remove("form");
            Sanitizer.AllowedAttributes.Add("src");
            Sanitizer.AllowedAttributes.Add("class");
            Sanitizer.AllowedAttributes.Add("style");
        }



        /// <summary>
        /// 标准的防止html的xss净化器
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlStandardXssFilter(this string html)
        {
            return Sanitizer.Sanitize(html);
        }




        /// <summary>
        /// 自定义的防止html的xss净化器
        /// </summary>
        /// <param name="html">源html</param>
        /// <param name="labels">需要移除的标签集合</param>
        /// <param name="attributes">需要移除的属性集合</param>
        /// <param name="styles">需要移除的样式集合</param>
        /// <returns></returns>
        public static string HtmlCustomXssFilter(this string html, string[]? labels = null, string[]? attributes = null, string[]? styles = null)
        {
            if (labels != null)
            {
                foreach (string label in labels)
                {
                    Sanitizer.AllowedTags.Remove(label);
                }
            }

            if (attributes != null)
            {
                foreach (string attr in attributes)
                {
                    Sanitizer.AllowedAttributes.Remove(attr);
                }
            }

            if (styles != null)
            {
                foreach (string p in styles)
                {
                    Sanitizer.AllowedCssProperties.Remove(p);
                }
            }

            Sanitizer.KeepChildNodes = true;
            return Sanitizer.Sanitize(html);
        }



        /// <summary>
        /// 去除html标签后并截取字符串
        /// </summary>
        /// <param name="html">源html</param>
        /// <param name="length">截取长度, 小于等于0将返回全部</param>
        /// <returns></returns>
        public static string RemoveHtmlTag(this string html, int length = 0)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var doc = context.OpenAsync(req => req.Content(html)).Result;
            var strText = doc.Body?.TextContent??"";
            if (length > 0 && strText.Length > length)
            {
                return strText.Substring(0, length);
            }

            return strText;
        }



        /// <summary>
        /// 替换html的img路径为绝对路径
        /// </summary>
        /// <param name="html"></param>
        /// <param name="imgDest"></param>
        /// <returns></returns>
        public static string ReplaceHtmlImgSource(this string html, string imgDest) => html.Replace("<img src=\"", "<img src=\"" + imgDest + "/");


        /// <summary>
        /// 将src的绝对路径换成相对路径
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConvertImgSrcToRelativePath(this string s)
        {
            return Regex.Replace(s, @"<img src=""(http:\/\/.+?)/", @"<img src=""/");
        }

        /// <summary>
        /// 匹配html的所有img标签集合
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlCollection<IElement>? MatchImgTags(this string html)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var doc = context.OpenAsync(req => req.Content(html)).Result;
            return doc?.Body?.GetElementsByTagName("img");
        }

        /// <summary>
        /// 匹配html的所有img标签的src集合
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IEnumerable<string?>? MatchImgSrcs(this string html)
        {
            return MatchImgTags(html)?.Where(n => n.HasAttribute("src")).Select(n => n.GetAttribute("src"));
        }


        /// <summary>
        /// 匹配html的所有超链接标签集合
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlCollection<IElement>? MatchHyperLinkTags(this string html)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var doc = context.OpenAsync(req => req.Content(html)).Result;
            return doc?.Body?.GetElementsByTagName("a");
        }
        /// <summary>
        /// 匹配html的所有超链接标签的href集合
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IEnumerable<string?>? MatchHyperLinkHrefs(this string html)
        {
            return MatchHyperLinkTags(html)?.Where(n => n.HasAttribute("href")).Select(n => n.GetAttribute("href"));
        }


        /// <summary>
        /// 获取html中第一个img标签的src
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string? MatchFirstImgSrc(this string html)
        {
            return MatchImgSrcs(html)?.FirstOrDefault();
        }

        /// <summary>
        /// 随机获取html代码中的img标签的src属性
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string? MatchRandomImgSrc(this string html)
        {
            var srcs = MatchImgSrcs(html)?.ToList();
            var rnd = new Random();
            return srcs?.Count > 0 ? srcs[rnd.Next(srcs.Count)] : default;
        }

        /// <summary>
        /// 按顺序优先获取html代码中的img标签的src属性
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string? MatchSeqRandomImgSrc(this string html)
        {
            var srcs = MatchImgSrcs(html)?.ToList();
            return srcs?.Count > 0 ? srcs.Select((s, i) => new WeightedItem<string?>(s, srcs.Count - i)).WeightedItem() : default;
        }


        /// <summary>
        /// 替换回车换行符为html换行符
        /// </summary>
        /// <param name="str">html</param>
        public static string StrFormat(this string str)
        {
            return str.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }


#endif

#if NET462
        /// <summary>
        /// html img标签解析
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<string> MatchImgSrcs(this string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var imagesNode = doc.DocumentNode.SelectNodes("//img");
            var images = new List<string>();
            if (imagesNode != null)
            {
                foreach (var image in imagesNode)
                {
                    images.Add(image.GetAttributeValue("src", string.Empty));
                }
            }
            return images;
        }


        /// <summary>
        /// html img标签解析
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<string> MatchHyperLinkHrefs(this string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//a");
            var hrefs = new List<string>();
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    hrefs.Add(item.GetAttributeValue("href", string.Empty));
                }
            }
            return hrefs;
        }

        /// <summary>
        /// html video src解析
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<string> MatchVideoSrcs(this string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var videos = new List<string>();
            var videosNode = doc.DocumentNode.SelectNodes("//video");
            if (videosNode != null)
            {
                foreach (var video in videosNode)
                {
                    videos.Add(video.GetAttributeValue("src", string.Empty));
                }
            }

            return videos;
        }

        /// <summary>
        /// html audio src解析
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<string> HtmlAudioSrcs(this string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var audios = new List<string>();
            var nodes = doc.DocumentNode.SelectNodes("//audio");
            if (nodes != null)
            {
                foreach (var video in nodes)
                {
                    audios.Add(video.GetAttributeValue("src", string.Empty));
                }
            }

            return audios;
        }


        /// <summary>
        /// html 标签样式中background background-url
        /// </summary>
        /// <param name="doc"></param>
        /// <remarks>暂时支持一张url的情况</remarks>
        /// <returns></returns>
        public static List<string> MatchTagStyleImageSrcs(this string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var images = new List<string>();
            var r = new Regex(@"(background|background\-image)\s*:\s*url\s*\((?<src>\s*(('|)?)\s*([^\s]+)\2\s*)\)",
                RegexOptions.IgnoreCase);
            var nodes = doc.DocumentNode.Descendants().Where(n => n.Attributes.Any(x => x.Name == "style"));
            if (nodes != null && nodes.Any())
            {
                foreach (var n in nodes)
                {
                    var input = n.Attributes["style"].Value;
                    foreach (Match d in r.Matches(input))
                    {
                        images.Add(d.Groups["src"].Value);
                    }
                }
            }

            return images;
        }
#endif

    }
}

