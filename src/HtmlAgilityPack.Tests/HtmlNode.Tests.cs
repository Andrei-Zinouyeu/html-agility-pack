using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace HtmlAgilityPack.Tests
{
	[TestFixture]
	public class HtmlNode
	{
		[Test(Description="Attributes should maintain their original character casing if OptionOutputOriginalCase is true")]
		public void EnsureAttributeOriginalCaseIsPreserved()
		{
			var html = "<html><body><div AttributeIsThis=\"val\"></div></body></html>";
			var doc = new HtmlDocument
				          {
					          OptionOutputOriginalCase = true
				          };
			doc.LoadHtml(html);
			var div = doc.DocumentNode.Descendants("div").FirstOrDefault();
			var writer = new StringWriter();
			div.WriteAttributes(writer, false);
			var result = writer.GetStringBuilder().ToString();
			Assert.AreEqual(" AttributeIsThis=\"val\"", result);
		}

        [Test]
        public void OptionMaxNestedChildNodes_NotSet_IsNotEnforced()
        {
            var html = "<html><body><div>1<div>2</div>3</div></body></html>";
            var doc = new HtmlDocument();
            
            doc.LoadHtml(html);

            Assert.IsNotNull(doc);
            Assert.AreEqual(html, doc.Text);
        }

        [Test]
        public void OptionMaxNestedChildNodes_SetToZero_IsNotEnforced()
        {
            var html = "<html><body><div>1<div>2</div>3</div></body></html>";
            var doc = new HtmlDocument();
            doc.OptionMaxNestedChildNodes = 0;

            doc.LoadHtml(html);

            Assert.IsNotNull(doc);
            Assert.AreEqual(html, doc.Text);
        }

        [Test]
        public void OptionMaxNestedChildNodes_WithinMax_NoException()
        {
            var html = "<div id='1'><div id='2'><div id='3'><div id='4'><div id='5'><div id='6'><div id='7'><div id='8'></div></div></div></div></div></div></div></div>";
            var doc = new HtmlDocument();
            doc.OptionMaxNestedChildNodes = 8;

            doc.LoadHtml(html);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Document has more than 7 nested tags. This is likely due to the page not closing tags properly.")]
        public void OptionMaxNestedChildNodes_AbotMax()
        {
            var html = "<div id='1'><div id='2'><div id='3'><div id='4'><div id='5'><div id='6'><div id='7'><div id='8'></div></div></div></div></div></div></div></div>";
            var doc = new HtmlDocument();
            doc.OptionMaxNestedChildNodes = 7;

            doc.LoadHtml(html);
        }
	}
}
