using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Models.Requests
{
    public class ConfirmRequest
    {
        public string Code { get; set; }
        public string EmailСiphered { get; set; }
        public string PasswordСiphered { get; set; }
    }
}
