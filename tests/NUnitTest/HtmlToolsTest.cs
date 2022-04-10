using System;
using System.IO;
using System.Text;

using Dncy.Tools;
using Dncy.Tools.Security;
#if NETCOREAPP || NETSTANDARD || NET46_OR_GREATER
using Dncy.Tools.RichText;
#endif

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnitTest
{
    public class HtmlToolsTest
    {
        private string _html = "";
        [SetUp]
        public void Setup()
        {
            _html = $@"<a href='http://www.baidu.com'>hello</a><br /> <img src='https://gitee.com/zyllbx/static-files/raw/master/20210319/20210319053514017.png' />
                <h1 style=\""background:url('https://gitee.com/zyllbx/static-files/raw/master/20210319/20210319053514017.png')\"">你好呀</h1>";
        }


        [Test]
        public void HtmlTools_Test()
        {
#if NETCOREAPP || NETSTANDARD || (NET46_OR_GREATER&&!NET462)

            var dd = _html.MatchImgSrcs();


            var cc = _html.MatchHyperLinkHrefs();

            
#endif

#if NET462
            var cc = _html.MatchImgSrcs();
            var tags = _html.MatchTagStyleImageSrcs();
#endif
        }



    }
}
