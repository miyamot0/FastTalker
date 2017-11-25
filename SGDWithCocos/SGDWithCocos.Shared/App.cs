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

using SGDWithCocos.Data;
using SGDWithCocos.Interface;
using SGDWithCocos.Shared.Pages;
using SGDWithCocos.Tags;
using Xamarin.Forms;
using CocosSharp;
using SGDWithCocos.Layers;
using SGDWithCocos.Shared.Layers;
using SGDWithCocos.Scenes;
using SGDWithCocos.Utilities;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace SGDWithCocos.Shared
{
    public class App : Application
	{
        public static GamePage MainGamePage;
        public static CCGameView GameView;

        public static GameStartScene StartScene;

        public static GameSGDScene GameScene;
        public static GameLayer GamingLayer;

        public static UserInput InputFactory;

        private static Assembly assembly;
        public static Assembly MainAssembly
        {
            get
            {
                if (assembly == null)
                {
                    assembly = typeof(App).GetTypeInfo().Assembly;
                }

                return assembly;
            }
        }

        public static string MainAddress
        {
            get 
            {
                #if __IOS__
                    return "SGDWithCocos.iOS.ServerPages.";
                #endif

                #if __ANDROID__
                    return "SGDWithCocos.Droid.ServerPages.";
                #endif                
            }
        }

        public static bool HasAdmin;

        public static bool Loaded = false;

        public static int Height = -1, 
                          Width = -1;

        public static bool Debugging = true;

        /// <summary>
        /// Segmented categories for less clunky selections
        /// </summary>
        static string[] _categoryChunks;
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

        static BoardDatabase database;
        public static BoardDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new BoardDatabase(DependencyService.Get<ISaveAndLoad>().GetDatabaseFilePath("BoardDatabase.db3"));
                }

                return database;
            }
        }

        /// <summary>
        /// XF cross platform controls
        /// </summary>
		public App ()
		{
            Database.Init();           

            MainGamePage = new GamePage();

            if (Device.RuntimePlatform == Device.Android)
            {
                HasAdmin = DependencyService.Get<IAdmin>().IsAdmin();
                DependencyService.Get<IAdmin>().RequestAdmin(HasAdmin);
            }

            MainPage = MainGamePage;
		}

        /// <summary>
        /// Lifecycle override: check for ownership if on android
        /// </summary>
		protected override void OnStart() {}

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
