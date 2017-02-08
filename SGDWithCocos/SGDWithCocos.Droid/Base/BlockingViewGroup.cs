//----------------------------------------------------------------------------------------------
// <copyright file="MainActivity.cs" 
// Copyright February 8, 2017 Shawn Gilroy
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
using Android.Content;
using Android.Views;

namespace SGDWithCocos.Droid.Base
{
    public class BlockingViewGroup : ViewGroup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public BlockingViewGroup(Context context) : base(context) { }

        /// <summary>
        /// Base method
        /// </summary>
        /// <param name="changed"></param>
        /// <param name="l"></param>
        /// <param name="t"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        protected override void OnLayout(bool changed, int l, int t, int r, int b) { }

        /// <summary>
        /// Override and return true for all interactions
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        [Android.Runtime.Register("onInterceptTouchEvent", "(Landroid/view/MotionEvent;)Z", "GetOnInterceptTouchEvent_Landroid_view_MotionEvent_Handler")]
        public override Boolean OnInterceptTouchEvent(MotionEvent ev)
        {
            return true;
        }
    }
}