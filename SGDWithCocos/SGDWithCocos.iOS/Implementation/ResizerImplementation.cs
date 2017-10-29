//----------------------------------------------------------------------------------------------
// <copyright file="MainActivity.cs" 
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

using SGDWithCocos.Interface;
using SGDWithCocos.iOS.Implementation;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResizerImplementation))]
namespace SGDWithCocos.iOS.Implementation
{
    public class ResizerImplementation : IResizer
    {
        /// <summary>
        /// Empty method
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="photoPath">Photo path.</param>
        public byte[] RotateImage(string photoPath) { return null; }

        /// <summary>
        /// Empty method
        /// </summary>
        /// <param name="photoPath">Photo path.</param>
        /// <param name="newPhotoPath">New photo path.</param>
        void IResizer.ResizeBitmaps(string photoPath, string newPhotoPath) { }
    }
}