﻿//----------------------------------------------------------------------------------------------
// <copyright file="GamePage.cs" 
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

using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CocosSharp;
using SGDWithCocos.Views;
using System.Threading.Tasks;
using SGDWithCocos.Interface;
using SGDWithCocos.Models;
using Newtonsoft.Json;
using SGDWithCocos.Tags;
using SGDWithCocos.Scenes;
using SGDWithCocos.Shared.Layers;
using SGDWithCocos.Utilities;
using System.Diagnostics;

namespace SGDWithCocos.Shared.Pages
{
    /// <summary>
    /// X-platform page for Xamarin Forms 
    /// </summary>
    public class GamePage : ContentPage
    {
        public CocosSharpView gameView;
        public CCScene gameScene;
        public UserInput mInputFactory;
        GameLayer mLayer;

        /// <summary>
        /// NativeGame object
        /// </summary>
        public CocosSharpView GameView { get; private set; }

        /// <summary>
        /// Constructor, with DependencyService calls to native display metrics
        /// </summary>
        public GamePage()
        {
            App.Width = DependencyService.Get<IDisplay>().Width;
            App.Height = DependencyService.Get<IDisplay>().Height;

            if (App.Width > App.Height)
            {
                App.Width = DependencyService.Get<IDisplay>().Width;
                App.Height = DependencyService.Get<IDisplay>().Height;
            }
            else
            {
                App.Height = DependencyService.Get<IDisplay>().Width;
                App.Width = DependencyService.Get<IDisplay>().Height;
            }

            gameView = new CocosSharpView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                DesignResolution = new Size(App.Width, App.Height),
                ResolutionPolicy = CocosSharpView.ViewResolutionPolicy.ExactFit,
                ViewCreated = LoadGame,
            };

            Content = gameView;
        }

        /// <summary>
        /// Base methods
        /// </summary>
        protected override void OnDisappearing()
        {
            if (gameView != null)
            {
                gameView.Paused = true;
            }

            base.OnDisappearing();
        }

        /// <summary>
        /// Base methods
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (gameView != null)
            {
                gameView.Paused = false;
            }
        }

        /// <summary>
        /// Base methods
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        /// <summary>
        /// Loading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadGame(object sender, EventArgs e)
        {
            var nativeGameView = sender as CCGameView;

            if (nativeGameView != null)
            {
                nativeGameView.ContentManager.SearchPaths = new List<string>() { "Stored" };

                // Launch stats
                nativeGameView.Stats.Enabled = true;
                nativeGameView.Stats.Scale = 2;

                // Show the start screen
                nativeGameView.RunWithScene(new GameStartScene(nativeGameView, App.Width, App.Height, this));
                
                // Begin building the icon-based scene
                ConstructGameScene(nativeGameView);
            }
        }

        /// <summary>
        /// Construct the icon communication board scene
        /// </summary>
        /// <param name="nativeGameView"></param>
        public void ConstructGameScene(CCGameView nativeGameView)
        {
            gameScene = new GameSGDScene(nativeGameView);

            //string json = FileTools.GetBoards("IconBoard");

            /*
            TODO: remove json stuff
            IconStorageObject jsonObject = null;

            if (json != "")
            {
                try
                {
                    // Attempt to deserialize icons
                    jsonObject = JsonConvert.DeserializeObject<IconStorageObject>(json);
                }
                catch
                {
                    jsonObject = null;
                }
            }
            */

            IconStorageObject savedBoard = new IconStorageObject();

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    savedBoard.Icons = await App.Database.GetIconsAsync();
                    savedBoard.Folders = await App.Database.GetFolderIconsAsync();
                    savedBoard.StoredIcons = await App.Database.GetStoredIconsAsync();

                    TableSettings mSavedSettings = await App.Database.GetSettingsAsync();

                    savedBoard.SingleMode = mSavedSettings.SingleMode;
                    savedBoard.AutoUnselectSingleMode = mSavedSettings.AutoUnselectSingleMode;

                    mSavedSettings = null;

                }
                catch (Exception ex)
                {
                    Debug.WriteIf(App.Debugging, "FastTalker: " + ex.ToString());
                }
            });

            // Create layer for icon board scene
            mLayer = new GameLayer(savedBoard, this);

            mInputFactory = new UserInput
            {
                mLayer = mLayer,
                hasScreenLock = true,
                hasAdmin = false,
                height = App.Height,
                width = App.Width
            };

            // Do JSON parsing AOT
            mLayer.LoadJsonContent();

            // Add layer to icon board scene
            gameScene.AddLayer(mLayer);
        }
    }
}
