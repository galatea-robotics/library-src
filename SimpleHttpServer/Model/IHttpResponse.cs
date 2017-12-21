using System.Collections.Generic;
using System.IO;
using System.Net;
using ISocketLite.PCL.Interface;

namespace SimpleHttpServer.Model
{
    public interface IHttpResponse : IHttpCommon, IParseControl
    {
        int StatusCode { get; }
        string ResponseReason { get; }
    }
}
