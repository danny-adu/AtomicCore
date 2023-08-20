using Newtonsoft.Json;
using System;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// block cypher block json
    /// https://www.blockcypher.com/dev/bitcoin/?shell#block
    /// </summary>
    public class BlockCypherBlockJson
    {
        /// <summary>
        /// The hash of the block; in Bitcoin, the hashing function is SHA256(SHA256(block))
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// The height of the block in the blockchain; i.e., there are height earlier blocks in its blockchain.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// The depth of the block in the blockchain; i.e., there are depth later blocks in its blockchain.
        /// </summary>
        [JsonProperty("depth")]
        public int Depth { get; set; }

        /// <summary>
        /// The name of the blockchain represented, in the form of $COIN.$CHAIN
        /// </summary>
        [JsonProperty("chain")]
        public string Chain { get; set; }

        /// <summary>
        /// The total number of satoshis transacted in this block.
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        /// <summary>
        /// The total number of fees---in satoshis---collected by miners in this block.
        /// </summary>
        [JsonProperty("fees")]
        public int Fees { get; set; }

        /// <summary>
        /// Optional Raw size of block (including header and all transactions) in bytes. Not returned for bitcoin blocks earlier than height 389104.
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// Optional Raw size of block (including header and all transactions) in virtual bytes. Not returned for bitcoin blocks earlier than height 670850.
        /// </summary>
        [JsonProperty("vsize")]
        public int Vsize { get; set; }

        /// <summary>
        /// Block version.
        /// </summary>
        [JsonProperty("ver")]
        public int Ver { get; set; }

        /// <summary>
        /// Recorded time at which block was built. Note: Miners rarely post accurate clock times.
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; set; }

        /// <summary>
        /// The time BlockCypher's servers receive the block. Our servers' clock is continuously adjusted and accurate.
        /// </summary>
        [JsonProperty("received_time")]
        public DateTime ReceivedTime { get; set; }

        /// <summary>
        /// Address of the peer that sent BlockCypher's servers this block.
        /// </summary>
        [JsonProperty("relayed_by")]
        public string RelayedBy { get; set; }

        /// <summary>
        /// The block-encoded difficulty target.
        /// </summary>
        [JsonProperty("bits")]
        public int Bits { get; set; }

        /// <summary>
        /// The number used by a miner to generate this block.
        /// </summary>
        [JsonProperty("nonce")]
        public int Nonce { get; set; }

        /// <summary>
        /// Number of transactions in this block.
        /// </summary>
        [JsonProperty("n_tx")]
        public int NTx { get; set; }

        /// <summary>
        /// The hash of the previous block in the blockchain.
        /// </summary>
        [JsonProperty("prev_block")]
        public string PrevBlock { get; set; }

        /// <summary>
        /// The BlockCypher URL to query for more information on the previous block.
        /// </summary>
        [JsonProperty("prev_block_url")]
        public string PrevBlockUrl { get; set; }

        /// <summary>
        /// The base BlockCypher URL to receive transaction details. To get more details about specific transactions, you must concatenate this URL with the desired transaction hash(es).
        /// </summary>
        [JsonProperty("tx_url")]
        public string TxUrl { get; set; }

        /// <summary>
        /// The Merkle root of this block.
        /// </summary>
        [JsonProperty("mrkl_root")]
        public string MrklRoot { get; set; }

        /// <summary>
        /// An array of transaction hashes in this block. By default, only 20 are included.
        /// </summary>
        [JsonProperty("txids")]
        public string[] Txids { get; set; }

        /// <summary>
        /// Optional If there are more transactions that couldn't fit in the txids array, this is the BlockCypher URL to query the next set of transactions (within a Block object).
        /// </summary>
        [JsonProperty("next_txids")]
        public string NextTxids { get; set; }
        
    }
}
