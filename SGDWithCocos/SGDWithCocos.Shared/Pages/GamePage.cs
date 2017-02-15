﻿//----------------------------------------------------------------------------------------------
// <copyright file="GamePage.cs" 
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
using System.Linq;
using SGDWithCocos.Utilities;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System.Text;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using SGDWithCocos.Server;

namespace SGDWithCocos.Shared.Pages
{
    /// <summary>
    /// X-platform page for Xamarin Forms 
    /// </summary>
    public class GamePage : ContentPage
    {
        public bool IsAdmin = false;
        public CocosSharpView gameView;
        public CCScene gameScene;
        GameLayer mLayer;
        int width, height;
        SimpleIconServer mServer;
        private bool isScreenLocked = true;

        /// <summary>
        /// NativeGame object
        /// </summary>
        public CocosSharpView GameView { get; private set; }

        /// <summary>
        /// Constructor, with DependencyService calls to native display metrics
        /// </summary>
        public GamePage()
        {
            int mWidth = DependencyService.Get<IDisplay>().Width;
            int mHeight = DependencyService.Get<IDisplay>().Height;

            if (mWidth > mHeight)
            {
                width = DependencyService.Get<IDisplay>().Width;
                height = DependencyService.Get<IDisplay>().Height;
            }
            else
            {
                height = DependencyService.Get<IDisplay>().Width;
                width = DependencyService.Get<IDisplay>().Height;
            }

            gameView = new CocosSharpView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                DesignResolution = new Size(width, height),
                ResolutionPolicy = CocosSharpView.ViewResolutionPolicy.ExactFit,
                ViewCreated = LoadGame,
            };

            Content = gameView;
        }

        #region Life Cycle

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

        #endregion

        #region Image Modification decision-making

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
            else if (buttonSelect == StringTypes.ChangeSizeUp2)
            {
                mLayer.UpdateSpriteSize(counter, EditTypes.UpdateSizeUp2);
            }
            else if (buttonSelect == StringTypes.ChangeSizeDefault)
            {
                mLayer.UpdateSpriteSize(counter, EditTypes.UpdateSizeDefault);
            }
            else if (buttonSelect == StringTypes.ChangeSizeDown)
            {
                mLayer.UpdateSpriteSize(counter, EditTypes.UpdateSizeDown);
            }
            else if (buttonSelect == StringTypes.ChangeSizeDown2)
            {
                mLayer.UpdateSpriteSize(counter, EditTypes.UpdateSizeDown2);
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

        #endregion

        #region Open Board Format

        /// <summary>
        /// Parse OBF file individually
        /// </summary>
        /// <param name="jsonContent"></param>
        private void ParseOBF(string fileText)
        {
            OpenBoardModel jsonContent = JsonConvert.DeserializeObject<OpenBoardModel>(fileText);

            try
            {
                mLayer.MaskBackground();

                #region Parse Images out

                List<OpenBoardModelImageReference> mImages = new List<OpenBoardModelImageReference>();

                foreach (var image in jsonContent.images)
                {
                    if (image.data != null)
                    {
                        var imgRef = new OpenBoardModelImageReference();
                        imgRef.id = image.id;
                        imgRef.url = null;
                        imgRef.base64 = image.data.Split(',')[1];

                        mImages.Add(imgRef);
                    }
                    else if (image.url != null)
                    {
                        var imgRef = new OpenBoardModelImageReference();
                        imgRef.url = image.url;
                        imgRef.id = image.id;

                        using (WebClient webClient = new WebClient())
                        {
                            byte[] data = webClient.DownloadData(image.url);
                            imgRef.base64 = Convert.ToBase64String(data);
                        }

                        mImages.Add(imgRef);
                    }
                }

                #endregion

                #region Loop through buttons

                foreach (var button in jsonContent.buttons)
                {
                    var buttonLabel = button.label;
                    var base64string = "";
                    var buttonImageType = "png";

                    // Exclude hidden buttons
                    if (button.hidden != null && button.hidden == true)
                    {
                        continue;
                    }

                    // Exclude clearing buttons, as we have them fixed natively
                    if (button.action != null && button.action == ":clear")
                    {
                        continue;
                    }

                    // Select corresponding image from cached list
                    var matchingImg = mImages.Where(i => i.id == button.image_id).FirstOrDefault();

                    if (matchingImg != null)
                    {
                        base64string = matchingImg.base64;
                    }

                    if (button.load_board == null)
                    {
                        mLayer.CallBackIcon(base64string, buttonLabel, buttonImageType, button.image_id);
                    }
                    else
                    {
                        mLayer.MakeIconFolder(null, buttonLabel, base64string, button.image_id);
                    }
                }

                #endregion

                #region Loop through existing images, provide grid-like arrangement initially

                double rows = Convert.ToDouble(jsonContent.grid.rows),
                       cols = Convert.ToDouble(jsonContent.grid.columns);

                for (int i = 0; i < rows; i++)
                {
                    var row = jsonContent.grid.order.ElementAt(i);

                    for (int j = 0; j < cols; j++)
                    {
                        string currItem = row[j];

                        if (currItem == null) continue;

                        double xProp = (((double)j / (cols + 1.0)) * width) + (width * 0.1);
                        double yProp = height - (((double)i / (rows + 1.0)) * height) - (height * 0.1);

                        var mButton = jsonContent.buttons.Where(b => b.id == currItem).FirstOrDefault();
                        mLayer.UpdatePositionNamedSprite(mButton.image_id, (int)xProp, (int)yProp);
                    }
                }

                #endregion
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                mLayer.RemoveAllChildrenByTag(SpriteTypes.ColorLayerTag, true);
            }
        }

        /// <summary>
        /// Parse OBZ files (stubbed)
        /// </summary>
        /// <param name="fileText"></param>
        private void ParseOBZ(byte[] fileBytes)
        {
            #region Stubbed out parsing

            /*

            try
            {
                Stream stream = new MemoryStream(fileBytes);
                ZipFile zf = null;

                try
                {
                    zf = new ZipFile(stream);

                    #region Open Manifest

                    string rootFolder = ParseOBZManifest(zf);

                    #endregion

                    #region Open Root Folder

                    OpenBoardModel jsonContentModel = ParseOBZRoot(zf, rootFolder);

                    List<OpenBoardModelImageReference> mImages = new List<OpenBoardModelImageReference>();

                    foreach (var image in jsonContentModel.images)
                    {
                        if (image.data != null)
                        {
                            var imgRef = new OpenBoardModelImageReference();
                            imgRef.id = image.id;
                            imgRef.url = null;
                            imgRef.base64 = image.data.Split(',')[1];

                            mImages.Add(imgRef);
                        }
                        else if (image.url != null)
                        {
                            var imgRef = new OpenBoardModelImageReference();
                            imgRef.url = image.url;
                            imgRef.id = image.id;

                            using (WebClient webClient = new WebClient())
                            {
                                byte[] data = webClient.DownloadData(image.url);
                                imgRef.base64 = Convert.ToBase64String(data);
                            }

                            mImages.Add(imgRef);
                        }
                    }

                    foreach (var button in jsonContentModel.buttons)
                    {
                        var buttonLabel = button.label;
                        var base64string = "";
                        var buttonImageType = "png";

                        // Exclude hidden buttons
                        if (button.hidden != null && button.hidden == true)
                        {
                            continue;
                        }

                        // Exclude clearing buttons, as we have them
                        if (button.action != null && button.action == ":clear")
                        {
                            continue;
                        }

                        var matchingImg = mImages.Where(i => i.id == button.image_id).FirstOrDefault();

                        if (matchingImg != null)
                        {
                            base64string = matchingImg.base64;
                        }

                        if (button.load_board == null)
                        {
                            mLayer.CallBackIcon(base64string, buttonLabel, buttonImageType, button.image_id);
                        }
                        else
                        {
                            // Add folder to board
                            mLayer.MakeIconFolder(null, buttonLabel, base64string, button.image_id);

                            // Add children to folder
                            ParseOBZChild(zf, button.load_board.path, buttonLabel);

                        }
                    }

                    #region Arrange for grid-like appearance, root page only

                    double rows = Convert.ToDouble(jsonContentModel.grid.rows),
                           cols = Convert.ToDouble(jsonContentModel.grid.columns);

                    for (int i = 0; i < rows; i++)
                    {
                        var row = jsonContentModel.grid.order.ElementAt(i);

                        for (int j = 0; j < cols; j++)
                        {
                            string currItem = row[j];

                            if (currItem == null) continue;

                            double xProp = (((double)j / (cols + 1.0)) * width) + (width * 0.1);
                            double yProp = height - (((double)i / (rows + 1.0)) * height) - (height * 0.1);

                            var mButton = jsonContentModel.buttons.Where(b => b.id == currItem).FirstOrDefault();
                            mLayer.UpdatePositionNamedSprite(mButton.image_id, (int)xProp, (int)yProp);
                        }
                    }

                    #endregion

                    #endregion

                }
                finally
                {
                    if (zf != null)
                    {
                        zf.IsStreamOwner = true;
                        zf.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            */

            #endregion
        }

        /// <summary>
        /// Parse the manifest from the zipped OBZ
        /// </summary>
        /// <param name="zf">Zipped file</param>
        /// <returns></returns>
        private string ParseOBZManifest(ZipFile zf)
        {
            ZipEntry zEntry = zf.GetEntry("manifest.json");

            byte[] buffer = new byte[4096];
            Stream zipStream = zf.GetInputStream(zEntry);
            OpenBoardManifestModel jsonContent = null;

            using (MemoryStream ms = new MemoryStream())
            {
                StreamUtils.Copy(zipStream, ms, buffer);
                ms.Position = 0;
                string myStr = Encoding.UTF8.GetString(ms.ToArray());
                jsonContent = JsonConvert.DeserializeObject<OpenBoardManifestModel>(myStr);
            }

            return jsonContent.root;
        }

        /// <summary>
        /// Parse only the specific root folder, for the organizational structure
        /// </summary>
        /// <param name="zf">Zipped File</param>
        /// <param name="rootFolder">Name of root board</param>
        /// <returns></returns>
        private OpenBoardModel ParseOBZRoot(ZipFile zf, string rootFolder)
        {
            ZipEntry zEntry = zf.GetEntry(rootFolder);

            byte[] buffer = new byte[4096];
            Stream zipStream = zf.GetInputStream(zEntry);

            OpenBoardModel jsonContentModel = null;

            using (MemoryStream ms = new MemoryStream())
            {
                StreamUtils.Copy(zipStream, ms, buffer);
                ms.Position = 0;
                string myStr = Encoding.UTF8.GetString(ms.ToArray());
                jsonContentModel = JsonConvert.DeserializeObject<OpenBoardModel>(myStr);
            }

            return jsonContentModel;
        }

        /// <summary>
        /// Parse a child folder, enabling foldered structure
        /// </summary>
        /// <param name="zf">Zipped File</param>
        /// <param name="boardName">Name of board to parse</param>
        /// <param name="folderName">Name of folder to place children in</param>
        private void ParseOBZChild(ZipFile zf, string boardName, string folderName)
        {
            ZipEntry zEntry = zf.GetEntry(boardName);

            byte[] buffer = new byte[4096];
            Stream zipStream = zf.GetInputStream(zEntry);

            OpenBoardModel jsonContentModel = null;

            using (MemoryStream ms = new MemoryStream())
            {
                StreamUtils.Copy(zipStream, ms, buffer);
                ms.Position = 0;
                string myStr = Encoding.UTF8.GetString(ms.ToArray());
                jsonContentModel = JsonConvert.DeserializeObject<OpenBoardModel>(myStr);
            }

            List<OpenBoardModelImageReference> mImages = new List<OpenBoardModelImageReference>();

            #region Cache Images

            foreach (var image in jsonContentModel.images)
            {
                if (image.data != null)
                {
                    var imgRef = new OpenBoardModelImageReference();
                    imgRef.id = image.id;
                    imgRef.url = null;
                    imgRef.base64 = image.data.Split(',')[1];

                    mImages.Add(imgRef);
                }
                else if (image.url != null)
                {
                    var imgRef = new OpenBoardModelImageReference();
                    imgRef.url = image.url;
                    imgRef.id = image.id;

                    using (WebClient webClient = new WebClient())
                    {
                        byte[] data = webClient.DownloadData(image.url);
                        imgRef.base64 = Convert.ToBase64String(data);
                    }

                    mImages.Add(imgRef);
                }
            }

            #endregion

            foreach (var button in jsonContentModel.buttons)
            {
                var buttonLabel = button.label;
                var base64string = "";
                //var buttonImageType = "png";

                // Exclude hidden buttons
                if (button.hidden != null && button.hidden == true)
                {
                    continue;
                }

                // Exclude clearing buttons, as we have them
                if (button.action != null && button.action == ":clear")
                {
                    continue;
                }

                var matchingImg = mImages.Where(i => i.id == button.image_id).FirstOrDefault();

                if (matchingImg != null)
                {
                    base64string = matchingImg.base64;
                }

                if (button.load_board == null)
                {
                    mLayer.CallBackIconStored(base64string, buttonLabel, folderName);
                    //mLayer.CallBackIcon(base64string, buttonLabel, buttonImageType, button.image_id);
                    //mLayer.CallBackIcon(base64string, buttonLabel, buttonImageType, button.image_id);
                }
                else
                {
                    // TODO note all 2nd order children omitted!

                    //mLayer.MakeIconFolder(null, buttonLabel, base64string, button.image_id);


                    /*

                    #region Load into Folder

                    string boardTitle = button.load_board.name;

                    ZipEntry zEntryPage = zf.GetEntry(boardTitle);

                    byte[] buffer2 = new byte[4096];     // 4K is optimum
                    Stream zipStream2 = zf.GetInputStream(zEntryPage);

                    OpenBoardModel jsonContentModel2 = null;

                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        StreamUtils.Copy(zipStream2, ms2, buffer2);
                        ms2.Position = 0;

                        var myStr2 = Encoding.UTF8.GetString(ms2.ToArray());

                        jsonContentModel2 = JsonConvert.DeserializeObject<OpenBoardModel>(myStr2);
                    }

                    List<OpenBoardModelImageReference> mImages2 = new List<OpenBoardModelImageReference>();

                    foreach (var image2 in jsonContentModel2.images)
                    {
                        if (image2.data != null)
                        {
                            var imgRef = new OpenBoardModelImageReference();
                            imgRef.id = image2.id;
                            imgRef.url = null;
                            imgRef.base64 = image2.data.Split(',')[1];

                            mImages2.Add(imgRef);
                        }
                        else if (image2.url != null)
                        {
                            var imgRef = new OpenBoardModelImageReference();
                            imgRef.url = image2.url;
                            imgRef.id = image2.id;

                            using (WebClient webClient = new WebClient())
                            {
                                byte[] data = webClient.DownloadData(image2.url);
                                imgRef.base64 = Convert.ToBase64String(data);
                            }

                            mImages2.Add(imgRef);
                        }
                    }

                    foreach (var button2 in jsonContentModel.buttons)
                    {
                        var buttonLabel2 = button.label;
                        var base64string2 = "";

                        // Exclude hidden buttons
                        if (button2.hidden != null && button2.hidden == true)
                        {
                            continue;
                        }

                        // Exclude clearing buttons, as we have them
                        if (button2.action != null && button2.action == ":clear")
                        {
                            continue;
                        }

                        var matchingImg2 = mImages2.Where(i => i.id == button2.image_id).FirstOrDefault();

                        if (matchingImg2 != null)
                        {
                            base64string2 = matchingImg2.base64;
                        }

                        #region Add to folder here

                        if (buttonLabel2 != "")
                        {
                            mLayer.CallBackIconStored(base64string2, buttonLabel2, buttonLabel);
                        }

                        #endregion

                    }

                    #endregion
                    */
                }
            }
        }

        #endregion
        
        #region Settings

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
                    StringTypes.ChangeSizeUp2,
                    StringTypes.ChangeSizeDown,
                    StringTypes.ChangeSizeDown2,
                    StringTypes.ChangeSizeDefault,

                    StringTypes.ChangeText,
                    StringTypes.HideText,
                    StringTypes.ChangeTextSizeUp,
                    StringTypes.ChangeTextSizeDown,
                    StringTypes.ChangeTextSizeDefault);
                tcs.SetResult(mAction);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Branching logic for pictures
        /// </summary>
        public async void CallActionSheetChoice(List<IconReference> mIcons)
        {
            string buttonSelect = await GetActionTypeActionSheet();

            if (buttonSelect == null)
            {
                return;
            }

            #region Change SGD Presentation Type
            
            if (buttonSelect == StringTypes.ChangeSettings)
            {
                string modeSelect = await GetSGDMode();

                if (modeSelect == StringTypes.SingleMode)
                {
                    mLayer.SetSingleMode(true);
                }
                else if (modeSelect == StringTypes.FrameMode)
                {
                    mLayer.SetSingleMode(false);
                }
            }

            #endregion

            #region Add Icons

            else if (buttonSelect == StringTypes.AddIcon)
            {
                string actionSelect = await GetActionSheetChoice();

                if (actionSelect == StringTypes.LocalImage)
                {
                    CallCategoryPicker();
                }
                else if (actionSelect == StringTypes.DownloadedImage)
                {
                    CallImagePicker();
                }
            }

            #endregion

            #region Take Photo

            else if (buttonSelect == StringTypes.TakePhoto)
            {
                CallImageTaker();
            }

            #endregion

            #region Add Folder

            else if (buttonSelect == StringTypes.AddFolder)
            {
                // Get Active, foldered icons
                var mList = mIcons.Where(t => t.Sprite.Tag == SpriteTypes.FolderTag).ToList();

                var nameList = new List<string>();

                // For icons with a Folder value, add to a list, to avoid dupes
                mList.ForEach(p =>
                {
                    var returnedString = SpriteTools.SpriteHasLabel(p.Sprite);

                    if (returnedString != "")
                    {
                        nameList.Add(returnedString.ToLower());
                    }
                });

                GetFolderSetup(nameList);
            }

            #endregion

            #region Import Board (STUBBED)

            else if (buttonSelect == StringTypes.ImportBoard)
            {
                try
                {
                    FileData file = await CrossFilePicker.Current.PickFile();

                    try
                    {
                        var extension = Path.GetExtension(file.FileName);

                        if (extension == ".obf")
                        {
                            MemoryStream stream = new MemoryStream(file.DataArray);
                            string decoded = Encoding.UTF8.GetString(stream.ToArray());

                            ParseOBF(decoded);
                        }
                        else if (extension == ".obz")
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await DisplayAlert("Not Supported", "OBZ files aren't compatible with FastTalker's layout", "Close");
                            });
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await DisplayAlert("Not Supported", "This file is not Open Board Format (*.obf)", "Close");
                            });
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else if (buttonSelect == StringTypes.ForceSave)
            {
                mLayer.SaveJsonContent();
            }

            #endregion

            #region Resume Operation

            else if (buttonSelect == StringTypes.ResumeOperation)
            {
                mLayer.SetEditMode(false);
            }

            else if (buttonSelect.Contains("activate Lock"))
            {
                isScreenLocked = !isScreenLocked;

                if (Device.OS == TargetPlatform.Android)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<IAdmin>().RequestAdmin(isScreenLocked);
                    });
                }
            }

            #endregion

            #region Server Operation

            else if (buttonSelect == StringTypes.ServerStart)
            {
                StartServer();
            }

            else if (buttonSelect == StringTypes.ServerShutdown)
            {
                StopServer();
            }

            #endregion
        }

        /// <summary>
        /// Open action sheet related to tweaks and additions, single button interface
        /// </summary>
        /// <returns></returns>
        public Task<string> GetActionTypeActionSheet()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            string[] mOptions = null;

            if (IsAdmin)
            {
                mOptions = new string[] {
                    StringTypes.ResumeOperation,

                    // Stub out server code
                    //!mLayer.ServerActive ? StringTypes.ServerStart : StringTypes.ServerShutdown,

                    StringTypes.ForceSave,

                    // Stub out board import
                    //StringTypes.ImportBoard,

                    (isScreenLocked) ? "Deactivate Lock" : "Reactivate Lock",
                    StringTypes.ChangeSettings,
                    StringTypes.AddIcon,
                    StringTypes.TakePhoto,
                    StringTypes.AddFolder };
            }
            else
            {
                mOptions = new string[] {
                    StringTypes.ResumeOperation,

                    // Stub out server code
                    //!mLayer.ServerActive ? StringTypes.ServerStart : StringTypes.ServerShutdown,

                    StringTypes.ForceSave,

                    // Stub out board import
                    //StringTypes.ImportBoard,

                    StringTypes.ChangeSettings,
                    StringTypes.AddIcon,
                    StringTypes.TakePhoto,
                    StringTypes.AddFolder };
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mAction = await DisplayActionSheet("Change settings or icons?", "Cancel", "OK", mOptions);
                tcs.SetResult(mAction);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Call an action sheet related to image location
        /// </summary>
        /// <returns></returns>
        public Task<string> GetActionSheetChoice()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mAction = await DisplayActionSheet("Select Image Source ", "Cancel", "OK",
                    StringTypes.LocalImage,
                    StringTypes.DownloadedImage);
                tcs.SetResult(mAction);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Open action sheet related to tweaks and additions, single button interface
        /// </summary>
        /// <returns></returns>
        public Task<string> GetSGDMode()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mAction = await DisplayActionSheet("Select a response type ", "Cancel", "OK",
                    StringTypes.SingleMode,
                    StringTypes.FrameMode);
                tcs.SetResult(mAction);
            });

            return tcs.Task;
        }

        #endregion

        #region Selection of Embedded Icons

        /// <summary>
        /// Action list for icon selection
        /// </summary>
        /// <returns></returns>
        public Task<string> SelectCategory()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var mChunk = await DisplayActionSheet("Lookup icon category by name range: ",
                    "Cancel", null,
                    App.CategoryChunks);

                if (mChunk != null || mChunk != "Cancel")
                {
                    char mFirst = mChunk[0];
                    char mLast = mChunk[mChunk.Length - 1];

                    var mItems = mLayer.Categories.Where((s) => s.ToUpper()[0] >= mFirst && s.ToUpper()[0] <= mLast);

                    var mAction = await DisplayActionSheet("What type of icon? ",
                        "Cancel", null,
                        mItems.ToArray());

                    tcs.SetResult(mAction);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Select Category for icons
        /// </summary>
        public async void CallCategoryPicker()
        {
            try
            {
                var result = await SelectCategory();

                if (result == null || result == "Cancel")
                {
                    return;
                }

                var matches = await GetMatchingTasks(result);
                mLayer.ShowStoredWindow(matches);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Linq-based query for icon category action-sheet 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Task<List<Storage>> GetMatchingTasks(string result)
        {
            TaskCompletionSource<List<Storage>> tcs = new TaskCompletionSource<List<Storage>>();

            List<string> listTag = new List<string>() { result };

            Device.BeginInvokeOnMainThread(() =>
            {
                StorageContainer copiedList = mLayer.StoredIcons;

                List<Storage> mMatchingIcons = copiedList
                .StoredIcons
                .Where(l => l.Tags.Intersect(listTag).Any())
                .ToList();

                tcs.SetResult(mMatchingIcons);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Async naming call with callback to layer
        /// </summary>
        /// <param name="node"></param>
        public async void NameEmbeddedIcon(CCNode node)
        {
            var results = await NameWindow(node);
            mLayer.CallBackIcon(results[0], results[1], results[2], null);
        }

        /// <summary>
        /// Await-able window for assigning icon label
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Task<string[]> NameWindow(CCNode node)
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();

            Device.BeginInvokeOnMainThread(() =>
            {
                var popup = new PopUpWindow("Please name the Icon", string.Empty, "OK", "Cancel");
                popup.PopupClosed += (o, closedArgs) =>
                {
                    var sprite = node as CCSprite;
                    var mContent = sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                    if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                    {
                        tcs.SetResult(new string[] { mContent.Text, closedArgs.Text.Trim(), "Embedded" });
                    }
                    else
                    {
                        tcs.SetResult(new string[] { "", "", "" });
                    }
                };

                popup.Show();
            });

            return tcs.Task;
        }

        #endregion

        #region Selection of Local Icons
        
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
                    mLayer.CallBackIcon(results[0], results[1], results[2], null);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    // If permissions not granted, report back to user

                    await DisplayAlert("Permissions Denied", "Can not continue, try again.", "OK");
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
                await CrossMedia.Current.Initialize();

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

                                byte[] imageArray = null;

                                if (Device.OS == TargetPlatform.Android)
                                {
                                    imageArray = DependencyService.Get<IResizer>().RotateImage(@file.Path);
                                }
                                else
                                {
                                    imageArray = File.ReadAllBytes(@file.Path);
                                }

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

                    mLayer.MakeIconFolder(assetName, folderName, null, null);
                }
            }
        }

        #endregion

        #region Capture Pictures from Camera

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
                    mLayer.CallBackIcon(results[0], results[1], results[2], null);
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
                    Name = $"{DateTime.UtcNow}.jpg",
                    SaveToAlbum = true,
                    AllowCropping = true,                    
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

        #endregion

        #region File IO

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
        public string SaveBoards(List<IconReference> icons, List<StoredIconReference> storedIcons, bool inSingleMode)
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
                        var spriteLabel = iconRef.Sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

                        var mModel = new FolderModel("", "", "", 0, 0, -1, iconRef.Sprite.ScaleX, spriteLabel.Color);

                        if (spriteLabel != null)
                        {
                            mModel.Text = spriteLabel.Text;
                        }

                        if (iconRef.Base64.Contains("FolderOpen"))
                        {
                            mModel.AssetName = iconRef.Base64;
                            mModel.Base64 = null;
                        }
                        else
                        {
                            mModel.AssetName = null;
                            mModel.Base64 = iconRef.Base64;
                        }

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
                    mModel.X = (int)mStoredRef.Sprite.PositionX;
                    mModel.Y = (int)mStoredRef.Sprite.PositionY;
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
                mJsonString = JsonConvert.SerializeObject(mIconStorage);

                // Cross-platform call to save a JSON-based text file
                DependencyService.Get<ISaveAndLoad>().SaveJSON("IconBoard", mJsonString);

            });

            return mJsonString;
        }

        #endregion

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
        /// Loading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadGame(object sender, EventArgs e)
        {
            var nativeGameView = sender as CCGameView;

            if (nativeGameView != null)
            {
                var contentSearchPaths = new List<string>() { "Stored" };
                nativeGameView.ContentManager.SearchPaths = contentSearchPaths;
                
                int mWidth = DependencyService.Get<IDisplay>().Width;
                int mHeight = DependencyService.Get<IDisplay>().Height;

                if (mWidth > mHeight)
                {
                    width = DependencyService.Get<IDisplay>().Width;
                    height = DependencyService.Get<IDisplay>().Height;
                }
                else
                {
                    height = DependencyService.Get<IDisplay>().Width;
                    width = DependencyService.Get<IDisplay>().Height;
                }

                nativeGameView.Stats.Enabled = false;

                // Show the start screen
                nativeGameView.RunWithScene(new GameStartScene(nativeGameView, width, height, this));
                
                // Begin building the icon-based scene
                ConstructGameScene(nativeGameView);
            }
        }

        /// <summary>
        /// Start listening on a free port, enabling limited browser access to state
        /// </summary>
        public void StartServer()
        {
            mLayer.SaveJsonContent();

            mServer = new SimpleIconServer(GetBoards("IconBoard"));
            mLayer.ServerActive = true;
            
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Server Active", string.Format("{0}:{1}", mServer.IP, mServer.Port), "Close Server");
            });
        }

        /// <summary>
        /// Shut down server methods
        /// </summary>
        public void StopServer()
        {
            if (mServer != null)
            {
                mServer.Stop();
                mLayer.ServerActive = false;
            }
        }

        /// <summary>
        /// Construct the icon communication board scene
        /// </summary>
        /// <param name="nativeGameView"></param>
        public void ConstructGameScene(CCGameView nativeGameView)
        {
            gameScene = new GameSGDScene(nativeGameView);

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

            int mWidth = DependencyService.Get<IDisplay>().Width;
            int mHeight = DependencyService.Get<IDisplay>().Height;

            if (mWidth > mHeight)
            {
                width = DependencyService.Get<IDisplay>().Width;
                height = DependencyService.Get<IDisplay>().Height;
            }
            else
            {
                height = DependencyService.Get<IDisplay>().Width;
                width = DependencyService.Get<IDisplay>().Height;
            }

            // Create layer for icon board scene
            mLayer = new GameLayer(width, height, jsonObject, this);

            // Do JSON parsing AOT
            mLayer.LoadJsonContent();

            // Add layer to icon board scene
            gameScene.AddLayer(mLayer);
        }
    }
}
