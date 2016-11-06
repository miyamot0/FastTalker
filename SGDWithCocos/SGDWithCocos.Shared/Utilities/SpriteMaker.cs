//----------------------------------------------------------------------------------------------
// <copyright file="SpriteMaker.cs" 
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

using CocosSharp;
using SGDWithCocos.Tags;

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
        public CCSprite MakeSentenceFrame()
        {
            var sentenceFrame = new CCSprite("frameWhite");
            sentenceFrame.ContentSize = new CCSize(DynamicWidth * sentenceFrameWidth - padding, DynamicHeight * sentenceFrameHeight);
            sentenceFrame.PositionX = 0 + (sentenceFrame.ContentSize.Width / 2) + padding;
            sentenceFrame.PositionY = DynamicHeight - (sentenceFrame.ContentSize.Height / 2) - padding;

            return sentenceFrame;
        }

        /// <summary>
        /// Construct and return speaker icon sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeSpeakerFrame()
        {
            var speakerFrame = new CCSprite("Speaker_Icon");

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
        public CCSprite MakeAddButton()
        {
            var parentSprite = new CCSprite("BlankFrame")
            {
                Tag = SpriteTypes.AddTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);

            parentSprite.PositionX = DynamicWidth - (parentSprite.ContentSize.Width / 2) - padding;
            parentSprite.PositionY = (parentSprite.ContentSize.Height / 2) + padding;

            var subIconFrame = new CCSprite("AddNew")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.5f, parentSprite.ContentSize.Height * 0.5f)
            };

            subIconFrame.PositionX = parentSprite.ContentSize.Width - (subIconFrame.ContentSize.Width);
            subIconFrame.PositionY = parentSprite.ContentSize.Height - (subIconFrame.ContentSize.Height);

            var label = new CCLabel("Add Icon", "Arial", 18, CCLabelFormat.SystemFont)
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
        /// Construct and return "Add Folder" sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeAddFolderButton()
        {
            var parentSprite = new CCSprite("BlankFrame")
            {
                Tag = SpriteTypes.FolderTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);
            parentSprite.PositionX = DynamicWidth - (parentSprite.ContentSize.Width / 2) - padding;
            parentSprite.PositionY = parentSprite.ContentSize.Height * 2 + (parentSprite.ContentSize.Height / 2) + padding * 2 + margin;

            var subIconFrame = new CCSprite("FolderClosed")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.5f, parentSprite.ContentSize.Height * 0.5f)
            };

            subIconFrame.PositionX = parentSprite.ContentSize.Width - (subIconFrame.ContentSize.Width);
            subIconFrame.PositionY = parentSprite.ContentSize.Height - (subIconFrame.ContentSize.Height);

            var label = new CCLabel("Add Folder", "Arial", 18, CCLabelFormat.SystemFont)
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
        public CCSprite MakeRemoveButton()
        {
            var parentSprite = new CCSprite("BlankFrame")
            {
                Tag = SpriteTypes.RemoveTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);
            parentSprite.PositionX = (parentSprite.ContentSize.Width / 2) + padding;
            parentSprite.PositionY = (parentSprite.ContentSize.Height / 2) + padding;

            var subIconFrame = new CCSprite("Trash")
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
        /// Construct and return "Take Photo" sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite TakePhotoButton()
        {
            var parentSprite = new CCSprite("BlankFrame")
            {
                Tag = SpriteTypes.AddTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);

            parentSprite.PositionX = DynamicWidth - (parentSprite.ContentSize.Width / 2) - padding;
            parentSprite.PositionY = parentSprite.ContentSize.Height + (parentSprite.ContentSize.Height / 2) + padding + margin;

            var subIconFrame = new CCSprite("CameraIcon")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.5f, parentSprite.ContentSize.Height * 0.5f),
            };

            subIconFrame.PositionX = parentSprite.ContentSize.Width - (subIconFrame.ContentSize.Width);
            subIconFrame.PositionY = parentSprite.ContentSize.Height - (subIconFrame.ContentSize.Height);

            var label = new CCLabel("Take Photo", "Arial", 18, CCLabelFormat.SystemFont)
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
        /// Construct and return "Single" frame sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeSingleButton()
        {
            var parentSprite = new CCSprite("BlankFrame")
            {
                Tag = SpriteTypes.AddTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);
            parentSprite.PositionX = (parentSprite.ContentSize.Width / 2) + padding;
            parentSprite.PositionY = parentSprite.ContentSize.Height * 2 + (parentSprite.ContentSize.Height / 2) + padding * 2 + margin;

            var subIconFrame = new CCSprite("Single")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.5f, parentSprite.ContentSize.Height * 0.5f)
            };

            subIconFrame.PositionX = parentSprite.ContentSize.Width - (subIconFrame.ContentSize.Width);
            subIconFrame.PositionY = parentSprite.ContentSize.Height - (subIconFrame.ContentSize.Height);

            var label = new CCLabel("Single Mode", "Arial", 18, CCLabelFormat.SystemFont)
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
        /// Construct and return "Multi" frame sprite
        /// </summary>
        /// <returns></returns>
        public CCSprite MakeMultiButton()
        {
            var parentSprite = new CCSprite("BlankFrame")
            {
                Tag = SpriteTypes.AddTag
            };

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);
            parentSprite.PositionX = (parentSprite.ContentSize.Width / 2) + padding;
            parentSprite.PositionY = parentSprite.ContentSize.Height + (parentSprite.ContentSize.Height / 2) + padding + margin;

            var subIconFrame = new CCSprite("Frame")
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.5f, parentSprite.ContentSize.Height * 0.5f)
            };

            subIconFrame.PositionX = parentSprite.ContentSize.Width - (subIconFrame.ContentSize.Width);
            subIconFrame.PositionY = parentSprite.ContentSize.Height - (subIconFrame.ContentSize.Height);

            var label = new CCLabel("Multi Mode", "Arial", 18, CCLabelFormat.SystemFont)
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
        public CCSprite MakeTitleIcon(string letter, float positionX, float positionY)
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
        public CCSprite MakeIconBase64(string base64string, string iconSpeechText, float positionX, float positionY, float scale, float textScale, bool textVisible)
        {
            scale = (scale == -1) ? 1 : scale;

            var parentSprite = new CCSprite("BlankFrame")
            {
                PositionX = positionX,
                PositionY = positionY,
                ContentSize = new CCSize(dimension, dimension),
                Scale = scale,
                Tag = SpriteTypes.IconTag
            };

            byte[] bytes = System.Convert.FromBase64String(base64string);
                var testTexture = new CCTexture2D(bytes);
                var testFrame = new CCSpriteFrame(testTexture, new CCRect(0, 0, testTexture.PixelsWide, testTexture.PixelsHigh));

            var dimToScale = System.Math.Max(testFrame.ContentSize.Width, testFrame.ContentSize.Height);
            var scalingImg =  (parentSprite.ContentSize.Height * 0.75f) / dimToScale;

            var subIconFrame = new CCSprite(testFrame)
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(testFrame.ContentSize.Width * scalingImg, testFrame.ContentSize.Height * scalingImg),
                PositionX = parentSprite.ContentSize.Width / 2f,
                PositionY = parentSprite.ContentSize.Height / 2f + parentSprite.ContentSize.Height * 0.075f,
                Tag = SpriteTypes.ImageTag
            };
            
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
        /// Construct and return folder frame
        /// </summary>
        /// <param name="assetName">name of folder sprite</param>
        /// <param name="folderName">name of folder</param>
        /// <param name="positionX">x pos</param>
        /// <param name="positionY">y pos</param>
        /// <param name="scale">scaled size</param>
        /// <returns></returns>
        public CCSprite MakeFolder(string assetName, string folderName, float positionX, float positionY, float scale)
        {
            var parentSprite = new CCSprite("BlankFrame");
            parentSprite.PositionX = positionX;
            parentSprite.PositionY = positionY;

            var scaling = (DynamicWidth * 0.1f) / parentSprite.ContentSize.Width;
            parentSprite.ContentSize = new CCSize(parentSprite.ContentSize.Width * scaling, parentSprite.ContentSize.Height * scaling);
            
            parentSprite.Scale = scale;
            parentSprite.Tag = SpriteTypes.FolderTag;

            var subIconFrame = new CCSprite(assetName)
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.9f, parentSprite.ContentSize.Height * 0.9f),
                PositionX = parentSprite.ContentSize.Width / 2,
                PositionY = parentSprite.ContentSize.Height / 2,
            };

            var label = new CCLabel(folderName, "Arial", 18, CCLabelFormat.SystemFont)
            {                
                Color = CCColor3B.White,
                AnchorPoint = CCPoint.AnchorMiddle,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                ContentSize = new CCSize(parentSprite.ContentSize.Width, parentSprite.ContentSize.Height * 0.25f),
                Position = new CCPoint(parentSprite.ContentSize.Width / 2, parentSprite.ContentSize.Height / 3),
                Tag = SpriteTypes.ContentTag
            };

            parentSprite.AddChild(subIconFrame);
            parentSprite.AddChild(label);

            return parentSprite;
        }
    }
}
