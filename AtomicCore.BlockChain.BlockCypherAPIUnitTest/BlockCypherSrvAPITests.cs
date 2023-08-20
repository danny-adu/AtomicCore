using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.BlockChain.BlockCypherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.BlockCypherAPI.Tests
{
    [TestClass()]
    public class BlockCypherSrvAPITests
    {
        private const string AGENT_GET = "http://agent.intoken.club/Remote/Get?url={0}";
        private const string AGENT_POST = "http://agent.intoken.club/Remote/Post?url={0}&contentType={1}";
        private IBlockCypherAPI client;

        public BlockCypherSrvAPITests()
        {
            client = new BlockCypherSrvAPI(AGENT_GET, AGENT_POST);
        }

        [TestMethod()]
        public void ChainEndpointTest()
        {
            var result = client.ChainEndpoint(BlockCypherNetwork.BtcMainnet);

            Assert.IsTrue(result.Height > 0);
        }
    }
}