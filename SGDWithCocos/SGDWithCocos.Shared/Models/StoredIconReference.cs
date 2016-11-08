//----------------------------------------------------------------------------------------------
// <copyright file="StoredIconReference.cs" 
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

using CocosSharp;

namespace SGDWithCocos.Models
{
    /// <summary>
    /// Serialization model for icons
    /// </summary>
    public class StoredIconReference
    {
        public CCSprite Sprite;
        public string Base64;
        public string FolderName;
        public float Scale;
        public float TextScale;
        public bool TextVisible;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="base64"></param>
        /// <param name="folderName"></param>
        /// <param name="scale"></param>
        /// <param name="textScale"></param>
        /// <param name="textVisible"></param>
        public StoredIconReference(CCSprite sprite, string base64, string folderName, float scale, float textScale, bool textVisible)
        {
            Sprite = sprite;
            Base64 = base64;
            FolderName = folderName;
            Scale = scale;
            TextScale = textScale;
            TextVisible = textVisible;
        }
    }
}
