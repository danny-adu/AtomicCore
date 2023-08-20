// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.Runtime.Serialization;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// RpcInternalServerErrorException
    /// </summary>
    [Serializable]
    public class RpcInternalServerErrorException : Exception
    {
        /// <summary>
        /// RpcInternalServerErrorException
        /// </summary>
        public RpcInternalServerErrorException()
        {
        }

        /// <summary>
        /// RpcInternalServerErrorException
        /// </summary>
        /// <param name="customMessage"></param>
        public RpcInternalServerErrorException(string customMessage) : base(customMessage)
        {
        }

        /// <summary>
        /// RpcInternalServerErrorException
        /// </summary>
        /// <param name="customMessage"></param>
        /// <param name="exception"></param>
        public RpcInternalServerErrorException(string customMessage, Exception exception) : base(customMessage, exception)
        {
        }

        /// <summary>
        /// RpcErrorCode
        /// </summary>
        public RpcErrorCode? RpcErrorCode { get; set; }

        /// <summary>
        /// GetObjectData
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("RpcErrorCode", RpcErrorCode, typeof(RpcErrorCode));
            base.GetObjectData(info, context);
        }
    }
}
