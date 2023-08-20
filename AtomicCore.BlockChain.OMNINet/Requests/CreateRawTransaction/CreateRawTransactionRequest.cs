// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System.Collections.Generic;
using System.Linq;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// CreateRawTransactionRequest
    /// </summary>
    public class CreateRawTransactionRequest
    {
        /// <summary>
        /// CreateRawTransactionRequest
        /// </summary>
        public CreateRawTransactionRequest()
        {
            Inputs = new List<CreateRawTransactionInput>();
            Outputs = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// CreateRawTransactionRequest
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public CreateRawTransactionRequest(IList<CreateRawTransactionInput> inputs, IDictionary<string, decimal> outputs) : this()
        {
            Inputs = inputs;
            Outputs = outputs;
        }

        /// <summary>
        /// inputs
        /// </summary>
        public IList<CreateRawTransactionInput> Inputs { get; private set; }

        /// <summary>
        /// outputs
        /// </summary>
        public IDictionary<string, decimal> Outputs { get; private set; }

        /// <summary>
        /// add input
        /// </summary>
        /// <param name="input"></param>
        public void AddInput(CreateRawTransactionInput input)
        {
            Inputs.Add(input);
        }

        /// <summary>
        /// add output
        /// </summary>
        /// <param name="output"></param>
        public void AddOutput(CreateRawTransactionOutput output)
        {
            Outputs.Add(output.Address, output.Amount);
        }

        /// <summary>
        /// add input
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="vout"></param>
        public void AddInput(string txId, int vout)
        {
            Inputs.Add(new CreateRawTransactionInput
            {
                TxId = txId,
                Vout = vout
            });
        }

        /// <summary>
        /// add output
        /// </summary>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        public void AddOutput(string address, decimal amount)
        {
            Outputs.Add(address, amount);
        }

        /// <summary>
        /// remove input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool RemoveInput(CreateRawTransactionInput input)
        {
            return Inputs.Contains(input) && Inputs.Remove(input);
        }

        /// <summary>
        /// remove output
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool RemoveOutput(CreateRawTransactionOutput output)
        {
            return RemoveOutput(output.Address, output.Amount);
        }

        /// <summary>
        /// remove input
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="vout"></param>
        /// <returns></returns>
        public bool RemoveInput(string txId, int vout)
        {
            CreateRawTransactionInput input = Inputs.FirstOrDefault(x => x.TxId == txId && x.Vout == vout);
            return input != null && Inputs.Remove(input);
        }

        /// <summary>
        /// remove output
        /// </summary>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool RemoveOutput(string address, decimal amount)
        {
            KeyValuePair<string, decimal> outputToBeRemoved = new KeyValuePair<string, decimal>(address, amount);
            return Outputs.Contains<KeyValuePair<string, decimal>>(outputToBeRemoved) && Outputs.Remove(outputToBeRemoved);
        }
    }
}