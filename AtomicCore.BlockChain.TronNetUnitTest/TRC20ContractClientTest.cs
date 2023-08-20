using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class TRC20ContractClientTest
    {
        private TronTestRecord _record;
        private ITronNetWalletClient _wallet;
        private IContractClientFactory _contractClientFactory;

        public TRC20ContractClientTest()
        {
            _record = TronTestServiceExtension.GetTestRecord();
            _wallet = _record.ServiceProvider.GetService<ITronNetWalletClient>();
            _contractClientFactory = _record.ServiceProvider.GetService<IContractClientFactory>();
        }

        [TestMethod()]
        public async Task TestTransferAsync()
        {
            //Main Account
            string privateKey = TronTestAccountCollection.TestMain.PirvateKey;
            ITronNetAccount account = _wallet.GetAccount(privateKey);

            //USDT Contract Address
            string contractAddress = TronTestAccountCollection.TestNetUsdtAddress;

            //to address
            string to = TronTestAccountCollection.TestA.Address;
            decimal amount = 10M; //USDT Amount
            long feeAmount = 5L * 1000000L;

            //contract client
            IContractClient contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);

            //transfer
            string result = await contractClient.TransferAsync(contractAddress, account, to, amount, string.Empty, feeAmount);

            Assert.IsNotNull(result);
        }
    }
}
