//----------------------------------------------------------------------------------------------
// <copyright file="GamePage.cs" 
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

using Xamarin.Forms;
using CocosSharp;
using Plugin.Media;
using System.IO;
using SGDWithCocos.Views;
using System.Threading.Tasks;
using SGDWithCocos.Interface;
using SGDWithCocos.Models;
using Newtonsoft.Json;
using SGDWithCocos.Tags;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SGDWithCocos.Scenes;
using SGDWithCocos.Shared.Layers;

namespace SGDWithCocos.Shared.Pages
{
    public class GamePage : ContentPage
    {
        public CocosSharpView gameView;
        public CCScene gameScene;
        GameLayer mLayer;
        int width, height;

        public CocosSharpView GameView { get; private set; }

        public GamePage()
        {
            width = DependencyService.Get<IDisplay>().Width;
            height = DependencyService.Get<IDisplay>().Height;

            gameView = new CocosSharpView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                DesignResolution = new Size(width, height),
                ResolutionPolicy = CocosSharpView.ViewResolutionPolicy.ExactFit,
                ViewCreated = LoadGame
            };

            Content = gameView;
        }

        protected override void OnDisappearing()
        {
            if (gameView != null)
            {
                gameView.Paused = true;
            }

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (gameView != null)
                gameView.Paused = false;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        public async void CallActionSheet(int counter)
        {
            string buttonSelect = await GetActionSheet();
            
            if (buttonSelect == StringTypes.HideImage)
            {
                // TODO
                //mLayer.ToggleSpriteImage(counter);
            }
            else if (buttonSelect == StringTypes.ChangeSizeUp)
            {
                mLayer.UpdateSpriteSize(counter, EditTypes.UpdateSizeUp);
            }
            else if (buttonSelect == StringTypes.ChangeSizeDefault)
            {
                mLayer.UpdateSpriteSize(counter, EditTypes.UpdateSizeDefault);
            }
            else if (buttonSelect == StringTypes.ChangeSizeDown)
            {
                mLayer.UpdateSpriteSize(counter, EditTypes.UpdateSizeDown);
            }
            else if (buttonSelect == StringTypes.ChangeText)
            {
                var mPopUpResult = await GetNamingWindow("Please name the Icon");
                mLayer.UpdateSpriteText(counter, mPopUpResult);
            }
            else if (buttonSelect == StringTypes.HideText)
            {
                mLayer.ToggleSpriteText(counter);
            }
            else if (buttonSelect == StringTypes.ChangeTextSizeUp)
            {
                mLayer.UpdateSpriteTextSize(counter, EditTypes.UpdateLabelSizeUp);
            }
            else if (buttonSelect == StringTypes.ChangeTextSizeDefault)
            {
                mLayer.UpdateSpriteTextSize(counter, EditTypes.UpdateLabelSizeDefault);
            }
            else if (buttonSelect == StringTypes.ChangeTextSizeDown)
            {
                mLayer.UpdateSpriteTextSize(counter, EditTypes.UpdateLabelSizeDown);
            }
        }

        public Task<string> GetActionSheet()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mAction = await DisplayActionSheet("Edit Current Icon? ", "Cancel", "OK",
                    // TODO
                    //StringTypes.HideImage,
                    StringTypes.ChangeSizeUp,
                    StringTypes.ChangeSizeDefault,
                    StringTypes.ChangeSizeDown,
                    StringTypes.ChangeText,
                    StringTypes.HideText,
                    StringTypes.ChangeTextSizeUp,
                    StringTypes.ChangeTextSizeDefault,
                    StringTypes.ChangeTextSizeDown);
                tcs.SetResult(mAction);
            });

            return tcs.Task;
        }

        public Task<string> GetFolderColor()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mAction = await DisplayActionSheet("Color for Folder? ",
                    "Cancel", "OK",
                    "Red", "Blue", "Green", "Pink", "Light Blue", "Purple");
                tcs.SetResult(mAction);
            });

            return tcs.Task;
        }

        public Task<string> GetNamingWindow(string mQuestion)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(() =>
            {
                var popup = new PopUpWindow(mQuestion, string.Empty, "OK", "Cancel");
                popup.PopupClosed += (o, closedArgs) =>
                {
                    if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                    {
                        tcs.SetResult(closedArgs.Text);
                    }
                    else
                    {
                        tcs.SetResult("");
                    }
                };

                popup.Show();
            });

            return tcs.Task;
        }

        public async void CallImagePicker()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Photos);

                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                    {
                    await DisplayAlert("Need photos", "Need access to photos to make icon", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Photos });
                    status = results[Permission.Photos];
                }

                if (status == PermissionStatus.Granted)
                {
                    var results = await GetImageSelection();
                    mLayer.CallBackIcon(results[0], results[1], results[2]);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void CallImageTaker()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera);

                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await DisplayAlert("Need camera", "Will need it take pictures", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                    var results = await GetImageCamera();
                    mLayer.CallBackIcon(results[0], results[1], results[2]);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public async void GetFolderSetup(List<string> mList)
        {
            var folderName = await GetNamingWindow("Please name the folder");

            if (mList.Contains(folderName.Trim().ToLower()))
            {
                return;
            }
            else if (folderName.Trim().Length > 0)
            {
                var folderColor = await GetFolderColor();

                var assetName = "";

                if (folderColor == "Red")
                {
                    assetName = "FolderOpenRed";
                }
                else if (folderColor == "Blue")
                {
                    assetName = "FolderOpenDarkBlue";
                }
                else if (folderColor == "Green")
                {
                    assetName = "FolderOpenGreen";
                }
                else if (folderColor == "Pink")
                {
                    assetName = "FolderOpenDarkPink";
                }
                else if (folderColor == "Light Blue")
                {
                    assetName = "FolderOpenLightBlue";
                }
                else if (folderColor == "Purple")
                {
                    assetName = "FolderOpenDarkPurple";
                }

                if (folderColor.Trim().Length > 0)
                {
                    mLayer.MakeIconFolder(assetName, folderName);
                }
            }
        }

        public Task<string[]> GetImageSelection()
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    Console.WriteLine("not supported");
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    tcs.SetResult(new string[] { "", "", "" });
                }

                var file = await CrossMedia.Current.PickPhotoAsync();

                if (file == null || file.Path == null) return;

                if (File.Exists(@file.Path))
                {
                    Console.WriteLine("file found!");

                    var popup = new PopUpWindow("Please name the Icon", string.Empty, "OK", "Cancel");
                    popup.PopupClosed += (o, closedArgs) =>
                    {
                        if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                        {
                            byte[] imageArray = File.ReadAllBytes(@file.Path);
                            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                            var extension = Path.GetExtension(@file.Path);

                            tcs.SetResult(new string[] { base64ImageRepresentation, closedArgs.Text, extension });
                        }
                        else
                        {
                            tcs.SetResult(new string[] { "", "", "" });
                        }
                    };

                    popup.Show();
                }
                else
                {
                    tcs.SetResult(new string[] { "", "", "" });
                }

            });

            return tcs.Task;
        }

        public Task<string[]> GetImageCamera()
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();

            Device.BeginInvokeOnMainThread(async () =>
            {

                if (!(CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported))
                {
                    Console.WriteLine("not supported");
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    tcs.SetResult(new string[] { "", "", "" });
                }

                var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "SGDPhotos",
                    Name = $"{DateTime.UtcNow}.jpg"
                };

                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                string newPath = "";

                if (file == null || file.Path == null) return;

                if (File.Exists(@file.Path))
                {
                    Console.WriteLine("file found!");

                    var path = Path.GetDirectoryName(@file.Path);
                    var fName = Path.GetFileNameWithoutExtension(@file.Path);

                    fName = fName + "crop.jpg";

                    newPath = Path.Combine(path, fName);

                    DependencyService.Get<IResizer>().ResizeBitmaps(@file.Path, @newPath);

                    var popup = new PopUpWindow("Please name the Icon", string.Empty, "OK", "Cancel");
                    popup.PopupClosed += (o, closedArgs) =>
                    {
                        if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                        {
                            byte[] imageArray = File.ReadAllBytes(@newPath);
                            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                            var extension = Path.GetExtension(@newPath);

                            tcs.SetResult(new string[] { base64ImageRepresentation, closedArgs.Text, extension });
                        }
                        else
                        {
                            tcs.SetResult(new string[] { "", "", "" });
                        }
                    };

                    popup.Show();
                }
                else
                {
                    tcs.SetResult(new string[] { "", "", "" });
                }

            });

            return tcs.Task;
        }

        public string GetBoards(string boardName)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadJSON(boardName);
        }

        public void SaveBoards(List<IconReference> icons, List<StoredIconReference> storedIcons, bool inSingleMode)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                List<IconModel> mJsonIconArray = new List<IconModel>();
                List<FolderModel> mJsonFolderArray = new List<FolderModel>();
                List<StoredIconModel> mJsonStoredIconArray = new List<StoredIconModel>();

                foreach (IconReference iconRef in icons)
                {
                    if (iconRef.Sprite.Tag == SpriteTypes.IconTag)
                    {
                        var mModel = new IconModel("", "", 0, 0, -1, iconRef.Sprite.ScaleX);

                        var spriteLabel = iconRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        if (spriteLabel != null)
                        {
                            mModel.Text = spriteLabel.Text;
                        }

                        mModel.Base64 = iconRef.Base64;
                        mModel.X = (int)iconRef.Sprite.PositionX;
                        mModel.Y = (int)iconRef.Sprite.PositionY;
                        mModel.Tag = iconRef.Sprite.Tag;
                        mModel.TextScale = spriteLabel.ScaleX;
                        mModel.TextVisible = spriteLabel.Visible;

                        mJsonIconArray.Add(mModel);
                    }
                    else if (iconRef.Sprite.Tag == SpriteTypes.FolderTag)
                    {
                        var mModel = new FolderModel("", "", 0, 0, -1, iconRef.Sprite.ScaleX);

                        var spriteLabel = iconRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        if (spriteLabel != null)
                        {
                            mModel.Text = spriteLabel.Text;
                        }

                        mModel.AssetName = iconRef.Base64;
                        mModel.X = (int)iconRef.Sprite.PositionX;
                        mModel.Y = (int)iconRef.Sprite.PositionY;
                        mModel.Tag = iconRef.Sprite.Tag;
                        mModel.TextScale = spriteLabel.ScaleX;
                        mModel.TextVisible = spriteLabel.Visible;

                        mJsonFolderArray.Add(mModel);
                    }
                }

                foreach (StoredIconReference mStoredRef in storedIcons)
                {
                    var mModel = new StoredIconModel("", "", 0, 0, -1, mStoredRef.Base64, mStoredRef.Sprite.ScaleX);

                    var spriteLabel = mStoredRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (spriteLabel != null)
                    {
                        mModel.Text = spriteLabel.Text;
                    }

                    mModel.Base64 = mStoredRef.Base64;
                    mModel.Folder = mStoredRef.FolderName;
                    mModel.X = (int) mStoredRef.Sprite.PositionX;
                    mModel.Y = (int) mStoredRef.Sprite.PositionY;
                    mModel.Tag = mStoredRef.Sprite.Tag;
                    mModel.TextScale = spriteLabel.ScaleX;
                    mModel.TextVisible = spriteLabel.Visible;

                    mJsonStoredIconArray.Add(mModel);
                }

                var mIconStorage = new IconStorageObject();
                mIconStorage.Icons = mJsonIconArray;
                mIconStorage.Folders = mJsonFolderArray;
                mIconStorage.StoredIcons = mJsonStoredIconArray;
                mIconStorage.SingleMode = inSingleMode;

                var mJsonString = JsonConvert.SerializeObject(mIconStorage);

                Console.WriteLine("saved!");

                DependencyService.Get<ISaveAndLoad>().SaveJSON("IconBoard", mJsonString);
            });
        }

        void LoadGame(object sender, EventArgs e)
        {
            var nativeGameView = sender as CCGameView;

            if (nativeGameView != null)
            {
                var contentSearchPaths = new List<string>() { "Fonts" };

                int width = nativeGameView.DesignResolution.Width;
                int height = nativeGameView.DesignResolution.Height;

                CCSizeI viewSize = nativeGameView.ViewSize;
                CCSizeI designResolution = nativeGameView.DesignResolution;

                // Determine whether to use the high or low def versions of our images
                // Make sure the default texel to content size ratio is set correctly
                // Of course you're free to have a finer set of image resolutions e.g (ld, hd, super-hd)
                /*
                if (designResolution.Width < viewSize.Width)
                {
                    contentSearchPaths.Add("Images/Hd");
                    CCSprite.DefaultTexelToContentSizeRatio = 2.0f;
                }
                else
                {
                    contentSearchPaths.Add("Images/Ld");
                    CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
                }
                */

                nativeGameView.ContentManager.SearchPaths = contentSearchPaths;

                nativeGameView.RunWithScene(new GameStartScene(nativeGameView, width, height, this));

                ConstructGameScene(nativeGameView);
            }
        }

        public void ConstructGameScene(CCGameView nativeGameView)
        {
            gameScene = new CCScene(nativeGameView);

            string json = GetBoards("IconBoard");

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

            mLayer = new GameLayer(width, height, jsonObject, this);

            gameScene.AddLayer(mLayer);
        }
    }
}
