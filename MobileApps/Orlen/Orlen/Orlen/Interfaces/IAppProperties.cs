using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Interfaces
{
    public interface IAppProperties
    {
        string GetToken();
        void SetToken(string token);
    }
}
