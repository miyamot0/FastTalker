﻿//----------------------------------------------------------------------------------------------
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
using System.Diagnostics;
using SGDWithCocos.Shared;
using SGDWithCocos.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace SGDWithCocos.Server
{
    class SimpleIconServer
    {
        private Thread serverThread;
        private HttpListener httpListener;

        public IPAddress IP { get; set; }
        public int Port { get; set; }
        //public string IconJSON { get; set; }

        /// <summary>
        /// Initialize server, polling for an open port along the way
        /// </summary>
        public SimpleIconServer()
        {
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

                    // Important: Kill these off ASAP
                    context.Response.KeepAlive = false;

                    Process(context);
                }
                catch { }
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

            Debug.WriteLineIf(App.Debugging, "Method: " + context.Request.HttpMethod);

            if (context.Request.HttpMethod == "GET")
            {
                if (fullpath == "/")
                {
                    using (Stream stream = App.MainAssembly.GetManifestResourceStream(App.MainAddress + "Index.html"))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            OutputContent(context, reader.ReadToEnd());
                        }
                    }
                }
                else if (fullpath == "/LoadBoard")
                {
                    OutputContent(context, OutputCurrentField());
                }
                else
                {
                    OutputContent(context, HttpStatusCode.InternalServerError.ToString());
                }                            
            }
            else if (context.Request.HttpMethod == "POST")
            {
                if (fullpath == "/UploadIcon")
                {
                    if (context.Request.HasEntityBody)
                    {
                        // TODO: add an icon to field remotely

                        OutputContent(context, "Success");
                    }
                    else
                    {
                        OutputContent(context, "Failure");
                    }                   
                }
                else if (fullpath == "/UploadBoard")
                {
                    if (context.Request.HasEntityBody)
                    {
                        // TODO: handle json-encoded board

                        OutputContent(context, "Success");
                    }
                    else
                    {
                        OutputContent(context, "Failure");
                    }
                }
                else if (fullpath == "/Delete")
                {
                    Debug.WriteLineIf(App.Debugging, "In post");

                    if (context.Request.HasEntityBody)
                    {
                        Debug.WriteLineIf(App.Debugging, "has entity body");

                        using (StreamReader reader = new StreamReader(context.Request.InputStream))
                        {
                            string jsonString = reader.ReadToEnd();

                            ServerComms req = JsonConvert.DeserializeObject<ServerComms>(jsonString);

                            if (req != null)
                            {
                                App.GamingLayer.RemoteManageIcon(req);                                
                            }
                        }

                        OutputContent(context, "Success");
                    }
                    else
                    {
                        OutputContent(context, "Failure");
                    }                        
                }
            }
        }

        /// <summary>
        /// Outputs the current field.
        /// </summary>
        /// <returns>The current field.</returns>
        private string OutputCurrentField()
        {
            List<TableIcons> icons = App.Database.GetIconsAsync().Result;
            List<TableStoredIcons> iconsSaved = App.Database.GetStoredIconsAsync().Result;
            List<TableFolders> folders = App.Database.GetFolderIconsAsync().Result;
            TableSettings settings = App.Database.GetSettingsAsync().Result;

            bool comma = false;

            string mResponse = "{";

            mResponse += "\"TableIcons\": [";
            foreach (TableIcons icon in icons)
            {
                if (comma)
                {
                    mResponse += ",";
                }

                mResponse += "{\"Text\": \"" + icon.Text + "\",";
                mResponse += "\"X\": " + icon.X + ",";
                mResponse += "\"Y\": " + icon.Y + ",";
                mResponse += "\"HashCode\": " + icon.HashCode + ",";
                mResponse += "\"Base64\": \"" + icon.Base64 + "\",";
                mResponse += "\"Scale\": " + icon.Scale + "}";

                comma = true;
            }

            mResponse += "],";
            mResponse += "\"TableStoredIcons\": [";

            comma = false;

            foreach (TableStoredIcons icon in iconsSaved)
            {
                if (comma)
                {
                    mResponse += ",";
                }

                mResponse += "{\"Text\": \"" + icon.Text + "\",";
                mResponse += "\"Folder\": \"" + icon.Folder + "\",";
                mResponse += "\"X\": " + icon.X + ",";
                mResponse += "\"Y\": " + icon.Y + ",";
                mResponse += "\"HashCode\": " + icon.HashCode + ",";
                mResponse += "\"Base64\": \"" + icon.Base64 + "\",";
                mResponse += "\"Scale\": " + icon.Scale + "}";

                comma = true;
            }

            mResponse += "],";
            mResponse += "\"TableFolders\": [";

            comma = false;

            foreach (TableFolders icon in folders)
            {
                if (comma)
                {
                    mResponse += ",";
                }

                mResponse += "{\"Text\": \"" + icon.Text + "\",";
                mResponse += "\"X\": " + icon.X + ",";
                mResponse += "\"Y\": " + icon.Y + ",";
                mResponse += "\"HashCode\": " + icon.HashCode + ",";
                mResponse += "\"Base64\": \"" + icon.Base64 + "\",";
                mResponse += "\"Scale\": " + icon.Scale + "}";

                comma = true;
            }

            mResponse += "],";
            mResponse += "\"SingleModel\": ";
            mResponse += (settings.SingleMode) ? "true" : "false";
            mResponse += "}";

            return mResponse;
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
                    context.Response.ContentType = "text/html";
                    context.Response.ContentLength64 = s.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));

                    byte[] buffer = new byte[1024 * 16];

                    int nbytes;

                    while ((nbytes = s.Read(buffer, 0, buffer.Length)) > 0) 
                    {
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }

                context.Response.OutputStream.Close();
                context.Response.Close();
            }
            catch (Exception e) 
            { 
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                Debug.WriteLineIf(App.Debugging, e.ToString());
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
