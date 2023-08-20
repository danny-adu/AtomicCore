using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.BlockChain.TronNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using System.Text.RegularExpressions;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    /// <summary>
    /// https://developers.tron.network/reference#get-contract
    /// </summary>
    [TestClass()]
    public class WalletClientTests
    {
        #region Variables

        private const string blk_hash_mainnet = "0000000001f618ad6a2d6492db91395ee6cb9c1ea8c4a38c456aa3aa57b592e5";
        private const long bh_mainnet = 32905389;
        private const string txid_mainnet = "bfe65fb20e26225345ab6e20c9852ce5af411e5b18245b8a6abbeafb3582d802";

        private readonly TronTestRecord _record;
        private readonly ITronNetRest _restAPI;
        private readonly ITronNetWalletClient _wallet;
        private readonly Wallet.WalletClient _walletProtocol;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public WalletClientTests()
        {
            _record = TronTestServiceExtension.GetMainRecord();
            _restAPI = _record.TronClient.GetRestAPI();
            _wallet = _record.TronClient.GetWallet();
            _walletProtocol = _wallet.GetProtocol();
        }

        #endregion

        #region Query Network

        /// <summary>
        /// Get Current Block
        /// </summary>
        [TestMethod()]
        public void GetCurrentBlock()
        {
            BlockExtention block = _walletProtocol.GetNowBlock2(new EmptyMessage());

            string blockHash = block.Blockid.ToByteArray().ToHex();

            Assert.IsTrue(!string.IsNullOrEmpty(blockHash));
        }

        /// <summary>
        /// Get Block By Number 
        /// </summary>
        [TestMethod()]
        public void GetBlockByNumber()
        {
            BlockExtention block = _walletProtocol.GetBlockByNum2(new NumberMessage()
            {
                Num = bh_mainnet
            }, headers: _wallet.GetHeaders());

            string currentBlockHash = block.Blockid.ToByteArray().ToHex();

            Assert.IsTrue(blk_hash_mainnet.Equals(currentBlockHash, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get Block By Hash
        /// </summary>
        [TestMethod()]
        public void GetBlockByHash()
        {
            Block block = _walletProtocol.GetBlockById(new BytesMessage()
            {
                Value = ByteString.CopyFrom(blk_hash_mainnet.HexToByteArray())
            });

            Assert.IsTrue(block.BlockHeader.RawData.Number == bh_mainnet);
        }

        /// <summary>
        /// Get Transaction
        /// GRPC not available, use REST API
        /// </summary>
        [TestMethod()]
        public void GetTransaction()
        {
            //TRX转账 7953d52b688acc5d5f04a97f0f922269d7c35dc9548c44f6c252f3894db4beb6
            //TRC10转账 8f3d90cf3c3c09a74329afc537c9f8d6bce9a71461de2d864751a6afaf2bac63
            //TRC20转账 556f9d350dfae17fd79517b760358b37a1cb1dde95db0236ee85e64fd74f0eb5

            string txid = "76525fd3b9f92765ea30dc574dfd40997dc78588506994c9d724ab851445d8e8";



            //APIREST
            TronNetTransactionRestJson rest_txInfo = _restAPI.GetTransactionByID(txid);
            Assert.IsTrue(!string.IsNullOrEmpty(rest_txInfo.TxID));

            //GRPC
            var grpc_txInfo = _walletProtocol.GetTransactionById(new BytesMessage()
            {
                Value = ByteString.CopyFrom(txid.HexToByteArray())
            }, headers: _wallet.GetHeaders());

            Assert.IsTrue(!string.IsNullOrEmpty(grpc_txInfo.GetTxid()));
        }

        [TestMethod()]
        public void GetTransactionInfo()
        {
            //GRPC
            string txid = "7953d52b688acc5d5f04a97f0f922269d7c35dc9548c44f6c252f3894db4beb6";
            byte[] txidBuffer = txid.HexToByteArray();

            //GRPC
            var txInfo = _walletProtocol.GetTransactionInfoById(new BytesMessage()
            {
                Value = ByteString.CopyFrom(txidBuffer)
            }, headers: _wallet.GetHeaders());

            Assert.IsTrue(txInfo != null);
        }

        #endregion

        #region Address Untils

        /// <summary>
        /// hex address to tron address
        /// </summary>
        [TestMethod()]
        public void AddressHexToTron()
        {
            string scriptAddress = "41a614f803b6fd780986a42c78ec9c7f77e6ded13c";

            string tronAddress = TronNetECKey.ConvertToTronAddressFromHexAddress(scriptAddress);

            Assert.IsTrue("TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t".Equals(tronAddress, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region TRC10 Token Test

        /// <summary>
        /// Get Asset Issue By ID
        /// </summary>
        [TestMethod()]
        public void GetAssetIssueById()
        {
            string mainnet_trc10_assert_id = "31303033353333";
            var trc10 = _walletProtocol.GetAssetIssueById(new BytesMessage()
            {
                Value = ByteString.CopyFrom(mainnet_trc10_assert_id.HexToByteArray())
            }, headers: _wallet.GetHeaders());

            Assert.IsTrue(trc10.Precision > 0);
        }

        #endregion

        #region Smart Contracts

        /// <summary>
        /// Get Contract
        /// </summary>
        [TestMethod()]
        public void GetContract()
        {
            string mainnet_trc20_usdt_address = "TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t";
            byte[] addressBuffer = Base58Encoder.DecodeFromBase58Check(mainnet_trc20_usdt_address);
            //string addressHex = addressBuffer.ToHex();

            SmartContract contract = _walletProtocol.GetContract(new BytesMessage()
            {
                Value = ByteString.CopyFrom(addressBuffer)
            }, headers: _wallet.GetHeaders());

            Assert.IsTrue(contract.ContractAddress.Length > 0);
        }

        /// <summary>
        /// Tigger Contract
        /// Call USDT Deciaml
        /// </summary>
        [TestMethod()]
        public void TiggerContract()
        {
            //variables
            string methodName = "decimals()";
            string mainnet_trc20_usdt_address = "TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t";
            string methodTop8 = Nethereum.Web3.Web3.Sha3(methodName).Substring(0, 8);

            //my address hex
            string myAddressHex = Base58Encoder.DecodeFromBase58Check(TronTestAccountCollection.TestA.Address).ToHex();
            Assert.IsTrue(!string.IsNullOrEmpty(myAddressHex));

            //contract address hex(shatanet contract address)
            string contractAddressHex = Base58Encoder.DecodeFromBase58Check("TB7whW3J9jb5Amoi4R6WgTtMbWPeqMBjSw").ToHex();
            Assert.IsTrue(contractAddressHex.Length > 0);

            var invokeResult = _walletProtocol.TriggerConstantContract(new TriggerSmartContract()
            {
                OwnerAddress = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(TronTestAccountCollection.TestA.Address)),
                ContractAddress = ByteString.CopyFrom(Base58Encoder.DecodeFromBase58Check(mainnet_trc20_usdt_address)),
                Data = ByteString.CopyFrom(methodTop8.HexToByteArray())
            }, headers: _wallet.GetHeaders());

            ByteString decimalBytestring = invokeResult.ConstantResult.FirstOrDefault();
            byte[] deciaml_byte = decimalBytestring.ToByteArray();
            string deciaml_hex = string.Format("0x{0}", Regex.Replace(deciaml_byte.ToHex(), "^0*", string.Empty));
            Assert.IsTrue(!string.IsNullOrEmpty(deciaml_hex));

            int token_decimal = int.Parse(Regex.Replace(deciaml_byte.ToHex(), "^0*", string.Empty), System.Globalization.NumberStyles.HexNumber);

            Assert.IsTrue(6 == token_decimal);
        }

        #endregion
    }
}