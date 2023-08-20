﻿using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TRC20 Contract Client
    /// </summary>
    public class TRC20ContractClient : IContractClient
    {
        #region Variables

        private readonly ILogger<TRC20ContractClient> _logger;
        private readonly ITronNetWalletClient _walletClient;
        private readonly ITronNetTransactionClient _transactionClient;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="walletClient"></param>
        /// <param name="transactionClient"></param>
        public TRC20ContractClient(ILogger<TRC20ContractClient> logger, ITronNetWalletClient walletClient, ITronNetTransactionClient transactionClient)
        {
            _logger = logger;
            _walletClient = walletClient;
            _transactionClient = transactionClient;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Contract Protocol Enum
        /// </summary>
        public ContractProtocol Protocol => ContractProtocol.TRC20;

        #endregion

        #region Private Methods

        /// <summary>
        /// Get Decimals
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="contractAddressBytes"></param>
        /// <returns></returns>
        private long GetDecimals(Wallet.WalletClient wallet, byte[] contractAddressBytes)
        {
            var trc20Decimals = new DecimalsFunction();

            var callEncoder = new FunctionCallEncoder();
            var functionABI = ABITypedRegistry.GetFunctionABI<DecimalsFunction>();

            var encodedHex = callEncoder.EncodeRequest(trc20Decimals, functionABI.Sha3Signature);

            var trigger = new TriggerSmartContract
            {
                ContractAddress = ByteString.CopyFrom(contractAddressBytes),
                Data = ByteString.CopyFrom(encodedHex.HexToByteArray()),
            };

            var txnExt = wallet.TriggerConstantContract(trigger, headers: _walletClient.GetHeaders());

            var result = txnExt.ConstantResult[0].ToByteArray().ToHex();

            return new FunctionCallDecoder().DecodeOutput<long>(result, new Parameter("uint8", "d"));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transfer
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="ownerAccount"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="memo"></param>
        /// <param name="feeLimit"></param>
        /// <returns></returns>
        public async Task<string> TransferAsync(string contractAddress, ITronNetAccount ownerAccount, string toAddress, decimal amount, string memo, long feeLimit)
        {
            var contractAddressBytes = Base58Encoder.DecodeFromBase58Check(contractAddress);
            var callerAddressBytes = Base58Encoder.DecodeFromBase58Check(toAddress);
            var ownerAddressBytes = Base58Encoder.DecodeFromBase58Check(ownerAccount.Address);
            var wallet = _walletClient.GetProtocol();
            var functionABI = ABITypedRegistry.GetFunctionABI<TransferFunction>();
            try
            {
                var contract = await wallet.GetContractAsync(new BytesMessage
                {
                    Value = ByteString.CopyFrom(contractAddressBytes),
                }, headers: _walletClient.GetHeaders());

                var toAddressBytes = new byte[20];
                Array.Copy(callerAddressBytes, 1, toAddressBytes, 0, toAddressBytes.Length);

                var toAddressHex = "0x" + toAddressBytes.ToHex();

                var decimals = GetDecimals(wallet, contractAddressBytes);

                var tokenAmount = amount;
                if (decimals > 0)
                {
                    tokenAmount = amount * Convert.ToDecimal(Math.Pow(10, decimals));
                }

                var trc20Transfer = new TransferFunction
                {
                    To = toAddressHex,
                    TokenAmount = Convert.ToInt64(tokenAmount),
                };

                var encodedHex = new FunctionCallEncoder().EncodeRequest(trc20Transfer, functionABI.Sha3Signature);


                var trigger = new TriggerSmartContract
                {
                    ContractAddress = ByteString.CopyFrom(contractAddressBytes),
                    OwnerAddress = ByteString.CopyFrom(ownerAddressBytes),
                    Data = ByteString.CopyFrom(encodedHex.HexToByteArray()),
                };

                var transactionExtention = await wallet.TriggerConstantContractAsync(trigger, headers: _walletClient.GetHeaders());

                if (!transactionExtention.Result.Result)
                {
                    _logger.LogWarning($"[transfer]transfer failed, message={transactionExtention.Result.Message.ToStringUtf8()}.");
                    return null;
                }

                var transaction = transactionExtention.Transaction;

                if (transaction.Ret.Count > 0 && transaction.Ret[0].Ret == Transaction.Types.Result.Types.code.Failed)
                {
                    return null;
                }

                transaction.RawData.Data = ByteString.CopyFromUtf8(memo);
                transaction.RawData.FeeLimit = feeLimit;

                var transSign = _transactionClient.GetTransactionSign(transaction, ownerAccount.PrivateKey);

                var result = await _transactionClient.BroadcastTransactionAsync(transSign);

                return transSign.GetTxid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// BalanceOf
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="ownerAccount"></param>
        /// <returns></returns>
        public async Task<decimal> BalanceOfAsync(string contractAddress, ITronNetAccount ownerAccount)
        {
            var contractAddressBytes = Base58Encoder.DecodeFromBase58Check(contractAddress);
            var ownerAddressBytes = Base58Encoder.DecodeFromBase58Check(ownerAccount.Address);
            var wallet = _walletClient.GetProtocol();
            var functionABI = ABITypedRegistry.GetFunctionABI<BalanceOfFunction>();
            try
            {
                var addressBytes = new byte[20];
                Array.Copy(ownerAddressBytes, 1, addressBytes, 0, addressBytes.Length);

                var addressBytesHex = "0x" + addressBytes.ToHex();

                var balanceOf = new BalanceOfFunction { Owner = addressBytesHex };
                var decimals = GetDecimals(wallet, contractAddressBytes);

                var encodedHex = new FunctionCallEncoder().EncodeRequest(balanceOf, functionABI.Sha3Signature);

                var trigger = new TriggerSmartContract
                {
                    ContractAddress = ByteString.CopyFrom(contractAddressBytes),
                    OwnerAddress = ByteString.CopyFrom(ownerAddressBytes),
                    Data = ByteString.CopyFrom(encodedHex.HexToByteArray()),
                };

                var transactionExtention = await wallet.TriggerConstantContractAsync(trigger, headers: _walletClient.GetHeaders());

                if (!transactionExtention.Result.Result)
                {
                    throw new Exception(transactionExtention.Result.Message.ToStringUtf8());
                }
                if (transactionExtention.ConstantResult.Count == 0)
                {
                    throw new Exception($"result error, ConstantResult length=0.");
                }

                var result = new FunctionCallDecoder().DecodeFunctionOutput<BalanceOfFunctionOutput>(transactionExtention.ConstantResult[0].ToByteArray().ToHex());

                var balance = Convert.ToDecimal(result.Balance);
                if (decimals > 0)
                {
                    balance /= Convert.ToDecimal(Math.Pow(10, decimals));
                }

                return balance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #endregion
    }
}
