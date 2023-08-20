using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    /// <summary>
    /// Tron交易单元测试
    /// </summary>
    [TestClass()]
    public class TransactionTest
    {
        private readonly TronTestRecord _record;
        private readonly Wallet.WalletClient _wallet;

        public TransactionTest()
        {
            _record = TronTestServiceExtension.GetTestRecord();
            _wallet = _record.TronClient.GetWallet().GetProtocol();
        }

        /// <summary>
        /// 发送交易测试
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task TestTransferAsync()
        {
            var transactionClient = _record.ServiceProvider.GetService<ITronNetTransactionClient>();
            var privateKey = TronTestAccountCollection.TestMain.PirvateKey;
            var tronKey = new TronNetECKey(privateKey, _record.Options.Value.Network);
            var from = tronKey.GetPublicAddress();
            var to = TronTestAccountCollection.TestA.Address;
            var amount = 1_000_000L; // 1 TRX, api only receive trx in Sun, and 1 trx = 1000000 Sun

            var fromAddress = Base58Encoder.DecodeFromBase58Check(from);
            var toAddress = Base58Encoder.DecodeFromBase58Check(to);
            var block = await _wallet.GetNowBlock2Async(new EmptyMessage());

            //创建RAW交易报文对象
            TransactionExtention transaction = await transactionClient.CreateTransactionAsync(from, to, amount);
            Assert.IsTrue(transaction.Result.Result);

            //离线签名
            Transaction transactionSigned = transactionClient.GetTransactionSign(transaction.Transaction, privateKey);

            //广播上链
            Return result = await transactionClient.BroadcastTransactionAsync(transactionSigned);
            Assert.IsTrue(result.Result);
        }

        /// <summary>
        /// 验证交易签名是否正确
        /// 本地离线签 = GRPC远程签
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task TestSignAsync()
        {
            var transactionClient = _record.ServiceProvider.GetService<ITronNetTransactionClient>();
            var privateKey = TronTestAccountCollection.TestMain.PirvateKey;
            var ecKey = new TronNetECKey(privateKey, _record.Options.Value.Network);
            var from = ecKey.GetPublicAddress();
            var to = TronTestAccountCollection.TestA.Address;
            var amount = 1_000_000L;

            //创建RAW交易
            var result = await transactionClient.CreateTransactionAsync(from, to, amount);
            Assert.IsTrue(result.Result.Result);

            //离线签名
            var transactionSigned = transactionClient.GetTransactionSign(result.Transaction, privateKey);

            //远程签
            var remoteTransactionSigned = await _wallet.GetTransactionSign2Async(new TransactionSign
            {
                Transaction = result.Transaction,
                PrivateKey = ByteString.CopyFrom(privateKey.HexToByteArray()),
            });
            Assert.IsTrue(remoteTransactionSigned.Result.Result);

            Assert.IsTrue(remoteTransactionSigned.Transaction.Signature[0] == transactionSigned.Signature[0]);
        }
    }
}
