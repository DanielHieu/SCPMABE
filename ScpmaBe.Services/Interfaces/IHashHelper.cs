using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScpmaBe.Services.Interfaces
{
    public interface IHashHelper
    {
        string HashPassword(string password);
    }
}
