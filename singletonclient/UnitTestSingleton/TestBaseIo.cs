using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingletonClient.Implementation.Support;

namespace UnitTestSingleton
{
    [TestClass]
    public class TestBaseIo
    {
        [TestMethod]
        public void TestAccessService()
        {
            SingletonAccessService service = new SingletonAccessService();
            String text = service.HttpGet("https://github.com");
            Assert.AreEqual(text.Contains("GitHub"), true);

            text = service.HttpGet("https://github.next.com");
            Assert.AreEqual(text == null, true);

            text = service.HttpPost("https://www.yahoo.com", "{}");
            Assert.AreEqual(text.Contains("OK"), true);

            text = service.HttpPost("http://github.next.com", "{}");
            Assert.AreEqual(text == null, true);
        }
    }
}
