//----------------------------------------------------------------------------------------------
// <copyright file="DeviceAdminReceiverClass.cs" 
// Copyright February 8, 2016 Shawn Gilroy
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.App.Admin;
using Android.Util;

namespace SGDWithCocos.Droid.Base
{
    [BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", Name = "com.smallnstats.fasttalker.Base.DeviceAdminReceiverClass")]
    [MetaData("android.app.device_admin",  Resource = "@xml/admin")]
    [IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", Intent.ActionMain })]
    public class DeviceAdminReceiverClass : DeviceAdminReceiver { }
}