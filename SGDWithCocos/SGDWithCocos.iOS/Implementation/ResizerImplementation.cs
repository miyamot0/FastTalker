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
        public byte[] RotateImage(string photoPath)
        {
            return null;
        }

        void IResizer.ResizeBitmaps(string photoPath, string newPhotoPath)
        {
            UIImage sourceImage = UIImage.FromFile(photoPath);
            CGSize imgSize = sourceImage.Size;

            if (imgSize.Width >= imgSize.Height)
            {
                System.nfloat x = imgSize.Width / 2 - imgSize.Height / 2;
                System.nfloat y = 0;
                System.nfloat height = imgSize.Height;
                System.nfloat width = imgSize.Height;

                UIGraphics.BeginImageContext(new CGSize(width, height));
                CGContext context = UIGraphics.GetCurrentContext();

                //CGRect clippedRect = new CGRect(0, 0, width, height);
                //context.ClipToRect(clippedRect);
                context.ClipToRect(new CGRect(0, 0, width, height));

                //var drawRect = new CGRect(-x, -y, imgSize.Width, imgSize.Height);
                //sourceImage.Draw(drawRect);
                sourceImage.Draw(new CGRect(-x, -y, imgSize.Width, imgSize.Height));

                UIImage modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                
                modifiedImage.AsJPEG(1).Save(newPhotoPath, true);

                //sourceImage.Dispose();
                //modifiedImage.Dispose();
            }
            else
            {
                System.nfloat x = 0;
                System.nfloat y = imgSize.Height / 2 - imgSize.Width / 2;
                System.nfloat height = imgSize.Width;
                System.nfloat width = imgSize.Width;

                UIGraphics.BeginImageContext(new CGSize(width, height));
                CGContext context = UIGraphics.GetCurrentContext();

                //CGRect clippedRect = new CGRect(0, 0, width, height);
                //context.ClipToRect(clippedRect);
                context.ClipToRect(new CGRect(0, 0, width, height));
                
                //CGRect drawRect = new CGRect(-x, -y, imgSize.Width, imgSize.Height);
                //sourceImage.Draw(drawRect);
                sourceImage.Draw(new CGRect(-x, -y, imgSize.Width, imgSize.Height));

                UIImage modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                modifiedImage.AsJPEG(1).Save(newPhotoPath, true);

                //sourceImage.Dispose();
                //modifiedImage.Dispose();
            }
        }
    }
}