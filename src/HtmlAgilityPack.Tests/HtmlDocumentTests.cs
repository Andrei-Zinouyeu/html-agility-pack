using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;

namespace HtmlAgilityPack.Tests
{
	[TestFixture]
	public class HtmlDocumentTests
	{
		private string _contentDirectory;



		[SetUp]
		public void Setup()
		{
			_contentDirectory = TestContext.CurrentContext.TestDirectory + "\\files\\";
		}

		private string Get_html5_utf8_DocumentPath()
		{
			return _contentDirectory + "HTML5_UTF8.html";

		}

		private string Get_html5_w_1251_DocumentPath()
		{
			return _contentDirectory + "HTML5_w_1251.html";
		}


		private HtmlDocument GetMshomeDocument()
		{
			var doc = new HtmlDocument();
			doc.Load(_contentDirectory + "mshome.htm");
			return doc;
		}

		[Test]
		public void CreateAttribute()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href");
			Assert.AreEqual("href", a.Name);
		}

		[Test]
		public void CreateAttributeWithEncodedText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href", "http://something.com\"&<>");
			Assert.AreEqual("href", a.Name);
			Assert.AreEqual("http://something.com\"&<>", a.Value);
		}

		[Test]
		public void CreateAttributeWithText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateAttribute("href", "http://something.com");
			Assert.AreEqual("href", a.Name);
			Assert.AreEqual("http://something.com", a.Value);
		}

		//[Test]
		//public void CreateComment()
		//{
		//    var doc = new HtmlDocument();
		//    var a = doc.CreateComment();
		//    Assert.AreEqual(HtmlNode.HtmlNodeTypeNameComment, a.Name);
		//    Assert.AreEqual(a.NodeType, HtmlNodeType.Comment);
		//}

		//[Test]
		//public void CreateCommentWithText()
		//{
		//    var doc = new HtmlDocument();
		//    var a = doc.CreateComment("something");
		//    Assert.AreEqual(HtmlNode.HtmlNodeTypeNameComment, a.Name);
		//    Assert.AreEqual("something", a.InnerText);
		//    Assert.AreEqual(a.NodeType, HtmlNodeType.Comment);
		//}

		[Test]
		public void CreateElement()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateElement("a");
			Assert.AreEqual("a", a.Name);
			Assert.AreEqual(a.NodeType, HtmlNodeType.Element);
		}

		//[Test]
		//public void CreateTextNode()
		//{
		//    var doc = new HtmlDocument();
		//    var a = doc.CreateTextNode();
		//    Assert.AreEqual(HtmlNode.HtmlNodeTypeNameText, a.Name);
		//    Assert.AreEqual(a.NodeType, HtmlNodeType.Text);
		//}

		[Test]
		public void CreateTextNodeWithText()
		{
			var doc = new HtmlDocument();
			var a = doc.CreateTextNode("something");
			Assert.AreEqual("something", a.InnerText);
			Assert.AreEqual(a.NodeType, HtmlNodeType.Text);
		}

		[Test]
		public void HtmlEncode()
		{
			var result = HtmlDocument.HtmlEncode("http://something.com\"&<>");
			Assert.AreEqual("http://something.com&quot;&amp;&lt;&gt;", result);
		}

		[Test]
		public void EncDetection_UTF8()
		{
			{
				var docPath = Get_html5_utf8_DocumentPath();

				var doc = new HtmlDocument();
                doc.UseHtmlEncodingForStream = true;
                Encoding enc = null;
                using (StreamReader sr = new StreamReader(docPath))
                {
                    enc = doc.DetectEncoding(sr.BaseStream, true);

                }

                Assert.AreEqual(65001, enc.CodePage);

				doc.Load(docPath, enc);
				doc.Save(_contentDirectory + "utf8.html");
			}

			var doc1 = new HtmlDocument();
			var enc1 = doc1.DetectEncoding(_contentDirectory + "utf8.html");
			Assert.AreEqual(65001, enc1.CodePage);

		}


		[Test]
		public void EncDetection_w_1251()
		{
			var docPath = Get_html5_w_1251_DocumentPath();
			var doc = new HtmlDocument();
            doc.UseHtmlEncodingForStream = true;
            Encoding enc = null;
            using (StreamReader sr = new StreamReader(docPath))
            {
                enc = doc.DetectEncoding(sr.BaseStream, true);

            }
            Assert.AreEqual(1251, enc.CodePage);

			doc.Load(docPath, enc);
			doc.Save(_contentDirectory + "w-1251.html");

			var doc1 = new HtmlDocument();
			var enc1 = doc.DetectEncoding(_contentDirectory + "w-1251.html");
			Assert.AreEqual(1251, enc1.CodePage);

		}


		[Test]
		public void TestParse()
		{
			var doc = GetMshomeDocument();
			Assert.IsTrue(doc.DocumentNode.DescendantNodes().Count() > 0);
		}

		[Test]
		public void TestParseSaveParse()
		{
			var doc = GetMshomeDocument();
			var doc1desc =
				doc.DocumentNode.DescendantNodes().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();
			doc.Save(_contentDirectory + "testsaveparse.html");

			var doc2 = new HtmlDocument();
			doc2.Load(_contentDirectory + "testsaveparse.html");
			var doc2desc =
				doc2.DocumentNode.DescendantNodes().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();
			Assert.AreEqual(doc1desc.Count, doc2desc.Count);
			//for(var i=0; i< doc1desc.Count;i++)
			//{
			//    try
			//    {
			//        Assert.AreEqual(doc1desc[i].Name, doc2desc[i].Name);
			//    }catch(Exception e)
			//    {
			//        throw;
			//    }
			//}
		}

		[Test]
		public void TestRemoveUpdatesPreviousSibling()
		{
			var doc = GetMshomeDocument();
			var docDesc = doc.DocumentNode.DescendantNodes().ToList();
			var toRemove = docDesc[1200];
			var toRemovePrevSibling = toRemove.PreviousSibling;
			var toRemoveNextSibling = toRemove.NextSibling;
			toRemove.Remove();
			Assert.AreSame(toRemovePrevSibling, toRemoveNextSibling.PreviousSibling);
		}

		[Test]
		public void TestReplaceUpdatesSiblings()
		{
			var doc = GetMshomeDocument();
			var docDesc = doc.DocumentNode.DescendantNodes().ToList();
			var toReplace = docDesc[1200];
			var toReplacePrevSibling = toReplace.PreviousSibling;
			var toReplaceNextSibling = toReplace.NextSibling;
			var newNode = doc.CreateElement("tr");
			toReplace.ParentNode.ReplaceChild(newNode, toReplace);
			Assert.AreSame(toReplacePrevSibling, newNode.PreviousSibling);
			Assert.AreSame(toReplaceNextSibling, newNode.NextSibling);
		}

		[Test]
		public void TestInsertUpdateSiblings()
		{
			var doc = GetMshomeDocument();
			var newNode = doc.CreateElement("td");
			var toReplace = doc.DocumentNode.ChildNodes[2];
			var toReplacePrevSibling = toReplace.PreviousSibling;
			var toReplaceNextSibling = toReplace.NextSibling;
			doc.DocumentNode.ChildNodes.Insert(2, newNode);
			Assert.AreSame(newNode.NextSibling, toReplace);
			Assert.AreSame(newNode.PreviousSibling, toReplacePrevSibling);
			Assert.AreSame(toReplaceNextSibling, toReplace.NextSibling);
		}
	}
}