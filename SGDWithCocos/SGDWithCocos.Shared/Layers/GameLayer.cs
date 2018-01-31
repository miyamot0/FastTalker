//----------------------------------------------------------------------------------------------
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
using System.Threading.Tasks;
using System.Diagnostics;

namespace SGDWithCocos.Shared.Layers
{
    /// <summary>
    /// Game Layer
    /// </summary>
    public class GameLayer : CCLayerColor
    {
        GamePage GamePageParent { get; set; }

        CCEventListenerTouchOneByOne mListener;

        public SpriteMaker spriteModelFactory;

        CCSprite sentenceFrame,
                 speakerFrame,
                 deleteFrame,
                 addFrame,
                 closeButton = null,
                 windowFrame = null,
                 CurrentSpriteTouched = null;

        Tag touchType = Tags.Tag.None;

        IconReference tempWindow;

        List<IconReference> iconList2;
        List<StoredIconReference> storedList;

        // Information loaded from static JSON
        StorageContainer storageInformation;

        Random mRandom;

        DateTime startTime;

        // Logicals for editing, frame state
        bool inEditMode = false,
             inSingleMode = true,
             isModal = false;

        bool unselectAuto = false;
        public bool UnselectAuto
        {
            get { return unselectAuto; }
            set
            {
                unselectAuto = value;
            }
        }

        bool isServerUp = false;
        public bool ServerActive
        {
            get { return isServerUp; }
            set
            {
                isServerUp = value;
            }
        }

        // Time metrics, for screen press and save intervals
        float totalDuration = 0f, saveInterval = 15f;
        TimeSpan timeDiff;

        CCSpriteFrame backingSpriteFrame = null;
        CCSpriteSheet staticSpriteSheet;

        CCLabel tempContentLabel;

        List<string> CategoryList;
        string[] categories = null;
        public string[] Categories
        {
            get
            {
                if (categories == null)
                {
                    categories = CategoryList.Where(j => j.Trim() != "").OrderBy(j => j).ToArray();
                }

                return categories;
            }
        }

        /// <summary>
        /// Stored Icons
        /// </summary>
        public StorageContainer StoredIcons
        {
            get
            {
                return storageInformation;
            }
        }

        int zIndexTop = 999;
        int zIndexBack = 10;

        // Top holder
        List<IconReference> iconsOverlapping;
        CCSprite highSprite;
        CCSprite caller;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="json">JSON string for icons, nullable</param>
        /// <param name="_gamePage">Page reference</param>
        public GameLayer() : base(CCColor4B.Gray)
        {
            Color = CCColor3B.Gray;
            GamePageParent = App.MainGamePage;
            mRandom = new Random(DateTime.Now.Millisecond);
            
            spriteModelFactory = new SpriteMaker(App.Width, App.Height);
            spriteModelFactory.padding = 10;

            staticSpriteSheet = new CCSpriteSheet("static.plist");

            MakeListener();

            MakeStaticSprites();

            LoadSprites();

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

            var addSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Settings"));
            addFrame = spriteModelFactory.MakeAddButton(addSpriteFrame, backingSpriteFrame);
            addFrame.Visible = false;
                AddEventListener(mListener.Copy(), addFrame);
                AddChild(addFrame, 0, SpriteTypes.AddTag);

            var removeSpriteFrame = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("TrashCan"));
            deleteFrame = spriteModelFactory.MakeRemoveButton(removeSpriteFrame, backingSpriteFrame);
            deleteFrame.Visible = false;
                AddChild(deleteFrame, 0, SpriteTypes.RemoveTag);
        }

        /// <summary>
        /// Load sprites
        /// </summary>
        public async void LoadSprites()
        {
            iconList2 = new List<IconReference>();
            storedList = new List<StoredIconReference>();

            var icons = await App.Database.GetIconsAsync();

            foreach (TableIcons icon in icons)
            {
                if (icon.Tag == SpriteTypes.IconTag)
                {
                    // if IconTag matches, add to field at saved location
                    var newIcon = await spriteModelFactory.AsyncCreateBase64Sprite(backingSpriteFrame, 
                                                                                   icon.Base64, 
                                                                                   icon.Text, 
                                                                                   icon.X, 
                                                                                   icon.Y, 
                                                                                   icon.Scale, 
                                                                                   icon.TextScale, 
                                                                                   icon.TextVisible);

                    iconList2.Add(new IconReference(newIcon, icon.Base64, 1f, true));
                    AddEventListener(mListener.Copy(), newIcon);
                }
            }

            var folders = await App.Database.GetFolderIconsAsync();

            foreach (TableFolders icon in folders)
            {
                if (icon.Tag == SpriteTypes.FolderTag)
                {
                    // if IconTag matches, add to field at saved location
                    CCSpriteFrame content = null;

                    if (icon.AssetName != null)
                    {
                        content = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains(icon.AssetName));
                    }

                    var newIcon = await spriteModelFactory.AsyncCreateFolder(content, 
                                                                             backingSpriteFrame, 
                                                                             icon.Base64, 
                                                                             icon.Text, 
                                                                             icon.X, 
                                                                             icon.Y, 
                                                                             icon.Scale, 
                                                                             icon.TextScale, 
                                                                             icon.TextVisible);
                    newIcon.Tag = SpriteTypes.FolderTag;

                    var newIconRef = new IconReference(newIcon, 
                                                       icon.AssetName, 
                                                       icon.TextScale, 
                                                       icon.TextVisible);
                    
                    newIconRef.Base64 = (icon.AssetName == null) ? icon.Base64 : icon.AssetName;

                    iconList2.Add(newIconRef);
                    AddEventListener(mListener.Copy(), newIcon);
                }
            }

            var storedIcons = await App.Database.GetStoredIconsAsync();

            foreach (TableStoredIcons icon in storedIcons)
            {
                // add stored icons to the saved/cached field icons

                var newIcon = await spriteModelFactory.AsyncCreateBase64Sprite(backingSpriteFrame, 
                                                                               icon.Base64, 
                                                                               icon.Text, 
                                                                               icon.X, 
                                                                               icon.Y, 
                                                                               icon.Scale,
                                                                               icon.TextScale, 
                                                                               icon.TextVisible);
                
                var storedIconRef = new StoredIconReference(newIcon, 
                                                            icon.Base64,
                                                            icon.Folder, 
                                                            icon.Scale, 
                                                            icon.TextScale,
                                                            icon.TextVisible);

                storedList.Add(storedIconRef);
            }

            TableSettings settings = null;

            try 
            {
                settings = await App.Database.GetSettingsAsync();
            }
            catch {}


            if (settings != null)
            {
                SetSingleMode(settings.SingleMode);
                UnselectAuto = settings.AutoUnselectSingleMode;
            }
            else
            {
                SetSingleMode(inSingleMode);
                UnselectAuto = false;
            }

            var counter = 2;
            foreach (IconReference icon in iconList2)
            {
                if (icon.Sprite.Tag == SpriteTypes.FolderTag)
                {
                    // Add all designated sprites to the field as saved/stored
                    AddChild(icon.Sprite, 1, icon.Sprite.Tag);
                    counter++;                    
                }
                else
                {
                    // Add all designated sprites to the field as saved/stored
                    AddChild(icon.Sprite, counter, icon.Sprite.Tag);
                    counter++;
                }
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
                Color = CCColor3B.Orange;
                addFrame.Visible = true;
                deleteFrame.Visible = true;

                inEditMode = !inEditMode;
            }
            else
            {
                // Gray background = active mode
                Color = CCColor3B.Gray;
                addFrame.Visible = false;
                deleteFrame.Visible = false;

                inEditMode = !inEditMode;
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DependencyService.Get<IAdmin>().RequestAdmin(!edit);
                });
            }
        }

        /// <summary>
        /// Draw the field as a single mode
        /// </summary>
        public void DrawSingleField()
        {
            sentenceFrame.Visible = false;
        }

        /// <summary>
        /// Draw the field as a framed mode
        /// </summary>
        public void DrawFramedField()
        {
            sentenceFrame.Visible = true;
        }

        /// <summary>
        /// Update named sprite
        /// </summary>
        public void UpdatePositionNamedSprite(string name, int positionX, int positionY)
        {
            ScheduleOnce((dt) =>
            {
                var namedSprite = iconList2.Where(i => i.Sprite.Name == name).FirstOrDefault();

                if (namedSprite != null)
                {
                    namedSprite.Sprite.PositionX = positionX;
                    namedSprite.Sprite.PositionY = positionY;
                }
            }, 0f);
        }

        /// <summary>
        /// Add new icon to stored cache
        /// </summary>
        /// <param name="mContentTag"></param>
        /// <param name="base64string2"></param>
        public async void CallBackIconStored(string base64, string text, string folderTag)
        {
            var newIcon = await spriteModelFactory.AsyncCreateBase64Sprite(backingSpriteFrame, 
                                                                           base64, 
                                                                           text, 
                                                                           -1, 
                                                                           -1,
                                                                           1f, 
                                                                           1f, 
                                                                           true);
            
            var newItem = new StoredIconReference(newIcon, 
                                                  base64, 
                                                  folderTag,
                                                  1f,
                                                  1f, 
                                                  true);

            storedList.Add(newItem);
        }

        /// <summary>
        /// Method called back from main UI thread
        /// </summary>
        /// <param name="base64">base64 image string</param>
        /// <param name="text">image text/speech text</param>
        /// <param name="extension">file extension</param>
        public void CallBackIcon(string base64, string text, string extension, string name)
        {
            ScheduleOnce(async (dt) =>
            {
                // Loop into main game thread

                if (extension == "Embedded")
                {
                    string assetName = base64;
                    string contentName = "Stored/" + assetName;

                    var sprite = new CCSprite(contentName);

                    string base64ImageRepresentation = "";

                    using (var ms = new MemoryStream())
                    {
                        sprite.Texture.SaveAsPng(ms, (int)sprite.ContentSize.Width, (int)sprite.ContentSize.Height);
                        ms.Position = 0;
                        byte[] imageArray = ms.ToArray();
                        base64ImageRepresentation = Convert.ToBase64String(imageArray);

                        imageArray = null;
                    }

                    sprite.RemoveAllChildren();

                    CCTextureCache.SharedTextureCache.RemoveTexture(sprite.Texture);

                    sprite.Texture.Dispose();
                    sprite.RemoveFromParent(true);

                    sprite.Cleanup();
                    sprite.Dispose();

                    // Loop back in
                    CallBackIcon(base64ImageRepresentation, text, "png", null);
                }
                else if (base64 != "" && text != "")
                {
                    // Introduce some jitter into the positioning of the icon
                    var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));
                    var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));

                    var newIcons = await spriteModelFactory.AsyncCreateBase64Sprite(backingSpriteFrame, 
                                                                                    base64, 
                                                                                    text, 
                                                                                    xLocation, 
                                                                                    yLocation, 
                                                                                    1f, 
                                                                                    1f, 
                                                                                    true);

                    if (name != null)
                    {
                        newIcons.Name = name;
                    }

                    var mIconRef = new IconReference(newIcons, base64, 1f, true);
                    iconList2.Add(mIconRef);

                    // Assign listener event and tag
                    AddEventListener(mListener.Copy(), mIconRef.Sprite);

                    // Add child to field properly
                    AddChild(mIconRef.Sprite, iconList2.Count, SpriteTypes.IconTag);

                    // Pop up to forefront
                    ReorderChild(mIconRef.Sprite, 999);

                    // Add salient animation to icons added back to field
                    mIconRef.Sprite.AddAction(AnimationTools.iconAnimationRotate);
                }
                else if (base64 == "" & text != "")
                {
                    // Introduce some jitter into the positioning of the icon

                    var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));
                    var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));

                    var newIcons = await spriteModelFactory.AsyncCreateBase64Sprite(backingSpriteFrame, 
                                                                                    base64, 
                                                                                    text, 
                                                                                    xLocation, 
                                                                                    yLocation, 
                                                                                    1f, 
                                                                                    1f,
                                                                                    true);

                    if (name != null)
                    {
                        newIcons.Name = name;
                    }

                    var mIconRef = new IconReference(newIcons, base64, 1f, true);
                    iconList2.Add(mIconRef);

                    // Assign listener event and tag
                    AddEventListener(mListener.Copy(), mIconRef.Sprite);

                    // Add child to field properly
                    AddChild(mIconRef.Sprite, iconList2.Count, SpriteTypes.IconTag);

                    // Pop up to forefront
                    ReorderChild(mIconRef.Sprite, 999);

                    // Add salient animation to icons added back to field
                    mIconRef.Sprite.AddAction(AnimationTools.iconAnimationRotate);
                }
            }, 0.1f);
        }

        /// <summary>
        /// Method call back from main UI thread for folders
        /// </summary>
        /// <param name="assetName">Refers to the colored sprite</param>
        /// <param name="folderName">The name of the actual folder</param>
        public void MakeIconFolder(string assetName, string folderName, string base64, string name)
        {
            ScheduleOnce(async (dt) =>
            {
                // Introduce some jitter into the positioning of the icon
                var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));
                var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));

                CCSpriteFrame content = null;

                if (assetName != null)
                {
                    content = staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains(assetName));
                }

                var parentSprite = await spriteModelFactory.AsyncCreateFolder(content, 
                                                                              backingSpriteFrame, 
                                                                              base64, 
                                                                              folderName,
                                                                              xLocation, 
                                                                              yLocation,
                                                                              1f,
                                                                              1f, 
                                                                              true);

                var mIconRef = new IconReference(parentSprite, assetName, 1f, true);

                if (assetName == null)
                {
                    mIconRef.Base64 = base64;
                }

                iconList2.Add(mIconRef);

                // Assign listener event and tag
                AddEventListener(mListener.Copy(), mIconRef.Sprite);

                // Add child to field properly
                AddChild(mIconRef.Sprite, iconList2.Count, SpriteTypes.FolderTag);

                // Add salient animation to icons added back to field
                mIconRef.Sprite.AddAction(AnimationTools.iconAnimationRotate);
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
                    tempContentLabel = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (tempContentLabel != null)
                    {
                        if (mSprite.Tag == SpriteTypes.FolderTag)
                        {
                            // If the icon being renamed is a folder, change the storage references too
                            for (var i = storedList.Count - 1; i >= 0; i--)
                            {
                                if (storedList[i].FolderName == tempContentLabel.Text)
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
                        tempContentLabel.Text = text;
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
                    tempContentLabel = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    tempContentLabel.Visible = !mSprite.Visible;
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
                    tempContentLabel = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (tempContentLabel != null)
                    {
                        tempContentLabel.Visible = !tempContentLabel.Visible;
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
                    tempContentLabel = mSprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (tempContentLabel != null)
                    {
                        switch(action)
                        {
                            case EditTypes.UpdateLabelSizeUp:
                                // Relative scale 110%

                                tempContentLabel.AddAction(new CCScaleBy(0.2f, 1.1f));
                                break;

                            case EditTypes.UpdateLabelSizeUp2:
                                // Relative scale 150%

                                tempContentLabel.AddAction(new CCScaleBy(0.2f, 1.5f));
                                break;

                            case EditTypes.UpdateLabelSizeDefault:
                                // Absolute scale to 100%

                                tempContentLabel.AddAction(new CCScaleTo(0.2f, 1f));
                                break;

                            case EditTypes.UpdateLabelSizeDown:
                                // Relative scale to 90%

                                tempContentLabel.AddAction(new CCScaleBy(0.2f, 0.9f));
                                break;

                            case EditTypes.UpdateLabelSizeDown2:
                                // Relative scale to 50%

                                tempContentLabel.AddAction(new CCScaleBy(0.2f, 0.5f));
                                break;
                        }

                        if (mSprite.Tag == SpriteTypes.FolderTag)
                        {
                            for (var i = storedList.Count - 1; i >= 0; i--)
                            {
                                if (storedList[i].FolderName == tempContentLabel.Text)
                                {
                                    var newStoredIconRef = new StoredIconReference(storedList[i].Sprite,
                                        storedList[i].Base64,
                                        storedList[i].FolderName,
                                        storedList[i].Sprite.ScaleX,
                                        tempContentLabel.ScaleX,
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
                    else if (edit == EditTypes.UpdateSizeUp2)
                    {
                        // The referenced icon, as retrieved from the list
                        var mIconRef = iconList2[index];

                        // The sprite from the reference
                        var mSprite = mIconRef.Sprite;

                        // Relative scale 110%
                        var action = new CCScaleBy(0.5f, 1.5f);

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
                    else if (edit == EditTypes.UpdateSizeDown2)
                    {
                        var mIconRef = iconList2[index];
                        var mSprite = mIconRef.Sprite;

                        // Relative scale 90%
                        var action = new CCScaleBy(0.5f, 0.5f);

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
        public void RemoveIconFromFieldAndCache(CCSprite sprite)
        {
            sprite.RemoveFromParent(true);

            CCSprite temp = sprite.GetChildByTag(Tags.SpriteTypes.ImageTag) as CCSprite;

            if (temp != null)
            {
                CCTextureCache.SharedTextureCache.RemoveTexture(temp.Texture);
                temp.Texture.Dispose();
                temp.RemoveFromParent();
            }

            tempContentLabel = sprite.GetChildByTag(Tags.SpriteTypes.ContentTag) as CCLabel;

            if (tempContentLabel != null)
            {
                CCTextureCache.SharedTextureCache.RemoveTexture(tempContentLabel.Texture);
                tempContentLabel.Texture.Dispose();
                tempContentLabel.RemoveFromParent();
            }

            sprite.RemoveAllChildren(true);

            CCTextureCache.SharedTextureCache.RemoveTexture(sprite.Texture);
            sprite.Texture.Dispose();

            CCTextureCache.SharedTextureCache.RemoveUnusedTextures();

            temp = null;
            tempContentLabel = null;
            sprite = null;
        }

        /// <summary>
        /// Clean the icons out of modal thoroughly, as there are many
        /// </summary>
        public void ClearIconsInModal()
        {
            var mIcons = windowFrame.Children.Where(t => t.Tag == SpriteTypes.IconTag || t.Tag == SpriteTypes.EmbeddedIconTag).ToList();

            CCSprite temp;

            for (var i = 0; i < mIcons.Count; i++)
            {
                temp = mIcons[i] as CCSprite;

                // If null, pass on
                if (temp == null)
                {
                    continue;
                }

                // If this matches, pass on
                if (temp.GetHashCode() == CurrentSpriteTouched.GetHashCode())
                {
                    continue;
                }

                if (temp.Texture.Equals(backingSpriteFrame.Texture))
                {
                    continue;
                }

                // If eligible, chuck this one out
                temp.RemoveFromParent(true);
                temp.RemoveEventListeners(false);
                CCTextureCache.SharedTextureCache.RemoveTexture(temp.Texture);

                temp.Texture.Dispose();
            }

            temp = null;
            mIcons.Clear();

            mIcons = null;
        }

        /// <summary>
        /// Deselect all icons in field (cast to white)
        /// </summary>
        public void DeSelectIcons()
        {
            foreach (IconReference iconRef in iconList2)
            {
                if (iconRef.Sprite.Color != ColorTools.White)
                {
                    iconRef.Sprite.Color = ColorTools.White;
                }
            }
        }

        /// <summary>
        /// Show window with icons matching query
        /// </summary>
        /// <param name="mMatchingIcons"></param>
        public void ShowCategoryWindow(List<Storage> mMatchingIcons)
        {
            // If already modal mode, just return
            if (isModal) return;

            MaskBackground();

            ScheduleOnce(async (dt) =>
            {
                var texture2 = new CCRenderTexture(new CCSize(200, 200), new CCSize(200, 200), CCSurfaceFormat.Color);
                texture2.BeginWithClear(CCColor4B.White);
                texture2.End();
                windowFrame = new CCSprite(texture2.Texture)
                {
                    PositionX = addFrame.Position.X,
                    PositionY = addFrame.Position.Y,
                    Tag = SpriteTypes.WindowTag
                };
                texture2.Dispose();

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
                windowFrame.AddChild(closeButton, 9999, SpriteTypes.CloseWindowTag);

                AddEventListener(mListener.Copy(), windowFrame);
                tempWindow = new IconReference(windowFrame, "Window", 1f, true);
                iconList2.Add(tempWindow);

                var dimension = Math.Min(spriteModelFactory.DynamicHeight, spriteModelFactory.DynamicWidth);
                var scale = (dimension / (windowFrame.ContentSize.Width)) * 1f;

                AddChild(windowFrame, 1000, SpriteTypes.WindowTag);

                // Execute actions
                windowFrame.AddActions(false,
                    new CCMoveTo(0.2f, new CCPoint(spriteModelFactory.DynamicWidth / 2f, spriteModelFactory.DynamicHeight / 2f)),
                    new CCScaleTo(0.2f, scale));

                isModal = true;

                for (var i = 0; i < mMatchingIcons.Count; i++)
                {
                    CCSprite parentSprite = await BuildCategorySpriteTask(i, mMatchingIcons[i], backingSpriteFrame, windowFrame, scaling, this, mListener);
                    windowFrame.AddChild(parentSprite, 1001, SpriteTypes.EmbeddedIconTag);
                }
            }, 0);
        }

        /// <summary>
        /// Build sprite task from memory
        /// </summary>
        /// <param name="i"></param>
        /// <param name="mCategoryIconRef"></param>
        /// <param name="backingSpriteFrame"></param>
        /// <param name="windowFrame"></param>
        /// <param name="scaling"></param>
        /// <param name="gameLayer"></param>
        /// <param name="mListener"></param>
        /// <returns></returns>
        public static Task<CCSprite> BuildCategorySpriteTask(int i, Storage mCategoryIconRef, CCSpriteFrame backingSpriteFrame, CCSprite windowFrame, float scaling, GameLayer gameLayer, CCEventListenerTouchOneByOne mListener)
        {
            TaskCompletionSource<CCSprite> tcs = new TaskCompletionSource<CCSprite>();
            var mSprite = BuildCategorySprite(i, mCategoryIconRef, backingSpriteFrame, windowFrame, scaling, gameLayer, mListener);
            tcs.SetResult(mSprite);

            return tcs.Task;
        }

        /// <summary>
        /// Build sprite from memory
        /// </summary>
        /// <param name="i"></param>
        /// <param name="mStoredIconRef"></param>
        /// <param name="backingSpriteFrame"></param>
        /// <param name="windowFrame"></param>
        /// <param name="scaling"></param>
        /// <param name="gameLayer"></param>
        /// <param name="mListener"></param>
        /// <returns></returns>
        public static CCSprite BuildCategorySprite(int i, Storage mStoredIconRef, CCSpriteFrame backingSpriteFrame, CCSprite windowFrame, float scaling, GameLayer gameLayer, CCEventListenerTouchOneByOne mListener)
        {
            // The sprite from the reference
            var parentSprite = new CCSprite(backingSpriteFrame);

            parentSprite.ContentSize = new CCSize(windowFrame.ContentSize.Width * 0.25f, windowFrame.ContentSize.Height * 0.25f);

            var pSpacing = parentSprite.ContentSize.Width * 0.15f;
            var xSpacing = parentSprite.ContentSize.Width * ((i % 3)) + pSpacing * ((i % 3) + 1);
            var ySpacing = parentSprite.ContentSize.Height * ((i / 3)) + pSpacing * ((i / 3) + 1);

            parentSprite.PositionX = (parentSprite.ContentSize.Width * 0.5f) + xSpacing;
            parentSprite.PositionY = windowFrame.ContentSize.Height - (parentSprite.ContentSize.Height * 0.5f) - ySpacing;

            parentSprite.Tag = SpriteTypes.IconTag;

            var subIconFrame = new CCSprite("Stored/" + mStoredIconRef.Name)
            {
                AnchorPoint = CCPoint.AnchorMiddle,
                ContentSize = new CCSize(parentSprite.ContentSize.Width * 0.75f, parentSprite.ContentSize.Height * 0.75f),
                PositionX = parentSprite.ContentSize.Width * 0.5f,
                PositionY = parentSprite.ContentSize.Height * 0.5f + parentSprite.ContentSize.Height * 0.075f,
                Tag = SpriteTypes.ImageTag
            };

            var label = new CCLabel(mStoredIconRef.Name, "Arial", 22, CCLabelFormat.SystemFont)
            {
                Scale = 0.25f * scaling,
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
            gameLayer.AddEventListener(mListener.Copy(), parentSprite);

            parentSprite.AddChild(label);

            return parentSprite;
        }

        /// <summary>
        /// Show folder contents window
        /// </summary>
        /// <param name="currentSprite">Current sprite (folder) touched</param>
        /// <param name="folderName">Name of the folder</param>
        public void ShowStoredWindow(CCSprite currentSprite, string folderName)
        {
            // If already modal mode, just return
            if (isModal) return;

            MaskBackground();

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

                AddEventListener(mListener.Copy(), windowFrame);
                tempWindow = new IconReference(windowFrame, "Window", 1f, true);
                iconList2.Add(tempWindow);
                AddChild(windowFrame, 1000, SpriteTypes.WindowTag);

                var dimension = Math.Min(spriteModelFactory.DynamicHeight, spriteModelFactory.DynamicWidth);
                var scale = (dimension / (windowFrame.ContentSize.Width)) * 1f;

                // Blur background, to focus the listener
                // var maskBackground = new CCCallFunc(MaskBackground);

                // Reveal the icons after scaling
                var revealIcons = new CCCallFunc(ShowIconsInModal);

                // Execute actions
                windowFrame.AddActions(false,
                    new CCMoveTo(0.2f, new CCPoint(spriteModelFactory.DynamicWidth / 2f, spriteModelFactory.DynamicHeight / 2f)),
                    new CCScaleTo(0.2f, scale));

                isModal = true;
            }, 0);

            ShowStoredSprites(folderName, storedList, backingSpriteFrame, windowFrame, scaling, this, mListener);
        }

        /// <summary>
        /// Show icons stored in folder, main call from user loop
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="storedList"></param>
        /// <param name="backingSpriteFrame"></param>
        /// <param name="windowFrame"></param>
        /// <param name="scaling"></param>
        /// <param name="gameLayer"></param>
        /// <param name="mListener"></param>
        public static void ShowStoredSprites(string folderName, List<StoredIconReference> storedList, CCSpriteFrame backingSpriteFrame, CCSprite windowFrame, float scaling, GameLayer gameLayer, CCEventListenerTouchOneByOne mListener)
        {
            List<StoredIconReference> mEqualList;

            // !important: lock for concurrency issues
            lock (storedList)
            {
                mEqualList = storedList.Where(l => l.FolderName == folderName).ToList();
            }

            ShowStoredSprites(mEqualList, backingSpriteFrame, windowFrame, scaling, gameLayer, mListener);
        }

        /// <summary>
        /// Method to build icons for folder
        /// </summary>
        /// <param name="mEqualList"></param>
        /// <param name="backingSpriteFrame"></param>
        /// <param name="windowFrame"></param>
        /// <param name="scaling"></param>
        /// <param name="gameLayer"></param>
        /// <param name="mListener"></param>
        public static async void ShowStoredSprites(List<StoredIconReference> mEqualList, CCSpriteFrame backingSpriteFrame, CCSprite windowFrame, float scaling, GameLayer gameLayer, CCEventListenerTouchOneByOne mListener)
        {
            for (var i = 0; i < mEqualList.Count; i++)
            {
                var mSprite = await BuildStoredSpriteTask(i, mEqualList[i], backingSpriteFrame, windowFrame, scaling, gameLayer, mListener);

                if (mSprite != null)
                {
                    gameLayer.ScheduleOnce((dt) => 
                    {
                        windowFrame.AddChild(mSprite, 1001, SpriteTypes.IconTag);
                    }, 0);                    
                }
            }

            mEqualList = null;
        }

        /// <summary>
        /// Awaitable task for building stored icons
        /// </summary>
        /// <param name="i"></param>
        /// <param name="mStoredIconRef"></param>
        /// <param name="backingSpriteFrame"></param>
        /// <param name="windowFrame"></param>
        /// <param name="scaling"></param>
        /// <param name="gameLayer"></param>
        /// <param name="mListener"></param>
        /// <returns></returns>
        public static Task<CCSprite> BuildStoredSpriteTask(int i, StoredIconReference mStoredIconRef, CCSpriteFrame backingSpriteFrame, CCSprite windowFrame, float scaling, GameLayer gameLayer, CCEventListenerTouchOneByOne mListener)
        {
            TaskCompletionSource<CCSprite> tcs = new TaskCompletionSource<CCSprite>();
            var mSprite = BuildStoredSprite(i, mStoredIconRef, backingSpriteFrame, windowFrame, scaling, gameLayer, mListener);
            tcs.SetResult(mSprite);

            return tcs.Task;
        }

        /// <summary>
        /// Create icons stored in folders
        /// </summary>
        /// <param name="i"></param>
        /// <param name="mStoredIconRef"></param>
        /// <param name="backingSpriteFrame"></param>
        /// <param name="windowFrame"></param>
        /// <param name="scaling"></param>
        /// <param name="gameLayer"></param>
        /// <param name="mListener"></param>
        /// <returns></returns>
        public static CCSprite BuildStoredSprite(int i, StoredIconReference mStoredIconRef, CCSpriteFrame backingSpriteFrame, CCSprite windowFrame, float scaling, GameLayer gameLayer, CCEventListenerTouchOneByOne mListener)
        {
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

                bytes = null;

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
                gameLayer.AddEventListener(mListener.Copy(), parentSprite);

                parentSprite.AddChild(label);

                return parentSprite;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Mask background for modal window
        /// </summary>
        public void MaskBackground()
        {
            var borderBackGray = new CCSprite(staticSpriteSheet.Frames.Find((x) => x.TextureFilename.Contains("frameWhite")));
            borderBackGray.Color = CCColor3B.Gray;
            borderBackGray.Opacity = 200;
            borderBackGray.ContentSize = new CCSize(spriteModelFactory.DynamicWidth + 5, spriteModelFactory.DynamicHeight + 5);
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
                var fade = new CCFadeIn(i / 10f);
                mIcons[i].AddActions(false, opacity, show, fade);
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
            CurrentSpriteTouched = null;
            touchType = Tags.Tag.None;

            startTime = DateTime.Now;

            caller = touchEvent.CurrentTarget as CCSprite;

            #region Pull selected sprite to forefront, for clarity

            iconsOverlapping = iconList2.Where(i => i.Sprite.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location)).OrderByDescending(i => i.Sprite.ZOrder).ToList();

            if (iconsOverlapping.Any())
            {
                highSprite = iconsOverlapping.First().Sprite;

                ReorderChild(highSprite, zIndexTop);

                foreach (IconReference iconRef in iconList2.Where(i => i.Sprite.Tag == SpriteTypes.IconTag && i.Sprite.GetHashCode() != highSprite.GetHashCode()))
                {
                    ReorderChild(iconRef.Sprite, zIndexBack);
                }
            }

            #endregion

            #region Modal Condition, for Touch

            if (windowFrame != null)
            {
                // Window tagged for touch
                if (caller.Tag == SpriteTypes.WindowTag && windowFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    CurrentSpriteTouched = windowFrame;
                    touchType = Tags.Tag.Window;

                    return true;
                }
                // Point is within the window, but hit something else first
                else if (windowFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    // If it was the close button, tag as such first
                    if (closeButton.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                    {
                        closeButton.Opacity = 100;

                        touchType = Tags.Tag.CloseButton;
                        CurrentSpriteTouched = closeButton;

                        return true;
                    }

                    var mIcons = windowFrame.Children;

                    // Loop through children, tag as such if in the touch region
                    foreach (var mIcon in mIcons)
                    {
                        #region Icon Already in Folder, not embedded

                        if (mIcon.Tag == SpriteTypes.IconTag && mIcon.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                        {
                            touchType = Tags.Tag.TransitionIcon;
                            CurrentSpriteTouched = mIcon as CCSprite;

                            return true;
                        }

                        #endregion

                        #region Icon Embedded in Memory

                        else if (mIcon.Tag == SpriteTypes.EmbeddedIconTag && mIcon.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                        {
                            touchType = Tags.Tag.FolderedIcon;
                            CurrentSpriteTouched = mIcon as CCSprite;

                            return true;
                        }

                        #endregion
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

            #endregion

            #region Speak Frame Contents

            if (caller.GetHashCode() == speakerFrame.GetHashCode())
            {

                foreach (IconReference iconRef in iconList2)
                {
                    // If Icon is overlapping, prevent fire
                    if (speakerFrame.BoundingBoxTransformedToParent.IntersectsRect(iconRef.Sprite.BoundingBoxTransformedToWorld))
                    {
                        return false;
                    }
                }

                if (speakerFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.Speak;
                    CurrentSpriteTouched = speakerFrame;
                    speakerFrame.Opacity = 155;

                    return true;
                }
                else
                {
                    CurrentSpriteTouched = null;
                    speakerFrame.Opacity = 255;

                    return false;
                }
            }

            #endregion

            #region Add Icon Button

            else if (caller.GetHashCode() == addFrame.GetHashCode())
            {
                // Prevent calling add event, if icon overlaps
                foreach (IconReference iconRef in iconList2)
                {
                    if (addFrame.BoundingBoxTransformedToParent.IntersectsRect(iconRef.Sprite.BoundingBoxTransformedToWorld))
                    {
                        return false;
                    }
                }

                if (addFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    touchType = Tags.Tag.Add;
                    CurrentSpriteTouched = addFrame;
                    addFrame.Opacity = 155;

                    return true;
                }
                else
                {
                    CurrentSpriteTouched = null;
                    addFrame.Opacity = 255;

                    return false;
                }
            }

            #endregion
            
            #region Field Icon Touch

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
                                var iconsTouched = iconList2.Where(i => i.Sprite.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location)).OrderBy(i => i.Sprite.ZOrder).ToList();

                                if (iconsTouched != null && iconsTouched.Count() > 1)
                                {
                                    var highestIcon = iconsTouched.Last();

                                    touchType = Tags.Tag.Icon;

                                    CurrentSpriteTouched = highestIcon.Sprite;
                                    highestIcon.Sprite.Opacity = 155;

                                    DeSelectIcons();

                                    ReorderChild(highestIcon.Sprite, zIndexTop);

                                    highestIcon.Sprite.Color = ColorTools.Green;
                                }
                                else
                                {
                                    touchType = Tags.Tag.Icon;

                                    CurrentSpriteTouched = iconRef.Sprite;
                                    caller.Opacity = 155;

                                    DeSelectIcons();

                                    ReorderChild(caller, zIndexTop);

                                    caller.Color = ColorTools.Green;
                                }

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
                                var iconsTouched = iconList2.Where(i => i.Sprite.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location)).OrderByDescending(i => i.Sprite.ZOrder);

                                if (iconsTouched != null && iconsTouched.Count() > 1)
                                {
                                    var highestIcon = iconsTouched.Last();

                                    touchType = Tags.Tag.Icon;
                                    CurrentSpriteTouched = highestIcon.Sprite;
                                    highestIcon.Sprite.Opacity = 155;

                                    if (!inEditMode)
                                    {
                                        highestIcon.Sprite.Color = ColorTools.Green;
                                    }
                                }
                                else
                                {
                                    touchType = Tags.Tag.Icon;
                                    CurrentSpriteTouched = iconRef.Sprite;
                                    iconRef.Sprite.Opacity = 155;

                                    if (!inEditMode)
                                    {
                                        iconRef.Sprite.Color = ColorTools.Green;
                                    }
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

            #endregion

            return false;

        }

        /// <summary>
        /// Touch end listener
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
        void OnTouchesEnded(CCTouch touch, CCEvent touchEvent)
        {
            if (CurrentSpriteTouched != null)
            {
                timeDiff = DateTime.Now - startTime;

                #region Ended on Active Icon in Field

                if (touchType == Tags.Tag.Icon)
                {
                    caller = (CCSprite)touchEvent.CurrentTarget;
                    CurrentSpriteTouched.Opacity = 255;
                    CurrentSpriteTouched.ZOrder = 999;

                    #region Check if icon over a folder

                    List<IconReference> mFolders = iconList2.Where(t => t.Sprite.Tag == SpriteTypes.FolderTag).ToList();
                    var mIntersect = mFolders.Where(t => t.Sprite.BoundingBoxTransformedToParent.IntersectsRect(caller.BoundingBoxTransformedToWorld)).ToList();

                    if (mIntersect.Count == 1)
                    {
                        string mContentTag = SpriteTools.SpriteHasLabel(mIntersect[0].Sprite);

                        if (mContentTag != "")
                        {
                            var mCloneCopy = iconList2.Where(t => t.Sprite.GetHashCode() == caller.GetHashCode()).FirstOrDefault();

                            tempContentLabel = mCloneCopy.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                            if (tempContentLabel == null)
                            {
                                return;
                            }

                            var newItem = new StoredIconReference(mCloneCopy.Sprite, mCloneCopy.Base64, mContentTag, mCloneCopy.Sprite.ScaleX, tempContentLabel.ScaleX, tempContentLabel.Visible);

                            CurrentSpriteTouched = null;

                            ScheduleOnce((dt) =>
                            {
                                var moveAction = new CCMoveTo(0.2f, mIntersect[0].Sprite.Position);
                                var scaleAction = new CCScaleTo(0.2f, 0.1f);
                                var clearColor = new CCCallFuncN(node => node.Color = ColorTools.White);
                                var setInvisible = new CCCallFuncN(node => node.Visible = false);
                                var scaleAction2 = new CCScaleTo(0.01f, (float)mCloneCopy.Sprite.ScaleX);

                                // Animation to make identified folder target salient
                                var danceAction = new CCCallFunc(() => {
                                    mIntersect[0].Sprite.AddAction(AnimationTools.iconAnimationRotate);
                                });

                                var endAction = new CCCallFuncN(node => node.RemoveFromParent(true));

                                caller.AddActions(false,
                                    moveAction,
                                    scaleAction,
                                    clearColor,
                                    setInvisible,
                                    scaleAction2,
                                    danceAction,
                                    endAction);
                            }, 0);

                            iconList2.Remove(mCloneCopy);
                            storedList.Add(newItem);

                            return;
                        }
                    }

                    #endregion

                    #region Check if icon over frame

                    if (!inEditMode && sentenceFrame.BoundingBoxTransformedToWorld.IntersectsRect(caller.BoundingBoxTransformedToWorld))
                    {
                        float xScale = caller.ScaleX,
                        yScale = caller.ScaleX;

                        // Catch for potentially overlapping rescaling
                        if (caller.NumberOfRunningActions == 0)
                        {
                            CCSequence iconAnimationFocus = new CCSequence(
                                new CCDelayTime(0.1f),
                                new CCScaleTo(0.1f, xScale * 1.1f, yScale * 1.1f),
                                new CCScaleTo(0.1f, xScale, yScale));

                            caller.AddAction(iconAnimationFocus);
                        }
                    }

                    #endregion

                    #region Check if over delete field

                    if (deleteFrame.BoundingBoxTransformedToParent.IntersectsRect(caller.BoundingBoxTransformedToWorld) && inEditMode)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == caller.GetHashCode()).FirstOrDefault();

                        ScheduleOnce((dt) => {
                            //RemoveChild(target);

                            // <!-- Note: Edited Cleanup Here 

                            CCSprite temp = caller.GetChildByTag(Tags.SpriteTypes.ImageTag) as CCSprite;

                            if (temp != null)
                            {
                                CCTextureCache.SharedTextureCache.RemoveTexture(temp.Texture);
                                temp.Texture.Dispose();
                            }

                            caller.RemoveAllChildren();
                            caller.RemoveEventListeners(false);
                            caller.RemoveFromParent();

                            // -->
                        }, 0);

                        iconList2.Remove(mSprite);
                    }

                    #endregion

                    #region Ended on icon, in edit mode

                    else if (inEditMode && timeDiff.TotalSeconds < 0.25)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == caller.GetHashCode()).FirstOrDefault();
                        var mCounter = iconList2.IndexOf(mSprite);

                        //GamePageParent.mInputFactory.CallActionSheet(mCounter);
                        App.InputFactory.CallActionSheet(mCounter);
                    }

                    #endregion
                }

                #endregion

                #region Ended on Active Folder in Field, in Edit Mode, short touch, open mod window

                else if (touchType == Tags.Tag.FolderIcon && inEditMode && timeDiff.TotalSeconds < 0.25)
                {
                    CurrentSpriteTouched.Opacity = 255;

                    var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == touchEvent.CurrentTarget.GetHashCode()).FirstOrDefault();
                    var mCounter = iconList2.IndexOf(mSprite);

                    mSprite = null;

                    //GamePageParent.mInputFactory.CallActionSheet(mCounter);
                    App.InputFactory.CallActionSheet(mCounter);               
                }

                #endregion

                #region Ended on Active Folder in Field, NOT in Edit Mode

                else if (touchType == Tags.Tag.FolderIcon && !inEditMode)
                {
                    CurrentSpriteTouched.Opacity = 255;

                    string contentTag = SpriteTools.SpriteHasLabel(CurrentSpriteTouched);

                    if (contentTag != "")
                    {
                        ShowStoredWindow(CurrentSpriteTouched, contentTag);
                    }
                }

                #endregion

                #region Ended on Add Icon Button

                else if (touchType == Tags.Tag.Add && inEditMode)
                {
                    CurrentSpriteTouched.Opacity = 255;

                    //GamePageParent.mInputFactory.CallActionSheetChoice(new List<IconReference>(iconList2));
                    App.InputFactory.CallActionSheetChoice(new List<IconReference>(iconList2));
                }

                #endregion
                
                #region Ended on Folder Icon (active), both short and for possible delete

                else if (touchType == Tags.Tag.FolderIcon && inEditMode)
                {
                    caller = (CCSprite)touchEvent.CurrentTarget;
                    CurrentSpriteTouched.Opacity = 255;

                    if (deleteFrame.BoundingBoxTransformedToParent.IntersectsRect(caller.BoundingBoxTransformedToWorld) && inEditMode)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == caller.GetHashCode()).FirstOrDefault();

                        ScheduleOnce((dt) => {
                            RemoveChild(caller);
                        }, 0);

                        iconList2.Remove(mSprite);

                        mSprite = null;
                    }
                    else if (timeDiff.TotalSeconds < 0.25f)
                    {
                        var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == caller.GetHashCode()).FirstOrDefault();
                        var mCounter = iconList2.IndexOf(mSprite);

                        //GamePageParent.mInputFactory.CallActionSheet(mCounter);
                        App.InputFactory.CallActionSheet(mCounter);

                        mSprite = null;
                    }
                }

                #endregion

                #region Ended on Speech Emitter, when held for under 5 seconds

                else if (touchType == Tags.Tag.Speak && timeDiff.TotalSeconds < 5.0f)
                {
                    CurrentSpriteTouched.Opacity = 255;

                    List<IconReference> mList = null;

                    if (!inSingleMode)
                    {
                        mList = iconList2.Where(t => sentenceFrame.BoundingBoxTransformedToParent.IntersectsRect(t.Sprite.BoundingBoxTransformedToParent) == true).ToList();
                    }
                    else
                    {
                        mList = iconList2.Where(t => t.Sprite.Color == ColorTools.Green).ToList();
                    }

                    if (mList.Count > 1)
                    {
                        mList = mList.OrderBy(i => i.Sprite.PositionX).ToList();
                    }

                    List<string> labelList = new List<string>();

                    string contentTag;

                    foreach (IconReference mIcon in mList)
                    {
                        contentTag = SpriteTools.SpriteHasLabel(mIcon.Sprite);

                        if (contentTag != "")
                        {
                            labelList.Add(contentTag);
                        }
                    }

                    var outputString = string.Join(" ", labelList.ToArray());

                    DependencyService.Get<ITextToSpeech>().Speak(outputString);

                    if (unselectAuto && inSingleMode)
                    {
                        DeSelectIcons();
                    }
                }

                #endregion

                #region Ended on Speech Emitter, when held for 5 seconds or more

                // Toggles edit mode
                else if (touchType == Tags.Tag.Speak && timeDiff.TotalSeconds >= 5.0f)
                {
                    CurrentSpriteTouched.Opacity = 255;

                    SetEditMode(!inEditMode);
                }

                #endregion
                
                #region Ended on Close Modal Window Button

                else if (touchType == Tags.Tag.CloseButton)
                {
                    CurrentSpriteTouched.Opacity = 255;

                    if (closeButton.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                    {
                        isModal = false;

                        ClearIconsInModal();

                        RemoveAllChildrenByTag(windowFrame.Tag, true);
                        RemoveAllChildrenByTag(SpriteTypes.ColorLayerTag, true);

                        var windowRef = iconList2.Where(t => t.Sprite.Tag == windowFrame.Tag).FirstOrDefault();
                        iconList2.Remove(windowRef);

                        windowFrame = null;
                        closeButton = null;
                    }
                }

                #endregion

                #region Ended on Embedded Icon in Window, short touch 

                else if (touchType == Tags.Tag.FolderedIcon)
                {
                    bool isAlreadySelected = (CurrentSpriteTouched.Opacity == 55);

                    var mIconsChoice = windowFrame.Children.ToList();

                    foreach (CCSprite loopIcon in mIconsChoice)
                    {
                        loopIcon.Opacity = 255;
                    }

                    if (CurrentSpriteTouched.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location) && timeDiff.TotalSeconds < 0.25)
                    {
                        if (!isAlreadySelected)
                        {
                            CurrentSpriteTouched.Opacity = 55;
                        }
                        else
                        {
                            // TODO: this is the reference that is preventing cleanup

                            //GamePageParent.mInputFactory.NameEmbeddedIcon(CurrentSpriteTouched);
                            App.InputFactory.NameEmbeddedIcon(CurrentSpriteTouched);
                           
                            isModal = false;

                            ClearIconsInModal();

                            // Remove Window
                            RemoveAllChildrenByTag(windowFrame.Tag, true);
                            windowFrame.RemoveEventListeners(false);

                            // Remove Mask
                            RemoveAllChildrenByTag(SpriteTypes.ColorLayerTag, true);

                            var windowRef = iconList2.Where(t => t.Sprite.Tag == windowFrame.Tag).FirstOrDefault();
                            iconList2.Remove(windowRef);

                            windowFrame = null;
                            closeButton = null;
                        }
                    }
                }

                #endregion

                #region Ended on Foldered Icon in Window, short touch 

                else if (touchType == Tags.Tag.TransitionIcon)
                {
                    bool isAlreadySelected = (CurrentSpriteTouched.Opacity == 55);

                    var mIconsChoice = windowFrame.Children.ToList();

                    foreach (CCSprite loopIcon in mIconsChoice)
                    {
                        loopIcon.Opacity = 255;
                    }

                    if (CurrentSpriteTouched.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location) && timeDiff.TotalSeconds < 0.25)
                    {
                        if (!isAlreadySelected)
                        {
                            CurrentSpriteTouched.Opacity = 55;
                        }
                        else
                        {
                            tempContentLabel = CurrentSpriteTouched.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                            if (tempContentLabel != null)
                            {
                                StoredIconReference mStoredRef = null;

                                lock (storedList)
                                {
                                    foreach (StoredIconReference storedRef in storedList)
                                    {
                                        caller = storedRef.Sprite;
                                        var mLoopContent = caller.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                                        if (mLoopContent != null && mLoopContent.Text == tempContentLabel.Text)
                                        {
                                            var xMin = (spriteModelFactory.DynamicHeight * 0.1f) / 2;
                                            var yLocation = mRandom.Next((int)(spriteModelFactory.DynamicHeight * 0.3f), (int)(spriteModelFactory.DynamicHeight - (spriteModelFactory.DynamicHeight * 0.3f)));
                                            var xLocation = mRandom.Next((int)(spriteModelFactory.DynamicWidth * 0.3f), (int)(spriteModelFactory.DynamicWidth - (spriteModelFactory.DynamicWidth * 0.3f)));

                                            ScheduleOnce(async (dt) =>
                                            {
                                                var newIcon = await spriteModelFactory.AsyncCreateBase64Sprite(backingSpriteFrame, 
                                                                                                               storedRef.Base64, 
                                                                                                               mLoopContent.Text, 
                                                                                                               xLocation, 
                                                                                                               yLocation, 
                                                                                                               storedRef.Scale, 
                                                                                                               storedRef.TextScale, 
                                                                                                               storedRef.TextVisible);

                                                var mIconRef = new IconReference(newIcon, 
                                                                                 storedRef.Base64, 
                                                                                 1f, 
                                                                                 true);

                                                AddEventListener(mListener.Copy(), mIconRef.Sprite);

                                                iconList2.Add(mIconRef);

                                                AddChild(mIconRef.Sprite);

                                                // Add salient animation to icons added back to field
                                                mIconRef.Sprite.AddAction(AnimationTools.iconAnimationRotate);
                                            }, 0.01f);

                                            mStoredRef = storedRef;
                                        }
                                    }

                                    if (mStoredRef != null)
                                    {
                                        storedList.Remove(mStoredRef);
                                    }
                                }
                            }

                            // TODO same here, ref to CurrentIconSelected throws off texture removals

                            isModal = false;

                            ClearIconsInModal();

                            RemoveAllChildrenByTag(windowFrame.Tag, true);
                            RemoveAllChildrenByTag(SpriteTypes.ColorLayerTag, true);

                            var windowRef = iconList2.Where(t => t.Sprite.Tag == windowFrame.Tag).FirstOrDefault();
                            iconList2.Remove(windowRef);

                            windowFrame = null;
                            closeButton = null;
                        }
                    }
                }

                #endregion

                CurrentSpriteTouched = null;
            }
        }

        public void RemoteManageIcon(ServerComms serverReq)
        {
            if (serverReq.Method.Contains("Delete"))
            {
                Debug.WriteLineIf(App.Debugging, "in Delete call");

                var mSprite = iconList2.Where(t => t.Sprite.GetHashCode() == serverReq.HashCode).FirstOrDefault();

                ScheduleOnce((dt) => {

                    CCSprite temp = caller.GetChildByTag(Tags.SpriteTypes.ImageTag) as CCSprite;

                    if (temp != null)
                    {
                        CCTextureCache.SharedTextureCache.RemoveTexture(temp.Texture);
                        temp.Texture.Dispose();
                    }

                    mSprite.Sprite.RemoveAllChildren();
                    mSprite.Sprite.RemoveEventListeners(false);
                    mSprite.Sprite.RemoveFromParent();
                }, 0);

                iconList2.Remove(mSprite);

                // Force saving

                lock (iconList2)
                {
                    lock (storedList)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLineIf(App.Debugging, "Saving...");

                            FileTools.SaveToDatabase(iconList2, storedList, inSingleMode, unselectAuto);
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLineIf(App.Debugging, "Ex: " + ex.ToString());
                        }
                    }
                }


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
                #region Icon in Active Field

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

                #endregion

                #region Touching speaker button, at least initially

                else if (touchType == Tags.Tag.Speak)
                {
                    // If the touch veers from bounds of speaker, reset 
                    if (!speakerFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                    {
                        CurrentSpriteTouched = null;
                        speakerFrame.Opacity = 255;
                        return;
                    }
                }

                #endregion

                #region Touching add icon button, at least initially

                else if (touchType == Tags.Tag.Add)
                {
                    // If the touch veers from bounds of add frame, reset 
                    if (!addFrame.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                    {
                        CurrentSpriteTouched = null;
                        addFrame.Opacity = 255;
                        return;
                    }
                }

                #endregion
                
                #region Touching window, directly

                else if (touchType == Tags.Tag.Window || touchType == Tags.Tag.TransitionIcon)
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
                                mIcons[i].Visible = (mIcons[i].Position.Y > (clampHigh + mIcons[i].ContentSize.Width) ||
                                    mIcons[i].Position.Y < (clampLow - mIcons[i].ContentSize.Width)) ? false : true;
                                mIcons[i].PositionY = mIcons[i].PositionY + deltaPos;
                            }
                        }
                    }
                }

                #endregion

                #region Touch Close Window Button, directly

                else if (touchType == Tags.Tag.CloseButton)
                {
                    if (closeButton.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                    {
                        closeButton.Opacity = 100;
                    }
                    else
                    {
                        closeButton.Opacity = 255;
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// In game logic
        /// </summary>
        /// <param name="frameTimeInSeconds"></param>
        void RunGameLogic(float frameTimeInSeconds)
        {
            totalDuration += frameTimeInSeconds;

            #region Manually null out the older scene

            if (App.Loaded)
            {
                ScheduleOnce((dt) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        App.StartScene = null;                       
                    });
                }, 1f);

                App.Loaded = false;
            }

            #endregion

            #region Save Interval

            if (totalDuration > saveInterval)
            {
                SaveContent();

                totalDuration = 0f;

                CCTextureCache.SharedTextureCache.DumpCachedTextureInfo();
                CCTextureCache.SharedTextureCache.RemoveUnusedTextures();
            }

            #endregion 

            #region Loop through items in field, alter colors if necessary

            foreach (var t in iconList2)
            {
                if (t.Sprite.Tag == SpriteTypes.IconTag || t.Sprite.Tag == SpriteTypes.FolderTag)
                {
                    if (!inSingleMode)
                    {
                        if (!sentenceFrame.BoundingBoxTransformedToParent.IntersectsRect(t.Sprite.BoundingBoxTransformedToParent))
                        {
                            if (t.Sprite.Color != ColorTools.White)
                            {
                                t.Sprite.Color = ColorTools.White;
                            }
                        }
                        else
                        {
                            if (t.Sprite.Color != ColorTools.Green)
                            {
                                t.Sprite.Color = ColorTools.Green;
                            }
                        }
                    }

                    if (inEditMode)
                    {
                        if (deleteFrame.BoundingBoxTransformedToParent.IntersectsRect(t.Sprite.BoundingBoxTransformedToParent))
                        {
                            if (t.Sprite.Color != ColorTools.Red)
                            {
                                t.Sprite.Color = ColorTools.Red;
                            }
                        }
                        else
                        {
                            if (t.Sprite.Color != ColorTools.White)
                            {
                                t.Sprite.Color = ColorTools.White;
                            }
                        }
                    }
                }
            }

            #endregion

        }

        /// <summary>
        /// Call to save json, made public to enable force saving
        /// </summary>
        public void SaveContent()
        {
            if (ServerActive)
            {
                return;
            }

            lock (iconList2)
            {
                lock (storedList)
                {
                    try 
                    {
                        System.Diagnostics.Debug.WriteLineIf(App.Debugging, "Saving...");

                        FileTools.SaveToDatabase(iconList2, storedList, inSingleMode, unselectAuto);
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLineIf(App.Debugging, "Ex: " + ex.ToString());
                    }

                    // TODO: check if listeners assigned?
                }
            }
        }

        /// <summary>
        /// Load JSON data into memory, for parsing later.  Categories derived at start.
        /// </summary>
        public void LoadJsonContent()
        {
            using (Stream stream = App.MainAssembly.GetManifestResourceStream(App.JsonAddress))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    storageInformation = JsonConvert.DeserializeObject<StorageContainer>(reader.ReadToEnd());
                }
            }

            CategoryList = storageInformation.StoredIcons
                .SelectMany(m => m.Tags)
                .Distinct()
                .ToList();
        }
    }
}
