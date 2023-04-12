using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IBookService BookService { get; }
    }
}
