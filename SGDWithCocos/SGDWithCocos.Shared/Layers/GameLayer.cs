//----------------------------------------------------------------------------------------------
// <copyright file="GameLayer.cs" 
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
using System.Collections.Generic;

using CocosSharp;
using SGDWithCocos.Interface;
using Xamarin.Forms;
using System.Linq;
using SGDWithCocos.Models;
using SGDWithCocos.Tags;
using SGDWithCocos.Utilities;
using Newtonsoft.Json;
using SGDWithCocos.Shared.Pages;

// TODO
// Load and/or Selection window
// 1 - Add new/Load existing

// Intro window, first time use
// 1 - Hold speaker for 10 s and release for add/edit mode
// 2 - Add downloaded pictures from gallery/album 
// 3 - Take a picture with camera
// 4 - Resize images and words as needed!

namespace SGDWithCocos.Shared.Layers
{
    public class GameLayer : CCLayerColor
    {
        GamePage GamePageParent { get; set; }

        CCEventListenerTouchOneByOne mListener;

        SpriteMaker spriteModelFactory;

        CCSprite sentenceFrame,
                 speakerFrame,
                 deleteFrame,
                 addFrame,
                 addFolderFrame,
                 takePhotoFrame,
                 singleFrame,
                 multiFrame,
                 closeButton = null,
                 windowFrame = null,
                 CurrentSpriteTouched = null;

        Tag touchType = Tags.Tag.None;

        IconReference tempWindow;

        List<IconReference> iconList2;
        List<StoredIconReference> storedList;

        Random mRandom;

        DateTime startTime,
                 endTime;

        CCColor3B White = new CCColor3B(255, 255, 255),
                  Red = new CCColor3B(255, 0, 0),
                  Green = new CCColor3B(0, 255, 0),
                  LightBlue = new CCColor3B(0, 0, 155);

        bool inEditMode = false,
             inSingleMode = false,
             isModal = false;

        float totalDuration = 0f,
              saveInterval = 60f;

        public GameLayer(float _dynamicWidth, float _dynamicHeight, IconStorageObject json, GamePage _gamePage) : base(CCColor4B.Gray)
        {
            this.Color = CCColor3B.Gray;
            this.GamePageParent = _gamePage;
            this.mRandom = new Random(DateTime.Now.Millisecond);
            
            spriteModelFactory = new SpriteMaker(_dynamicWidth, _dynamicHeight);

            MakeListener();

            MakeStaticSprites();

            LoadSprites(json);          

            Schedule(RunGameLogic);
        }

        #region Draw Methods

        public void MakeListener()
        {
            mListener = new CCEventListenerTouchOneByOne();
            mListener.IsSwallowTouches = true;
            mListener.OnTouchBegan = OnTouchBegan;
            mListener.OnTouchEnded = OnTouchesEnded;
            mListener.OnTouchMoved = HandleTouchesMoved;
        }

        public void MakeStaticSprites()
        {
            sentenceFrame = spriteModelFactory.MakeSentenceFrame();
                AddChild(sentenceFrame, 0, SpriteTypes.FrameTag);

            speakerFrame = spriteModelFactory.MakeSpeakerFrame();
                AddEventListener(mListener, speakerFrame);
                AddChild(speakerFrame, 0, SpriteTypes.SpeakerTag);

            addFrame = spriteModelFactory.MakeAddButton();
            addFrame.Visible = false;
                AddEventListener(mListener.Copy(), addFrame);
                AddChild(addFrame, 0, SpriteTypes.AddTag);

            takePhotoFrame = spriteModelFactory.TakePhotoButton();
            takePhotoFrame.Visible = false;
                AddEventListener(mListener.Copy(), takePhotoFrame);
                AddChild(takePhotoFrame, 0, SpriteTypes.TakePhotoTag);

            addFolderFrame = spriteModelFactory.MakeAddFolderButton();
            addFolderFrame.Visible = false;
                AddEventListener(mListener.Copy(), addFolderFrame);
                AddChild(addFolderFrame, 0, SpriteTypes.FolderTag);

            singleFrame = spriteModelFactory.MakeSingleButton();
            singleFrame.Visible = false;
                AddEventListener(mListener.Copy(), singleFrame);
                AddChild(singleFrame, 0, SpriteTypes.SingleModeTag);

            multiFrame = spriteModelFactory.MakeMultiButton();
            multiFrame.Visible = false;
                AddEventListener(mListener.Copy(), multiFrame);
                AddChild(multiFrame, 0, SpriteTypes.MultiModeTag);

            deleteFrame = spriteModelFactory.MakeRemoveButton();
            deleteFrame.Visible = false;
                AddChild(deleteFrame, 0, SpriteTypes.RemoveTag);
        }

        public void LoadSprites(IconStorageObject json)
        {
            iconList2 = new List<IconReference>();
            storedList = new List<StoredIconReference>();

            if (json != null)
            {
                foreach (IconModel icon in json.Icons)
                {
                    if (icon.Tag == SpriteTypes.IconTag)
                    {
                        var newIcon = spriteModelFactory.MakeIconBase64(icon.Base64, icon.Text, icon.X, icon.Y, icon.Scale, icon.TextScale, icon.TextVisible);

                        iconList2.Add(new IconReference(newIcon, icon.Base64, 1f, true));
                        AddEventListener(mListener.Copy(), newIcon);
                    }
                }

                foreach (FolderModel icon in json.Folders)
                {
                    if (icon.Tag == SpriteTypes.FolderTag)
                    {
                        var newIcon = spriteModelFactory.MakeFolder(icon.AssetName, icon.Text, icon.X, icon.Y, icon.Scale);
                        newIcon.Tag = SpriteTypes.FolderTag;
                        iconList2.Add(new IconReference(newIcon, icon.AssetName, icon.TextScale, icon.TextVisible));
                        AddEventListener(mListener.Copy(), newIcon);
                    }
                }

                foreach (StoredIconModel icon in json.StoredIcons)
                {
                    var newIcon = spriteModelFactory.MakeIconBase64(icon.Base64, icon.Text, icon.X, icon.Y, icon.Scale, icon.TextScale, icon.TextVisible);
                    var storedIconRef = new StoredIconReference(newIcon, icon.Base64, icon.Folder, icon.Scale, icon.TextScale, icon.TextVisible);

                    storedList.Add(storedIconRef);
                }
                
                SetSingleMode(json.SingleMode);
            }

            var counter = 1;

            foreach (IconReference icon in iconList2)
            {
                AddChild(icon.Sprite, counter, icon.Sprite.Tag);
                counter++;
            }

        }

        public void SetSingleMode(bool status)
        {
            inSingleMode = status;

            if (status)
            {
                DrawSingleField();
            }
            else
            {
                DrawFramedField();
            }
        }

        public void SetEditMode(bool edit)
        {
            if (edit)
            {
                this.Color = CCColor3B.Orange;
                addFrame.Visible = true;
                takePhotoFrame.Visible = true;
                addFolderFrame.Visible = true;
                deleteFrame.Visible = true;
                singleFrame.Visible = true;
                multiFrame.Visible = true;

                inEditMode = !inEditMode;
            }
            else
            {
                this.Color = CCColor3B.Gray;
                addFrame.Visible = false;
                takePhotoFrame.Visible = false;
                addFolderFrame.Visible = false;
                deleteFrame.Visible = false;
                singleFrame.Visible = false;
                multiFrame.Visible = false;

                inEditMode = !inEditMode;
            }
        }

        public void DrawSingleField()
        {
            sentenceFrame.Visible = false;

            singleFrame.Color = Green;
            multiFrame.Color = White;
        }

        public void DrawFramedField()
        {
            sentenceFrame.Visible = true;

            singleFrame.Color = White;
            multiFrame.Color = Green;
        }

        public void CallBackIcon(string base64, string text, string extension)
        {
            ScheduleOnce((dt) => {
                if (base64 != "" && text != "")
                {
                    var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));
                    var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));

                    var newIcons = spriteModelFactory.MakeIconBase64(base64, text, xLocation, yLocation, 1f, 1f, true);
                    var mIconRef = new IconReference(newIcons, base64, 1f, true);
                    iconList2.Add(mIconRef);

                    AddEventListener(mListener.Copy(), mIconRef.Sprite);
                    AddChild(mIconRef.Sprite, iconList2.Count, SpriteTypes.IconTag);
                }
            }, 0);
        }

        public void MakeIconFolder(string assetName, string folderName)
        {
            ScheduleOnce((dt) => {

                var parentSprite = spriteModelFactory.MakeFolder(assetName, folderName, 400, 400, 1f);

                var mIconRef = new IconReference(parentSprite, assetName, 1f, true);
                iconList2.Add(mIconRef);

                AddEventListener(mListener.Copy(), mIconRef.Sprite);
                AddChild(mIconRef.Sprite, iconList2.Count, SpriteTypes.FolderTag);

            }, 0);
        }
        
        #endregion

        #region Update Methods

        public void UpdateSpriteText(int index, string text)
        {
            ScheduleOnce((dt) => {
                if (index != -1 && text != "")
                {
                    var mIconRef = iconList2[index];
                    var mSprite = mIconRef.Sprite;
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (mContent != null)
                    {
                        if (mSprite.Tag == SpriteTypes.FolderTag)
                        {
                            for (var i = storedList.Count - 1; i >= 0; i--)
                            {
                                if (storedList[i].FolderName == mContent.Text)
                                {
                                    var newStoredIconRef = new StoredIconReference(storedList[i].Sprite, 
                                        storedList[i].Base64, 
                                        text, 
                                        storedList[i].Sprite.ScaleX,
                                        storedList[i].TextScale,
                                        storedList[i].TextVisible);

                                    storedList.RemoveAt(i);

                                    storedList.Add(newStoredIconRef);
                                }
                            }
                        }

                        mContent.Text = text;
                    }
                }
            }, 0);
        }

        // TODO
        public void ToggleSpriteImage(int index)
        {
            ScheduleOnce((dt) => {
                if (index != -1)
                {
                    var mIconRef = iconList2[index];
                    var mSprite = mIconRef.Sprite;
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ImageTag) as CCSprite;

                    mContent.Visible = !mSprite.Visible;
                }
            }, 0);
        }

        public void ToggleSpriteText(int index)
        {
            ScheduleOnce((dt) => {
                if (index != -1)
                {
                    var mIconRef = iconList2[index];
                    var mSprite = mIconRef.Sprite;
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (mContent != null)
                    {
                        mContent.Visible = !mContent.Visible;
                    }
                }
            }, 0);
        }

        public void UpdateSpriteTextSize(int index, int action)
        {
            ScheduleOnce((dt) => {
                if (index != -1 && action != EditTypes.None)
                {
                    var mIconRef = iconList2[index];
                    var mSprite = mIconRef.Sprite;
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (mContent != null)
                    {
                        switch(action)
                        {
                            case EditTypes.UpdateLabelSizeUp:
                                mContent.AddAction(new CCScaleBy(0.2f, 1.1f));
                                break;

                            case EditTypes.UpdateLabelSizeDefault:
                                mContent.AddAction(new CCScaleTo(0.2f, 1f));
                                break;

                            case EditTypes.UpdateLabelSizeDown:
                                mContent.AddAction(new CCScaleBy(0.2f, 0.9f));
                                break;
                        }

                        if (mSprite.Tag == SpriteTypes.FolderTag)
                        {
                            for (var i = storedList.Count - 1; i >= 0; i--)
                            {
                                if (storedList[i].FolderName == mContent.Text)
                                {
                                    var newStoredIconRef = new StoredIconReference(storedList[i].Sprite,
                                        storedList[i].Base64,
                                        storedList[i].FolderName,
                                        storedList[i].Sprite.ScaleX,
                                        mContent.ScaleX,
                                        storedList[i].TextVisible);

                                    storedList.RemoveAt(i);
                                    storedList.Add(newStoredIconRef);
                                }
                            }
                        }
                    }
                }
            }, 0);
        }

        public void UpdateSpriteSize(int index, int edit)
        {
            ScheduleOnce((dt) => {
                if (index != -1 && edit != EditTypes.None)
                {
                    if (edit == EditTypes.UpdateSizeUp)
                    {
                        var mIconRef = iconList2[index];
                        var mSprite = mIconRef.Sprite;

                        var action = new CCScaleBy(0.5f, 1.1f);

                        mSprite.AddAction(action);

                    }
                    else if (edit == EditTypes.UpdateSizeDown)
                    {
                        var mIconRef = iconList2[index];
                        var mSprite = mIconRef.Sprite;

                        var action = new CCScaleBy(0.5f, 0.9f);

                        mSprite.AddAction(action);
                    }
                    else if (edit == EditTypes.UpdateSizeDefault)
                    {
                        var mIconRef = iconList2[index];
                        var mSprite = mIconRef.Sprite;

                        var action = new CCScaleTo(0.5f, 1f);

                        mSprite.AddAction(action);
                    }
                }
            }, 0);
        }

        public void RemoveCurrentIcon()
        {
            RemoveChild(CurrentSpriteTouched);
        }

        public void ClearWindow()
        {
            iconList2.Remove(tempWindow);

            windowFrame.RemoveChild(closeButton);

            RemoveChild(windowFrame);

            windowFrame = closeButton = null;
            tempWindow = null;
        }

        public void DeSelectIcons()
        {
            foreach (IconReference iconRef in iconList2)
            {
                if (iconRef.Sprite.Color != White)
                {
                    iconRef.Sprite.Color = White;
                }
            }
        }

        public void ShowWindow(CCSprite currentSprite, string folderName)
        {
            if (isModal) return;

            windowFrame = new CCSprite("BlankFrame")
            {
                PositionX = currentSprite.Position.X,
                PositionY = currentSprite.Position.Y,
                Tag = SpriteTypes.WindowTag
            };

            var scaling = (spriteModelFactory.DynamicWidth * 0.1f) / windowFrame.ContentSize.Width;
            windowFrame.ContentSize = new CCSize(windowFrame.ContentSize.Width * scaling, windowFrame.ContentSize.Height * scaling);

            closeButton = new CCSprite("IconClose");
            closeButton.ContentSize = new CCSize(windowFrame.ContentSize.Width * 0.075f, windowFrame.ContentSize.Width * 0.075f);
            closeButton.PositionX = windowFrame.ContentSize.Width - closeButton.ContentSize.Width / 2 - 3;
            closeButton.PositionY = windowFrame.ContentSize.Height - closeButton.ContentSize.Height / 2 - 3;
            closeButton.Tag = SpriteTypes.CloseWindowTag;


            AddEventListener(mListener.Copy(), closeButton);

            ScheduleOnce((dt) => {

                windowFrame.AddChild(closeButton, 1001, SpriteTypes.CloseWindowTag);

                lock (storedList)
                {
                    var mEqualList = storedList.Where(l => l.FolderName == folderName).ToList();

                    for (var i = 0; i < mEqualList.Count; i++)
                    {
                        // Pull respective icon
                        var mStoredIconRef = mEqualList[i];

                        var mSprite = mStoredIconRef.Sprite;
                        var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        if (mContent != null)
                        {
                            var parentSprite = new CCSprite("BlankFrame");

                            parentSprite.ContentSize = new CCSize(windowFrame.ContentSize.Width * 0.25f, windowFrame.ContentSize.Height * 0.25f);

                            var pSpacing = parentSprite.ContentSize.Width * 0.15f;
                            var xSpacing = parentSprite.ContentSize.Width * ((i % 3)) + pSpacing * ((i % 3) + 1);
                            var ySpacing = parentSprite.ContentSize.Height * ((i / 3)) + pSpacing * ((i / 3) + 1);

                            parentSprite.PositionX = (parentSprite.ContentSize.Width * 0.5f) + xSpacing;
                            parentSprite.PositionY = windowFrame.ContentSize.Height - (parentSprite.ContentSize.Height * 0.5f) - ySpacing;

                            parentSprite.Tag = SpriteTypes.IconTag;

                            mStoredIconRef.Sprite.ContentSize = parentSprite.ContentSize;
                            mStoredIconRef.Sprite.PositionX = parentSprite.PositionX;
                            mStoredIconRef.Sprite.PositionY = parentSprite.PositionY;

                            byte[] bytes = Convert.FromBase64String(mStoredIconRef.Base64);
                            var testTexture = new CCTexture2D(bytes);
                            var testFrame = new CCSpriteFrame(testTexture, new CCRect(0, 0, testTexture.PixelsWide, testTexture.PixelsHigh));
                            var subIconFrame = new CCSprite(testFrame)
                            {
                                AnchorPoint = CCPoint.AnchorMiddle,
                                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.75f, parentSprite.ContentSize.Height * 0.75f),
                                PositionX = parentSprite.ContentSize.Width * 0.5f,
                                PositionY = parentSprite.ContentSize.Height * 0.5f + parentSprite.ContentSize.Height * 0.075f,
                                Tag = SpriteTypes.ImageTag
                            };

                            var label = new CCLabel(mContent.Text, "Arial", 22, CCLabelFormat.SystemFont)
                            {
                                Scale = 0.25f * scaling,
                                AnchorPoint = CCPoint.AnchorMiddle,
                                HorizontalAlignment = CCTextAlignment.Center,
                                VerticalAlignment = CCVerticalTextAlignment.Center,
                                PositionX = parentSprite.ContentSize.Width * 0.5f,
                                PositionY = parentSprite.ContentSize.Height * 0.075f,
                                Color = CCColor3B.Black,
                                Visible = mStoredIconRef.TextVisible,
                                Tag = SpriteTypes.ContentTag
                            };

                            parentSprite.AddChild(subIconFrame);
                            AddEventListener(mListener.Copy(), parentSprite);

                            windowFrame.AddChild(parentSprite, 1001, SpriteTypes.IconTag);
                            parentSprite.AddChild(label);
                        }
                    }
                }

                AddEventListener(mListener.Copy(), windowFrame);
                tempWindow = new IconReference(windowFrame, "Window", 1f, true);
                iconList2.Add(tempWindow);
                AddChild(windowFrame, 1000, SpriteTypes.WindowTag);

                var moveAction = new CCMoveTo(0.2f, new CCPoint(spriteModelFactory.DynamicWidth / 2f, spriteModelFactory.DynamicHeight / 2f));

                var dimension = Math.Min(spriteModelFactory.DynamicHeight, spriteModelFactory.DynamicWidth);
                var scale = (dimension / windowFrame.ContentSize.Width) * 0.9f;
                var scaleAction = new CCScaleTo(0.2f, scale);

                windowFrame.AddActions(false, moveAction, scaleAction);
                isModal = true;
            }, 0);
        }

        public static string SpriteHasLabel(CCSprite sprite)
        {
            var contentTag = sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

            if (contentTag != null)
            {
                return contentTag.Text;
            }
            else
            {
                return "";
            }
        }

        #endregion
        
        #region ListenerDelegates

        bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            CCSprite caller = touchEvent.CurrentTarget as CCSprite;

            if (caller.Tag == SpriteTypes.IconTag)
            {
                ReorderChild(caller, 999);
            }

            CurrentSpriteTouched = null;
            touchType = Tags.Tag.None;

            startTime = DateTime.Now;

            if (windowFrame != null)
            {
                if (windowFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    if (closeButton.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                    {
                        isModal = false;

                        var scaleAction = new CCScaleTo(0.2f, 0.05f);
                        var functionAction = new CCCallFunc(ClearWindow);
                        windowFrame.AddActions(false, scaleAction, functionAction);

                        return true;
                    }

                    var mIcons = windowFrame.Children;

                    foreach (var mIcon in mIcons)
                    {
                        if (mIcon.Tag == SpriteTypes.IconTag && mIcon.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                        {
                            touchType = Tags.Tag.TransitionIcon;

                            var mContent = mIcon.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                            if (mContent != null)
                            {
                                StoredIconReference mStoredRef = null;
                                foreach (StoredIconReference storedRef in storedList)
                                {
                                    var mLoopSprite = storedRef.Sprite;
                                    var mLoopContent = mLoopSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                                    if (mLoopContent != null && mLoopContent.Text == mContent.Text)
                                    {
                                        var xMin = (spriteModelFactory.DynamicHeight * 0.1f) / 2;
                                        var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));
                                        var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));

                                        var newIcon = spriteModelFactory.MakeIconBase64(storedRef.Base64, mLoopContent.Text,
                                             xLocation, yLocation, storedRef.Scale, storedRef.TextScale, storedRef.TextVisible);

                                        var mIconRef = new IconReference(newIcon, storedRef.Base64, 1f, true);

                                        AddEventListener(mListener.Copy(), mIconRef.Sprite);
                                        iconList2.Add(mIconRef);

                                        AddChild(mIconRef.Sprite);

                                        mStoredRef = storedRef;
                                    }
                                }

                                if (mStoredRef != null)
                                {
                                    storedList.Remove(mStoredRef);
                                }
                            }

                            isModal = false;

                            iconList2.Remove(tempWindow);

                            windowFrame.RemoveChild(closeButton);

                            RemoveChild(windowFrame);

                            windowFrame = closeButton = null;
                            tempWindow = null;

                            return true;
                        }
                    }

                    return false;
                }
            }

            #region ButtonSpecific Listeners

            if (caller.GetHashCode() == speakerFrame.GetHashCode())
            {
                foreach (IconReference iconRef in iconList2)
                {
                    var rect = iconRef.Sprite.BoundingBoxTransformedToWorld;

                    if (speakerFrame.BoundingBoxTransformedToParent.IntersectsRect(rect))
                    {
                        return false;
                    }
                }

                if (speakerFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.Speak;
                    CurrentSpriteTouched = speakerFrame;
                    caller.Opacity = 155;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (caller.GetHashCode() == addFrame.GetHashCode())
            {
                foreach (IconReference iconRef in iconList2)
                {
                    var rect = iconRef.Sprite.BoundingBoxTransformedToWorld;

                    if (addFrame.BoundingBoxTransformedToParent.IntersectsRect(rect))
                    {
                        return false;
                    }
                }

                if (addFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.Add;
                    CurrentSpriteTouched = addFrame;
                    caller.Opacity = 155;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (caller.GetHashCode() == takePhotoFrame.GetHashCode())
            {
                foreach (IconReference iconRef in iconList2)
                {
                    var rect = iconRef.Sprite.BoundingBoxTransformedToWorld;

                    if (takePhotoFrame.BoundingBoxTransformedToParent.IntersectsRect(rect))
                    {
                        return false;
                    }
                }

                if (takePhotoFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.TakePhoto;
                    CurrentSpriteTouched = takePhotoFrame;
                    caller.Opacity = 155;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (caller.GetHashCode() == addFolderFrame.GetHashCode())
            {
                foreach (IconReference iconRef in iconList2)
                {
                    var rect = iconRef.Sprite.BoundingBoxTransformedToWorld;

                    if (addFolderFrame.BoundingBoxTransformedToParent.IntersectsRect(rect))
                    {
                        return false;
                    }
                }

                if (addFolderFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.Folder;
                    CurrentSpriteTouched = addFolderFrame;
                    caller.Opacity = 155;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (caller.GetHashCode() == singleFrame.GetHashCode())
            {
                foreach (IconReference iconRef in iconList2)
                {
                    var rect = iconRef.Sprite.BoundingBoxTransformedToWorld;

                    if (singleFrame.BoundingBoxTransformedToParent.IntersectsRect(rect))
                    {
                        return false;
                    }
                }

                if (singleFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.SingleMode;
                    CurrentSpriteTouched = singleFrame;
                    caller.Opacity = 155;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (caller.GetHashCode() == multiFrame.GetHashCode())
            {
                foreach (IconReference iconRef in iconList2)
                {
                    var rect = iconRef.Sprite.BoundingBoxTransformedToWorld;

                    if (multiFrame.BoundingBoxTransformedToParent.IntersectsRect(rect))
                    {
                        return false;
                    }
                }

                if (multiFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.MultiMode;
                    CurrentSpriteTouched = multiFrame;
                    caller.Opacity = 155;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            #endregion

            foreach (IconReference iconRef in iconList2)
            {
                if (iconRef.Sprite.Tag == SpriteTypes.IconTag)
                {
                    if (inSingleMode)
                    {
                        if (caller.GetHashCode() == iconRef.Sprite.GetHashCode())
                        {
                            if (iconRef.Sprite.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                            {
                                touchType = Tags.Tag.Icon;

                                CurrentSpriteTouched = iconRef.Sprite;
                                caller.Opacity = 155;

                                DeSelectIcons();

                                caller.Color = Green;

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (caller.GetHashCode() == iconRef.Sprite.GetHashCode())
                        {
                            if (iconRef.Sprite.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                            {
                                touchType = Tags.Tag.Icon;
                                CurrentSpriteTouched = iconRef.Sprite;
                                caller.Opacity = 155;

                                if (!sentenceFrame.Visible)
                                {
                                    caller.Color = Green;
                                }

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                else if (iconRef.Sprite.Tag == SpriteTypes.FolderTag)
                {
                    if (caller.GetHashCode() == iconRef.Sprite.GetHashCode())
                    {
                        if (iconRef.Sprite.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                        {
                            touchType = Tags.Tag.FolderIcon;
                            CurrentSpriteTouched = iconRef.Sprite;
                            caller.Opacity = 155;

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;

        }

        void OnTouchesEnded(CCTouch touch, CCEvent touchEvent)
        {
            endTime = DateTime.Now;

            var timeDiff = endTime - startTime;

            if (CurrentSpriteTouched != null)
            {
                if (touchType == Tags.Tag.Icon)
                {
                    var target = (CCSprite)touchEvent.CurrentTarget;
                    CCRect rect = target.BoundingBoxTransformedToWorld;

                    List<IconReference> mFolders = iconList2.Where(t => t.Sprite.Tag == SpriteTypes.FolderTag).ToList();
                    var mIntersect = mFolders.Where(t => t.Sprite.BoundingBoxTransformedToParent.IntersectsRect(rect)).ToList();

                    if (mIntersect.Count == 1)
                    {
                        string mContentTag = SpriteHasLabel(mIntersect[0].Sprite);

                        if (mContentTag != "")
                        {
                            List<StoredIconReference> mInFolder = storedList.Where(t => t.FolderName == mContentTag).ToList();

                            if (mInFolder.Count < 9)
                            {
                                var mCloneCopy = iconList2.Where(t => t.Sprite.GetHashCode() == target.GetHashCode()).FirstOrDefault();

                                var spriteLabel = mCloneCopy.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                                if (spriteLabel == null)
                                {
                                    return;
                                }

                                var newItem = new StoredIconReference(mCloneCopy.Sprite, mCloneCopy.Base64, mContentTag, mCloneCopy.Sprite.ScaleX, spriteLabel.ScaleX, spriteLabel.Visible);

                                var savedScale = (float) mCloneCopy.Sprite.ScaleX;

                                CurrentSpriteTouched = null;

                                ScheduleOnce((dt) =>
                                {
                                    var moveAction = new CCMoveTo(0.2f, mIntersect[0].Sprite.Position);
                                    var scaleAction = new CCScaleTo(0.2f, 0.1f);
                                    var clearColor = new CCCallFuncN(node => node.Color = White);
                                    var setInvisible = new CCCallFuncN(node => node.Visible = false);
                                    var scaleAction2 = new CCScaleTo(0.01f, savedScale);
                                    var endAction = new CCCallFuncN(node => node.RemoveFromParent(true));

                                    target.AddActions(false,
                                        moveAction,
                                        scaleAction,
                                        clearColor,
                                        setInvisible,
                                        scaleAction2,
                                        endAction);

                                }, 0);

                                iconList2.Remove(mCloneCopy);
                                //newItem.Item1.Scale = savedScale;
                                storedList.Add(newItem);
                                //storedList.Add(trueClone);

                                return;
                            }
                        }
                    }
                    
                    if (deleteFrame.BoundingBoxTransformedToParent.IntersectsRect(rect) && inEditMode)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == target.GetHashCode()).FirstOrDefault();

                        ScheduleOnce((dt) => {
                            RemoveChild(target);
                        }, 0);

                        iconList2.Remove(mSprite);
                    }
                    else if (inEditMode && timeDiff.TotalSeconds < 0.25)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == target.GetHashCode()).FirstOrDefault();
                        var mCounter = iconList2.IndexOf(mSprite);

                        GamePageParent.CallActionSheet(mCounter);
                    }
                }
                else if (touchType == Tags.Tag.FolderIcon && inEditMode && timeDiff.TotalSeconds < 0.25)
                {
                    var target = (CCSprite)touchEvent.CurrentTarget;
                    var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == target.GetHashCode()).FirstOrDefault();
                    var mCounter = iconList2.IndexOf(mSprite);

                    GamePageParent.CallActionSheet(mCounter);
                }
                else if (touchType == Tags.Tag.FolderIcon && !inEditMode)
                {
                    string contentTag = SpriteHasLabel(CurrentSpriteTouched);

                    if (contentTag != "")
                    {
                        ShowWindow(CurrentSpriteTouched, contentTag);
                    }
                }
                else if (touchType == Tags.Tag.Add && inEditMode)
                {
                    GamePageParent.CallImagePicker();
                }
                else if (touchType == Tags.Tag.TakePhoto && inEditMode)
                {
                    GamePageParent.CallImageTaker();
                }
                else if (touchType == Tags.Tag.Folder && inEditMode)
                {
                    var mList = iconList2.Where(t => t.Sprite.Tag == SpriteTypes.FolderTag).ToList();

                    var nameList = new List<string>();

                    mList.ForEach(p =>
                    {
                        var returnedString = SpriteHasLabel(p.Sprite);

                        if (returnedString != "")
                        {
                            nameList.Add(returnedString.ToLower());
                        }
                    });

                    lock(nameList)
                    {
                        GamePageParent.GetFolderSetup(nameList);
                    }
                }
                else if (touchType == Tags.Tag.FolderIcon && inEditMode)
                {
                    var target = (CCSprite)touchEvent.CurrentTarget;
                    CCRect rect = target.BoundingBoxTransformedToWorld;

                    if (deleteFrame.BoundingBoxTransformedToParent.IntersectsRect(rect) && inEditMode)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == target.GetHashCode()).FirstOrDefault();

                        ScheduleOnce((dt) => {
                            RemoveChild(target);
                        }, 0);

                        iconList2.Remove(mSprite);
                    }
                    else if (timeDiff.TotalSeconds < 0.25f)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == target.GetHashCode()).FirstOrDefault();
                        var mCounter = iconList2.IndexOf(mSprite);
                        GamePageParent.CallActionSheet(mCounter);
                    }
                }
                else if (touchType == Tags.Tag.Speak && timeDiff.TotalSeconds < 5.0f)
                {
                    List<IconReference> mList = null;

                    if (!inSingleMode)
                    {
                        mList = iconList2.Where(t => sentenceFrame.BoundingBoxTransformedToParent.IntersectsRect(t.Sprite.BoundingBoxTransformedToParent) == true).ToList();
                    }
                    else
                    {
                        mList = iconList2.Where(t => t.Sprite.Color == Green).ToList();
                    }

                    if (mList.Count > 1)
                    {
                        mList = mList.OrderBy(i => i.Sprite.PositionX).ToList();
                    }

                    List<string> labelList = new List<string>();

                    foreach (IconReference mIcon in mList)
                    {
                        string contentTag = SpriteHasLabel(mIcon.Sprite);

                        if (contentTag != "")
                        {
                            labelList.Add(contentTag);
                        }
                    }

                    var outputString = string.Join(" ", labelList.ToArray());

                    DependencyService.Get<ITextToSpeech>().Speak(outputString);
                }
                else if (touchType == Tags.Tag.Speak && timeDiff.TotalSeconds >= 5.0f)
                {
                    SetEditMode(!inEditMode);                    
                }
                else if (touchType == Tags.Tag.SingleMode && inEditMode)
                {
                    SetSingleMode(true);
                }
                else if (touchType == Tags.Tag.MultiMode && inEditMode)
                {
                    SetSingleMode(false);
                }

                CurrentSpriteTouched = null;

                CCSprite caller = touchEvent.CurrentTarget as CCSprite;
                caller.Opacity = 255;                
            }
        }

        void HandleTouchesMoved(CCTouch touch, CCEvent touchEvent)
        {
            if (CurrentSpriteTouched != null)
            {
                if (touchType == Tags.Tag.Icon || (touchType == Tags.Tag.FolderIcon && inEditMode))
                {
                    var pos = touch.Location;

                    pos.X = (pos.X < 0 + CurrentSpriteTouched.ScaledContentSize.Width / 2) ? 0 + CurrentSpriteTouched.ScaledContentSize.Width / 2 : pos.X;
                    pos.Y = (pos.Y < 0 + CurrentSpriteTouched.ScaledContentSize.Height / 2) ? 0 + CurrentSpriteTouched.ScaledContentSize.Height / 2 : pos.Y;

                    pos.X = (pos.X > spriteModelFactory.DynamicWidth - CurrentSpriteTouched.ScaledContentSize.Width / 2) ? 
                        spriteModelFactory.DynamicWidth - CurrentSpriteTouched.ScaledContentSize.Width / 2 : pos.X;
                    pos.Y = (pos.Y > spriteModelFactory.DynamicHeight - CurrentSpriteTouched.ScaledContentSize.Height / 2) ? 
                        spriteModelFactory.DynamicHeight - CurrentSpriteTouched.ScaledContentSize.Height / 2 : pos.Y;

                    CurrentSpriteTouched.Position = pos;
                }
                else if (touchType == Tags.Tag.TransitionIcon)
                {
                    var pos = touch.Location;

                    pos.X = (pos.X < 0 + CurrentSpriteTouched.ScaledContentSize.Width / 2) ? 0 + CurrentSpriteTouched.ScaledContentSize.Width / 2 : pos.X;
                    pos.Y = (pos.Y < 0 + CurrentSpriteTouched.ScaledContentSize.Height / 2) ? 0 + CurrentSpriteTouched.ScaledContentSize.Height / 2 : pos.Y;

                    pos.X = (pos.X > spriteModelFactory.DynamicWidth - CurrentSpriteTouched.ScaledContentSize.Width / 2) ?
                        spriteModelFactory.DynamicWidth - CurrentSpriteTouched.ScaledContentSize.Width / 2 : pos.X;
                    pos.Y = (pos.Y > spriteModelFactory.DynamicHeight - CurrentSpriteTouched.ScaledContentSize.Height / 2) ?
                        spriteModelFactory.DynamicHeight - CurrentSpriteTouched.ScaledContentSize.Height / 2 : pos.Y;

                    CurrentSpriteTouched.Position = pos;

                }
            }
        }

        #endregion

        void RunGameLogic(float frameTimeInSeconds)
        {
            totalDuration += frameTimeInSeconds;

            if (totalDuration > saveInterval)
            {   
                lock(iconList2)
                {
                    lock(storedList)
                    {
                        GamePageParent.SaveBoards(iconList2, storedList, inSingleMode);
                    }
                }

                totalDuration = 0f;
            }

            foreach(var t in iconList2)
            {
                if (t.Sprite.Tag == SpriteTypes.IconTag)
                {
                    if (!inSingleMode)
                    {
                        if (!sentenceFrame.BoundingBoxTransformedToParent.IntersectsRect(t.Sprite.BoundingBoxTransformedToParent))
                        {
                            if (t.Sprite.Color != White)
                            {
                                t.Sprite.Color = White;
                            }
                        }
                        else
                        {
                            if (t.Sprite.Color != Green)
                            {
                                t.Sprite.Color = Green;
                            }
                        }
                    }

                    if (inEditMode)
                    {
                        if (deleteFrame.BoundingBoxTransformedToParent.IntersectsRect(t.Sprite.BoundingBoxTransformedToParent))
                        {
                            if (t.Sprite.Color != Red)
                            {
                                t.Sprite.Color = Red;
                            }
                        }
                        else
                        {
                            if (t.Sprite.Color != White)
                            {
                                t.Sprite.Color = White;
                            }
                        }
                    }
                }
            }            
        }

        public static CCScene GameStartLayerScene(CCGameView mainWindow, float _dynamicWidth, float _dynamicHeight, string json, GamePage gamePage)
        {
            var scene = new CCScene(mainWindow);

            IconStorageObject jsonObject = null;

            if (json != "")
            {
                try
                {
                    jsonObject = JsonConvert.DeserializeObject<IconStorageObject>(json);
                }
                catch
                {
                    jsonObject = null;
                }
            }

            var layer = new GameLayer(_dynamicWidth, _dynamicHeight, jsonObject, gamePage);

            scene.AddLayer(layer);

            return scene;
        }
    }
}
