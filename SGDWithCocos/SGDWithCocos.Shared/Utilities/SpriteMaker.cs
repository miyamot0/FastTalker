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
    public class SpriteMaker
    {
        public float DynamicWidth = 0f,
                     DynamicHeight = 0f,
                     padding = 10,
                     margin = 10,
                     adjustment = 1f;

        private const float sentenceFrameWidth = 0.75f,
                            sentenceFrameHeight = 0.2f;

        private float dimension = -1f;

        public SpriteMaker() { }

        public SpriteMaker(float dynamicWidth, float dynamicHeight)
        {
            DynamicWidth = dynamicWidth;
            DynamicHeight = dynamicHeight;

            dimension = (DynamicWidth * 0.1f);
        }

        public CCSprite MakeSentenceFrame()
        {
            var sentenceFrame = new CCSprite("frameWhite");
            sentenceFrame.ContentSize = new CCSize(DynamicWidth * sentenceFrameWidth - padding, DynamicHeight * sentenceFrameHeight);
            sentenceFrame.PositionX = 0 + (sentenceFrame.ContentSize.Width / 2) + padding;
            sentenceFrame.PositionY = DynamicHeight - (sentenceFrame.ContentSize.Height / 2) - padding;

            return sentenceFrame;
        }

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
