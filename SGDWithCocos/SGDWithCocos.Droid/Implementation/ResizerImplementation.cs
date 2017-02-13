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
using Android.Media;
using Java.Lang;

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
                   bitmap.Height);
            }
            else
            {
                croppedBitmap = Bitmap.CreateBitmap(
                   bitmap,
                   0,
                   bitmap.Height / 2 - bitmap.Width / 2,
                   bitmap.Width,
                   bitmap.Width);
            }

            FileStream stream = null;

            try
            {
                stream = new FileStream(newPhotoPath, FileMode.Create);
                //croppedBitmap = ModifyOrientation(photoPath, croppedBitmap);
                croppedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Failed to write: " + e.ToString());
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Failed to close: " + e.ToString());
                }
            }
        }

        public byte[] RotateImage(string photoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPath, options);

            try
            {
                ExifInterface exifInterface = new ExifInterface(photoPath);
                int orientation = exifInterface.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

                System.Diagnostics.Debug.WriteLine("Orientation of image" + orientation.ToString());
                Console.WriteLine("Orientation of image" + orientation.ToString());

                int rotate = 0;

                switch (orientation)
                {
                    case (int)Android.Media.Orientation.Normal:
                        rotate = 0;
                        break;

                    case (int)Android.Media.Orientation.Rotate90:
                        rotate = 90;
                        break;

                    case (int)Android.Media.Orientation.Rotate270:
                        rotate = 270;
                        break;

                    case (int)Android.Media.Orientation.Rotate180:
                        rotate = 180;
                        break;

                    default:
                        rotate = 0;
                        break;
                }

                using (var ms = new MemoryStream())
                {
                    Bitmap croppedBitmap = null;

                    Matrix mtx = new Matrix();
                    mtx.PreRotate(rotate);

                    if (bitmap.Width >= bitmap.Height)
                    {
                        croppedBitmap = Bitmap.CreateBitmap(
                           bitmap,
                           bitmap.Width / 2 - bitmap.Height / 2,
                           0,
                           bitmap.Height,
                           bitmap.Height,
                           mtx,
                           false);
                    }
                    else
                    {
                        croppedBitmap = Bitmap.CreateBitmap(
                           bitmap,
                           0,
                           bitmap.Height / 2 - bitmap.Width / 2,
                           bitmap.Width,
                           bitmap.Width,
                           mtx,
                           false);
                    }
                                        
                    croppedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);

                    return ms.ToArray();
                }
            }
            catch { }

            return null;
        }
    }
}