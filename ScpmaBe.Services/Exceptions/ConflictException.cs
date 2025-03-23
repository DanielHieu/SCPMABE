using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmBe.Services.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string msgId, string message) : base(msgId, message)
        {

        }
    }
}
