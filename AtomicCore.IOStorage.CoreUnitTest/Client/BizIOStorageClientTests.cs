using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AtomicCore.IOStorage.Core.Tests
{
    [TestClass()]
    public class BizIOStorageClientTests
    {
        private const string endpointUrl = "http://1.13.6.53:8777";

        static BizIOStorageClientTests()
        {
            AtomicCore.AtomicKernel.Initialize();
        }

        [TestMethod()]
        public void UploadFileTest()
        {
            string basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            string path = string.Format("{0}test.jpg", basePath);

            BizIOSingleUploadJsonResult result;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(0, SeekOrigin.Begin);

                BizIOStorageClient client = new BizIOStorageClient(endpointUrl);
                result = client.UploadFile(new BizIOUploadFileInput()
                {
                    APIKey = "a6e2f27ee1f544cc889898e4397f7b07",
                    BizFolder = "Test",
                    SubFolder = "dog",
                    FileStream = fs,
                    FileName = "test.jpg"
                });
            }

            Assert.IsTrue(null != result && result.Code == BizIOStateCode.Success);
        }

        [TestMethod()]
        public void UploadTxtTest()
        {
            string basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            string path = string.Format("{0}test.txt", basePath);

            BizIOSingleUploadJsonResult result;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(0, SeekOrigin.Begin);

                BizIOStorageClient client = new BizIOStorageClient(endpointUrl);
                result = client.UploadFile(new BizIOUploadFileInput()
                {
                    APIKey = "a6e2f27ee1f544cc889898e4397f7b07",
                    BizFolder = "Test",
                    SubFolder = "txt",
                    FileStream = fs,
                    FileName = "test.txt"
                });
            }

            Assert.IsTrue(null != result && result.Code == BizIOStateCode.Success);
        }
    }
}