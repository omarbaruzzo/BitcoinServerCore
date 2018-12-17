using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinServerCore.Models
{
    class WalletCredentials
    {
        public string RPCAddress { get; set; }
        public string RPCPort { get; set; }
        public string RPCUsername { get; set; }
        public string RPCPassword { get; set; }
        public string QTPassword { get; set; }
    }
}
