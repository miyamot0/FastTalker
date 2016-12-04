// Copyright (C) 2016 by David Jeske, Barend Erasmus and donated to the public domain

using SimpleHttpServer;
using SGDWithCocos.Shared.Server.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xamarin.Forms;
using SGDWithCocos.Interface;

namespace SimpleServer
{
    public class HttpServer
    {
        #region Fields

        private int Port;
        private TcpListener Listener;
        private HttpProcessor Processor;
        private bool IsActive = true;

        #endregion

        #region Public Methods

        public HttpServer(int port, List<Route> routes)
        {
            this.Port = port;
            this.Processor = new HttpProcessor();

            foreach (var route in routes)
            {
                this.Processor.AddRoute(route);
            }
        }

        public void Listen()
        {
            string ipaddress = DependencyService.Get<INetwork>().GetIP();
            var newIp = IPAddress.Parse(ipaddress);
            this.Listener = new TcpListener(newIp, this.Port);

            this.Listener.Start();
            while (this.IsActive)
            {
                TcpClient s = this.Listener.AcceptTcpClient();
                Thread thread = new Thread(() =>
                {
                    this.Processor.HandleClient(s);
                });
                thread.Start();
                Thread.Sleep(1);
            }
        }

        #endregion
    }
}