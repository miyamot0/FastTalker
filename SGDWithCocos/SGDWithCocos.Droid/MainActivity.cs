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
        private BlockingViewGroup view;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            global::Xamarin.Forms.Forms.Init (this, bundle);

            DisplayImplementation.Init();

            LoadApplication (new App ());
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            var mManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            WindowManagerLayoutParams localLayoutParams = new WindowManagerLayoutParams();
            localLayoutParams.Type = WindowManagerTypes.SystemError;
            localLayoutParams.Gravity = GravityFlags.Top;
            localLayoutParams.Flags = WindowManagerFlags.NotFocusable | WindowManagerFlags.NotTouchModal | WindowManagerFlags.LayoutInScreen;

            localLayoutParams.Width = WindowManagerLayoutParams.MatchParent;

            var metrics = new DisplayMetrics();
            mManager.DefaultDisplay.GetMetrics(metrics);

            localLayoutParams.Height = (int)(50 * metrics.ScaledDensity);

            localLayoutParams.Format = Android.Graphics.Format.Transparent;

            view = new BlockingViewGroup(this);

            mManager.AddView(view, localLayoutParams);

        }

        protected override void OnPause()
        {
            base.OnPause();

            /* Stub for now
            var am = Android.App.Application.Context.GetSystemService(Context.ActivityService) as ActivityManager;

            if (am != null)
            {
                am.MoveTaskToFront(TaskId, 0);
            }
            */
        }

        /// <summary>
        /// Remove overlay, if/when destroyed
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            var mManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            mManager.RemoveView(view);
        }

        /// <summary>
        /// Method to detect longer home-press action, overriding with a return to the therapeutic activity
        /// </summary>
        protected override void OnUserLeaveHint()
        {
            Intent intent = new Intent(Android.Content.Intent.ActionMain)
                .SetClass(this, typeof(MainActivity))
                .AddFlags(ActivityFlags.ReorderToFront);

            FireIntent(this, intent);

            base.OnUserLeaveHint();
        }

        /// <summary>
        /// Create a pending intent, returning to the therapeutic activity
        /// </summary>
        /// <param name="paramContext"></param>
        /// <param name="paramIntent"></param>
        public static void FireIntent(Context paramContext, Intent paramIntent)
        {
            if (paramIntent != null)
            {
                var mAlarm = Application.Context.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();
                mAlarm.Set(AlarmType.Rtc, Java.Lang.JavaSystem.CurrentTimeMillis(), PendingIntent.GetActivity(paramContext, 0, paramIntent, 0));
            }
        }
    }
}

