//----------------------------------------------------------------------------------------------
// <copyright file="GameStartScene.cs" 
// Copyright November 6, 2016 Shawn Gilroy
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

using System;
using CocosSharp;
using SGDWithCocos.Layers;
using SGDWithCocos.Shared.Pages;
using System.Collections.Generic;
using SGDWithCocos.Utilities;
using SGDWithCocos.Tags;
using Xamarin.Forms;

namespace SGDWithCocos.Scenes
{
    /// <summary>
    /// Base scene for entering the mobile application
    /// </summary>
    public class GameStartScene : CCScene
    {
        private int mWidth, mHeight;
        private GamePage mGamePage;
        private CCLayer mainLayer;
        private CCControlButton buttonControl;
        private CCEventListenerTouchOneByOne mListener;

        /// <summary>
        /// Scene constructor for game start
        /// </summary>
        /// <param name="gameView">Native game object reference</param>
        /// <param name="width">native width</param>
        /// <param name="height">native height</param>
        /// <param name="gamePage">Reference to page in game view</param>
        public GameStartScene(CCGameView gameView, int width, int height, GamePage gamePage) : base(gameView)
        {
            mGamePage = gamePage;
            mWidth = width;
            mHeight = height;

            CreateLayers();

            LoadStartLabels();

            LoadMenu();
        }

        /// <summary>
        /// Construct and return background sprite
        /// </summary>
        void CreateLayers()
        {
            mainLayer = new StartLayer();

            var blueBackground = new CCScale9Sprite("blueButton");
            blueBackground.AnchorPoint = CCPoint.AnchorMiddle;
            blueBackground.CapInsets = new CCRect(20, 20, 42, 42);
            blueBackground.ContentSize = new CCSize(mWidth, mHeight);
            blueBackground.PositionX = mWidth / 2f;
            blueBackground.PositionY = mHeight / 2f;

            mainLayer.AddChild(blueBackground, 0);

            AddLayer(mainLayer);

        }

        /// <summary>
        /// Construct actions and necessary bounds/locations
        /// </summary>
        void LoadStartLabels()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            var scaleAction = new CCScaleBy(0.5f, 1.0f, 1.5f);
            var fadeAction = new CCFadeIn(0.5f);
        }

        /// <summary>
        /// Construct Menu and control buttons to begin application
        /// </summary>
        void LoadMenu()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            var startGameButton = new CCScale9Sprite("redButton");
            startGameButton.AnchorPoint = CCPoint.AnchorMiddle;
            startGameButton.CapInsets = new CCRect(20, 20, 42, 42);
            startGameButton.ContentSize = new CCSize((mWidth * 0.4f), (mHeight * 0.2f));

            var startGameLabel = new CCLabel("Load Icon Board", "Arial", 72, CCLabelFormat.SystemFont);

            buttonControl = new CCControlButton(startGameLabel, startGameButton);

            var scale = (startGameLabel.ContentSize.Width * 0.8f) / startGameButton.ContentSize.Width;

            startGameLabel.Scale = scale;

            buttonControl.PositionX = center.X;
            buttonControl.PositionY = bounds.Size.Height / 3.0f;
            buttonControl.Clicked += PressedButton;

            mainLayer.AddChild(buttonControl);

            var spriteMaker = new SpriteMaker(mWidth, mHeight);

            var mX = mWidth / 2f;
            var mY = mHeight * 0.75f;
            var spacer = 1;

            var mRandom = new Random();

            var mLetters = new List<string>() { "Ftitle", "Atitle", "Stitle", "Ttitle", "", "Ttitle", "Atitle", "Ltitle", "Ktitle", "Etitle", "Rtitle" };

            for (var i = 1; i <= mLetters.Count; i++)
            {
                mX = mWidth * (i / 12f) + ((mWidth / 20) * spacer);

                var mLetter = mLetters[i - 1];

                if (mLetter == "")
                {
                    spacer = -1;
                    continue;
                }

                var mRotation = (float)(mRandom.Next(0, 28) - 14);

                var move = new CCMoveTo(1f, new CCPoint(mX, mY));

                var mIcon = spriteMaker.MakeTitleIcon(mLetter, mWidth / 2f, mHeight / 2f);

                mIcon.Rotation = mRotation;

                mainLayer.AddChild(mIcon, 10 + i);

                mIcon.RunActionsAsync(new CCDelayTime(i / 10f), new CCFadeIn(0.5f), new CCMoveTo((i / 10f), new CCPoint(mX, mY)));
            }

            ShowLogos();
        }

        /// <summary>
        /// Show logos on start screen
        /// </summary>
        private void ShowLogos()
        {
            mListener = new CCEventListenerTouchOneByOne();
            mListener.IsSwallowTouches = true;
            mListener.OnTouchBegan = OnTouchBegan;
            mListener.OnTouchEnded = OnTouchesEnded;

            var widthClamp = (mWidth * 0.3f > 250f) ? 250 : mWidth * 0.3f;

            var respectSprite = new CCSprite("respect_logo");
            var respectScale = widthClamp / respectSprite.ContentSize.Width;
            respectSprite.Scale = respectScale;
            respectSprite.PositionX = respectSprite.ScaledContentSize.Width / 2f + (mWidth * 0.02f);
            respectSprite.PositionY = respectSprite.ScaledContentSize.Height / 2f + (mHeight * 0.01f);

            mainLayer.AddChild(respectSprite);

            var nuiSprite = new CCSprite("nuig_logo");
            var nuiScale = widthClamp / nuiSprite.ContentSize.Width;
            nuiSprite.Scale = nuiScale;
            nuiSprite.PositionX = mWidth - nuiSprite.ScaledContentSize.Width / 2f - (mWidth * 0.01f);
            nuiSprite.PositionY = respectSprite.PositionY;

            mainLayer.AddChild(nuiSprite);

            var label = new CCLabel("\"Straight Sheet\" icons by Paxtoncrafts Charitable Trust, licensed CC-BY-SA 3.0 ", "Arial", 72, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.White,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                Tag = SpriteTypes.LicenseTag
            };

            var labelScale = (mWidth * 0.75f) / label.ContentSize.Width;
            label.Scale = labelScale;
            label.PositionX = mainLayer.VisibleBoundsWorldspace.Center.X;
            label.PositionY = mHeight - label.ScaledContentSize.Height / 2f - (mHeight * 0.02f);
            label.AddEventListener(mListener);

            mainLayer.AddChild(label);
        }

        bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            return true;
        }

        void OnTouchesEnded(CCTouch touch, CCEvent touchEvent)
        {
            if (touchEvent.CurrentTarget.Tag == SpriteTypes.LicenseTag)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Device.OpenUri(new Uri("http://straight-street.com/lic.php"));
                });
            }
        }

        /// <summary>
        /// Button press event, begin scene-based communication app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PressedButton(object sender, EventArgs e)
        {
            GameView.Director.ReplaceScene(mGamePage.gameScene);
        }
    }
}
