using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ISocketLite.PCL.Interface;
using SocketLite.Model;

namespace SimpleHttpServer.Helper
{
    using SimpleHttpServer.Service;
    using SimpleHttpServer.Model;

    public static class Initializer
    {
        private static string _method;
        private static string _path;

        public static string Method { get { return _method; } }
        public static string Path { get { return _path; } }


        internal static ICommunicationInterface GetDefaultCommunicationsInterface()
        {
            var comm = new CommunicationsInterface();
            var allComms = comm.GetAllInterfaces();
            var networkComm = allComms.FirstOrDefault(x => x.GatewayAddress != null);

            return networkComm;
        }

        public static async Task StartListener()
        {
            var networkComm = GetDefaultCommunicationsInterface();

            var httpListener = new HttpListener(communicationInterface: networkComm, timeout: TimeSpan.FromSeconds(3));
            await httpListener.StartTcpRequestListener(port: 8000);
            await httpListener.StartTcpResponseListener(port: 8001);
            await httpListener.StartUdpMulticastListener(ipAddr: "239.255.255.250", port: 1900);


            var observeHttpRequests = httpListener
                .HttpRequestObservable
                // Must observe on Dispatcher for XAML to work
                .ObserveOnDispatcher().Subscribe(async
                request =>
                {
                    if (!request.IsUnableToParseHttp)
                    {
                        _method = request?.Method ?? "N/A";
                        _path = request?.Path ?? "N/A";
                        if (request.RequestType == RequestType.TCP)
                        {
                            var response = new HttpReponse
                            {
                                StatusCode = (int)HttpStatusCode.OK,
                                ResponseReason = HttpStatusCode.OK.ToString(),
                                Headers = new Dictionary<string, string>
                                {
                                    {"Date", DateTime.UtcNow.ToString("r")},
                                    {"Content-Type", "text/html; charset=UTF-8" },
                                },
                                Body = new MemoryStream(Encoding.UTF8.GetBytes($"<html>\r\n<body>\r\n<h1>Hello, World! {DateTime.Now}</h1>\r\n</body>\r\n</html>"))
                            };

                            await httpListener.HttpReponse(request, response).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        _method = "Unable to parse request";
                    }
                },
                // Exception
                ex =>
                {
                });

            // Remember to dispose of subscriber when done
            observeHttpRequests.Dispose();
        }

        public static ListenerCommunicationInterface GetListener(
            string ipAddress,
            int port,
            TimeSpan timeout = default(TimeSpan))
        {

            if (timeout == default(TimeSpan))
            {
                timeout = TimeSpan.FromSeconds(30);
            }

            var communicationInterface = new CommunicationsInterface();
            var allInterfaces = communicationInterface.GetAllInterfaces();

            var firstUsableInterface = allInterfaces.FirstOrDefault(x => x.IpAddress == ipAddress);

            if (firstUsableInterface == null) throw new ArgumentException($"Unable to locate any network communication interface with the ip address: {ipAddress}");

            return (GetListener(firstUsableInterface, port));
        }

        public static ListenerCommunicationInterface GetListener(
            ICommunicationInterface communicationInterface,
            int port,
            TimeSpan timeout = default(TimeSpan))
        {
            if (timeout == default(TimeSpan))
            {
                timeout = TimeSpan.FromSeconds(30);
            }

            var httpListener = new HttpListener(communicationInterface, timeout);

            //await httpListener.StartTcpRequestListener(port, communicationInterface);

            return new ListenerCommunicationInterface(httpListener, communicationInterface);
        }
    }

    public struct ListenerCommunicationInterface
    {
        public ListenerCommunicationInterface(IHttpListener httpListener, ICommunicationInterface communicationInterface)
        {
            HttpListener = httpListener;
            CommunicationInterface = communicationInterface;
        }
        IHttpListener HttpListener { get; }
        ICommunicationInterface CommunicationInterface { get; }
    }
}
