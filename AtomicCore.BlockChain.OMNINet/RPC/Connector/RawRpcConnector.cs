﻿// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.IO;
using System.Net;
using System.Text;

namespace AtomicCore.BlockChain.OMNINet
{
    //  This class is disconnected from the core logic and its sole purpose is to serve as a quick and dirty means of debugging
    public static class RawRpcConnector
    {
        //  Usage example:  string networkDifficultyJsonResult = RawRpcConnector.MakeRequest("{\"method\":\"getdifficulty\",\"params\":[],\"id\":1}", "http://127.0.0.1:8332/", "MyRpcUsername", "MyRpcPassword");
        public static string MakeRequest(string jsonRequest, string daemonUrl, string rpcUsername, string rpcPassword)
        {
            try
            {
                CookieContainer tempCookies = new CookieContainer();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byteData = encoding.GetBytes(jsonRequest);
                HttpWebRequest postReq = (HttpWebRequest) WebRequest.Create(daemonUrl);
                postReq.Credentials = new NetworkCredential(rpcUsername, rpcPassword);
                postReq.Method = "POST";
                postReq.KeepAlive = true;
                postReq.CookieContainer = tempCookies;
                postReq.ContentType = "application/json";
                postReq.ContentLength = byteData.Length;
                Stream postreqstream = postReq.GetRequestStream();
                postreqstream.Write(byteData, 0, byteData.Length);
                postreqstream.Close();
                HttpWebResponse postresponse = (HttpWebResponse) postReq.GetResponse();
                StreamReader postreqreader = new StreamReader(postresponse.GetResponseStream());
                return postreqreader.ReadToEnd();
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        //  Usage example:  string networkDifficultyJsonResult = RawRpcConnector.MakeRequest("{\"method\":\"getdifficulty\",\"params\":[],\"id\":1}", new BitcoinService());
        public static string MakeRequest(string jsonRequest, ICoinService coinService)
        {
            return MakeRequest(jsonRequest, coinService.Parameters.SelectedDaemonUrl, coinService.Parameters.RpcUsername, coinService.Parameters.RpcPassword);
        }
    }
}