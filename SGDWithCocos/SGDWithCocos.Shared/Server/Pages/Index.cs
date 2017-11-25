//----------------------------------------------------------------------------------------------
// <copyright file="Index.cs" 
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
//----------------------------------------------------------------------------------------------

namespace SGDWithCocos.Server.Pages
{
    public static class Index
    {
        public static string Html = "<html><head> <meta charset='utf-8'> <title>Fast Talker Web Setup</title> <meta name='description' content='Fast Talker Local Server'> <meta name='author' content='Shawn Gilroy'> <script src='https://code.jquery.com/jquery-3.1.1.min.js'></script> <script>var serverLocation; function LoadCurrentLocation(){serverLocation=window.location.href.toString(); serverLocation=serverLocation.replace('http://', ''); serverLocation=serverLocation.replace('/', ''); var address=document.getElementById('ipAddressInput'); address.value=serverLocation;}function ClickedRequest(){var httpResponse=httpGet(serverLocation, 'LoadBoard');}function httpGet(url, rest){url=url + '/' + rest;var request=$.ajax({url: '/Testing', method: 'GET', dataType: 'html'});request.done(function( msg ){var content=document.createTextNode(msg);document.body.appendChild(content);console.log(msg);});request.fail(function( jqXHR, textStatus ){alert( 'Request failed: ' + textStatus );});/* $.ajax({url: url, headers:{'Access-Control-Allow-Origin': '*', 'Access-Control-Allow-Methods': 'GET,HEAD,OPTIONS,POST,PUT', 'Access-Control-Allow-Headers': 'Origin, X-Requested-With, Content-Type, Accept, Authorization'}, method: 'GET', dataType: 'text/html', success: function(e){console.log(e); //return e;}}); */}</script></head><body onload='LoadCurrentLocation()'><input id='ipAddressInput' type='text' name='ipAddress' value=''/><button id='getRequestStart' onclick='ClickedRequest()'>Hit button</button></body></html>";
    }
}
