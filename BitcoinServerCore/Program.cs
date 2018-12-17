using BitcoinServerCore.Crypto;
using BitcoinServerCore.Models;
using System;

namespace BitcoinServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            
            #region SETTING_PARAMS
            Response esitoCrypto = new Response();
            WalletCredentials parametri = new WalletCredentials()
            {
                RPCAddress = "127.0.0.1",
                RPCPort = "9999",
                RPCUsername = "username",
                RPCPassword = "password",
                QTPassword = ""
            };
            #endregion

            Console.WriteLine("RPC Crypto Services .Net");
            #region CRIPTO_RPC_SERVICE
            //uso normale del QT
            esitoCrypto = CryptoService.CryptoTransaction(parametri, 60000, "getinfo");
            #endregion

            Console.WriteLine("Wahshing Coin .Net");
            #region WASH_COIN
            //lavaggio crypto
            esitoCrypto = CryptoService.WashCoin(parametri, 60000, "SEND", "yT41N9ugzCRBYB3YABdDVh7H4apCRf5DMZ", 50);
            #endregion
        }
    }
}
