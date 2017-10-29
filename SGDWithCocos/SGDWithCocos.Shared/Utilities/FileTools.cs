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

namespace SGDWithCocos.Utilities
{
    public static class FileTools
    {
        /// <summary>
        /// Cross-platform call to get JSON-saved boards from local storage
        /// </summary>
        /// <param name="boardName">name for JSON file</param>
        /// <returns>JSON string</returns>
        public static string GetBoards(string boardName)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadJSON(boardName);
        }

        /// <summary>
        /// Cross-platform calls to save all icons/folders into local storage
        /// </summary>
        /// <param name="icons">All icons in field, folder or typical</param>
        /// <param name="storedIcons">All saved icons</param>
        /// <param name="inSingleMode">If in single or framed mode</param>
        public static string SaveBoards(List<IconReference> icons, List<StoredIconReference> storedIcons, bool inSingleMode, bool autoDeselect)
        {
            string mJsonString = "";

            Device.BeginInvokeOnMainThread(() =>
            {
                // List of icons
                List<IconModel> mJsonIconArray = new List<IconModel>();

                // List of folders
                List<FolderModel> mJsonFolderArray = new List<FolderModel>();

                // List of icons stored in folders
                List<StoredIconModel> mJsonStoredIconArray = new List<StoredIconModel>();

                // Temp models
                IconModel mModel;
                FolderModel fModel;

                CCLabel spriteLabel;

                // For active icons, save in respective lists
                foreach (IconReference iconRef in icons)
                {
                    if (iconRef.Sprite.Tag == SpriteTypes.IconTag)
                    {
                        mModel = new IconModel("", "", 0, 0, -1, iconRef.Sprite.ScaleX);

                        spriteLabel = iconRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        if (spriteLabel != null)
                        {
                            mModel.Text = spriteLabel.Text;
                        }

                        mModel.Base64 = iconRef.Base64;
                        mModel.X = (int) iconRef.Sprite.PositionX;
                        mModel.Y = (int) iconRef.Sprite.PositionY;
                        mModel.Tag = iconRef.Sprite.Tag;
                        mModel.TextScale = spriteLabel.ScaleX;
                        mModel.TextVisible = spriteLabel.Visible;

                        mJsonIconArray.Add(mModel);
                    }
                    else if (iconRef.Sprite.Tag == SpriteTypes.FolderTag)
                    {
                        spriteLabel = iconRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        fModel = new FolderModel("", "", "", 0, 0, -1, iconRef.Sprite.ScaleX, spriteLabel.Color);

                        if (spriteLabel != null)
                        {
                            fModel.Text = spriteLabel.Text;
                        }

                        if (iconRef.Base64.Contains("FolderOpen"))
                        {
                            fModel.AssetName = iconRef.Base64;
                            fModel.Base64 = null;
                        }
                        else
                        {
                            fModel.AssetName = null;
                            fModel.Base64 = iconRef.Base64;
                        }

                        fModel.X = (int) iconRef.Sprite.PositionX;
                        fModel.Y = (int) iconRef.Sprite.PositionY;
                        fModel.Tag = iconRef.Sprite.Tag;
                        fModel.TextScale = spriteLabel.ScaleX;
                        fModel.TextVisible = spriteLabel.Visible;

                        mJsonFolderArray.Add(fModel);
                    }
                }

                // Temp model
                StoredIconModel sModel;

                foreach (StoredIconReference mStoredRef in storedIcons)
                {
                    sModel = new StoredIconModel("", "", 0, 0, -1, mStoredRef.Base64, mStoredRef.Sprite.ScaleX);

                    spriteLabel = mStoredRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CocosSharp.CCLabel;

                    if (spriteLabel != null)
                    {
                        sModel.Text = spriteLabel.Text;
                    }

                    sModel.Base64 = mStoredRef.Base64;
                    sModel.Folder = mStoredRef.FolderName;
                    sModel.X = (int) mStoredRef.Sprite.PositionX;
                    sModel.Y = (int) mStoredRef.Sprite.PositionY;
                    sModel.Tag = mStoredRef.Sprite.Tag;
                    sModel.TextScale = spriteLabel.ScaleX;
                    sModel.TextVisible = spriteLabel.Visible;

                    mJsonStoredIconArray.Add(sModel);
                }

                // Json storage object
                var mIconStorage = new IconStorageObject();
                mIconStorage.Icons = mJsonIconArray;
                mIconStorage.Folders = mJsonFolderArray;
                mIconStorage.StoredIcons = mJsonStoredIconArray;
                mIconStorage.SingleMode = inSingleMode;
                mIconStorage.AutoUnselectSingleMode = autoDeselect;

                // Serialized object
                mJsonString = JsonConvert.SerializeObject(mIconStorage);

                // Cross-platform call to save a JSON-based text file
                DependencyService.Get<ISaveAndLoad>().SaveJSON("IconBoard", mJsonString);

                // <!-- Note: clear up lingering refs
                mIconStorage = null;
                mJsonString = null;

                mJsonIconArray.Clear();
                mJsonIconArray = null;

                mJsonFolderArray.Clear();
                mJsonFolderArray = null;

                mJsonStoredIconArray.Clear();
                mJsonStoredIconArray = null;
                // -->
            });

            return mJsonString;
        }

    }
}
