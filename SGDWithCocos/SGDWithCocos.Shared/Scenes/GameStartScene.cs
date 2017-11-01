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
using SGDWithCocos.Shared;

namespace SGDWithCocos.Scenes
{
    /// <summary>
    /// Base scene for entering the mobile application
    /// </summary>
    public class GameStartScene : CCScene
    {
        private CCLayer mainLayer;
        private CCControlButton buttonControl;
        private CCLabel iconsLabel, launcherLabel;
        private CCSpriteSheet spriteSheet;

        private CCSpriteFrame ccSpriteFrameB;
        private CCScale9Sprite blueBackground;

        private CCSpriteFrame ccSpriteFrameR;
        private CCScale9Sprite startGameButton;

        private CCLabel startGameLabel;

        List<string> mLetters = new List<string>() 
        { 
            "Ftitle", 
            "Atitle", 
            "Stitle", 
            "Ttitle", 
            "", 
            "Ttitle", 
            "Atitle", 
            "Ltitle", 
            "Ktitle", 
            "Etitle", 
            "Rtitle" 
        };

        List<CCSpriteFrame> mFrames = new List<CCSpriteFrame>();
        List<CCSprite> mSprites = new List<CCSprite>();

        private CCSpriteFrame respectLogo;
        private CCSprite respectSprite;

        private CCSpriteFrame nuiLogo;
        private CCSprite nuiSprite;

        /// <summary>
        /// Scene constructor for game start
        /// </summary>
        /// <param name="gameView">Native game object reference</param>
        /// <param name="width">native width</param>
        /// <param name="height">native height</param>
        /// <param name="gamePage">Reference to page in game view</param>
        public GameStartScene() : base(App.GameView)
        {
            spriteSheet = new CCSpriteSheet("title.plist");

            CreateLayers();

            LoadMenu();
        }

        /// <summary>
        /// Construct and return background sprite
        /// </summary>
        void CreateLayers()
        {
            mainLayer = new StartLayer();

            ccSpriteFrameB = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("blueButton"));
            blueBackground = new CCScale9Sprite(ccSpriteFrameB);

            blueBackground.AnchorPoint = CCPoint.AnchorMiddle;
            blueBackground.CapInsets = new CCRect(20, 20, 42, 42);
            blueBackground.ContentSize = new CCSize(App.Width, App.Height);
            blueBackground.PositionX = App.Width / 2f;
            blueBackground.PositionY = App.Height / 2f;

            mainLayer.AddChild(blueBackground, 0);

            AddLayer(mainLayer);
        }

        /// <summary>
        /// Construct Menu and control buttons to begin application
        /// </summary>
        void LoadMenu()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            ccSpriteFrameR = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("redButton"));
            startGameButton = new CCScale9Sprite(ccSpriteFrameR);
            startGameButton.AnchorPoint = CCPoint.AnchorMiddle;
            startGameButton.CapInsets = new CCRect(20, 20, 42, 42);
            startGameButton.ContentSize = new CCSize((App.Width * 0.4f), (App.Height * 0.2f));

            startGameLabel = new CCLabel("Load Icon Board", "Arial", 72, CCLabelFormat.SystemFont);

            buttonControl = new CCControlButton(startGameLabel, startGameButton);

            var scale = (startGameLabel.ContentSize.Width * 0.8f) / startGameButton.ContentSize.Width;

            startGameLabel.Scale = scale;

            buttonControl.PositionX = center.X;
            buttonControl.PositionY = bounds.Size.Height / 3.0f;
            buttonControl.Clicked += PressedButton;

            mainLayer.AddChild(buttonControl);

            var spriteMaker = new SpriteMaker(App.Width, App.Height);

            var mX = App.Width / 2f;
            var mY = App.Height * 0.75f;
            var spacer = 1;

            var mRandom = new Random();

            for (var i = 1; i <= mLetters.Count; i++)
            {
                mX = App.Width * (i / 12f) + ((App.Width / 20) * spacer);

                var mLetter = mLetters[i - 1];

                if (mLetter == "")
                {
                    spacer = -1;
                    continue;
                }

                mFrames.Add(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains(mLetter)));
                mSprites.Add(spriteMaker.MakeTitleIcon(mFrames[mFrames.Count - 1], App.Width / 2f, App.Height / 2f));
                mSprites[mSprites.Count - 1].Rotation = (float)(mRandom.Next(0, 28) - 14);
                mainLayer.AddChild(mSprites[mSprites.Count - 1], 10 + i);

                mSprites[mSprites.Count - 1].RunActionsAsync(new CCDelayTime(i / 10f),
                                                             new CCFadeIn(0.5f),
                                                             new CCMoveTo((i / 10f), new CCPoint(mX, mY)));
            }

            spriteMaker = null;
            mRandom = null;

            ShowLogos();
        }

        /// <summary>
        /// Show logos on start screen
        /// </summary>
        private void ShowLogos()
        {
            var widthClamp = (App.Width * 0.3f > 225f) ? 225 : App.Width * 0.3f;

            respectLogo = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("respect_logo"));
            respectSprite = new CCSprite(respectLogo);
            respectSprite.Scale = widthClamp / respectSprite.ContentSize.Width;
            respectSprite.PositionX = respectSprite.ScaledContentSize.Width / 2f + (App.Width * 0.02f);
            respectSprite.PositionY = respectSprite.ScaledContentSize.Height / 2f + (App.Height * 0.01f);

            mainLayer.AddChild(respectSprite);

            nuiLogo = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("nuig_logo"));
            nuiSprite = new CCSprite(nuiLogo);
            nuiSprite.Scale = widthClamp / nuiSprite.ContentSize.Width;
            nuiSprite.PositionX = App.Width - nuiSprite.ScaledContentSize.Width / 2f - (App.Width * 0.01f);
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

            iconsLabel.Scale = (App.Width * 0.75f) / iconsLabel.ContentSize.Width;
            iconsLabel.PositionX = mainLayer.VisibleBoundsWorldspace.Center.X;
            iconsLabel.PositionY = App.Height - iconsLabel.ScaledContentSize.Height / 2f - (App.Height * 0.02f);

            mainLayer.AddChild(iconsLabel, 0, SpriteTypes.IconLicenseTag);

            launcherLabel = new CCLabel("Code from \"Launcher Hijack\" Copyright 2017 Ethan Nelson-Moore. https://github.com/parrotgeek1/LauncherHijack", "Arial", 72, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.White,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                Tag = SpriteTypes.LauncherLicenseTag
            };

            launcherLabel.Scale = (App.Width * 0.75f) / iconsLabel.ContentSize.Width;
            launcherLabel.PositionX = mainLayer.VisibleBoundsWorldspace.Center.X;
            launcherLabel.PositionY = iconsLabel.PositionY - launcherLabel.ScaledContentSize.Height / 2f - (App.Height * 0.03f);

            mainLayer.AddChild(launcherLabel, 0, SpriteTypes.LauncherLicenseTag);
        }

        /// <summary>
        /// Button press event, begin scene-based communication app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PressedButton(object sender, EventArgs e)
        {
            RunActions(new CCSequence(
                new CCCallFunc(() => {
                    GameView.Director.ReplaceScene(App.GameScene);
                }),
                new CCCallFunc(() => {
                    CleanUpAndDispose();
                })));
        }

        /// <summary>
        /// Cleans up and dispose.
        /// </summary>
        public void CleanUpAndDispose()
        {
            spriteSheet.Frames.Clear();

            RemoveChild(blueBackground, true);
            CleanupMemory(ccSpriteFrameB);

            RemoveChild(buttonControl, true);
            RemoveChild(startGameButton, true);
            CleanupMemory(ccSpriteFrameR);

            CleanupMemory(startGameLabel);

            foreach (CCSpriteFrame frame in mFrames)
            {
                CleanupMemory(frame);
            }

            mFrames.Clear();

            mLetters.Clear();

            foreach (CCSprite sprite in mSprites)
            {
                CCTextureCache.SharedTextureCache.RemoveTexture(sprite.Texture);
                sprite.RemoveAllListeners();
                RemoveChild(sprite, true);
            }

            mSprites.Clear();

            RemoveChild(respectSprite, true);
            CleanupMemory(respectLogo);

            RemoveChild(nuiSprite, true);
            CleanupMemory(nuiLogo);

            RemoveChild(iconsLabel, true);

            RemoveChild(launcherLabel, true);

            RemoveChild(buttonControl, true);

            buttonControl.RemoveAllChildren(true);
            buttonControl.RemoveAllListeners();

            CCTextureCache.SharedTextureCache.RemoveUnusedTextures();

            App.Loaded = true;

            Dispose();
        }

        /// <summary>
        /// Clean up the memory.
        /// </summary>
        /// <param name="ccframe">Ccframe.</param>
        private void CleanupMemory(CCSpriteFrame ccframe)
        {
            CCTextureCache.SharedTextureCache.RemoveTexture(ccframe.Texture);

            CCTextureCache.SharedTextureCache.RemoveUnusedTextures();
        }

        /// <summary>
        /// Clean up the memory.
        /// </summary>
        /// <param name="cclabel">Cclabel.</param>
        private void CleanupMemory(CCLabel cclabel)
        {
            CCTextureCache.SharedTextureCache.RemoveTexture(cclabel.Texture);

            CCTextureCache.SharedTextureCache.RemoveUnusedTextures();
        }
    }
}