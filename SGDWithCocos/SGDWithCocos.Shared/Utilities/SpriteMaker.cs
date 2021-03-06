﻿//----------------------------------------------------------------------------------------------
// <copyright file="SpriteMaker.cs" 
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

using CocosSharp;
using SGDWithCocos.Tags;
using System.Threading.Tasks;

namespace SGDWithCocos.Utilities
{
    /// <summary>
    /// Class devoted to encapsulating necessary sprites
    /// </summary>
    public class SpriteMaker
    {
        /// <summary>
        /// Measures necessary for dynamic field size, both Android and iOS
        /// </summary>
        public float DynamicWidth = 0f,
                     DynamicHeight = 0f,
                     padding = 10,
                     margin = 10,
                     adjustment = 1f;

        /// <summary>
        /// Size agnostic proportions for framed sentence field
        /// </summary>
        private const float sentenceFrameWidth = 0.75f,
                            sentenceFrameHeight = 0.2f;

        /// <summary>
        /// Scaling factor
        /// </summary>
        private float dimension = -1f;

        /// <summary>
        /// Blank constructor
        /// </summary>
        public SpriteMaker() { }

        /// <summary>
        /// Constructor following assigned field size
        /// </summary>
        /// <param name="dynamicWidth"></param>
        /// <param name="dynamicHeight"></param>
        public SpriteMaker(float dynamicWidth, float dynamicHeight)
        {
            DynamicWidth = dynamicWidth;
            DynamicHeight = dynamicHeight;

            dimension = (DynamicWidth * 0.1f);
        }

        /// <summary>
        /// Construct and return sentence frame sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeSentenceFrame(CCSpriteFrame frame)
        {
            var sentenceFrame = new CCSprite(frame);
            sentenceFrame.ContentSize = new CCSize(DynamicWidth * sentenceFrameWidth - padding, DynamicHeight * sentenceFrameHeight);
            sentenceFrame.PositionX = 0 + (sentenceFrame.ContentSize.Width / 2) + padding;
            sentenceFrame.PositionY = DynamicHeight - (sentenceFrame.ContentSize.Height / 2) - padding;

            return sentenceFrame;
        }

        /// <summary>
        /// Construct and return speaker icon sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeSpeakerFrame(CCSpriteFrame frame)
        {
            var speakerFrame = new CCSprite(frame);

            var scaling = (DynamicHeight * sentenceFrameHeight) / speakerFrame.ContentSize.Height;
            var xPos = (DynamicWidth - (DynamicWidth * sentenceFrameWidth - padding)) / 2;

            speakerFrame.Scale = scaling;
            speakerFrame.PositionX = DynamicWidth - xPos;
            speakerFrame.PositionY = DynamicHeight - (speakerFrame.ScaledContentSize.Height / 2) - padding;

            return speakerFrame;
        }

        /// <summary>
        /// Construct and return "Add Icon" sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeAddButton(CCSpriteFrame frame, CCSpriteFrame backing)
        {
            var parentSprite = new CCSprite(backing)
            {
                Tag = SpriteTypes.AddTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);

            parentSprite.PositionX = DynamicWidth - (parentSprite.ContentSize.Width / 2) - padding;
            parentSprite.PositionY = (parentSprite.ContentSize.Height / 2) + padding;

            var subIconFrame = new CCSprite(frame)
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.5f, parentSprite.ContentSize.Height * 0.5f)
            };

            subIconFrame.PositionX = parentSprite.ContentSize.Width - (subIconFrame.ContentSize.Width);
            subIconFrame.PositionY = parentSprite.ContentSize.Height - (subIconFrame.ContentSize.Height);

            var label = new CCLabel("Settings", "Arial", 18, CCLabelFormat.SystemFont)
            {                
                Color = CCColor3B.Black,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                ContentSize = new CCSize(parentSprite.ContentSize.Width, parentSprite.ContentSize.Height * 0.25f),
                Position = new CCPoint(parentSprite.ContentSize.Width / 2, parentSprite.ContentSize.Height / 6),
                Tag = SpriteTypes.ContentTag
            };

            parentSprite.AddChild(subIconFrame);
            parentSprite.AddChild(label);

            return parentSprite;
        }

        /// <summary>
        /// Construct and return "Remove Icon" sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeRemoveButton(CCSpriteFrame frame, CCSpriteFrame backing)
        {
            var parentSprite = new CCSprite(backing)
            {
                Tag = SpriteTypes.RemoveTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);
            parentSprite.PositionX = (parentSprite.ContentSize.Width / 2) + padding;
            parentSprite.PositionY = (parentSprite.ContentSize.Height / 2) + padding;

            var subIconFrame = new CCSprite(frame)
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.5f, parentSprite.ContentSize.Height * 0.5f)
            };

            subIconFrame.PositionX = parentSprite.ContentSize.Width - (subIconFrame.ContentSize.Width);
            subIconFrame.PositionY = parentSprite.ContentSize.Height - (subIconFrame.ContentSize.Height);

            var label = new CCLabel("Delete Icon", "Arial", 18, CCLabelFormat.SystemFont)
            {                
                Color = CCColor3B.Black,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                ContentSize = new CCSize(parentSprite.ContentSize.Width, parentSprite.ContentSize.Height * 0.25f),
                Position = new CCPoint(parentSprite.ContentSize.Width / 2, parentSprite.ContentSize.Height / 6),
                Tag = SpriteTypes.ContentTag
            };

            parentSprite.AddChild(subIconFrame);
            parentSprite.AddChild(label);

            return parentSprite;
        }

        /// <summary>
        /// Construct and return templated letter sprite
        /// </summary>
        /// <param name="letter">Sprite Tag reference</param>
        /// <param name="positionX">x pos</param>
        /// <param name="positionY">y pos</param>
        /// <returns></returns>
        public CCSprite MakeTitleIcon(CCSpriteFrame letter, float positionX, float positionY)
        {
            var parentSprite = new CCSprite(letter)
            {
                PositionX = positionX,
                PositionY = positionY,
                ContentSize = new CCSize(dimension, dimension)
            };

            return parentSprite;
        }

        /// <summary>
        /// Async call to create base64 sprite
        /// </summary>
        /// <param name="backing"></param>
        /// <param name="base64string"></param>
        /// <param name="iconSpeechText"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="scale"></param>
        /// <param name="textScale"></param>
        /// <param name="textVisible"></param>
        /// <returns></returns>
        public Task<CCSprite> AsyncCreateBase64Sprite(CCSpriteFrame backing, string base64string, string iconSpeechText, float positionX, float positionY, float scale, float textScale, bool textVisible)
        {
            TaskCompletionSource<CCSprite> tcs = new TaskCompletionSource<CCSprite>();

            tcs.SetResult(MakeIconBase64(backing, base64string, iconSpeechText, positionX, positionY, scale, textScale, textVisible));

            return tcs.Task;
        }

        /// <summary>
        /// Construct and return icon sprite with image from base64 string
        /// </summary>
        /// <param name="base64string">base64 string</param>
        /// <param name="iconSpeechText">speech tag</param>
        /// <param name="positionX">x pos</param>
        /// <param name="positionY">y pos</param>
        /// <param name="scale">scale set</param>
        /// <param name="textScale">text set</param>
        /// <param name="textVisible">text visible?</param>
        /// <returns></returns>
        private CCSprite MakeIconBase64(CCSpriteFrame backing, string base64string, string iconSpeechText, float positionX, float positionY, float scale, float textScale, bool textVisible)
        {
            scale = (scale == -1) ? 1 : scale;

            var parentSprite = new CCSprite(backing)
            {
                PositionX = positionX,
                PositionY = positionY,
                ContentSize = new CCSize(dimension, dimension),
                Scale = scale,
                Tag = SpriteTypes.IconTag
            };

            CCSprite subIconFrame = null;

            // If passed a blank base64, assume its meant to be just white
            if (base64string != "")
            {
                byte[] bytes = System.Convert.FromBase64String(base64string);
                var testTexture = new CCTexture2D(bytes);
                var testFrame = new CCSpriteFrame(testTexture, new CCRect(0, 0, testTexture.PixelsWide, testTexture.PixelsHigh));

                var dimToScale = System.Math.Max(testFrame.ContentSize.Width, testFrame.ContentSize.Height);
                var scalingImg = (parentSprite.ContentSize.Height * 0.75f) / dimToScale;

                subIconFrame = new CCSprite(testFrame)
                {
                    AnchorPoint = CCPoint.AnchorMiddle,
                    ContentSize = new CCSize(testFrame.ContentSize.Width * scalingImg, testFrame.ContentSize.Height * scalingImg),
                    PositionX = parentSprite.ContentSize.Width / 2f,
                    PositionY = parentSprite.ContentSize.Height / 2f + parentSprite.ContentSize.Height * 0.075f,
                    Tag = SpriteTypes.ImageTag
                };
            }
            else
            {
                subIconFrame = new CCSprite("frameWhite");

                var dimToScale = System.Math.Max(subIconFrame.ContentSize.Width, subIconFrame.ContentSize.Height);
                var scalingImg = (parentSprite.ContentSize.Height * 0.75f) / dimToScale;

                subIconFrame.AnchorPoint = CCPoint.AnchorMiddle;
                subIconFrame.ContentSize = new CCSize(subIconFrame.ContentSize.Height * scalingImg, subIconFrame.ContentSize.Height * scalingImg);
                subIconFrame.PositionX = parentSprite.ContentSize.Width / 2f;
                subIconFrame.PositionY = parentSprite.ContentSize.Height / 2f + parentSprite.ContentSize.Height * 0.075f;
                subIconFrame.Tag = SpriteTypes.ImageTag;
                subIconFrame.Color = CCColor3B.White;
            }

            var label = new CCLabel(iconSpeechText, "Arial", 18, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.Black,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                ContentSize = new CCSize(parentSprite.ContentSize.Width, parentSprite.ContentSize.Height * 0.25f),
                Position = new CCPoint(parentSprite.ContentSize.Width / 2, parentSprite.ContentSize.Height * 0.1f),
                Scale = textScale,
                Visible = textVisible,
                Tag = SpriteTypes.ContentTag
            };

            parentSprite.AddChild(subIconFrame);
            parentSprite.AddChild(label);

            return parentSprite;
        }

        /// <summary>
        /// Async call to create folder sprite
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="backing"></param>
        /// <param name="base64string"></param>
        /// <param name="folderName"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="scale"></param>
        /// <param name="textScale"></param>
        /// <param name="textVisible"></param>
        /// <returns></returns>
        public Task<CCSprite> AsyncCreateFolder(CCSpriteFrame frame, CCSpriteFrame backing, string base64string, string folderName, float positionX, float positionY, float scale, float textScale, bool textVisible)
        {
            TaskCompletionSource<CCSprite> tcs = new TaskCompletionSource<CCSprite>();

            tcs.SetResult(MakeFolder(frame, backing, base64string, folderName, positionX, positionY, scale, textScale, textVisible));

            return tcs.Task;
        }

        /// <summary>
        /// Construct and return folder frame
        /// </summary>
        /// <param name="assetName">name of folder sprite</param>
        /// <param name="folderName">name of folder</param>
        /// <param name="positionX">x pos</param>
        /// <param name="positionY">y pos</param>
        /// <param name="scale">scaled size</param>
        /// <returns></returns>
        private CCSprite MakeFolder(CCSpriteFrame frame, CCSpriteFrame backing, string base64string, string folderName, float positionX, float positionY, float scale, float textScale, bool textVisible)
        {
            var parentSprite = new CCSprite(backing);
            parentSprite.PositionX = positionX;
            parentSprite.PositionY = positionY;

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);            
            parentSprite.Scale = scale;
            parentSprite.Tag = SpriteTypes.FolderTag;

            var label = new CCLabel(folderName, "Arial", 18, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.White,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                ContentSize = new CCSize(parentSprite.ContentSize.Width, parentSprite.ContentSize.Height * 0.25f),
                Position = new CCPoint(parentSprite.ContentSize.Width / 2, parentSprite.ContentSize.Height / 3),
                Tag = SpriteTypes.ContentTag,
                Scale = textScale,
                Visible = textVisible
            };

            CCSprite subIconFrame = null;

            // If passed a blank base64, assume its meant to be just white
            if (base64string != null && base64string != "")
            {
                byte[] bytes = System.Convert.FromBase64String(base64string);
                var testTexture = new CCTexture2D(bytes);
                var testFrame = new CCSpriteFrame(testTexture, new CCRect(0, 0, testTexture.PixelsWide, testTexture.PixelsHigh));

                var dimToScale = System.Math.Max(testFrame.ContentSize.Width, testFrame.ContentSize.Height);
                var scalingImg = (parentSprite.ContentSize.Height * 0.75f) / dimToScale;

                subIconFrame = new CCSprite(testFrame)
                {
                    AnchorPoint = CCPoint.AnchorMiddle,
                    ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.9f, parentSprite.ContentSize.Height * 0.9f),
                    PositionX = parentSprite.ContentSize.Width / 2,
                    PositionY = parentSprite.ContentSize.Height / 2,
                };

                label.Color = CCColor3B.Black;
            }
            else if (frame != null)
            {
                subIconFrame = new CCSprite(frame)
                {
                    AnchorPoint = CCPoint.AnchorMiddle,
                    ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.9f, parentSprite.ContentSize.Height * 0.9f),
                    PositionX = parentSprite.ContentSize.Width / 2,
                    PositionY = parentSprite.ContentSize.Height / 2,
                };
            }

            parentSprite.AddChild(subIconFrame);
            parentSprite.AddChild(label);
            parentSprite.ReorderChild(label, 999);

            return parentSprite;
        }
    }
}
