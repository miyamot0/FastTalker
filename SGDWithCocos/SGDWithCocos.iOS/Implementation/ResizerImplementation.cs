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

using CoreGraphics;
using SGDWithCocos.Interface;
using SGDWithCocos.iOS.Implementation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResizerImplementation))]
namespace SGDWithCocos.iOS.Implementation
{
    public class ResizerImplementation : IResizer
    {
        void IResizer.ResizeBitmaps(string photoPath, string newPhotoPath)
        {
            var sourceImage = UIImage.FromFile(photoPath);
            var imgSize = sourceImage.Size;

            if (imgSize.Width >= imgSize.Height)
            {
                var x = imgSize.Width / 2 - imgSize.Height / 2;
                var y = 0;
                var height = imgSize.Height;
                var width = imgSize.Height;

                UIGraphics.BeginImageContext(new CGSize(width, height));
                var context = UIGraphics.GetCurrentContext();
                var clippedRect = new CGRect(0, 0, width, height);
                context.ClipToRect(clippedRect);
                var drawRect = new CGRect(-x, -y, imgSize.Width, imgSize.Height);
                sourceImage.Draw(drawRect);
                var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                
                modifiedImage.AsJPEG(1).Save(newPhotoPath, true);

            }
            else
            {
                var x = 0;
                var y = imgSize.Height / 2 - imgSize.Width / 2;
                var height = imgSize.Width;
                var width = imgSize.Width;

                UIGraphics.BeginImageContext(new CGSize(width, height));
                var context = UIGraphics.GetCurrentContext();
                var clippedRect = new CGRect(0, 0, width, height);
                context.ClipToRect(clippedRect);
                var drawRect = new CGRect(-x, -y, imgSize.Width, imgSize.Height);
                sourceImage.Draw(drawRect);
                var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                modifiedImage.AsJPEG(1).Save(newPhotoPath, true);

            }
        }
    }
}