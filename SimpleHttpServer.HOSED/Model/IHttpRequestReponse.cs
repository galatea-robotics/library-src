using HttpMachine;

namespace SimpleHttpServer.Model
{
    [System.CLSCompliant(false)]
    public interface IHttpRequestReponse : IHttpResponse, IHttpRequest
    {
        MessageType MessageType { get; }
    }
}
