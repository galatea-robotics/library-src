using HttpMachine;

namespace SimpleHttpServer.Model
{
    public interface IHttpRequestReponse : IHttpResponse, IHttpRequest
    {
        MessageType MessageType { get; }
    }
}
