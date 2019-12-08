﻿using LazZiya.ImageResize.Exceptions;
using LazZiya.ImageResize.ResizeMethods;
using LazZiya.ImageResize.Tools;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LazZiya.ImageResize
{
    /// <summary>
    /// Resize images
    /// </summary>
    public static class ImageResize
    {
        /// <summary>
        /// Auto scale image by width or height till longest border (width/height) is equal to new width/height.
        /// Final image aspect ratio is equal to original image aspect ratio.
        /// If the aspect ratio of new w/h != aspect ratio of original image then 
        /// one border will be in different size than the given value in order to keep original aspect ratio
        /// </summary>
        /// <param name="img"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Image Scale(Image img, int newWidth, int newHeight)
        {
            var resize = new Scale(img.Size, new Size(newWidth, newHeight));

            return Resize(img, resize.SourceRect, resize.TargetRect);
        }

        /// <summary>
        /// Scale image by width and keep same aspect ratio of target image same as the original image.
        /// Height will be adjusted automatically
        /// </summary>
        /// <param name="img"></param>
        /// <param name="newWidth"></param>
        /// <returns></returns>
        public static Image ScaleByWidth(Image img, int newWidth)
        {
            var resize = new Scale(img.Size, new Size(newWidth, 0));

            return Resize(img, resize.SourceRect, resize.TargetRect);
        }

        /// <summary>
        /// Scale image by height and keep same aspect ratio of target image same as the original image.
        /// Width will be adjusted automatically
        /// </summary>
        /// <param name="img"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Image ScaleByHeight(Image img, int newHeight)
        {
            var resize = new Scale(img.Size, new Size(0, newHeight));

            return Resize(img, resize.SourceRect, resize.TargetRect);
        }

        /// <summary>
        /// Scale target image till shortest border are equal to target value, 
        /// then crop the additonal pixels from the longest border.
        /// Final image aspect ratio is equal to the given new width/height
        /// </summary>
        /// <param name="img"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <param name="spot"></param>
        /// <returns></returns>
        public static Image ScaleAndCrop(Image img, int newWidth, int newHeight, TargetSpot spot = TargetSpot.Center)
        {
            var resize = new ScaleAndCrop(img.Size, new Size(newWidth, newHeight), spot);

            return Resize(img, resize.SourceRect, resize.TargetRect);
        }

        /// <summary>
        /// Directly crop original image without scaling it.
        /// Final image aspect ratio is equal to given new width/height
        /// </summary>
        /// <param name="img"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <param name="spot">target spot to crop and save</param>
        /// <returns></returns>
        public static Image Crop(Image img, int newWidth, int newHeight, TargetSpot spot = TargetSpot.Center)
        {
            var resize = new Crop(img.Size, new Size(newWidth, newHeight), spot);
            return Resize(img, resize.SourceRect, resize.TargetRect);
        }

        /// <summary>
        /// Specify custom resize options
        /// </summary>
        /// <param name="img">the image to resize</param>
        /// <param name="source">The coordinates to read as source from the image, 
        /// can be the whole image or part of it</param>
        /// <param name="target">The coordinates of the target image size</param>
        /// <returns></returns>
        public static Image Resize(this Image img, Rectangle source, Rectangle target)
        {
            Bitmap outputImage = new Bitmap(target.Width, target.Height, img.PixelFormat);

            outputImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            try
            {
                using (var graphics = Graphics.FromImage(outputImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    graphics.DrawImage(
                        img,
                        target,
                        source,
                        GraphicsUnit.Pixel);

                }
            }
            catch (Exception e)
            {
                throw new ImageResizeException(new ImageResizeResult()
                {
                    Reason = FailureReasonType.GraphicsException,
                    Success = false,
                    Value = e.Message
                });
            }
            return outputImage;
        }
    }
}
