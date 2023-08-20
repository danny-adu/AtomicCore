// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OMNINet
{
    public sealed class RpcConnector : IRpcConnector
    {
        private readonly ICoinService _coinService;

        public RpcConnector(ICoinService coinService)
        {
            _coinService = coinService;
        }

        public T MakeRequest<T>(RpcMethods rpcMethod, params object[] parameters)
        {
            return MakeRequest<T>(rpcMethod, 0, parameters);
        }

        private T MakeRequest<T>(RpcMethods rpcMethod, short timedOutRequestsCount, params object[] parameters)
        {
            JsonRpcRequest jsonRpcRequest = new JsonRpcRequest(1, rpcMethod.ToString(), parameters);
            byte[] buffer = jsonRpcRequest.GetBytes();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_coinService.Parameters.SelectedDaemonUrl);
            SetBasicAuthHeader(webRequest, _coinService.Parameters.RpcUsername, _coinService.Parameters.RpcPassword);
            webRequest.Credentials = new NetworkCredential(_coinService.Parameters.RpcUsername, _coinService.Parameters.RpcPassword);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            webRequest.Proxy = null;
            webRequest.Timeout = _coinService.Parameters.RpcRequestTimeoutInSeconds * GlobalConstants.MillisecondsInASecond;

            Stream reqStream = null;
            try
            {
                webRequest.ContentLength = buffer.LongLength;

                reqStream = webRequest.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Close();
            }
            catch (Exception exception)
            {
                throw new RpcException("There was a problem sending the request to the wallet", exception);
            }

            try
            {
                string json;
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            string result = reader.ReadToEnd();
                            reader.Dispose();
                            json = result;
                        }
                    }
                }

                JsonRpcResponse<T> rpcResponse = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(json);
                return rpcResponse.Result;
            }
            catch (WebException webException)
            {
                #region RPC Internal Server Error (with an Error Code)

                if (webException.Response is HttpWebResponse webResponse)
                {
                    switch (webResponse.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                            {
                                using (Stream stream = webResponse.GetResponseStream())
                                {
                                    if (stream == null)
                                    {
                                        throw new RpcException("The RPC request was either not understood by the server or there was a problem executing the request", webException);
                                    }

                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        string result = reader.ReadToEnd();
                                        reader.Dispose();

                                        try
                                        {
                                            JsonRpcResponse<Object> jsonRpcResponseObject = JsonConvert.DeserializeObject<JsonRpcResponse<Object>>(result);

                                            RpcInternalServerErrorException internalServerErrorException = new RpcInternalServerErrorException(jsonRpcResponseObject.Error.Message, webException)
                                            {
                                                RpcErrorCode = jsonRpcResponseObject.Error.Code
                                            };

                                            throw internalServerErrorException;
                                        }
                                        catch (JsonException)
                                        {
                                            throw new RpcException(result, webException);
                                        }
                                    }
                                }
                            }

                        default:
                            throw new RpcException("The RPC request was either not understood by the server or there was a problem executing the request", webException);
                    }
                }

                #endregion

                #region RPC Time-Out

                if (webException.Message == "The operation has timed out")
                {
                    throw new RpcRequestTimeoutException(webException.Message);
                }

                #endregion

                throw new RpcException("An unknown web exception occured while trying to read the JSON response", webException);
            }
            catch (JsonException jsonException)
            {
                throw new RpcResponseDeserializationException("There was a problem deserializing the response from the wallet", jsonException);
            }
            catch (ProtocolViolationException protocolViolationException)
            {
                throw new RpcException("Unable to connect to the server", protocolViolationException);
            }
            catch (Exception exception)
            {
                string queryParameters = jsonRpcRequest.Parameters.Cast<string>().Aggregate(string.Empty, (current, parameter) => current + (parameter + " "));
                throw new Exception(string.Format("A problem was encountered while calling MakeRpcRequest() for: {0} with parameters: {1}. \nException: {2}", jsonRpcRequest.Method, queryParameters, exception.Message));
            }
            finally
            {
                reqStream.Dispose();
            }
        }

        private static void SetBasicAuthHeader(WebRequest webRequest, string username, string password)
        {
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webRequest.Headers["Authorization"] = "Basic " + authInfo;
        }

        public T MakeRequest<T>(string method, params object[] parameters)
        {
            JsonRpcRequest jsonRpcRequest = new JsonRpcRequest(1, method, parameters);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_coinService.Parameters.SelectedDaemonUrl);
            SetBasicAuthHeader(webRequest, _coinService.Parameters.RpcUsername, _coinService.Parameters.RpcPassword);
            webRequest.Credentials = new NetworkCredential(_coinService.Parameters.RpcUsername, _coinService.Parameters.RpcPassword);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            webRequest.Proxy = null;
            webRequest.Timeout = _coinService.Parameters.RpcRequestTimeoutInSeconds * GlobalConstants.MillisecondsInASecond;
            byte[] byteArray = jsonRpcRequest.GetBytes();
            webRequest.ContentLength = jsonRpcRequest.GetBytes().Length;

            try
            {
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Dispose();
                }
            }
            catch (Exception exception)
            {
                throw new RpcException("There was a problem sending the request to the wallet", exception);
            }

            try
            {
                string json;

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string result = reader.ReadToEnd();
                            reader.Dispose();
                            json = result;
                        }
                    }
                }

                JsonRpcResponse<T> rpcResponse = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(json);
                return rpcResponse.Result;
            }
            catch (WebException webException)
            {
                #region RPC Internal Server Error (with an Error Code)

                if (webException.Response is HttpWebResponse webResponse)
                {
                    switch (webResponse.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                            {
                                using (Stream stream = webResponse.GetResponseStream())
                                {
                                    if (stream == null)
                                    {
                                        throw new RpcException("The RPC request was either not understood by the server or there was a problem executing the request", webException);
                                    }

                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        string result = reader.ReadToEnd();
                                        reader.Dispose();

                                        try
                                        {
                                            JsonRpcResponse<Object> jsonRpcResponseObject = JsonConvert.DeserializeObject<JsonRpcResponse<Object>>(result);

                                            RpcInternalServerErrorException internalServerErrorException = new RpcInternalServerErrorException(jsonRpcResponseObject.Error.Message, webException)
                                            {
                                                RpcErrorCode = jsonRpcResponseObject.Error.Code
                                            };

                                            throw internalServerErrorException;
                                        }
                                        catch (JsonException)
                                        {
                                            throw new RpcException(result, webException);
                                        }
                                    }
                                }
                            }

                        default:
                            throw new RpcException("The RPC request was either not understood by the server or there was a problem executing the request", webException);
                    }
                }

                #endregion

                #region RPC Time-Out

                if (webException.Message == "The operation has timed out")
                    throw new RpcRequestTimeoutException(webException.Message);

                #endregion

                throw new RpcException("An unknown web exception occured while trying to read the JSON response", webException);
            }
            catch (JsonException jsonException)
            {
                throw new RpcResponseDeserializationException("There was a problem deserializing the response from the wallet", jsonException);
            }
            catch (ProtocolViolationException protocolViolationException)
            {
                throw new RpcException("Unable to connect to the server", protocolViolationException);
            }
            catch (Exception exception)
            {
                string queryParameters = jsonRpcRequest.Parameters.Cast<string>().Aggregate(string.Empty, (current, parameter) => current + (parameter + " "));
                throw new Exception(string.Format("A problem was encountered while calling MakeRpcRequest() for: {0} with parameters: {1}. \nException: {2}", jsonRpcRequest.Method, queryParameters, exception.Message));
            }
        }
    }
}