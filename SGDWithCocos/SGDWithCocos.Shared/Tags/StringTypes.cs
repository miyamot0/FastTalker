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
        public static string HideImage = "Toggle Image";
        public static string ChangeSizeUp = "Scale Image Up";
        public static string ChangeSizeDefault = "Scale Image to Default";
        public static string ChangeSizeDown = "Scale Image Down";

        public static string HideText = "Toggle Label";
        public static string ChangeText = "Edit Label";
        public static string ChangeTextSizeUp = "Scale Label Up";
        public static string ChangeTextSizeDefault = "Scale Label to Default";
        public static string ChangeTextSizeDown = "Scale Label Down";

        public static string LocalImage = "Local";
        public static string DownloadedImage = "Downloaded";

        public static string AddIcon = "Add Icon";
        public static string TakePhoto = "Take Photo";
        public static string AddFolder = "Add Folder";
    }
}
