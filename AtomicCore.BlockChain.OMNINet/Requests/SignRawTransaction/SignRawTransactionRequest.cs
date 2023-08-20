// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// SignRawTransactionRequest
    /// </summary>
    public class SignRawTransactionRequest
    {
        /// <summary>
        /// SignRawTransactionRequest
        /// </summary>
        /// <param name="rawTransactionHex"></param>
        public SignRawTransactionRequest(string rawTransactionHex)
        {
            RawTransactionHex = rawTransactionHex;
            Inputs = new List<SignRawTransactionInput>();
            PrivateKeys = new List<string>();
            SigHashType = AtomicCore.BlockChain.OMNINet.SigHashType.All;
        }

        /// <summary>
        /// RawTransaction Hex
        /// </summary>
        public string RawTransactionHex { get; set; }

        /// <summary>
        /// inputs
        /// </summary>
        public List<SignRawTransactionInput> Inputs { get; set; }

        /// <summary>
        /// private Keys
        /// </summary>
        public List<string> PrivateKeys { get; set; }

        /// <summary>
        /// sigHashType
        /// </summary>
        public string SigHashType { get; set; }

        /// <summary>
        /// add input
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="vout"></param>
        /// <param name="scriptPubKey"></param>
        /// <param name="redeemScript"></param>
        public void AddInput(string txId, int vout, string scriptPubKey, string redeemScript)
        {
            Inputs.Add(new SignRawTransactionInput
            {
                TxId = txId,
                Vout = vout,
                ScriptPubKey = scriptPubKey,
                RedeemScript = redeemScript
            });
        }

        /// <summary>
        /// add input
        /// </summary>
        /// <param name="signRawTransactionInput"></param>
        public void AddInput(SignRawTransactionInput signRawTransactionInput)
        {
            Inputs.Add(signRawTransactionInput);
        }

        /// <summary>
        /// add key
        /// </summary>
        /// <param name="privateKey"></param>
        public void AddKey(string privateKey)
        {
            PrivateKeys.Add(privateKey);
        }
    }
}