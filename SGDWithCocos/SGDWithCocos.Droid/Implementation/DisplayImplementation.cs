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

using SGDWithCocos.Interface;
using Android.Views;
using Android.Content;
using Android.Runtime;
using SGDWithCocos.Droid.Implementation;
using Android.OS;
using Android.App;
using Android.Graphics;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(DisplayImplementation))]
namespace SGDWithCocos.Droid.Implementation
{
    public class DisplayImplementation : IDisplay
    {
        public DisplayImplementation() { }

        public static void Init() { }

        [Obsolete("Message")]
        int IDisplay.Height
        {
            get
            {
                IWindowManager windowManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

                if (Build.VERSION.SdkInt < BuildVersionCodes.HoneycombMr2)
                {
                    return windowManager.DefaultDisplay.Height;
                }
                else
                {
                    Point size = new Point();
                    windowManager.DefaultDisplay.GetSize(size);
                    return size.Y;
                }
            }
        }

        [Obsolete("Message")]
        int IDisplay.Width
        {
            get
            {
                IWindowManager windowManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

                if (Build.VERSION.SdkInt < BuildVersionCodes.HoneycombMr2)
                {
                    return windowManager.DefaultDisplay.Width;
                }
                else
                {
                    Point size = new Point();
                    windowManager.DefaultDisplay.GetSize(size);
                    return size.X;
                }
            }
        }
    }
}