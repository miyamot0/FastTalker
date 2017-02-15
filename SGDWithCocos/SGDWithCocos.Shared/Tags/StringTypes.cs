//----------------------------------------------------------------------------------------------
// <copyright file="StringTypes.cs" 
// Copyright November 6, 2016 Shawn Gilroy
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

namespace SGDWithCocos.Tags
{
    /// <summary>
    /// Default string tags, static for comparative logic
    /// </summary>
    public static class StringTypes
    {
        #region Image Tags

        public static string HideImage = "Toggle Image";

        public static string ChangeSizeDefault = "Scale Image to Default";
        public static string ChangeSizeUp = "Scale Image Up (10%)";
        public static string ChangeSizeUp2 = "Scale Image Up Significantly (50%)";
        public static string ChangeSizeDown = "Scale Image Down (10%)";
        public static string ChangeSizeDown2 = "Scale Image Down Significantly (50%)";

        #endregion

        #region Label Tags

        public static string HideText = "Toggle Label";
        public static string ChangeText = "Edit Label";
        public static string ChangeTextSizeUp = "Scale Label Up (10%)";
        public static string ChangeTextSizeDefault = "Scale Label to Default";
        public static string ChangeTextSizeDown = "Scale Label Down (10%)";

        #endregion

        #region Location Tags

        public static string LocalImage = "Local";
        public static string DownloadedImage = "Downloaded";

        #endregion

        #region Server Tags (Stubbed for now)

        public static string ServerStart = "Start Server";
        public static string ServerShutdown = "Shut Down Server";

        #endregion

        #region Global Settings Tags

        public static string ResumeOperation = "Resume Child Operation";
        public static string ForceSave = "Force Save";
        public static string ImportBoard = "Import Open Board File";
        public static string ChangeSettings = "Change Output Mode";
        public static string AddIcon = "Add an Icon";
        public static string TakePhoto = "Take a Photo";
        public static string AddFolder = "Add a Folder";

        #endregion

        #region Presentation Format Tags

        public static string SingleMode = "Single Icons";
        public static string FrameMode = "Sentence Structure";

        #endregion
    }
}
