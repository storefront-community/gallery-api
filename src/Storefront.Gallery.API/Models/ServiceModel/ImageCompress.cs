using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Storefront.Gallery.API.Models.ServiceModel
{
    public sealed class ImageCompress
    {
        private readonly Image _image;

        public ImageCompress(Image image)
        {
            _image = image;
        }

        public ImageCompress(Stream stream) : this(Image.FromStream(stream)) { }

        public Stream Optimize(int maxWidth, int maxHeight, long quality = 90)
        {
            return ResizeKeepingAspectRatio(maxWidth, maxHeight).ToJpegStream(quality);
        }

        private ImageCompress ResizeKeepingAspectRatio(int maxWidth, int maxHeight)
        {
            var calculatedSize = CalculateAspectRatio(maxWidth, maxHeight);
            var rectangle = new Rectangle(0, 0, calculatedSize.Width, calculatedSize.Height);
            var resizedImage = new Bitmap(calculatedSize.Width, calculatedSize.Height);

            resizedImage.SetResolution(_image.HorizontalResolution, _image.VerticalResolution);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(_image, rectangle, 0, 0, _image.Width,
                        _image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return new ImageCompress(resizedImage);
        }

        private Stream ToJpegStream(long quality)
        {
            var stream = new MemoryStream();
            var codecJPEG = GetCodec("image/jpeg");
            var encoderParameters = new EncoderParameters(1);

            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            _image.Save(stream, codecJPEG, encoderParameters);

            return stream;
        }

        private Size CalculateAspectRatio(int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / _image.Width;
            double ratioY = (double)maxHeight / _image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int width = (int)(_image.Width * ratio);
            int height = (int)(_image.Height * ratio);

            return new Size(width, height);
        }

        private ImageCodecInfo GetCodec(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(c => c.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase));
        }
    }
}
