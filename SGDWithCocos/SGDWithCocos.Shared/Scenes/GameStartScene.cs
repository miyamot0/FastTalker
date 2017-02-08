//----------------------------------------------------------------------------------------------
// <copyright file="GameStartScene.cs" 
// Copyright November 11, 2016 Shawn Gilroy
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
using CocosSharp;
using SGDWithCocos.Layers;
using SGDWithCocos.Shared.Pages;
using System.Collections.Generic;
using SGDWithCocos.Utilities;
using SGDWithCocos.Tags;

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
        private CCLabel iconsLabel, launcherLabel;
        private CCSpriteSheet spriteSheet;

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

            spriteSheet = new CCSpriteSheet("title.plist");

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

            var ccSpriteFrame = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("blueButton"));
            var blueBackground = new CCScale9Sprite(ccSpriteFrame);
            
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
            
            var ccSpriteFrame = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("redButton"));
            var startGameButton = new CCScale9Sprite(ccSpriteFrame);
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

                var mSpriteFrame = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains(mLetter));
                var mIcon = spriteMaker.MakeTitleIcon(mSpriteFrame, mWidth / 2f, mHeight / 2f);

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
            var widthClamp = (mWidth * 0.3f > 225f) ? 225 : mWidth * 0.3f;

            var ccSpriteFrame = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("respect_logo"));
            var respectSprite = new CCSprite(ccSpriteFrame);
            var respectScale = widthClamp / respectSprite.ContentSize.Width;
            respectSprite.Scale = respectScale;
            respectSprite.PositionX = respectSprite.ScaledContentSize.Width / 2f + (mWidth * 0.02f);
            respectSprite.PositionY = respectSprite.ScaledContentSize.Height / 2f + (mHeight * 0.01f);

            mainLayer.AddChild(respectSprite);

            ccSpriteFrame = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("nuig_logo"));
            var nuiSprite = new CCSprite(ccSpriteFrame);
            var nuiScale = widthClamp / nuiSprite.ContentSize.Width;
            nuiSprite.Scale = nuiScale;
            nuiSprite.PositionX = mWidth - nuiSprite.ScaledContentSize.Width / 2f - (mWidth * 0.01f);
            nuiSprite.PositionY = respectSprite.PositionY;

            mainLayer.AddChild(nuiSprite);

            iconsLabel = new CCLabel("Visual symbols from \"Mulberry Symbol Set\" Copyright 2008-2012 Garry Paxton (CC-BY-SA 2.0). http://straight-street.com", "Arial", 72, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.White,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                Tag = SpriteTypes.IconLicenseTag
            };

            var labelScale = (mWidth * 0.75f) / iconsLabel.ContentSize.Width;
            iconsLabel.Scale = labelScale;
            iconsLabel.PositionX = mainLayer.VisibleBoundsWorldspace.Center.X;
            iconsLabel.PositionY = mHeight - iconsLabel.ScaledContentSize.Height / 2f - (mHeight * 0.02f);

            mainLayer.AddChild(iconsLabel, 0, SpriteTypes.IconLicenseTag);

            launcherLabel = new CCLabel("Code from \"Launcher Hijack\" Copyright 2017 parrotgeek1. https://github.com/parrotgeek1/LauncherHijack", "Arial", 72, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.White,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                Tag = SpriteTypes.LauncherLicenseTag
            };

            launcherLabel.Scale = labelScale;
            launcherLabel.PositionX = mainLayer.VisibleBoundsWorldspace.Center.X;
            launcherLabel.PositionY = iconsLabel.PositionY - launcherLabel.ScaledContentSize.Height / 2f - (mHeight * 0.03f);

            mainLayer.AddChild(launcherLabel, 0, SpriteTypes.LauncherLicenseTag);
        }

        /// <summary>
        /// Button press event, begin scene-based communication app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PressedButton(object sender, EventArgs e)
        {
            spriteSheet.Frames.Clear();

            GameView.Director.ReplaceScene(mGamePage.gameScene);
        }
    }
}
