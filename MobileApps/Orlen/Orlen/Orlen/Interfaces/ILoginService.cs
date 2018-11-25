using Orlen.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orlen.Interfaces
{
    public interface ILoginService
    {
        Task<WebStatusData> LoginAsync(User user);
    }
}
