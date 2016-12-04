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
        public static string Html = "<html><head><meta charset='utf-8'><title>Fast Talker Web Setup</title><meta name='description' content='Fast Talker Local Server'><meta name='author' content='Shawn Gilroy'><script src='https://code.jquery.com/jquery-3.1.1.min.js'></script><script>function LoadCurrentLocation(){var addressLocation=window.location.href.toString();addressLocation=addressLocation.replace('http://', '');addressLocation=addressLocation.replace('/', '');var address=document.getElementById('ipAddressInput');address.value=addressLocation;}function ClickedRequest(){var address=document.getElementById('ipAddressInput').value;var httpResponse=httpGet(address, 'LoadBoard');document.body.innerHTML +='<p>' + httpResponse + '</p>';}function httpGet(url, rest){url='http://' + url + '/' + rest;$.ajax({url: url, headers:{'Access-Control-Allow-Origin' : '*','Access-Control-Allow-Methods' : 'GET,HEAD,OPTIONS,POST,PUT','Access-Control-Allow-Headers' : 'Origin, X-Requested-With, Content-Type, Accept, Authorization'}, method: 'GET', dataType: 'json', success: function(e){console.log(e);}});}</script></head><body onload='LoadCurrentLocation()'><input id='ipAddressInput' type='text' name='ipAddress' value=''/><button id='getRequestStart' onclick='ClickedRequest()' >Hit button</button></body></html>";
    }
}
