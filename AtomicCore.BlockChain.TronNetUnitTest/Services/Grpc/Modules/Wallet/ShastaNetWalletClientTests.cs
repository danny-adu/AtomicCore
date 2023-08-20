using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using Grpc.Core;
using Google.Protobuf;
using System;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class ShastaNetWalletClientTests
    {
        #region Variables

        private readonly TronTestRecord _record;
        private readonly ITronNetWalletClient _wallet;
        readonly Wallet.WalletClient _cli;

        #endregion

        #region Constructor

        public ShastaNetWalletClientTests()
        {
            _record = TronTestServiceExtension.GetTestRecord();
            _wallet = _record.TronClient.GetWallet();
            _cli = _wallet.GetProtocol();
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// 获取测试网地址余额 # 注意有一个方法无法获取到TRX值
        /// </summary>
        [TestMethod()]
        public void GetAccountWithBalance()
        {
            string ownerAddress = "TSTPZsePkzsHxfDrkTCJhMv5bsHwWbkFHu";

            Block block = _cli.GetNowBlock(new EmptyMessage(), headers: _wallet.GetHeaders());
            string blockHash = block.GetBlockHash();
            byte[] blockHashBytes = blockHash.HexToByteArray();

            // get trx(这个可以获取到TRX)
            var account = _cli.GetAccount(new Account()
            {
                Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(ownerAddress))
            }, headers: _wallet.GetHeaders());
            var trx_sun = account.Balance;
            if (trx_sun < 0L)
                Assert.Fail();

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

        #endregion

        #region Modify Account Permission

        private const string priv_owner = "fb50b029d8c6cdd5db6bd1ec65b077c4a2d5329056f7a576250f9c6b97348dbc";
        private const string priv_active1 = "9e73bd4dbc1f89ee6f356522783178f5fc430380defeaf2e3d76b767c47f1db6";

        private const string addr_owner = "TSTPZsePkzsHxfDrkTCJhMv5bsHwWbkFHu";
        private const string addr_active1 = "TYKwvaqsE8raY2PUj5c5FiXx7qNKgwUbHA";
        private const string addr_receive = "TTgyNTGuic96nR8F2NL1TubBc3LxJL5SrY";

        /// <summary>
        /// 修改OWNER的权限
        /// </summary>
        [TestMethod()]
        public void ChangeOwnerPermissionToActive1()
        {
            // active pow hex
            string active_pow_hex = "0x7fff1fc0033e0300000000000000000000000000000000000000000000000000";
            var active_pow_bys = active_pow_hex.HexToByteArray();
            var bys = ByteString.CopyFrom(active_pow_bys);

            // 获取最新区块高度
            var newestBlock = _cli.GetNowBlock2(new EmptyMessage(), headers: _wallet.GetHeaders());

            // 开始构造交易报文
            var transaction = new Transaction();

            // 设置时间戳
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();
            var bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);
            var refBlockNum = bb.ToArray();

            // 修改owner权限设置 -> 将owner地址所有权交给active1
            var owner_permission = new Permission()
            {
                Id = 0,
                Type = Permission.Types.PermissionType.Owner,
                PermissionName = "owner",
                Threshold = 1
            };
            owner_permission.Keys.Add(new Key()
            {
                Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(addr_active1)),
                Weight = 1
            });

            // 修改active权限设置 -> 将owner地址所有权交给active1
            var active_permission = new Permission()
            {
                Id = 2,
                Type = Permission.Types.PermissionType.Active,
                PermissionName = "active",
                Threshold = 1,
                Operations = bys
            };
            active_permission.Keys.Add(new Key()
            {
                Address = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(addr_active1)),
                Weight = 1
            });

            // 赋值Owner对象
            var accountPermissionUpdateContract = new AccountPermissionUpdateContract()
            {
                OwnerAddress = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(addr_owner)),
                Owner = owner_permission
            };
            accountPermissionUpdateContract.Actives.Add(active_permission);

            // 构造合约信息
            var contract = new Transaction.Types.Contract();
            contract.Type = Transaction.Types.Contract.Types.ContractType.AccountPermissionUpdateContract;
            contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(accountPermissionUpdateContract);
            transaction.RawData = new Transaction.Types.raw
            {
                Timestamp = DateTime.Now.Ticks,
                Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000,
                RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8)),
                RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2))
            };
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.FeeLimit = TronNetUntils.TRXToSun(100);

            // 使用当前所有权的地址私钥进行签名
            var ecKey = new ECKey(priv_owner.HexToByteArray(), true);
            var transactionSigned = Transaction.Parser.ParseFrom(transaction.ToByteArray());
            var rawdata = transactionSigned.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();
            transactionSigned.Signature.Add(ByteString.CopyFrom(sign));

            // 广播信息
            var result = _cli.BroadcastTransaction(transactionSigned);

            Assert.IsTrue(result.Result);
        }

        /// <summary>
        /// 尝试使用修改权限后的地址进行转账
        /// </summary>
        [TestMethod()]
        public void OwnerTransferFail()
        {
            // 获取最新区块高度
            var newestBlock = _cli.GetNowBlock2(new EmptyMessage(), headers: _wallet.GetHeaders());

            // 开始构造交易报文
            var transaction = new Transaction();

            // 设置时间戳
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();
            var bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);
            var refBlockNum = bb.ToArray();

            // 构造合约信息
            var contract = new Transaction.Types.Contract();
            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(new TransferContract
            {
                OwnerAddress = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(addr_owner)),
                ToAddress = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(addr_receive)),
                Amount = TronNetUntils.TRXToSun(1)
            });
            transaction.RawData = new Transaction.Types.raw
            {
                Timestamp = DateTime.Now.Ticks,
                Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000,
                RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8)),
                RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2))
            };
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.FeeLimit = TronNetUntils.TRXToSun(15);

            // 使用当前所有权的地址私钥进行签名
            var ecKey = new ECKey(priv_owner.HexToByteArray(), true);
            var transactionSigned = Transaction.Parser.ParseFrom(transaction.ToByteArray());
            var rawdata = transactionSigned.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();
            transactionSigned.Signature.Add(ByteString.CopyFrom(sign));

            // 广播信息
            var result = _cli.BroadcastTransaction(transactionSigned);
            string error = result.Message.ToStringUtf8();
            Assert.IsTrue(result.Result, error);
        }

        /// <summary>
        /// 使用授权地址进行转账
        /// </summary>
        [TestMethod()]
        public void ActiveTransferTest()
        {
            // 获取最新区块高度
            var newestBlock = _cli.GetNowBlock2(new EmptyMessage(), headers: _wallet.GetHeaders());

            // 开始构造交易报文
            var transaction = new Transaction();

            // 设置时间戳
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();
            var bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);
            var refBlockNum = bb.ToArray();

            // 构造合约信息
            var contract = new Transaction.Types.Contract();
            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(new TransferContract
            {
                OwnerAddress = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(addr_owner)),
                ToAddress = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(addr_receive)),
                Amount = TronNetUntils.TRXToSun(1)
            });
            transaction.RawData = new Transaction.Types.raw
            {
                Timestamp = DateTime.Now.Ticks,
                Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000,
                RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8)),
                RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2))
            };
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.FeeLimit = TronNetUntils.TRXToSun(15);

            // 使用当前所有权的地址私钥进行签名
            var ecKey = new ECKey(priv_active1.HexToByteArray(), true);
            var transactionSigned = Transaction.Parser.ParseFrom(transaction.ToByteArray());
            var rawdata = transactionSigned.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();
            transactionSigned.Signature.Add(ByteString.CopyFrom(sign));

            // 广播信息
            var result = _cli.BroadcastTransaction(transactionSigned);
            string error = result.Message.ToStringUtf8();
            Assert.IsTrue(result.Result, error);
        }

        #endregion
    }
}
