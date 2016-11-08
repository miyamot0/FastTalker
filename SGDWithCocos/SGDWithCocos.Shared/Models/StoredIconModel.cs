//----------------------------------------------------------------------------------------------
// <copyright file="StoredIconModel.cs" 
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

namespace SGDWithCocos.Models
{
    /// <summary>
    /// Serialization model for icons
    /// </summary>
    [Serializable()]
    public class StoredIconModel 
    {
        public string Text { get; set; }
        public string Base64 { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale { get; set; }
        public int Tag { get; set; }
        public string Folder { get; set; }
        public float TextScale { get; set; }
        public bool TextVisible { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text"></param>
        /// <param name="base64"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tag"></param>
        /// <param name="folder"></param>
        /// <param name="scale"></param>
        public StoredIconModel(string text, string base64, float x, float y, int tag, string folder, float scale)
        {
            Text = text;
            Base64 = base64;
            X = x;
            Y = y;
            Tag = tag;
            Folder = folder;
            Scale = scale;
            TextScale = 1f;
            TextVisible = true;
        }
    }
}
