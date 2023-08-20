using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.Tests
{
    [TestClass()]
    public class Base64HandlerTests
    {
        [TestMethod()]
        public void ConvertToBase64Test()
        {
            string s = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Avatar = "https://www.fangjial.com/uploads/allimg/200502/16304C025-0.jpg",
                Nickname = "FakeNick",
                OpenID = "jHTZPPly3IxhD_f9zPvGAKaLMh1N4wv4",
                Pod = "jHTZPPly3IxhD_f9zPvGAKaLMh1N4wv1",
                TargetID = "jHTZPPly3IxhD_f9zPvGAKaLMh1N4wv3",
                Sex = 1,
                RandStr = "f73ad2bf8eff",
                Timestamp = 1644401167041,
            });

            string base64 = Base64Handler.ConvertToBase64(s);

            Assert.IsTrue(!string.IsNullOrEmpty(base64));
        }

        [TestMethod()]
        public void ConvertToOriginalTest()
        {
            string value = "eyJBdmF0YXIiOiJodHRwczovL3d3dy5mYW5namlhbC5jb20vdXBsb2Fkcy9hbGxpbWcvMjAwNTAyLzE2MzA0QzAyNS0wLmpwZyIsIk5pY2tuYW1lIjoiRmFrZU5pY2siLCJPcGVuSUQiOiJqSFRaUFBseTNJeGhEX2Y5elB2R0FLYUxNaDFONHd2NCIsIlBvZCI6ImpIVFpQUGx5M0l4aERfZjl6UHZHQUthTE1oMU40d3YxIiwiVGFyZ2V0SUQiOiJqSFRaUFBseTNJeGhEX2Y5elB2R0FLYUxNaDFONHd2MyIsIlNleCI6MSwiUmFuZFN0ciI6ImY3M2FkMmJmOGVmZiIsIlRpbWVzdGFtcCI6MTY0NDQwMTE2NzA0MX0";

            string orig = Base64Handler.ConvertToOriginal(value);

            Assert.IsTrue(!string.IsNullOrEmpty(orig));
        }

        [TestMethod()]
        public void IsBase64FormatTest()
        {
            string value = "eyJhdmF0YXIiOiIvYXZhdGFyL2RlZmF1bHQvdXNlcnMvMi5wbmciLCJuaWNrbmFtZSI6InRlc3Q1NjciLCJvcGVuSWQiOiJJUDNzUDRzV0FNYzhjX0JuXzd2Um0zTU9VeHVRSFlSSSIsInBvZCI6IldIMnpBVExiM1hMZHpiR29YTjVvS1UyMGpMMVRVVDR4IiwicmFuZHN0ciI6InNaZElOZ1JtVE4yMSIsInNleCI6MSwic2lnbiI6ImY1YmRiOTIyN2JiYjQyYzUxMWVkMjYwOWE2NjUyYjczIiwidGltZSI6MTY0NjI5MjUxMzE5NX0";

            var flag = Base64Handler.IsBase64Format(value);

            Assert.IsTrue(flag);
        }
    }
}