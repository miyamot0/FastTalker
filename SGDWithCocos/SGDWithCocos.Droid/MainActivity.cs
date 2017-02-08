//----------------------------------------------------------------------------------------------
// <copyright file="MainActivity.cs" 
// Copyright August 18, 2016 Shawn Gilroy
//
// This file is part of Cross Platform Communication App
//
// Cross Platform Communication App is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Cross Platform Communication App is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Cross Platform Communication App.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Cross Platform Communication App is a tool to assist clinicans and researchers in the treatment of communication disorders.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using SGDWithCocos.Droid.Base;
using SGDWithCocos.Droid.Implementation;
using SGDWithCocos.Shared;
using System;

namespace SGDWithCocos.Droid
{
    [Activity (Label = "FastTalker",
        AlwaysRetainTaskState = true,
        Icon = "@drawable/icon",
        ScreenOrientation = ScreenOrientation.ReverseLandscape,
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | 
            ConfigChanges.ScreenSize | 
            ConfigChanges.Keyboard | 
            ConfigChanges.KeyboardHidden)]
    [IntentFilter(new[] { Android.Content.Intent.ActionMain },
        Categories = new[] { Android.Content.Intent.CategoryHome, Android.Content.Intent.CategoryDefault })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
        public static Activity MainApplicationActivity;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            global::Xamarin.Forms.Forms.Init (this, bundle);

            DisplayImplementation.Init();
            MainApplicationActivity = this;

            LoadApplication (new App());
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
        }
    }
}

