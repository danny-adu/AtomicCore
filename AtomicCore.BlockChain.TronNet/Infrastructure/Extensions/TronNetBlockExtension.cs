using Google.Protobuf;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Block Extension
    /// https://github.com/tronprotocol/documentation/blob/master/TRX/Tron-overview.md
    /// </summary>
    public static class TronNetBlockExtension
    {
        /// <summary>
        /// Get Block Hash(ID)
        /// Block ID is a combination of block height and the hash of the blockheader’s raw data. 
        /// To get block ID, first hash the raw data of the blockheader 
        /// and replace the first 8 bytes of the hash with the blockheight
        /// </summary>
        /// <param name="block"></param>
        /// <returns>return block id</returns>
        public static string GetBlockHash(this Block block)
        {
            //first 8 bytes is block height bytes
            ByteBuffer bh_byteBuffer = ByteBuffer.Allocate(8);
            bh_byteBuffer.PutLong(block.BlockHeader.RawData.Number);
            byte[] bh_hash = bh_byteBuffer.ToArray();

            //block header raw data sha256Hash bytes
            byte[] block_hash = Sha256Sm3Hash.Of(block.BlockHeader.RawData.ToByteArray()).GetBytes();

            //block hash replace the first 8 bytes of the hash with the blockheight
            byte[] newHash = new byte[block_hash.Length];
            Array.Copy(bh_hash, 0, newHash, 0, 8);
            Array.Copy(block_hash, 8, newHash, 8, block_hash.Length - 8);

            return newHash.ToHex();
        }
    }
}
