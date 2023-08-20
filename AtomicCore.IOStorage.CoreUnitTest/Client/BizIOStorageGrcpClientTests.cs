using AtomicCore.IOStorage.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AtomicCore.IOStorage.Core.Tests
{
    [TestClass()]
    public class BizIOStorageGrcpClientTests
    {
        const string host = "13.251.38.103";
        const int http_port = 8777;
        const int grpc_port = 9003;
        static string baseUrl = $"http://{host}:{http_port}";
        const string apiKey = "a6e2f27ee1f544cc889898e4397f7b07";

        static BizIOStorageGrcpClientTests()
        {
            AtomicCore.AtomicKernel.Initialize();
        }

        [TestMethod()]
        public void UploadFileTest()
        {
            string basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            string path = string.Format("{0}test.jpg", basePath);

            BizIOSingleUploadJsonResult result;
            var client = new BizIOStorageGrcpClient(host, grpc_port, baseUrl, apiKey);

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(0, SeekOrigin.Begin);

                result = client.UploadFile("Test", null, string.Empty, ".jpg", fs).Result;
            }

            Assert.IsTrue(result.Code == BizIOStateCode.Success);
        }

        [TestMethod()]
        public void UploadBase64Test()
        {
            string base64 = "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

            var buffer = AtomicCore.Base64Handler.ConvertToBuffer(base64);

            BizIOSingleUploadJsonResult result;
            var client = new BizIOStorageGrcpClient(host, grpc_port, baseUrl, apiKey);

            using (var fs = new MemoryStream(buffer))
            {
                fs.Seek(0, SeekOrigin.Begin);

                // 方式1 直接指定文件名称，文件后缀允许为空
                result = client.UploadFile("Test", null, "abcd.jpg", string.Empty, fs).Result;

                // 方式2 指定文件格式，文件名称通过文件流的MD5总计算
                //result = client.UploadFile("Test", null, string.Empty, ".jpg", fs).Result;
            }

            Assert.IsTrue(result.Code == BizIOStateCode.Success);
        }

        [TestMethod()]
        public void DownLoadFileTest()
        {
            var client = new BizIOStorageGrcpClient(host, grpc_port, baseUrl, apiKey);

            var result = client.DownLoadFile("/test/dog/test.jpg").Result;

            Assert.IsTrue(result.Code == BizIOStateCode.Success);
        }

        [TestMethod()]
        public void SnapshotFileTest()
        {
            var client = new BizIOStorageGrcpClient(host, grpc_port, baseUrl, apiKey);

            var result = client.SnapshotFile("test", null, "abc", "http://edis-zim.oss-cn-beijing.aliyuncs.com/sdk/20230128022217823-o7u3yp6d1893242230296765943.jpg?Expires=1674930138&OSSAccessKeyId=LTAI5tNZJG7Rz5icyxCpxDNg&Signature=uRQzTb1ZRnayrxo8djw2EKIg%2BXM%3D").Result;

            Assert.IsTrue(result.Code == BizIOStateCode.Success);
        }
    }
}