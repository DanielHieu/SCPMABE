using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Common
{
    public class PaymentSettings
    {
        public string TmnCode { get; set; }

        public string HashSecretKey { get; set; }

        public string PaymentBaseUrl { get; set; }
    }
}
