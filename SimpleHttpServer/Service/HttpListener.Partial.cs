using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using HttpMachine;
using ISocketLite.PCL.Interface;
using SocketLite.Services;
using SimpleHttpServer.Model;
using SimpleHttpServer.Parser;
using SimpleHttpServer.Service.Base;

namespace SimpleHttpServer.Service
{
    public partial class HttpListener : ComposeBase, IHttpListener, IDisposable
    {
        private ITcpSocketListener _tcpListener = new TcpSocketListener();
        private IUdpSocketReceiver _udpListener = new UdpSocketReceiver();
        private ITcpSocketListener _tcpResponseListener = new TcpSocketListener();
        private ITcpSocketListener _tcpRequestListener = new TcpSocketListener();

        private IObservable<IHttpRequestReponse> UpdRequstReponseObservable =>
            _udpMultiCastListener.ObservableMessages
                .Merge(_udpListener.ObservableMessages)
                .Select(
                    udpSocket =>
                    {                        
                        MemoryStream stream = null;
                        HttpParserDelegate requestHandler = null;
                        HttpParserDelegate requestHandlerResult = null;
                        try
                        {
                            stream = new MemoryStream(udpSocket.ByteData);

                            requestHandler = new HttpParserDelegate();
                            requestHandler.HttpRequestReponse.RemoteAddress = udpSocket.RemoteAddress;
                            requestHandler.HttpRequestReponse.RemotePort = int.Parse(udpSocket.RemotePort, CultureInfo.CurrentCulture);
                            requestHandler.HttpRequestReponse.RequestType = RequestType.Udp;

                            // Finalize
                            requestHandlerResult = requestHandler;
                            requestHandler = null;

                            return HttpStreamParser.Parse(requestHandlerResult, stream, Timeout);
                        }
                        finally
                        {
                            requestHandler?.Dispose();
                            stream?.Dispose();
                        }
                    });

        private IObservable<IHttpRequestReponse> TcpRequestResponseObservable =>
            _tcpListener.ObservableTcpSocket
                .Merge(_tcpResponseListener.ObservableTcpSocket)
                .Select(
                    tcpSocket =>
                    {
                        Stream stream = tcpSocket.ReadStream;
                        HttpParserDelegate requestHandler = null;
                        HttpParserDelegate requestHandlerResult = null;
                        try
                        {
                            requestHandler = new HttpParserDelegate();
                            requestHandler.HttpRequestReponse.RemoteAddress = tcpSocket.RemoteAddress;
                            requestHandler.HttpRequestReponse.RemotePort = tcpSocket.RemotePort;
                            requestHandler.HttpRequestReponse.TcpSocketClient = tcpSocket;
                            requestHandler.HttpRequestReponse.RequestType = RequestType.Tcp;

                            // Finalize
                            requestHandlerResult = requestHandler;
                            requestHandler = null;

                            return HttpStreamParser.Parse(requestHandlerResult, stream, Timeout);
                        }
                        finally
                        {
                            if (requestHandler != null) requestHandler.Dispose();
                        }
                    })
            .ObserveOn(Scheduler.Default);

        // Listening to both UDP and TCP and merging the Http Request streams
        // into one unified IObservable stream of Http Requests

        private IObservable<IHttpRequest> _httpRequestObservable => Observable.Create<IHttpRequest>(
            obs =>
            {
                try
                {
                    var disp = TcpRequestResponseObservable
                        .Merge(UpdRequstReponseObservable)
                        .Where(x => x.MessageType == MessageType.Request)
                        .Select(x => x as IHttpRequest)
                        .Subscribe(
                            req =>
                            {
                                obs.OnNext(req);
                            },
                            ex =>
                            {
                                obs.OnError(ex);
                            },
                            () => obs.OnCompleted());

                    return disp;
                }
                catch(Exception ex)
                {
                    throw;
                }

            }).Publish().RefCount();

        [Obsolete("Deprecated")]
        public IObservable<IHttpRequest> HttpRequestObservable => _httpRequestObservable;

        private IObservable<IHttpResponse> _httpResponseObservable => Observable.Create<IHttpResponse>(
            obs =>
            {
                var disp = TcpRequestResponseObservable
                    .Merge(UpdRequstReponseObservable)
                    .Where(x => x.MessageType == MessageType.Response)
                    .Select(x => x as IHttpResponse)
                    .Subscribe(
                        res =>
                        {
                            obs.OnNext(res);
                        },
                        ex =>
                        {
                            obs.OnError(ex);
                        },
                        () => obs.OnCompleted());
                return disp;
            }).Publish().RefCount();

        [Obsolete("Deprecated")]
        public IObservable<IHttpResponse> HttpResponseObservable => _httpResponseObservable;

        [Obsolete("Deprecated")]
        public async Task StartTcpRequestListener(
            int port,
            ICommunicationInterface communicationInterface = null,
            bool allowMultipleBindToSamePort = true)
        {
            try
            {
                await _tcpListener.StartListeningAsync(port, communicationInterface, allowMultipleBindToSamePort);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
        }

        [Obsolete("Deprecated")]
        public async Task StartTcpResponseListener(
            int port,
            ICommunicationInterface communicationInterface = null,
            bool allowMultipleBindToSamePort = true)
        {
            await
                _tcpResponseListener.StartListeningAsync(port, communicationInterface, allowMultipleBindToSamePort);
        }

        [Obsolete("Deprecated")]
        public async Task StartUdpListener(
            int port,
            ICommunicationInterface communicationInterface = null,
            bool allowMultipleBindToSamePort = true)
        {
            await _udpListener.StartListeningAsync(port, communicationInterface, allowMultipleBindToSamePort);
        }

        [Obsolete("Deprecated")]
        public async Task StartUdpMulticastListener(
            string ipAddr,
            int port,
            IEnumerable<string> mcastIpv6AddressList,
            ICommunicationInterface communicationInterface = null,
            bool allowMultipleBindToSamePort = true)
        {
            if (communicationInterface == null && _communicationInterface !=null)
            {
                communicationInterface = _communicationInterface;
            }
            try
            {
                await
                    _udpMultiCastListener.JoinMulticastGroupAsync(
                        ipAddr,
                        port,
                        communicationInterface,
                        allowMultipleBindToSamePort);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [Obsolete("Deprecated")]
        public async Task StartUdpMulticastListener(
            string ipAddr,
            int port,
            ICommunicationInterface communicationInterface = null,
            bool allowMultipleBindToSamePort = true)
        {
            await StartUdpMulticastListener(
                ipAddr,
                port,
                null,
                communicationInterface,
                allowMultipleBindToSamePort);
        }

        [Obsolete("Deprecated")]
        public void StopTcpRequestListener()
        {
            _tcpListener?.StopListening();

        }

        [Obsolete("Deprecated")]
        public void StopTcpReponseListener()
        {
            _tcpResponseListener?.StopListening();
        }

        [Obsolete("Deprecated")]
        public void StopUdpMultiCastListener()
        {
            _udpMultiCastListener?.Disconnect();
        }

        [Obsolete("Deprecated")]
        public void StopUdpListener()
        {
            _udpListener?.StopListening();
        }

        [Obsolete("Deprecated")]
        public async Task HttpReponse(IHttpRequest request, IHttpResponse response)
        {
            await HttpSendReponseAsync(request, response);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    _tcpListener?.Dispose();
                    _tcpRequestListener?.Dispose();
                    _tcpResponseListener?.Dispose();
                    _udpListener?.Dispose();
                    _udpMultiCastListener?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.

                // set large fields to null.
                _tcpListener = null;
                _tcpRequestListener = null;
                _tcpResponseListener = null;
                _udpListener = null;
                _udpMultiCastListener = null;
                _udpMulticastRequestResponseObservable = null;

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
         ~HttpListener()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        protected ICommunicationInterface CommunicationInterface { get { return _communicationInterface; } }
    }

}


