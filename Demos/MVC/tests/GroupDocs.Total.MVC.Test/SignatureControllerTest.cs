using GroupDocs.Total.MVC.Controllers;
using NUnit.Framework;
using System.Web.Routing;
using MvcContrib.TestHelper;
using Huygens;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using GroupDocs.Total.MVC.Products.Signature.Entity.Web;

namespace GroupDocs.Total.MVC.Test
{
    [TestFixture]
    public class SignatureControllerTest
    {
        
        [SetUp]
        public void TestInitialize()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        [TearDown]
        public void TearDown()
        {
            RouteTable.Routes.Clear();
        }

        [Test]
        public void ViewStatusTest()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../src";            
            using (var server = new DirectServer(path))
            {
                var request = new SerialisableRequest
                {
                    Method = "GET",
                    RequestUri = "/signature",
                    Content = null
                };

                var result = server.DirectCall(request);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public void ViewMapControllerTest()
        {
            "~/signature".Route().ShouldMapTo<SignatureController>(x => x.Index());
        }

        [Test]
        public void FileTreeStatusCodeTest()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../src";
            using (var server = new DirectServer(path))
            {

                SignaturePostedDataEntity requestData = new SignaturePostedDataEntity();
                requestData.path = "";

                var request = new SerialisableRequest
                {
                    Method = "POST",
                    RequestUri = "/signature/loadfiletree",
                    Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestData)),
                    Headers = new Dictionary<string, string>{
                        { "Content-Type", "application/json"},
                        { "Content-Length", JsonConvert.SerializeObject(requestData).Length.ToString()}
                    }
                };

                var result = server.DirectCall(request);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }        
    }
}
