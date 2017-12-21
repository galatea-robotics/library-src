using System.Collections.Generic;
using System.IO;
using ISocketLite.PCL.Interface;


namespace SimpleHttpServer.Model
{
    public interface IHttpRequest : IParseControl, IHttpCommon
    {
        bool ShouldKeepAlive { get; }
        object UserContext { get; }
        string Method { get;}
        string RequestUri { get; }
        string Path { get; }
        string QueryString { get; }
        string Fragment { get;}
    }
}
