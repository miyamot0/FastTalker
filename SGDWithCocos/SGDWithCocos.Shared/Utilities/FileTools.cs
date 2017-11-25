//----------------------------------------------------------------------------------------------
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

using Newtonsoft.Json;
using SGDWithCocos.Interface;
using SGDWithCocos.Models;
using SGDWithCocos.Tags;
using System.Collections.Generic;
using Xamarin.Forms;
using CocosSharp;
using SGDWithCocos.Shared;
using System;
using System.Diagnostics;

namespace SGDWithCocos.Utilities
{
    public static class FileTools
    {
        public static void SaveToDatabase(List<IconReference> icons, List<StoredIconReference> storedIcons, bool inSingleMode, bool autoDeselect)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // List of icons
                List<TableIcons> mIconArray = new List<TableIcons>();
                List<TableFolders> mFolderArray = new List<TableFolders>();

                TableIcons iconModel;
                TableFolders folderModel;

                CCLabel spriteLabel;

                // For active icons, save in respective lists
                foreach (IconReference iconRef in icons)
                {
                    if (iconRef.Sprite.Tag == SpriteTypes.IconTag)
                    {
                        iconModel = new TableIcons("", "", 0, 0, -1, iconRef.Sprite.ScaleX);

                        spriteLabel = iconRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        if (spriteLabel != null)
                        {
                            iconModel.Text = spriteLabel.Text;
                        }

                        iconModel.ID = mIconArray.Count;
                        iconModel.Base64 = iconRef.Base64;
                        iconModel.X = (int)iconRef.Sprite.PositionX;
                        iconModel.Y = (int)iconRef.Sprite.PositionY;
                        iconModel.Tag = iconRef.Sprite.Tag;
                        iconModel.TextScale = spriteLabel.ScaleX;
                        iconModel.TextVisible = spriteLabel.Visible;

                        mIconArray.Add(iconModel);
                    }
                    else if (iconRef.Sprite.Tag == SpriteTypes.FolderTag)
                    {
                        spriteLabel = iconRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        folderModel = new TableFolders("", "", "", 0, 0, -1, iconRef.Sprite.ScaleX);

                        if (spriteLabel != null)
                        {
                            folderModel.Text = spriteLabel.Text;
                        }

                        if (iconRef.Base64.Contains("FolderOpen"))
                        {
                            folderModel.AssetName = iconRef.Base64;
                            folderModel.Base64 = null;
                        }
                        else
                        {
                            folderModel.AssetName = null;
                            folderModel.Base64 = iconRef.Base64;
                        }

                        folderModel.ID = mFolderArray.Count;
                        folderModel.X = (int)iconRef.Sprite.PositionX;
                        folderModel.Y = (int)iconRef.Sprite.PositionY;
                        folderModel.Tag = iconRef.Sprite.Tag;
                        folderModel.TextScale = spriteLabel.ScaleX;
                        folderModel.TextVisible = spriteLabel.Visible;

                        mFolderArray.Add(folderModel);
                    }
                }

                try
                {
                    await App.Database.SaveIconsAsync(mIconArray);

                    //mIconArray.Clear();
                    //mIconArray = null;

                    await App.Database.SaveFoldersAsync(mFolderArray);

                    //mFolderArray.Clear();
                    //mFolderArray = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(App.Debugging, ex.ToString());
                    
                }


                List<TableStoredIcons> mStoredIconArray = new List<TableStoredIcons>();
                TableStoredIcons storedIconModel;

                foreach (StoredIconReference mStoredRef in storedIcons)
                {
                    Debug.WriteLineIf(App.Debugging, mStoredRef.FolderName);

                    storedIconModel = new TableStoredIcons("", "", 0, 0, -1, mStoredRef.Base64, mStoredRef.Sprite.ScaleX);

                    spriteLabel = mStoredRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CocosSharp.CCLabel;

                    if (spriteLabel != null)
                    {
                        storedIconModel.Text = spriteLabel.Text;
                    }

                    storedIconModel.ID = mStoredIconArray.Count;
                    storedIconModel.Base64 = mStoredRef.Base64;
                    storedIconModel.Folder = mStoredRef.FolderName;
                    storedIconModel.X = (int)mStoredRef.Sprite.PositionX;
                    storedIconModel.Y = (int)mStoredRef.Sprite.PositionY;
                    storedIconModel.Tag = mStoredRef.Sprite.Tag;
                    storedIconModel.TextScale = spriteLabel.ScaleX;
                    storedIconModel.TextVisible = spriteLabel.Visible;

                    mStoredIconArray.Add(storedIconModel);
                }

                await App.Database.SaveStoredIconsAsync(mStoredIconArray);

                //mStoredIconArray.Clear();
                //mStoredIconArray = null;

                TableSettings settings = new TableSettings();
                settings.ID = 0;
                settings.SingleMode = inSingleMode;
                settings.AutoUnselectSingleMode = autoDeselect;

                await App.Database.SaveSettingsAsync(settings);

                //settings = null;
            });
        }        
    }
}
