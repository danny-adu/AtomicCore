using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Transaction Rest API
    /// </summary>
    public interface ITronNetTransactionsRest
    {
        /// <summary>
        /// Get Transaction Sign
        /// </summary>
        /// <param name="privateKey">address private key</param>
        /// <param name="createTransaction">createTransaction Object</param>
        /// <param name="visible">Optional.Whehter the address is in base58 format</param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        TronNetSignedTransactionRestJson GetTransactionSign(string privateKey, TronNetCreateTransactionRestJson createTransaction, bool visible = true);

        /// <summary>
        /// Broadcast Transaction
        /// </summary>
        /// <param name="signedTransaction">signedTransaction Object</param>
        /// <param name="signature">signature</param>
        /// <param name="visible">Optional.Whehter the address is in base58 format</param>
        /// <returns></returns>
        TronNetResultJson BroadcastTransaction(TronNetSignedTransactionRestJson signedTransaction, string[] signature, bool visible = true);

        /// <summary>
        /// Broadcast Hex
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        TronNetResultJson BroadcastHex(string hex);

        /// <summary>
        /// Easy Transfer
        /// </summary>
        /// <param name="passPhrase"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        TronNetEasyTransferJson EasyTransfer(string passPhrase, string toAddress, ulong amount);

        /// <summary>
        /// Easy Transfer By Private
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        TronNetEasyTransferJson EasyTransferByPrivate(string privateKey, string toAddress, ulong amount);

        /// <summary>
        /// Create a TRX transfer transaction. 
        /// If to_address does not exist, then create the account on the blockchain.
        /// </summary>
        /// <param name="ownerAddress">To_address is the transfer address</param>
        /// <param name="toAddress">Owner_address is the transfer address</param>
        /// <param name="amount">Amount is the transfer amount,the unit is trx</param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional.Whehter the address is in base58 format</param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson CreateTransaction(string ownerAddress, string toAddress, decimal amount, int? permissionID = null, bool visible = true);
    }
}
