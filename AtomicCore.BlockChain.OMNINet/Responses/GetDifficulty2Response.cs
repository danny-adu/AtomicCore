using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OMNINet
{
    public class GetDifficulty2Response
    {
        /// <summary>
        /// 挖坑难度
        /// </summary>
        [JsonProperty("proof-of-work")]
        public double Work { get; set; }

        /// <summary>
        /// 股权
        /// </summary>
        [JsonProperty("proof-of-stake")]
        public double Stake { get; set; }

        /// <summary>
        /// 搜索间隔
        /// </summary>
        [JsonProperty("search-interval")]
        public int SearchInterval { get; set; }
    }
}
