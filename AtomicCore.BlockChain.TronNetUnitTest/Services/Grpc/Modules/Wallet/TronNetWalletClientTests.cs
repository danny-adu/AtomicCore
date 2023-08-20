using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using Grpc.Core;
using Google.Protobuf;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class TronNetWalletClientTests
    {
        #region Variables

        private readonly TronTestRecord _record;
        private readonly ITronNetWalletClient _wallet;
        readonly Wallet.WalletClient _cli;

        #endregion

        #region Constructor

        public TronNetWalletClientTests()
        {
            _record = TronTestServiceExtension.GetMainRecord();
            _wallet = _record.TronClient.GetWallet();
            _cli = _wallet.GetProtocol();
        }

        #endregion

        [TestMethod()]
        public void GetHeadersTest()
        {
            Metadata headMetadata = _wallet.GetHeaders();

            Assert.IsTrue(headMetadata.Count > 0);
        }

        [TestMethod()]
        public void GetLastBlockTest()
        {
            Block block = _cli.GetNowBlock(new EmptyMessage());

            Assert.IsTrue(block.BlockHeader.RawData.Number > 0);
        }

        [TestMethod()]
        public void GetBlockByNumberTest()
        {
            Block block = _cli.GetBlockByNum(new NumberMessage()
            {
                Num = 38982180
            }, headers: _wallet.GetHeaders());

            string blockHash = block.GetBlockHash();

            Assert.IsTrue(!string.IsNullOrEmpty(blockHash));
            Assert.IsTrue(block.BlockHeader.RawData.Number > 0);
        }

        [TestMethod()]
        public void GetAccountBalanceTest1()
        {
            string ownerAddress = "TK7XWSuRi5PxYDUQ53L43baio7ZBWukcGm";

            Block block = _cli.GetNowBlock(new EmptyMessage(), headers: _wallet.GetHeaders());
            string blockHash = block.GetBlockHash();
            byte[] blockHashBytes = blockHash.HexToByteArray();

            // get trx(这个可以获取到TRX)
            var account = _cli.GetAccount(new Account()
            {
                Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(ownerAddress))
            }, headers: _wallet.GetHeaders());
            var trx_sun = account.Balance;

            //create tx（自己编译的节点这个无法获取到TRX）
            AccountBalanceResponse balance = _cli.GetAccountBalance(new AccountBalanceRequest()
            {
                AccountIdentifier = new AccountIdentifier()
                {
                    Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(ownerAddress))
                },
                BlockIdentifier = new BlockBalanceTrace.Types.BlockIdentifier()
                {
                    Hash = ByteString.CopyFrom(blockHashBytes),
                    Number = block.BlockHeader.RawData.Number
                }
            }, headers: _wallet.GetHeaders());

            Assert.IsTrue(balance.Balance >= 0);
        }

        [TestMethod()]
        public void GetAccountBalanceTest2()
        {
            string ownerAddress = "TK7XWSuRi5PxYDUQ53L43baio7ZBWukcGm";

            BlockExtention block = _cli.GetNowBlock2(new EmptyMessage(), headers: _wallet.GetHeaders());

            //create tx
            AccountBalanceResponse balance = _cli.GetAccountBalance(new AccountBalanceRequest()
            {
                AccountIdentifier = new AccountIdentifier()
                {
                    Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(ownerAddress))
                },
                BlockIdentifier = new BlockBalanceTrace.Types.BlockIdentifier()
                {
                    Hash = block.Blockid,
                    Number = block.BlockHeader.RawData.Number
                }
            }, headers: _wallet.GetHeaders());

            Assert.IsTrue(balance.Balance >= 0);
        }

        [TestMethod()]
        public void GetAccountResourceTest()
        {
            var resource = _cli.GetAccountResource(new Account()
            {
                Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check("TPqgW6WJurCtjCvrvy6PLY46sKbyiwSFg7"))
            }, headers: _wallet.GetHeaders());

            Assert.IsTrue(resource.AssetNetLimit.Count >= 0);
        }

        [TestMethod()]
        public void GetTransactionByIdTest()
        {
            string txid = "5c73f7e846928e44235d5188a56b028b3e59cfd5ac75f9d625783d254db957e2";

            //获取交易信息
            Transaction txInfo = _cli.GetTransactionById(new BytesMessage()
            {
                Value = ByteString.CopyFrom(txid.HexToByteArray())
            }, headers: _wallet.GetHeaders());
            int txSize = txInfo.CalculateSize();

            //拓展交易数据
            var txExt = new TransactionExtention()
            {
                Result = new Return()
                {
                    Code = Return.Types.response_code.Success,
                    Message = ByteString.CopyFrom("SUCCESS", Encoding.UTF8),
                    Result = true
                },
                Transaction = txInfo,
                Txid = ByteString.CopyFrom(txInfo.GetTxid().HexToByteArray())
            };
            txExt.ConstantResult.Add(ByteString.CopyFrom("SUCCESS", Encoding.UTF8));
            int extSize = txExt.CalculateSize();

            string tx_id = txInfo.GetTxid();
            Assert.IsTrue(!string.IsNullOrEmpty(tx_id));

            //获取交易票据
            TransactionInfo txReceipt = _cli.GetTransactionInfoById(new BytesMessage()
            {
                Value = ByteString.CopyFrom(txid.HexToByteArray())
            }, headers: _wallet.GetHeaders());
            if (null != txReceipt && txReceipt.BlockNumber > 0)
            {
                var tx_fee = txReceipt.Fee;
            }

            //解析RawData数据
            long bh = txInfo.RawData.RefBlockNum;
            string ref_block_hash = txInfo.RawData.RefBlockHash.ToStringUtf8();
            long expTs = txInfo.RawData.Expiration;
            long timestamp = txInfo.RawData.Timestamp;
            var ref_block_byte = txInfo.RawData.RefBlockBytes.ToStringUtf8();

            //tx contract info
            Transaction.Types.Contract txContract = txInfo.RawData.Contract.FirstOrDefault();
            string contract_name = System.Text.Encoding.UTF8.GetString(txContract.ContractName.ToByteArray());
            string contract_type = txContract.Type.ToString();

            //解析Parameter.Value(谷歌协议数据格式)
            var contractTransfer = TransferContract.Parser.ParseFrom(txContract.Parameter.Value);
            string ownerAddress = contractTransfer.OwnerAddress.ToStringUtf8();
            string toAddress = contractTransfer.ToAddress.ToStringUtf8();
            long amount = contractTransfer.Amount;
        }

        [TestMethod()]
        public void GetAccountPermissionUpdateTx()
        {
            // 9e83be9742adf54df17db0ed0610df9598b73d5363a0fcbb5315af4b171db397
            string txid = "9e83be9742adf54df17db0ed0610df9598b73d5363a0fcbb5315af4b171db397";

            //获取交易信息
            Transaction txInfo = _cli.GetTransactionById(new BytesMessage()
            {
                Value = ByteString.CopyFrom(txid.HexToByteArray())
            }, headers: _wallet.GetHeaders());
            if (null == txInfo)
                Assert.Fail();

            // 解析contract
            var contract = txInfo.RawData.Contract.FirstOrDefault();
            if (null == contract)
                Assert.Fail();
            if (contract.Type != Transaction.Types.Contract.Types.ContractType.AccountPermissionUpdateContract)
                Assert.Fail();
            var permissionUpdate = AccountPermissionUpdateContract.Parser.ParseFrom(contract.Parameter.Value);

            // addressies
            string ownerAddr = permissionUpdate.OwnerAddress.GetTronAddress(TronNetwork.MainNet);
            string op_addr = permissionUpdate.Owner.Keys.First().Address.GetTronAddress(TronNetwork.MainNet);
            string ap_addr = permissionUpdate.Actives.First().Keys.First().Address.GetTronAddress(TronNetwork.MainNet);

            // operation pows
            var bytes = permissionUpdate.Actives.FirstOrDefault().Operations.ToByteArray();
            var hex = bytes.ToHex();    // 0x7fff1fc0033e0300000000000000000000000000000000000000000000000000

            Assert.IsTrue(true);
        }
    }
}