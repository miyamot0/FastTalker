//----------------------------------------------------------------------------------------------
// <copyright file="AdminImplementation.cs" 
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
using SGDWithCocos.Interface;
using SGDWithCocos.Droid.Implementation;
using Android.App.Admin;
using SGDWithCocos.Droid.Base;

[assembly: Xamarin.Forms.Dependency(typeof(AdminImplementation))]
namespace SGDWithCocos.Droid.Implementation
{
    public class AdminImplementation : IAdmin
    {
        /// <summary>
        /// Method for checking if user has acquired owner rights.  If user is owner, lock that screen up tight
        /// </summary>
        void IAdmin.RequestAdmin(bool status)
        {
            DevicePolicyManager devicePolicyManager = (DevicePolicyManager)Application.Context.GetSystemService(Context.DevicePolicyService);
            ComponentName mDeviceAdminRcvr = new ComponentName(Application.Context, Java.Lang.Class.FromType(typeof(DeviceAdminReceiverClass)).Name);

            if (devicePolicyManager.IsAdminActive(mDeviceAdminRcvr))
            {
                if (devicePolicyManager.IsLockTaskPermitted(Application.Context.PackageName))
                {
                    if (status)
                    {
                        MainActivity.MainApplicationActivity.StartLockTask();
                    }
                    else
                    {
                        MainActivity.MainApplicationActivity.StopLockTask();
                    }
                }
            }
        }
    }
}