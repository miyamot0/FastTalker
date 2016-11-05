//----------------------------------------------------------------------------------------------
// <copyright file="GameStartScene.cs" 
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

using System;
using CocosSharp;
using SGDWithCocos.Layers;
using SGDWithCocos.Shared.Pages;

namespace SGDWithCocos.Scenes
{
    public class GameStartScene : CCScene
    {
        GamePage mGamePage;
        int mWidth, mHeight;

        CCLayer mainLayer;

        CCControlButton buttonControl;

        CCFiniteTimeAction scaleLabelAction;
        CCFiniteTimeAction tintLabelAction;

        public GameStartScene(CCGameView gameView, int width, int height, GamePage gamePage) : base(gameView)
        {
            mGamePage = gamePage;
            mWidth = width;
            mHeight = height;

            CreateLayers();

            LoadStartLabels();

            LoadMenu();
        }

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

        void LoadStartLabels()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            var scaleAction = new CCScaleBy(0.5f, 1.0f, 1.5f);
            var fadeAction = new CCFadeIn(0.5f);

            scaleLabelAction = new CCSequence(new CCSpawn(scaleAction, fadeAction),
                new CCEaseElasticInOut(scaleAction.Reverse(), 1.0f));

            tintLabelAction = new CCSequence(new CCTintTo(1.0f, 100, 152, 219),
                new CCTintTo(1.0f, 255, 255, 255));
        }

        void LoadMenu()
        {
            var bounds = mainLayer.VisibleBoundsWorldspace;
            var center = bounds.Center;

            var startGameButton = new CCScale9Sprite("redButton");
            startGameButton.AnchorPoint = CCPoint.AnchorMiddle;
            startGameButton.CapInsets = new CCRect(20, 20, 42, 42);
            startGameButton.ContentSize = new CCSize((mWidth * 0.4f), (mHeight * 0.2f));

            var startGameLabel = new CCLabel("Load Icon Board", "Fonts/MarkerFelt", 22, CCLabelFormat.SpriteFont);

            buttonControl = new CCControlButton(startGameLabel, startGameButton);

            var scale = (mWidth * 0.4f) / buttonControl.ContentSize.Width;

            startGameLabel.Scale = scale;

            buttonControl.PreferredSize = new CCSize(startGameButton.ContentSize.Width * scale, startGameButton.ContentSize.Height * scale);
            buttonControl.PositionX = center.X;
            buttonControl.PositionY = bounds.Size.Height / 4.0f;
            buttonControl.Clicked += PressedButton;

            mainLayer.AddChild(buttonControl);

        }

        private void PressedButton(object sender, EventArgs e)
        {
            GameView.Director.ReplaceScene(mGamePage.gameScene);
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        void StartGamePressed(object sender)
        {
            GameView.Director.ReplaceScene(mGamePage.gameScene);
        }
    }
}
