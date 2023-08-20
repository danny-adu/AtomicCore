using Google.Protobuf;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Transaction Client Provider
    /// </summary>
    public class TronNetTransactionClient : ITronNetTransactionClient
    {
        #region Variables

        private readonly ITronNetWalletClient _walletClient;
        private readonly IOptions<TronNetOptions> _options;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="walletClient"></param>
        /// <param name="options"></param>
        public TronNetTransactionClient(ITronNetWalletClient walletClient, IOptions<TronNetOptions> options)
        {
            _walletClient = walletClient;
            _options = options;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create Transaction
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount)
        {
            //GRPC
            Wallet.WalletClient wallet = _walletClient.GetProtocol();

            //Address Parse
            ByteString fromAddress = _walletClient.ParseAddress(from);
            ByteString toAddress = _walletClient.ParseAddress(to);

            //create transaction type
            Transaction.Types.Contract contract = new Transaction.Types.Contract();
            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            try
            {
                contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(new TransferContract
                {
                    OwnerAddress = fromAddress,
                    ToAddress = toAddress,
                    Amount = amount
                });
            }
            catch (Exception)
            {
                return new TransactionExtention
                {
                    Result = new Return
                    {
                        Result = false,
                        Code = Return.Types.response_code.OtherError
                    }
                };
            }

            //Get Current Block Infomation
            BlockExtention newestBlock = await wallet.GetNowBlock2Async(
                new EmptyMessage(), 
                headers: _options.Value.GetgRPCHeaders()
            );

            //Get Block Height && BlockHash
            long blockHeight = newestBlock.BlockHeader.RawData.Number;
            byte[] blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();

            ByteBuffer bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);
            byte[] refBlockNum = bb.ToArray();

            //Create Raw Transaction
            Transaction transaction = new Transaction
            {
                RawData = new Transaction.Types.raw()
            };
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.Timestamp = DateTime.Now.Ticks;
            transaction.RawData.Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000;
            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8));
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2));

            //Create Raw Transaction Message
            TransactionExtention transactionExtension = new TransactionExtention
            {
                Transaction = transaction,
                Txid = ByteString.CopyFromUtf8(transaction.GetTxid()),
                Result = new Return { Result = true, Code = Return.Types.response_code.Success },
            };

            return transactionExtension;
        }

        /// <summary>
        /// Transaction Sign
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public Transaction GetTransactionSign(Transaction transaction, string privateKey)
        {
            //Restore ECKey From Private Key
            ECKey ecKey = new ECKey(privateKey.HexToByteArray(), true);

            //Get Transaction Data
            Transaction transactionSigned = Transaction.Parser.ParseFrom(transaction.ToByteArray());
            
            //hash sign
            byte[] rawdata = transactionSigned.RawData.ToByteArray();
            byte[] hash = rawdata.ToSHA256Hash();
            byte[] sign = ecKey.Sign(hash).ToByteArray();

            //add sign data
            transactionSigned.Signature.Add(ByteString.CopyFrom(sign));

            return transactionSigned;
        }

        /// <summary>
        /// Broadcast Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<Return> BroadcastTransactionAsync(Transaction transaction)
        {
            //GRPC
            Wallet.WalletClient wallet = _walletClient.GetProtocol();

            //Broadcast to Chains
            Return result = await wallet.BroadcastTransactionAsync(
                transaction, 
                headers: _options.Value.GetgRPCHeaders()
            );

            return result;
        }

        #endregion
    }
}
