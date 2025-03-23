using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmBe.Services.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string msgId, string message) : base(msgId, message)
        {

        }
    }
}
