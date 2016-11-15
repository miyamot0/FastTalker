﻿//----------------------------------------------------------------------------------------------
// <copyright file="GameLayer.cs" 
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

using CocosSharp;
using SGDWithCocos.Interface;
using Xamarin.Forms;
using System.Linq;
using SGDWithCocos.Models;
using SGDWithCocos.Tags;
using SGDWithCocos.Utilities;
using Newtonsoft.Json;
using SGDWithCocos.Shared.Pages;
using SGDWithCocos.Data;
using System.IO;

namespace SGDWithCocos.Shared.Layers
{
    /// <summary>
    /// Game Layer
    /// </summary>
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

        // Information loaded from static JSON
        //StorageContainer storageInformation;

        // Categories
        List<string> mCategories;

        Random mRandom;

        DateTime startTime,
                 endTime;

        CCColor3B White = new CCColor3B(255, 255, 255),
                  Red = new CCColor3B(255, 0, 0),
                  Green = new CCColor3B(0, 255, 0),
                  LightBlue = new CCColor3B(0, 0, 155);

        // Logicals for editing, frame state
        bool inEditMode = false,
             inSingleMode = false,
             isModal = false;

        // Time metrics, for screen press and save intervals
        float totalDuration = 0f,
              saveInterval = 60f;

        CCSpriteFrame backingSpriteFrame = null;

        CCSpriteSheet staticSpriteSheet;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_dynamicWidth">Device width</param>
        /// <param name="_dynamicHeight">Device height</param>
        /// <param name="json">JSON string for icons, nullable</param>
        /// <param name="_gamePage">Page reference</param>
        public GameLayer(float _dynamicWidth, float _dynamicHeight, IconStorageObject json, GamePage _gamePage) : base(CCColor4B.Gray)
        {
            this.Color = CCColor3B.Gray;
            this.GamePageParent = _gamePage;
            this.mRandom = new Random(DateTime.Now.Millisecond);
            
            spriteModelFactory = new SpriteMaker(_dynamicWidth, _dynamicHeight);
            spriteModelFactory.padding = 10;

            staticSpriteSheet = new CCSpriteSheet("static.plist");

            MakeListener();

            MakeStaticSprites();

            LoadSprites(json);

            Schedule(RunGameLogic);
        }

        /// <summary>
        /// Touch listener, swallow touches by default
        /// </summary>
        public void MakeListener()
        {
            mListener = new CCEventListenerTouchOneByOne();
            mListener.IsSwallowTouches = true;
            mListener.OnTouchBegan = OnTouchBegan;
            mListener.OnTouchEnded = OnTouchesEnded;
            mListener.OnTouchMoved = HandleTouchesMoved;
        }

        /// <summary>
        /// Construct the static, consistently present sprites in field
        /// </summary>
        public void MakeStaticSprites()
        {
            if (backingSpriteFrame == null)
            {
                backingSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Blank"));
            }

            sentenceFrame = spriteModelFactory.MakeSentenceFrame(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("frameWhite")));
                AddChild(sentenceFrame, 0, SpriteTypes.FrameTag);

            speakerFrame = spriteModelFactory.MakeSpeakerFrame(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Speaker_Icon")));
                AddEventListener(mListener, speakerFrame);
                AddChild(speakerFrame, 0, SpriteTypes.SpeakerTag);

            var addSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("AddNew"));
            addFrame = spriteModelFactory.MakeAddButton(addSpriteFrame, backingSpriteFrame);
            addFrame.Visible = false;
                AddEventListener(mListener.Copy(), addFrame);
                AddChild(addFrame, 0, SpriteTypes.AddTag);

            var takePhotoSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("CameraIcon"));
            takePhotoFrame = spriteModelFactory.TakePhotoButton(takePhotoSpriteFrame, backingSpriteFrame);
            takePhotoFrame.Visible = false;
                AddEventListener(mListener.Copy(), takePhotoFrame);
                AddChild(takePhotoFrame, 0, SpriteTypes.TakePhotoTag);

            var addFolderSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("FolderClosed"));
            addFolderFrame = spriteModelFactory.MakeAddFolderButton(addFolderSpriteFrame, backingSpriteFrame);
            addFolderFrame.Visible = false;
                AddEventListener(mListener.Copy(), addFolderFrame);
                AddChild(addFolderFrame, 0, SpriteTypes.FolderTag);

            var singleSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Single"));
            singleFrame = spriteModelFactory.MakeSingleButton(singleSpriteFrame, backingSpriteFrame);
            singleFrame.Visible = false;
                AddEventListener(mListener.Copy(), singleFrame);
                AddChild(singleFrame, 0, SpriteTypes.SingleModeTag);

            var multiSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Frame"));
            multiFrame = spriteModelFactory.MakeMultiButton(multiSpriteFrame, backingSpriteFrame);
            multiFrame.Visible = false;
                AddEventListener(mListener.Copy(), multiFrame);
                AddChild(multiFrame, 0, SpriteTypes.MultiModeTag);

            var removeSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Trash"));
            deleteFrame = spriteModelFactory.MakeRemoveButton(removeSpriteFrame, backingSpriteFrame);
            deleteFrame.Visible = false;
                AddChild(deleteFrame, 0, SpriteTypes.RemoveTag);
        }

        /// <summary>
        /// Load sprites from the saved JSON object
        /// </summary>
        /// <param name="json">Saved object</param>
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
                        // if IconTag matches, add to field at saved location

                        var newIcon = spriteModelFactory.MakeIconBase64(backingSpriteFrame, icon.Base64, icon.Text, icon.X, icon.Y, icon.Scale, icon.TextScale, icon.TextVisible);

                        iconList2.Add(new IconReference(newIcon, icon.Base64, 1f, true));
                        AddEventListener(mListener.Copy(), newIcon);
                    }
                }

                foreach (FolderModel icon in json.Folders)
                {
                    if (icon.Tag == SpriteTypes.FolderTag)
                    {
                        // if IconTag matches, add to field at saved location

                        var newIcon = spriteModelFactory.MakeFolder(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains(icon.AssetName)),
                            backingSpriteFrame, icon.Text, icon.X, icon.Y, icon.Scale, icon.TextScale, icon.TextVisible);
                        newIcon.Tag = SpriteTypes.FolderTag;
                        iconList2.Add(new IconReference(newIcon, icon.AssetName, icon.TextScale, icon.TextVisible));
                        AddEventListener(mListener.Copy(), newIcon);
                    }
                }

                foreach (StoredIconModel icon in json.StoredIcons)
                {
                    // add stored icons to the saved/cached field icons

                    var newIcon = spriteModelFactory.MakeIconBase64(backingSpriteFrame, icon.Base64, icon.Text, icon.X, icon.Y, icon.Scale, icon.TextScale, icon.TextVisible);
                    var storedIconRef = new StoredIconReference(newIcon, icon.Base64, icon.Folder, icon.Scale, icon.TextScale, icon.TextVisible);

                    storedList.Add(storedIconRef);
                }
                
                // Set the display mode

                SetSingleMode(json.SingleMode);
            }

            var counter = 1;

            foreach (IconReference icon in iconList2)
            {
                // Add all designated sprites to the field as saved/stored

                AddChild(icon.Sprite, counter, icon.Sprite.Tag);
                counter++;
            }
        }

        /// <summary>
        /// Sets the stage of the field, namely the selection logic and hotspot location
        /// </summary>
        /// <param name="status">true if single-mode</param>
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

        /// <summary>
        /// Sets the stage as either "Edit" or active mode, can modify icons in edit mode
        /// </summary>
        /// <param name="edit">is in edit mode?</param>
        public void SetEditMode(bool edit)
        {
            if (edit)
            {
                // Orange background = edit mode

                this.Color = CCColor3B.Orange;
                addFrame.Visible = true;
                takePhotoFrame.Visible = true;
                addFolderFrame.Visible = true;
                deleteFrame.Visible = true;
                singleFrame.Visible = true;
                multiFrame.Visible = true;

                inEditMode = !inEditMode;

                /*
                foreach (var t in iconList2)
                {
                    if (t.Sprite.Tag == SpriteTypes.IconTag || t.Sprite.Tag == SpriteTypes.FolderTag)
                    {
                        var rotateRight = new CCRotateTo(0.3f, 8f);
                        var rotateLeft = new CCRotateTo(0.4f, -8f);
                        var rotateTDC = new CCRotateTo(0.3f, 0f);

                        var sequence = new CCSequence(rotateRight, rotateLeft, rotateTDC);

                        t.Sprite.RepeatForever(sequence);
                    }
                }
                */
            }
            else
            {
                // Gray background = active mode

                this.Color = CCColor3B.Gray;
                addFrame.Visible = false;
                takePhotoFrame.Visible = false;
                addFolderFrame.Visible = false;
                deleteFrame.Visible = false;
                singleFrame.Visible = false;
                multiFrame.Visible = false;

                inEditMode = !inEditMode;

                /*
                foreach (var t in iconList2)
                {
                    if (t.Sprite.Tag == SpriteTypes.IconTag || t.Sprite.Tag == SpriteTypes.FolderTag)
                    {
                        t.Sprite.StopAllActions();
                        t.Sprite.Rotation = 0f;
                    }
                }
                */
            }
        }

        /// <summary>
        /// Draw the field as a single mode
        /// </summary>
        public void DrawSingleField()
        {
            sentenceFrame.Visible = false;

            singleFrame.Color = Green;
            multiFrame.Color = White;
        }

        /// <summary>
        /// Draw the field as a framed mode
        /// </summary>
        public void DrawFramedField()
        {
            sentenceFrame.Visible = true;

            singleFrame.Color = White;
            multiFrame.Color = Green;
        }

        // TODO precache

        /// <summary>
        /// Method called back from main UI thread
        /// </summary>
        /// <param name="base64">base64 image string</param>
        /// <param name="text">image text/speech text</param>
        /// <param name="extension">file extension</param>
        public void CallBackIcon(string base64, string text, string extension)
        {
            ScheduleOnce((dt) => {
                // Loop into main game thread

                /*
                if (extension == "Embedded")
                {
                    string assetName = base64;
                    string contentName = "Stored/" + assetName;
                    var sprite = new CCSprite(contentName);

                    string location = DependencyService.Get<ISaveAndLoad>().GetDirectory(assetName);

                    FileStream mFile = new FileStream(@location,
                       FileMode.OpenOrCreate,
                       FileAccess.ReadWrite,
                       FileShare.None);

                    sprite.Texture.SaveAsPng(mFile, (int)sprite.ContentSize.Width, (int)sprite.ContentSize.Height);
                    mFile.Flush();
                    mFile.Close();
                    mFile.Dispose();

                    byte[] imageArray = File.ReadAllBytes(location);
                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                    sprite.RemoveAllChildren();
                    sprite.Cleanup();
                    sprite.Dispose();

                    // Loop back in
                    CallBackIcon(base64ImageRepresentation, text, "png");
                }
                else
                */ 
                if (base64 != "" && text != "")
                {
                    // Introduce some jitter into the positioning of the icon

                    var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));
                    var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));

                    var newIcons = spriteModelFactory.MakeIconBase64(backingSpriteFrame, base64, text, xLocation, yLocation, 1f, 1f, true);
                    var mIconRef = new IconReference(newIcons, base64, 1f, true);
                    iconList2.Add(mIconRef);

                    // Assign listener event and tag
                    AddEventListener(mListener.Copy(), mIconRef.Sprite);

                    // Add child to field properly
                    AddChild(mIconRef.Sprite, iconList2.Count, SpriteTypes.IconTag);

                    /*
                    var rotateRight = new CCRotateTo(0.3f, 8f);
                    var rotateLeft = new CCRotateTo(0.4f, -8f);
                    var rotateTDC = new CCRotateTo(0.3f, 0f);

                    var sequence = new CCSequence(rotateRight, rotateLeft, rotateTDC);

                    mIconRef.Sprite.RepeatForever(sequence);
                    */
                }
            }, 0);
        }

        /// <summary>
        /// Method call back from main UI thread for folders
        /// </summary>
        /// <param name="assetName">Refers to the colored sprite</param>
        /// <param name="folderName">The name of the actual folder</param>
        public void MakeIconFolder(string assetName, string folderName)
        {
            ScheduleOnce((dt) => {
                // Introduce some jitter into the positioning of the icon

                var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));
                var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));

                var parentSprite = spriteModelFactory.MakeFolder(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains(assetName)),
                            backingSpriteFrame, folderName, xLocation, yLocation, 1f, 1f, true);

                var mIconRef = new IconReference(parentSprite, assetName, 1f, true);
                iconList2.Add(mIconRef);

                // Assign listener event and tag
                AddEventListener(mListener.Copy(), mIconRef.Sprite);

                // Add child to field properly
                AddChild(mIconRef.Sprite, iconList2.Count, SpriteTypes.FolderTag);
            }, 0);
        }

        /// <summary>
        /// Update an icon's text
        /// </summary>
        /// <param name="index">Location in the master list of references</param>
        /// <param name="text">The text to be modified</param>
        public void UpdateSpriteText(int index, string text)
        {
            ScheduleOnce((dt) => {
                if (index != -1 && text != "")
                {
                    // The referenced icon, as retrieved from the list
                    var mIconRef = iconList2[index];

                    // The sprite from the reference
                    var mSprite = mIconRef.Sprite;

                    // The text sprite, from mSprite, cast to CCLabel
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (mContent != null)
                    {
                        if (mSprite.Tag == SpriteTypes.FolderTag)
                        {
                            // If the icon being renamed is a folder, change the storage references too

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

                        // Apply new text 

                        mContent.Text = text;
                    }
                }
            }, 0);
        }

        /// <summary>
        /// Toggle image (NOT IMPLEMENTED YET), TODO
        /// </summary>
        /// <param name="index">index in icon reference list</param>
        public void ToggleSpriteImage(int index)
        {
            ScheduleOnce((dt) => {
                if (index != -1)
                {
                    // The referenced icon, as retrieved from the list
                    var mIconRef = iconList2[index];

                    // The sprite from the reference
                    var mSprite = mIconRef.Sprite;

                    // The text sprite, from mSprite, cast to CCLabel
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    mContent.Visible = !mSprite.Visible;
                }
            }, 0);
        }

        /// <summary>
        /// Toggle the display of the icon text
        /// </summary>
        /// <param name="index">index in icon reference list</param>
        public void ToggleSpriteText(int index)
        {
            ScheduleOnce((dt) => {
                if (index != -1)
                {
                    // The referenced icon, as retrieved from the list
                    var mIconRef = iconList2[index];

                    // The sprite from the reference
                    var mSprite = mIconRef.Sprite;

                    // The text sprite, from mSprite, cast to CCLabel
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (mContent != null)
                    {
                        mContent.Visible = !mContent.Visible;
                    }
                }
            }, 0);
        }

        /// <summary>
        /// Updates the size of the sprite's text, in either direction
        /// </summary>
        /// <param name="index">index in icon reference list</param>
        /// <param name="action">Enum related to size change</param>
        public void UpdateSpriteTextSize(int index, int action)
        {
            ScheduleOnce((dt) => {
                if (index != -1 && action != EditTypes.None)
                {
                    // The referenced icon, as retrieved from the list
                    var mIconRef = iconList2[index];

                    // The sprite from the reference
                    var mSprite = mIconRef.Sprite;

                    // The text sprite, from mSprite, cast to CCLabel
                    var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (mContent != null)
                    {
                        switch(action)
                        {
                            case EditTypes.UpdateLabelSizeUp:
                                // Relative scale 110%

                                mContent.AddAction(new CCScaleBy(0.2f, 1.1f));
                                break;

                            case EditTypes.UpdateLabelSizeDefault:
                                // Absolute scale to 100%

                                mContent.AddAction(new CCScaleTo(0.2f, 1f));
                                break;

                            case EditTypes.UpdateLabelSizeDown:
                                // Relative scale to 90%

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

        /// <summary>
        /// Updates the size of a sprite overall, in either direction
        /// </summary>
        /// <param name="index">index in icon reference list</param>
        /// <param name="edit">Enum related to size change</param>
        public void UpdateSpriteSize(int index, int edit)
        {
            ScheduleOnce((dt) => {
                if (index != -1 && edit != EditTypes.None)
                {
                    if (edit == EditTypes.UpdateSizeUp)
                    {
                        // The referenced icon, as retrieved from the list
                        var mIconRef = iconList2[index];

                        // The sprite from the reference
                        var mSprite = mIconRef.Sprite;

                        // Relative scale 110%
                        var action = new CCScaleBy(0.5f, 1.1f);

                        mSprite.AddAction(action);

                    }
                    else if (edit == EditTypes.UpdateSizeDown)
                    {
                        var mIconRef = iconList2[index];
                        var mSprite = mIconRef.Sprite;

                        // Relative scale 90%
                        var action = new CCScaleBy(0.5f, 0.9f);

                        mSprite.AddAction(action);
                    }
                    else if (edit == EditTypes.UpdateSizeDefault)
                    {
                        // The referenced icon, as retrieved from the list
                        var mIconRef = iconList2[index];

                        // The sprite from the reference
                        var mSprite = mIconRef.Sprite;

                        // Absolute scale 100%
                        var action = new CCScaleTo(0.5f, 1f);

                        mSprite.AddAction(action);
                    }
                }
            }, 0);
        }

        /// <summary>
        /// Remove current icon
        /// </summary>
        public void RemoveCurrentIcon()
        {
            RemoveChild(CurrentSpriteTouched);
        }

        /// <summary>
        /// Clear the modal window
        /// </summary>
        public void ClearWindow()
        {
            iconList2.Remove(tempWindow);

            CCCallFunc hideIcons = new CCCallFunc(ClearIconsInModal);
            CCCallFuncN removeClose = new CCCallFuncN(node => node.RemoveChild(closeButton));
            CCCallFuncN removeWindow = new CCCallFuncN(node => node.RemoveFromParent());
            CCCallFunc cleanUpWindow = new CCCallFunc(CleanUpWindow);

            windowFrame.RunActions(hideIcons, removeClose, removeWindow, cleanUpWindow);
        }

        /// <summary>
        /// Deferred cleanup
        /// </summary>
        public void CleanUpWindow()
        {
            RemoveChildByTag(SpriteTypes.ColorLayerTag);

            windowFrame.Cleanup();
            windowFrame.Dispose();
                windowFrame = null;

            closeButton.Cleanup();
            closeButton.Dispose();
                closeButton = null;

            tempWindow = null;
        }

        /// <summary>
        /// Deselect all icons in field (cast to white)
        /// </summary>
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

        /// <summary>
        /// Show window with icons matching query
        /// </summary>
        /// <param name="mMatchingIcons"></param>
        public void ShowStoredWindow(string mType)
        {
            // If already modal mode, just return
            if (isModal) return;

            ScheduleOnce((dt) => {

                var texture = new CCRenderTexture(new CCSize(200, 200), new CCSize(200, 200), CCSurfaceFormat.Color);
                texture.BeginWithClear(CCColor4B.White);
                texture.End();
                windowFrame = new CCSprite(texture.Texture)
                {
                    PositionX = addFolderFrame.Position.X,
                    PositionY = addFolderFrame.Position.Y,
                    Tag = SpriteTypes.WindowTag
                };
                texture.Dispose();

                windowFrame.Color = CCColor3B.White;

                // Scale up to near-field size
                var scaling = (spriteModelFactory.DynamicWidth * 0.1f) / windowFrame.ContentSize.Width;
                windowFrame.ContentSize = new CCSize(windowFrame.ContentSize.Width * scaling, windowFrame.ContentSize.Height * scaling);

                // Button to close window
                closeButton = new CCSprite(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("IconClose")));
                closeButton.ContentSize = new CCSize(windowFrame.ContentSize.Width * 0.075f, windowFrame.ContentSize.Width * 0.075f);
                closeButton.PositionX = windowFrame.ContentSize.Width - closeButton.ContentSize.Width / 2f - windowFrame.ContentSize.Width * 0.05f;
                closeButton.PositionY = windowFrame.ContentSize.Height - closeButton.ContentSize.Height / 2f - windowFrame.ContentSize.Height * 0.05f;
                closeButton.Tag = SpriteTypes.CloseWindowTag;

                // Add listener to close button
                AddEventListener(mListener.Copy(), closeButton);

                // Add close window
                windowFrame.AddChild(closeButton, 1001, SpriteTypes.CloseWindowTag);

                var ccSpriteSheetDisplay = new CCSpriteSheet(mType + ".plist");

                var mMatchingTextures = ccSpriteSheetDisplay.Frames;

                for (var i = 0; i < mMatchingTextures.Count; i++)
                {
                    var mStoredIconTexture = mMatchingTextures[i];

                    var parentSprite = new CCSprite(backingSpriteFrame);

                    parentSprite.ContentSize = new CCSize(windowFrame.ContentSize.Width * 0.25f, windowFrame.ContentSize.Height * 0.25f);

                    var pSpacing = parentSprite.ContentSize.Width * 0.15f;
                    var xSpacing = parentSprite.ContentSize.Width * ((i % 3)) + pSpacing * ((i % 3) + 1);
                    var ySpacing = parentSprite.ContentSize.Height * ((i / 3)) + pSpacing * ((i / 3) + 1);

                    parentSprite.PositionX = (parentSprite.ContentSize.Width * 0.5f) + xSpacing;
                    parentSprite.PositionY = windowFrame.ContentSize.Height - (parentSprite.ContentSize.Height * 0.5f) - ySpacing;

                    parentSprite.Tag = SpriteTypes.IconTag;

                    var subIconFrame = new CCSprite(ccSpriteSheetDisplay.Frames.Find((x) => x.TextureFilename.Contains(mStoredIconTexture.TextureFilename)))
                    {
                        AnchorPoint = CCPoint.AnchorMiddle,
                        ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.75f, parentSprite.ContentSize.Height * 0.75f),
                        PositionX = parentSprite.ContentSize.Width * 0.5f,
                        PositionY = parentSprite.ContentSize.Height * 0.5f + parentSprite.ContentSize.Height * 0.075f,
                        Tag = SpriteTypes.ImageTag
                    };

                    var label = new CCLabel(mStoredIconTexture.TextureFilename.Replace(".png", ""), "Arial", 22, CCLabelFormat.SystemFont)
                    {
                        Scale = 0.20f * scaling,
                        AnchorPoint = CCPoint.AnchorMiddle,
                        HorizontalAlignment = CCTextAlignment.Center,
                        VerticalAlignment = CCVerticalTextAlignment.Center,
                        PositionX = parentSprite.ContentSize.Width * 0.5f,
                        PositionY = parentSprite.ContentSize.Height * 0.075f,
                        Color = CCColor3B.Black,
                        Visible = true,
                        Tag = SpriteTypes.ContentTag
                    };

                    parentSprite.AddChild(subIconFrame);
                    AddEventListener(mListener.Copy(), parentSprite);

                    parentSprite.Visible = false;

                    windowFrame.AddChild(parentSprite, 1001, SpriteTypes.EmbeddedIconTag);
                    parentSprite.AddChild(label);

                    mStoredIconTexture = null;
                }

                AddEventListener(mListener.Copy(), windowFrame);
                tempWindow = new IconReference(windowFrame, "Window", 1f, true);
                iconList2.Add(tempWindow);
                AddChild(windowFrame, 1000, SpriteTypes.WindowTag);

                // Moves window to center
                var moveAction = new CCMoveTo(0.2f, new CCPoint(spriteModelFactory.DynamicWidth / 2f, spriteModelFactory.DynamicHeight / 2f));

                var dimension = Math.Min(spriteModelFactory.DynamicHeight, spriteModelFactory.DynamicWidth);
                var scale = (dimension / (windowFrame.ContentSize.Width)) * 1f;

                // Scale to center, 90% of screen or so
                var scaleAction = new CCScaleTo(0.2f, scale);

                // Blur background, to focus the listener
                var maskBackground = new CCCallFunc(MaskBackground);

                // Reveal the icons after scaling
                var revealIcons = new CCCallFunc(ShowIconsInModal);

                // Hacky workaround to give firm borders to window
                var drawBorders = new CCCallFunc(AddBorders);

                // Execute actions
                windowFrame.AddActions(false, moveAction, maskBackground, scaleAction, drawBorders, revealIcons);

                isModal = true;

                mMatchingTextures.Clear();
                mMatchingTextures = null;

                ccSpriteSheetDisplay.Frames.Clear();
                ccSpriteSheetDisplay = null;
            }, 0);
        }

        /// <summary>
        /// Show folder contents window
        /// </summary>
        /// <param name="currentSprite">Current sprite (folder) touched</param>
        /// <param name="folderName">Name of the folder</param>
        public void ShowWindow(CCSprite currentSprite, string folderName)
        {
            // If already modal mode, just return
            if (isModal) return;

            var texture = new CCRenderTexture(new CCSize(200, 200), new CCSize(200, 200), CCSurfaceFormat.Color);
            texture.BeginWithClear(CCColor4B.White);
            texture.End();
            windowFrame = new CCSprite(texture.Texture)
            {
                PositionX = currentSprite.Position.X,
                PositionY = currentSprite.Position.Y,
                Tag = SpriteTypes.WindowTag
            };
            texture.Dispose();

            // Scale up to near-field size
            var scaling = (spriteModelFactory.DynamicWidth * 0.1f) / windowFrame.ContentSize.Width;
            windowFrame.ContentSize = new CCSize(windowFrame.ContentSize.Width * scaling, windowFrame.ContentSize.Height * scaling);

            // Button to close window
            closeButton = new CCSprite(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("IconClose")));
            closeButton.ContentSize = new CCSize(windowFrame.ContentSize.Width * 0.075f, windowFrame.ContentSize.Width * 0.075f);
            closeButton.PositionX = windowFrame.ContentSize.Width - closeButton.ContentSize.Width/2f - windowFrame.ContentSize.Width * 0.05f;
            closeButton.PositionY = windowFrame.ContentSize.Height - closeButton.ContentSize.Height/2f - windowFrame.ContentSize.Height * 0.05f;
            closeButton.Tag = SpriteTypes.CloseWindowTag;

            // Add listener to close button
            AddEventListener(mListener.Copy(), closeButton);

            ScheduleOnce((dt) => {

                // Add close window
                windowFrame.AddChild(closeButton, 1001, SpriteTypes.CloseWindowTag);

                // !important: lock for concurrency issues
                lock (storedList)
                {
                    // Pull items where folder reference holds
                    var mEqualList = storedList.Where(l => l.FolderName == folderName).ToList();

                    for (var i = 0; i < mEqualList.Count; i++)
                    {
                        // The referenced icon, as retrieved from the list
                        var mStoredIconRef = mEqualList[i];

                        // The sprite from the reference
                        var mSprite = mStoredIconRef.Sprite;

                        // The text sprite, from mSprite, cast to CCLabel
                        var mContent = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        if (mContent != null)
                        {
                            var parentSprite = new CCSprite(backingSpriteFrame);

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

                            parentSprite.Visible = false;

                            windowFrame.AddChild(parentSprite, 1001, SpriteTypes.IconTag);
                            parentSprite.AddChild(label);
                        }
                    }
                }

                AddEventListener(mListener.Copy(), windowFrame);
                tempWindow = new IconReference(windowFrame, "Window", 1f, true);
                iconList2.Add(tempWindow);
                AddChild(windowFrame, 1000, SpriteTypes.WindowTag);

                // Moves window to center
                var moveAction = new CCMoveTo(0.2f, new CCPoint(spriteModelFactory.DynamicWidth / 2f, spriteModelFactory.DynamicHeight / 2f));

                var dimension = Math.Min(spriteModelFactory.DynamicHeight, spriteModelFactory.DynamicWidth);
                var scale = (dimension / (windowFrame.ContentSize.Width)) * 1f;

                // Scale to center, 90% of screen or so
                var scaleAction = new CCScaleTo(0.2f, scale);

                // Blur background, to focus the listener
                var maskBackground = new CCCallFunc(MaskBackground);

                // Reveal the icons after scaling
                var revealIcons = new CCCallFunc(ShowIconsInModal);

                // Hacky workaround to give firm borders to window
                var drawBorders = new CCCallFunc(AddBorders);

                // Execute actions
                windowFrame.AddActions(false, moveAction, maskBackground, scaleAction, drawBorders, revealIcons);

                isModal = true;
            }, 0);
        }

        /// <summary>
        /// Mask background for modal window
        /// </summary>
        public void MaskBackground()
        {
            //var borderBackGray = new CCSprite("frameWhite");
            var borderBackGray = new CCSprite(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("frameWhite")));
            borderBackGray.Color = CCColor3B.Gray;
            borderBackGray.Opacity = 200;
            borderBackGray.ContentSize = new CCSize(spriteModelFactory.DynamicWidth, spriteModelFactory.DynamicHeight);
            borderBackGray.PositionX = spriteModelFactory.DynamicWidth / 2f;
            borderBackGray.PositionY = spriteModelFactory.DynamicHeight / 2f;
            borderBackGray.Tag = SpriteTypes.ColorLayerTag;

            AddEventListener(mListener.Copy(), borderBackGray);

            AddChild(borderBackGray, 999, SpriteTypes.ColorLayerTag);
        }

        /// <summary>
        /// Defer visibility until model is popped up
        /// </summary>
        public void ShowIconsInModal()
        {
            var mIcons = windowFrame.Children.Where(t => t.Tag == SpriteTypes.IconTag || t.Tag == SpriteTypes.EmbeddedIconTag).ToList();

            for (var i=0; i<mIcons.Count; i++)
            {
                var opacity = new CCCallFuncN(node => node.Opacity = 0);
                var show = new CCCallFuncN(node => node.Visible = true);
                var fade = new CCFadeIn(i / 20f);
                mIcons[i].AddActions(false, opacity, show, fade);
            }
        }

        /// <summary>
        /// Clean the icons out of modal thoroughly, as there are many
        /// </summary>
        public void ClearIconsInModal()
        {
            var mIcons = windowFrame.Children.Where(t => t.Tag == SpriteTypes.IconTag || t.Tag == SpriteTypes.EmbeddedIconTag).ToList();

            for (var i = 0; i < mIcons.Count; i++)
            {
                var fade = new CCFadeOut((mIcons.Count / 20f) - i / 20f);
                var clean = new CCCallFuncN(node => node.RemoveFromParent());
                var unlisten = new CCCallFuncN(node => node.RemoveAllListeners());
                var cleanup = new CCCallFuncN(node => node.Cleanup());
                var dispose = new CCCallFuncN(node => node.Dispose());
                mIcons[i].AddActions(false, fade, unlisten, clean, cleanup, dispose);
            }
        }

        /// <summary>
        /// Draw borders w/ high priority, since CCS really isn't good with clipping
        /// </summary>
        public void AddBorders()
        {

        }

        /// <summary>
        /// Static call, extract label
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Touch begin listener
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
        /// <returns></returns>
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
                if (caller.Tag == SpriteTypes.WindowTag && windowFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    CurrentSpriteTouched = windowFrame;
                    touchType = Tags.Tag.Window;

                    return true;
                }
                else if (windowFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
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

                                        var newIcon = spriteModelFactory.MakeIconBase64(backingSpriteFrame, storedRef.Base64, mLoopContent.Text,
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

                            ClearWindow();

                            return true;
                        }
                        else if (mIcon.Tag == SpriteTypes.EmbeddedIconTag && mIcon.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                        {
                            GamePageParent.NameEmbeddedIcon(mIcon);

                            isModal = false;

                            ClearWindow();

                            return true;
                        }
                    }

                    return false;
                }
                else
                {
                    CurrentSpriteTouched = null;
                    touchType = Tags.Tag.ColorLayer;

                    return true;
                }
            }

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

        /// <summary>
        /// Touch end listener
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
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

                            if (true)
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
                                storedList.Add(newItem);

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
                    GamePageParent.CallActionSheetChoice();
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

        /// <summary>
        /// Active move listener
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
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
                else if (touchType == Tags.Tag.Window)
                {
                    var mIcons = windowFrame.Children.Where(t => t.Tag == SpriteTypes.IconTag || t.Tag == SpriteTypes.EmbeddedIconTag).ToList();

                    if (mIcons.Count < 9)
                    {
                        return;
                    }

                    var deltaPos = touch.Delta.Y;

                    var highestHeight = mIcons
                        .Aggregate((agg, next) =>
                            next.PositionY > agg.PositionY ? next : agg);

                    var lowestHeight = mIcons
                        .Aggregate((agg, next) =>
                            next.PositionY < agg.PositionY ? next : agg);

                    var padding = highestHeight.ContentSize.Width * 0.15f;

                    var clampHigh = windowFrame.ContentSize.Height - highestHeight.ContentSize.Height / 2f - padding;

                    var clampLow = highestHeight.ContentSize.Height / 2f + padding;

                    if (lowestHeight.PositionY + deltaPos >= clampLow)
                    {
                        return;
                    }
                    else if (highestHeight.PositionY + deltaPos <= clampHigh)
                    {
                        return;
                    }
                    else
                    {
                        for (var i = 0; i < mIcons.Count; i++)
                        {
                            if (mIcons[i].Tag == SpriteTypes.IconTag || mIcons[i].Tag == SpriteTypes.EmbeddedIconTag)
                            {
                                mIcons[i].PositionY = mIcons[i].PositionY + deltaPos;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// In game logic
        /// </summary>
        /// <param name="frameTimeInSeconds"></param>
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

        /// <summary>
        /// Load JSON data into memory, for parsing later.  Categories derived at start.
        /// </summary>
        public void LoadJsonContent()
        {
            mCategories = JsonConvert.DeserializeObject<List<string>>(StoredJson.LoadIconTypes());
        }

        /// <summary>
        /// Access categories
        /// </summary>
        /// <returns></returns>
        public string[] GetCategories()
        {
            return mCategories.ToArray();
        }

        /// <summary>
        /// Loading call
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="_dynamicWidth"></param>
        /// <param name="_dynamicHeight"></param>
        /// <param name="json"></param>
        /// <param name="gamePage"></param>
        /// <returns></returns>
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
