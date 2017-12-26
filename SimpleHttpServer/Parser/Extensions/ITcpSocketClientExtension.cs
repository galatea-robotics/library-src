using System;
using System.IO;
using System.Linq;
using HttpMachine;
using ISocketLite.PCL.Interface;
using SimpleHttpServer.Model;
using System.Reactive;
using SimpleHttpServer.Parser;

namespace SimpleHttpServer.Parser
{
    [CLSCompliant(false)]
    public static class ITcpSocketClientExtension
    {
        public static IHttpRequest GethttpRequest(this ITcpSocketClient tcpSocket, TimeSpan timeout)
        {
            Stream stream = tcpSocket.ReadStream;

            HttpParserDelegate requestHandler = new HttpParserDelegate();
            requestHandler.HttpRequestReponse.RemoteAddress = tcpSocket.RemoteAddress;
            requestHandler.HttpRequestReponse.RemotePort = tcpSocket.RemotePort;
            requestHandler.HttpRequestReponse.TcpSocketClient = tcpSocket;
            requestHandler.HttpRequestReponse.RequestType = RequestType.Tcp;

            return HttpStreamParser.Parse(requestHandler, stream, timeout);
        }
    }
}
