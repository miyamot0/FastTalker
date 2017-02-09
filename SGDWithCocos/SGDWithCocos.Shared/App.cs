//----------------------------------------------------------------------------------------------
// <copyright file="App.cs" 
// Copyright November 6, 2016 Shawn Gilroy
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

using SGDWithCocos.Interface;
using SGDWithCocos.Shared.Pages;
using SGDWithCocos.Tags;
using System;
using Xamarin.Forms;

namespace SGDWithCocos.Shared
{
    public class App : Application
	{
        public static GamePage MainGamePage;

        /// <summary>
        /// Segmented categories for less clunky selections
        /// </summary>
        private static string[] _categoryChunks;
        public static string[] CategoryChunks
        {
            get
            {
                if (_categoryChunks == null)
                {
                    _categoryChunks = new string[] { CategoryTypes.AtoF, CategoryTypes.GtoL, CategoryTypes.MtoR, CategoryTypes.StoZ };
                }

                return _categoryChunks;
            }
        }

        /// <summary>
        /// XF cross platform controls
        /// </summary>
		public App ()
		{
            MainGamePage = new GamePage();

            if (Device.OS == TargetPlatform.Android)
            {
                MainGamePage.IsAdmin = DependencyService.Get<IAdmin>().IsAdmin();

                DependencyService.Get<IAdmin>().RequestAdmin(MainGamePage.IsAdmin);
            }

            MainPage = MainGamePage;
		}

        /// <summary>
        /// Lifecycle override: check for ownership if on android
        /// </summary>
		protected override void OnStart () {}

        /// <summary>
        /// Lifecycle overrides
        /// </summary>
        protected override void OnSleep () {}

        /// <summary>
        /// Lifecycle overrides
        /// </summary>
        protected override void OnResume () {}

	}
}
