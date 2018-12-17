
using BitcoinServerCore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//ci sono 2 tipi di Errore da Gestire "ERROR" e "MANCA_FILE_INIT", tutto il resto sono stringhe valide

namespace BitcoinServerCore.Crypto
{
    class JsonRpcRequest
    {
        public JsonRpcRequest(int id, string method, params object[] parameters)
        {
            Id = id;
            Method = method;
            Parameters = parameters?.ToList() ?? new List<object>();
        }

        [JsonProperty(PropertyName = "method", Order = 0)]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "params", Order = 1)]
        public IList<object> Parameters { get; set; }

        [JsonProperty(PropertyName = "id", Order = 2)]
        public int Id { get; set; }

        public byte[] GetBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public String GetString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    class CryptoService
    {
        public static Response CryptoTransaction(WalletCredentials Parameters, int Timeout, string method, params object[] parameters)
        {
            Response esito = new Response();
            esito.Error = "";
            esito.WebError = "";
            esito.Responses = "";

            //----------------------------------------------------------------------------------

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://" + Parameters.RPCAddress+":"+Parameters.RPCPort);
            //Credenziali e indicazioni HEAD POST
            webRequest.Credentials = new NetworkCredential(Parameters.RPCUsername, Parameters.RPCPassword);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            webRequest.Timeout = Timeout * 1000;

            //Fino a questa parte si potrebbe richiamare in forma statica (una chiamata soltanto)

            JsonRpcRequest r = new JsonRpcRequest(1, method, parameters);
            byte[] byteArray = r.GetBytes();

            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            StreamReader streamReader = null;
            try
            {
                WebResponse webResponse = webRequest.GetResponse();
                streamReader = new StreamReader(webResponse.GetResponseStream(), true);
                string respVal = string.Empty;
                respVal = streamReader.ReadToEnd();

                var json = JsonConvert.DeserializeObject(respVal).ToString();

                esito.Responses = json;
                return esito;
            }
            catch (WebException WEx)
            {
                var webResponse = WEx.Response as HttpWebResponse;
                esito.WebError = "Errore sconosciuto - " + WEx.Message;
            }
            catch (Exception Ex)
            {
                esito.Error = "Errore sconosciuto - " + Ex.Message;

                return esito;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
            }
            //----------------------------------------------------------------------------------------

            return esito;
        }

        public static Response WashCoin(WalletCredentials Parameters, int Timeout, string SenderAccount, string ReceveAddress, decimal amount)
        {
            Response esito = new Response();
            esito.Responses = "";
            //------------------------------------------------------------------------------------------
            string[] account = new string[21];
            string[] address = new string[21];
            account[20] = "WSend" + SenderAccount + "A";
            esito = CryptoTransaction(Parameters, Timeout, "getnewaddress", account[20]);
            if (esito.Responses != "")
            {
                address[20] = esito.Responses.ToString();
            }
            else
            {
                return esito;
            }
            for (var i = 0; i < 20; i = i + 1)
            {
                esito = new Response(); //pulisco esito ad ogni loop
                esito.Responses = "";
                account[i] = "W" + i.ToString() + SenderAccount;
                esito = CryptoTransaction(Parameters, Timeout, "getnewaddress", account[i]);
                if (esito.Responses != "")
                {
                    address[i] = esito.Responses.ToString();
                }
                else
                {
                    return esito;
                }
                Thread.Sleep(100);
            }
            for (var y = 0; y < 20; y = y + 2)
            {
                Response[] esitoArray = new Response[3]; //pulisco esito
                esitoArray[0] = new Response();
                esitoArray[0].Responses = false;
                esitoArray[0] = CryptoTransaction(Parameters, Timeout, "move",SenderAccount, account[y], amount / 10, 1, "");
                esitoArray[1] = new Response();
                esitoArray[1].Responses = false;
                esitoArray[1] = CryptoTransaction(Parameters, Timeout, "move", account[y], account[y + 1], amount / 10, 1, "");
                esitoArray[2] = new Response();
                esitoArray[2].Responses = false;
                esitoArray[2] = CryptoTransaction(Parameters, Timeout, "move", account[y + 1], account[20], amount / 10, 1, "");
            }
            esito = new Response(); //pulisco esito ad ogni loop
            esito.Responses = "";
            esito = CryptoTransaction(Parameters, Timeout, "sendfrom", account[20], ReceveAddress, amount);
            //------------------------------------------------------------------------------------------
            return esito;
        }
    }
}
