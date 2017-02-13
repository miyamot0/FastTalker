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
                croppedBitmap = ModifyOrientation(photoPath, croppedBitmap);
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

        private Bitmap ModifyOrientation(string photoPath, Bitmap bitmap)
        {
            ExifInterface exifInterface = new ExifInterface(photoPath);
            int orientation = exifInterface.GetAttributeInt(ExifInterface.TagOrientation, 0);

            var matrix = new Matrix();
            switch (orientation)
            {
                case 2:
                    matrix.SetScale(-1, 1);
                    break;

                case 3:
                    matrix.SetRotate(180);
                    break;

                case 4:
                    matrix.SetRotate(180);
                    matrix.PostScale(-1, 1);
                    break;

                case 5:
                    matrix.SetRotate(90);
                    matrix.PostScale(-1, 1);
                    break;

                case 6:
                    matrix.SetRotate(90);
                    break;

                case 7:
                    matrix.SetRotate(-90);
                    matrix.PostScale(-1, 1);
                    break;

                case 8:
                    matrix.SetRotate(-90);
                    break;

                default:
                    return bitmap;
            }

            try
            {
                Bitmap oriented = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
                bitmap.Recycle();

                return oriented;
            }
            catch (OutOfMemoryError e)
            {
                e.PrintStackTrace();

                return bitmap;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);

                return bitmap;
            }
        }
    }
}