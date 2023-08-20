// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OMNINet
{
    public class JsonRpcRequest
    {
        public JsonRpcRequest(int id, string method, params object[] parameters)
        {
            JsonRpc = "1.0";
            Id = id;
            Method = method;
            Parameters = parameters != null ? parameters.ToList() : new List<object>();
        }

        [JsonProperty(PropertyName = "jsonrpc", Order = 0)]
        public string JsonRpc { get; set; }

        [JsonProperty(PropertyName = "method", Order = 1)]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "params", Order = 2)]
        public IList<object> Parameters { get; set; }

        [JsonProperty(PropertyName = "id", Order = 3)]
        public int Id { get; set; }

        public byte[] GetBytes()
        {
            string json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}