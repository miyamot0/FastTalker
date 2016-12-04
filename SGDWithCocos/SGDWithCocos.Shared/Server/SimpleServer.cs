//----------------------------------------------------------------------------------------------
// <copyright file="SimpleIconServer.cs" 
// Copyright December 4, 2016 Shawn Gilroy
//
// This file is part of Fast Talker
//
// Fast Talker is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Fast Talker is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//
//----------------------------------------------------------------------------------------------
//
// This file is based on earlier work developed by Can Güney Aksakalli licensed under the 
// MIT License - Copyright (c) 2016 Can Güney Aksakalli
//
// This work can be viewed at : https://gist.github.com/aksakalli/9191056
// 
//----------------------------------------------------------------------------------------------

using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using Xamarin.Forms;
using SGDWithCocos.Interface;
using SGDWithCocos.Server.Pages;

namespace SGDWithCocos.Server
{
    class SimpleIconServer
    {
        private Thread serverThread;
        private HttpListener httpListener;

        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public string IconJSON { get; set; }

        /// <summary>
        /// Initialize server, polling for an open port along the way
        /// </summary>
        public SimpleIconServer(string iconJson)
        {
            IconJSON = iconJson;
            IP = IPAddress.Parse(DependencyService.Get<INetwork>().GetIP());

            TcpListener tcpListener = new TcpListener(IP, 0);
            tcpListener.Start();

            Port = ((IPEndPoint) tcpListener.LocalEndpoint).Port;

            tcpListener.Stop();

            Initialize(Port);
        }

        /// <summary>
        /// Kill server
        /// </summary>
        public void Stop()
        {
            serverThread.Abort();
            httpListener.Stop();
        }

        /// <summary>
        /// Listen on assigned port
        /// </summary>
        private void Listen()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:" + Port.ToString() + "/");
            httpListener.Start();

            while (true)
            {
                try
                {
                    HttpListenerContext context = httpListener.GetContext();
                    Process(context);
                }
                catch (Exception ex) { }
            }
        }

        /// <summary>
        /// Prepare stored strings for transmission
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(s);
            streamWriter.Flush();
            memoryStream.Position = 0;

            return memoryStream;
        }

        /// <summary>
        /// Process response back to the user
        /// </summary>
        /// <param name="context"></param>
        private void Process(HttpListenerContext context)
        {
            string fullpath = context.Request.Url.AbsolutePath.Trim();

            if (fullpath == "/")
            {
                OutputContent(context, Index.Html);
            }
            else if (fullpath == "/LoadBoard")
            {
                OutputContent(context, IconJSON);
            }
            else
            {
                OutputContent(context, HttpStatusCode.InternalServerError.ToString());
            }            
        }

        /// <summary>
        /// Convenience wrapper, stream up the outputted information
        /// </summary>
        /// <param name="context"></param>
        /// <param name="content"></param>
        private void OutputContent(HttpListenerContext context, string content)
        {
            try
            {
                using (Stream s = GenerateStreamFromString(content))
                {
                    context.Response.ContentLength64 = s.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));

                    byte[] buffer = new byte[1024 * 16];
                    int nbytes;
                    while ((nbytes = s.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();

                }

                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Init class
        /// </summary>
        /// <param name="port"></param>
        private void Initialize(int port)
        {
            Port = port;

            serverThread = new Thread(Listen);
            serverThread.IsBackground = true;
            serverThread.Start();
        }
    }
}
