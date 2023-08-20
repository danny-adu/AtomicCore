using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    /// <summary>
    /// 交易签名单元测试
    /// </summary>
    [TestClass()]
    public class TransactionSignTest
    {
        private readonly TronTestRecord _record;
        private readonly Wallet.WalletClient _wallet;

        public TransactionSignTest()
        {
            _record = TronTestServiceExtension.GetTestRecord();
            _wallet = _record.TronClient.GetWallet().GetProtocol();
        }

        /// <summary>
        /// 测试交易离线签名,并广播在测试网络
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task TestTransactionSignAsync()
        {
            var privateStr = TronTestAccountCollection.TestMain.PirvateKey;
            var tronKey = new TronNetECKey(privateStr, _record.Options.Value.Network);
            var from = tronKey.GetPublicAddress();
            var to = TronTestAccountCollection.TestA.Address;
            var amount = 1_000_000L; // 1 TRX, api only receive trx in Sun, and 1 trx = 1000000 Sun

            //创建交易报文
            var transactionExtension = await CreateTransactionAsync(from, to, amount);
            Assert.IsTrue(transactionExtension.Result.Result);

            //原始交易报文（未签名）
            var transaction = transactionExtension.Transaction;

            string json_txt_1 = transactionExtension.Transaction.ToString();

            //交易GRPC签名（远程调用）
            var transactionSignExtention = await _wallet.GetTransactionSign2Async(new TransactionSign
            {
                PrivateKey = ByteString.CopyFrom(privateStr.HexToByteArray()),
                Transaction = transaction
            });
            Assert.IsNotNull(transactionSignExtention);
            Assert.IsTrue(transactionSignExtention.Result.Result);

            string json_txt_2 = transactionSignExtention.Transaction.ToString();

            //已经签名交易报文（GRPC签名的结果）
            var transactionSigned = transactionSignExtention.Transaction;
            var transaction5 = transactionSigned.ToByteArray();

            //使发起人私钥对原始交易报文进行签名（本地离线签名）
            var transactionBytes = transaction.ToByteArray();
            var transaction4 = SignTransaction2Byte(transactionBytes, privateStr.HexToByteArray());

            //对比两次签名结果
            Assert.IsTrue(transaction4.ToHex().Equals(transaction5.ToHex(), StringComparison.OrdinalIgnoreCase));

            //将本次交易发布广播出去
            var result = await _wallet.BroadcastTransactionAsync(transactionSigned);

            Assert.IsTrue(result.Result);
        }

        private async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount)
        {

            var fromAddress = Base58Encoder.DecodeFromBase58Check(from);
            var toAddress = Base58Encoder.DecodeFromBase58Check(to);

            var transferContract = new TransferContract
            {
                OwnerAddress = ByteString.CopyFrom(fromAddress),
                ToAddress = ByteString.CopyFrom(toAddress),
                Amount = amount
            };

            var transaction = new Transaction();

            var contract = new Transaction.Types.Contract();

            try
            {
                contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(transferContract);
            }
            catch (Exception)
            {
                return new TransactionExtention
                {
                    Result = new Return { Result = false, Code = Return.Types.response_code.OtherError },
                };
            }
            var newestBlock = await _wallet.GetNowBlock2Async(new EmptyMessage());

            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            transaction.RawData = new Transaction.Types.raw();
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.Timestamp = DateTime.Now.Ticks;
            transaction.RawData.Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000;
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();

            var bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);

            var refBlockNum = bb.ToArray();

            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8));
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2));

            var transactionExtension = new TransactionExtention
            {
                Transaction = transaction,
                Txid = ByteString.CopyFromUtf8(transaction.GetTxid()),
                Result = new Return { Result = true, Code = Return.Types.response_code.Success },
            };
            return transactionExtension;
        }


        private static byte[] SignTransaction2Byte(byte[] transaction, byte[] privateKey)
        {
            var ecKey = new ECKey(privateKey, true);
            var transaction1 = Transaction.Parser.ParseFrom(transaction);
            var rawdata = transaction1.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();

            transaction1.Signature.Add(ByteString.CopyFrom(sign));

            return transaction1.ToByteArray();
        }
    }
}
