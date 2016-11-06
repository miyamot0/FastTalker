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
    /// <summary>
    /// X-platform page for Xamarin Forms 
    /// </summary>
    public class GamePage : ContentPage
    {
        public CocosSharpView gameView;
        public CCScene gameScene;
        GameLayer mLayer;
        int width, height;

        /// <summary>
        /// NativeGame object
        /// </summary>
        public CocosSharpView GameView { get; private set; }

        /// <summary>
        /// Constructor, with DependencyService calls to native display metrics
        /// </summary>
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

        /// <summary>
        /// Base methods
        /// </summary>
        protected override void OnDisappearing()
        {
            if (gameView != null)
            {
                gameView.Paused = true;
            }

            base.OnDisappearing();
        }

        /// <summary>
        /// Base methods
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (gameView != null)
            {
                gameView.Paused = false;
            }
        }

        /// <summary>
        /// Base methods
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

        /// <summary>
        /// Call action sheet with possible modifications to icon
        /// </summary>
        /// <param name="counter">Icon reference</param>
        public async void CallActionSheet(int counter)
        {
            string buttonSelect = await GetActionSheet();
            
            /*
            
            TODO toggle hiding of image

            if (buttonSelect == StringTypes.HideImage)
            {
                //mLayer.ToggleSpriteImage(counter);
            }

            */

            if (buttonSelect == StringTypes.ChangeSizeUp)
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

        /// <summary>
        /// Async-able task related to a scrollable, action sheet
        /// </summary>
        /// <returns>Task w/ result</returns>
        public Task<string> GetActionSheet()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mAction = await DisplayActionSheet("Edit Current Icon? ", "Cancel", "OK",
                    /*                     
                    TODO Hide image 
                    StringTypes.HideImage                     
                    */

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

        /// <summary>
        /// Async-able task related to the color for a folder
        /// </summary>
        /// <returns></returns>
        public Task<string> GetFolderColor()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mAction = await DisplayActionSheet("Color for Folder? ",
                    "Cancel", "OK",
                    "Red", 
                    "Blue", 
                    "Green", 
                    "Pink", 
                    "Light Blue", 
                    "Purple");
                tcs.SetResult(mAction);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Async-able task related to naming an icon
        /// </summary>
        /// <param name="mQuestion">Query text for user</param>
        /// <returns></returns>
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

        /// <summary>
        /// Cross-platform call to gallery for Android and iOS
        /// </summary>
        public async void CallImagePicker()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);

                if (status != PermissionStatus.Granted)
                {
                    // Request permissions
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                    {
                        await DisplayAlert("Need photos", "Need access to photos to make icon", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Photos });
                    status = results[Permission.Photos];
                }

                if (status == PermissionStatus.Granted)
                {
                    // If permissions granted, query selection and call back

                    var results = await GetImageSelection();
                    mLayer.CallBackIcon(results[0], results[1], results[2]);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    // If permissions not granted, report back to user

                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Async-able task related to selecting existing photos
        /// </summary>
        /// <returns>String array of [base64, image label, photo extension]</returns>
        public Task<string[]> GetImageSelection()
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    // If photo picking isn't supported, return with blank array

                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    tcs.SetResult(new string[] { "", "", "" });
                }

                try
                {
                    var file = await CrossMedia.Current.PickPhotoAsync();

                    if (file == null || file.Path == null) return;

                    if (File.Exists(@file.Path))
                    {
                        // If the photo can be found, query user for icon label

                        var popup = new PopUpWindow("Please name the Icon", string.Empty, "OK", "Cancel");
                        popup.PopupClosed += (o, closedArgs) =>
                        {
                            if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                            {
                                // If user provides valid name, convert image file to base64 and return

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
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Cross-platform call to camera for Android and iOS
        /// </summary>
        public async void CallImageTaker()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

                if (status != PermissionStatus.Granted)
                {
                    // Request permissions

                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await DisplayAlert("Need camera", "Will need it take pictures", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                    // If permissions granted, query selection and call back

                    var results = await GetImageCamera();
                    mLayer.CallBackIcon(results[0], results[1], results[2]);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    // If permissions not granted, report back to user

                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Async-able task related to taking photos
        /// </summary>
        /// <returns></returns>
        public Task<string[]> GetImageCamera()
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!(CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported))
                {
                    // If photo taking isn't supported, return with blank array

                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    tcs.SetResult(new string[] { "", "", "" });
                }

                // Options related to image storage
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
                    // To accomodate varying camera sizes, crop center-square for uniform display

                    var path = Path.GetDirectoryName(@file.Path);
                    var fName = Path.GetFileNameWithoutExtension(@file.Path);
                    fName = fName + "crop.jpg";
                    newPath = Path.Combine(path, fName);
                    DependencyService.Get<IResizer>().ResizeBitmaps(@file.Path, @newPath);

                    // If the photo can be found, query user for icon label

                    var popup = new PopUpWindow("Please name the Icon", string.Empty, "OK", "Cancel");
                    popup.PopupClosed += (o, closedArgs) =>
                    {
                        if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                        {
                            // If user provides valid name, convert image file to base64 and return

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

        /// <summary>
        /// Construct and name/specify Folder sprite
        /// </summary>
        /// <param name="mList">List of existing folder names</param>
        public async void GetFolderSetup(List<string> mList)
        {
            var folderName = await GetNamingWindow("Please name the folder");

            if (mList.Contains(folderName.Trim().ToLower()))
            {
                // return if the existing name already exists

                return;
            }
            else if (folderName.Trim().Length > 0)
            {
                // Query user related to folder color

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
                    // if a color was supplied, create the necessary folder in the field

                    mLayer.MakeIconFolder(assetName, folderName);
                }
            }
        }
        
        /// <summary>
        /// Cross-platform call to get JSON-saved boards from local storage
        /// </summary>
        /// <param name="boardName">name for JSON file</param>
        /// <returns>JSON string</returns>
        public string GetBoards(string boardName)
        {
            return DependencyService.Get<ISaveAndLoad>().LoadJSON(boardName);
        }

        /// <summary>
        /// Cross-platform calls to save all icons/folders into local storage
        /// </summary>
        /// <param name="icons">All icons in field, folder or typical</param>
        /// <param name="storedIcons">All saved icons</param>
        /// <param name="inSingleMode">If in single or framed mode</param>
        public void SaveBoards(List<IconReference> icons, List<StoredIconReference> storedIcons, bool inSingleMode)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // List of icons
                List<IconModel> mJsonIconArray = new List<IconModel>();

                // List of folders
                List<FolderModel> mJsonFolderArray = new List<FolderModel>();

                // List of icons stored in folders
                List<StoredIconModel> mJsonStoredIconArray = new List<StoredIconModel>();

                // For active icons, save in respective lists
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

                // Json storage object
                var mIconStorage = new IconStorageObject();
                mIconStorage.Icons = mJsonIconArray;
                mIconStorage.Folders = mJsonFolderArray;
                mIconStorage.StoredIcons = mJsonStoredIconArray;
                mIconStorage.SingleMode = inSingleMode;

                // Serialized object
                var mJsonString = JsonConvert.SerializeObject(mIconStorage);

                // Cross-platform call to save a JSON-based text file
                DependencyService.Get<ISaveAndLoad>().SaveJSON("IconBoard", mJsonString);
            });
        }

        /// <summary>
        /// Loading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadGame(object sender, EventArgs e)
        {
            var nativeGameView = sender as CCGameView;

            if (nativeGameView != null)
            {
                var contentSearchPaths = new List<string>() { "Fonts" };

                int width = nativeGameView.DesignResolution.Width;
                int height = nativeGameView.DesignResolution.Height;

                // Show the start screen
                nativeGameView.ContentManager.SearchPaths = contentSearchPaths;
                nativeGameView.RunWithScene(new GameStartScene(nativeGameView, width, height, this));

                // Begin building the icon-based scene
                ConstructGameScene(nativeGameView);
            }
        }

        /// <summary>
        /// Construct the icon communication board scene
        /// </summary>
        /// <param name="nativeGameView"></param>
        public void ConstructGameScene(CCGameView nativeGameView)
        {
            gameScene = new CCScene(nativeGameView);

            string json = GetBoards("IconBoard");

            IconStorageObject jsonObject = null;

            if (json != "")
            {
                try
                {
                    // Attempt to deserialize icons

                    jsonObject = JsonConvert.DeserializeObject<IconStorageObject>(json);
                }
                catch
                {
                    jsonObject = null;
                }
            }

            // Create layer for icon board scene
            mLayer = new GameLayer(width, height, jsonObject, this);

            // Add layer to icon board scene
            gameScene.AddLayer(mLayer);
        }
    }
}
