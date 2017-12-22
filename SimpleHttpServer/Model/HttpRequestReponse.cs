using System;
using System.IO;
using HttpMachine;
using ISocketLite.PCL.Interface;
using SimpleHttpServer.Model;
using SimpleHttpServer.Model.Base;

namespace SimpleHttpServer.Model
{
    [CLSCompliant(false)]
    public class HttpRequestReponse : HttpHeaderBase, IHttpRequestReponse, System.IDisposable
    {
        public HttpRequestReponse()
        {
            Body = new MemoryStream();
        }
        public MessageType MessageType { get; internal set; }
        public int StatusCode { get; internal set; }
        public string ResponseReason { get; internal set; }
        public int MajorVersion { get; internal set; }
        public int MinorVersion { get; internal set; }
        public bool ShouldKeepAlive { get; internal set; }
        public object UserContext { get; internal set; }
        public string Method { get; internal set; }
        public System.Uri RequestUri { get; internal set; }
        public string Path { get; internal set; }
        public string QueryString { get; internal set; }
        public string Fragment { get; internal set; }
        public bool IsChunked { get; internal set; }
        public MemoryStream Body { get; internal set; }       

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Body.Dispose();
                TcpSocketClient.Dispose();
            }
        }
    }
}
