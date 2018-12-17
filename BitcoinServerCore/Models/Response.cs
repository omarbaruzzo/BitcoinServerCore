using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinServerCore.Models
{
    public class Response
    {
        public Response()
        {
        
        }

        public object Responses { get; set; }
        public object WebError { get; set; }
        public object Error { get; set; }
    }
}
