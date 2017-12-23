using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer.Model
{
    public interface IParseControl
    {
        bool IsEndOfRequest { get; }

        bool IsRequestTimedOut { get; }

        bool IsUnableToParseHttp { get; }
    }
}
