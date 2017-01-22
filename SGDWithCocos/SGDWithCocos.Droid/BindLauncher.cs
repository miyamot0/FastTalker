//----------------------------------------------------------------------------------------------
// <copyright file="BindLauncher.cs" 
// Copyright 2017 Shawn Gilroy
//
// This file is part of FastTalker.
//
// FastTalker is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// FastTalker is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with FastTalker.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The FastTalker is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// FastTalker utilizes LauncherHijack to leverage interactions with XML file formats
//
//    LauncherHijack is distributed under the following terms:
//
//    Copyright (c) parrotgeek1
//    https://github.com/parrotgeek1/LauncherHijack
//
//    The license is basically "you can do whatever you want except you have to credit 
//    me somewhere in the app, including a link to the repo." The modified version doesn't 
//    have to be open source. You have to send me a link to the modified version if it is ever public,
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
using Android;
using Android.AccessibilityServices;
using Android.Views.Accessibility;
using Java.Lang;

namespace SGDWithCocos.Droid
{
    /// <summary>
    /// Service to override persistent native launcher, as common with certain devices
    /// </summary>
    [Service(Label = "Override Default Launcher with FastTalker", Permission = Manifest.Permission.BindAccessibilityService)]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    public class BindLauncher : AccessibilityService
    {
        private const int delay = 300;

        Intent intent = new Intent(Android.Content.Intent.ActionMain)
            .AddCategory(Android.Content.Intent.CategoryHome)
            .SetPackage("com.smallnstats.fasttalker")
            .AddFlags(ActivityFlags.NewTask |
                    ActivityFlags.ExcludeFromRecents |
                    ActivityFlags.ClearTop |
                    ActivityFlags.ReorderToFront);

        /// <summary>
        /// Event raised
        /// </summary>
        /// <param name="e"></param>
        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            if (e.PackageName.Contains("com.amazon.firelauncher"))
            {
                PerformGlobalAction(Android.AccessibilityServices.AccessibilityService.GlobalActionRecents);

                try
                {
                    Thread.Sleep(delay);
                }
                catch (InterruptedException exc) { }

                PerformGlobalAction(Android.AccessibilityServices.AccessibilityService.Back);
                StartActivity(intent);
            }
        }

        /// <summary>
        /// OnInterrupt (no actions)
        /// </summary>
        public override void OnInterrupt() { }

        /// <summary>
        /// Service connected event, override default launcher
        /// </summary>
        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();

            AccessibilityServiceInfo info = new AccessibilityServiceInfo();
            info.Flags = AccessibilityServiceInfo.Default;
            info.EventTypes = EventTypes.WindowStateChanged;
            info.FeedbackType = Android.AccessibilityServices.FeedbackFlags.Generic;
            info.PackageNames = new string[] { "com.amazon.firelauncher" };

            SetServiceInfo(info);

            PerformGlobalAction(Android.AccessibilityServices.GlobalAction.Recents);

            try
            {
                Thread.Sleep(delay);
            }
            catch (InterruptedException e) { }

            PerformGlobalAction(Android.AccessibilityServices.GlobalAction.Back);

            StartActivity(intent);
        }
    }
}