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
using Android.Graphics;
using System;
using System.IO;
using SGDWithCocos.Droid.Implementation;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResizerImplementation))]
namespace SGDWithCocos.Droid.Implementation
{
    class ResizerImplementation : IResizer
    {
        void IResizer.ResizeBitmaps(string photoPath, string newPhotoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPath, options);
            Bitmap croppedBitmap = null;

            if (bitmap.Width >= bitmap.Height)
            {
                croppedBitmap = Bitmap.CreateBitmap(
                   bitmap,
                   bitmap.Width / 2 - bitmap.Height / 2,
                   0,
                   bitmap.Height,
                   bitmap.Height
                   );

            }
            else
            {
                croppedBitmap = Bitmap.CreateBitmap(
                   bitmap,
                   0,
                   bitmap.Height / 2 - bitmap.Width / 2,
                   bitmap.Width,
                   bitmap.Width
                   );
            }

            FileStream stream = null;

            try
            {
                stream = new FileStream(newPhotoPath, FileMode.Create);
                croppedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Failed to write: " + e.ToString());
            }
            finally
            {
                try
                {
                    if (stream != null) {
                        stream.Close();
                    }
                }
                catch (System.IO.IOException e)
                {
                    System.Console.WriteLine("Failed to close: " + e.ToString());
                }
            }
        }
    }
}