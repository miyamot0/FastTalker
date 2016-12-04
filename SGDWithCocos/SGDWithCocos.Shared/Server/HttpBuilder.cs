// Copyright (C) 2016 by Barend Erasmus and donated to the public domain

using SGDWithCocos.Shared.Server.Models;

namespace SGDWithCocos.Shared.Server
{
    class HttpBuilder
    {
        public static HttpResponse InternalServerError()
        {
            return new HttpResponse()
            {
                ReasonPhrase = "InternalServerError",
                StatusCode = "500",
                ContentAsUTF8 = "500"
            };
        }

        public static HttpResponse NotFound()
        {
            return new HttpResponse()
            {
                ReasonPhrase = "NotFound",
                StatusCode = "404",
                ContentAsUTF8 = "404"
            };
        }
    }
}