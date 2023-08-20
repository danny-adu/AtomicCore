using System;
using System.IO;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.DrawingCore.Drawing2D;
using System.Net;

namespace AtomicCore
{
    /// <summary>
    /// 图片缩略策略
    /// </summary>
    public enum ImageThumbStrategy
    {
        /// <summary>
        /// 指定高宽裁减（不变形）
        /// </summary>
        Cut,
        /// <summary>
        /// 大于指定比例则智能等比压缩后居中,小于指定比例则直接居中,周围填冲指定色
        /// </summary>
        SmartCut,
        /// <summary>
        /// 指定宽，高按比例
        /// </summary>
        Width,
        /// <summary>
        /// 指定高，宽按比例
        /// </summary>
        Height,
        /// <summary>
        /// 智能填充
        /// </summary>
        SmartFill
    }

    /// <summary>
    /// 图片处理类
    /// </summary>
    public static class ImageHandler
    {
        #region 图片流类型

        /// <summary>
        /// 根据图片的ImageFormat的GUID值获取图片的类型
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static ImageFormat GetFormat(Guid guid)
        {
            if (ImageFormat.Bmp.Guid == guid)
            {
                return ImageFormat.Bmp;
            }
            else if (ImageFormat.Emf.Guid == guid)
            {
                return ImageFormat.Emf;
            }
            else if (ImageFormat.Exif.Guid == guid)
            {
                return ImageFormat.Exif;
            }
            else if (ImageFormat.Gif.Guid == guid)
            {
                return ImageFormat.Gif;
            }
            else if (ImageFormat.Icon.Guid == guid)
            {
                return ImageFormat.Icon;
            }
            else if (ImageFormat.Jpeg.Guid == guid)
            {
                return ImageFormat.Jpeg;
            }
            else if (ImageFormat.MemoryBmp.Guid == guid)
            {
                return ImageFormat.MemoryBmp;
            }
            else if (ImageFormat.Png.Guid == guid)
            {
                return ImageFormat.Png;
            }
            else if (ImageFormat.Tiff.Guid == guid)
            {
                return ImageFormat.Tiff;
            }
            else if (ImageFormat.Wmf.Guid == guid)
            {
                return ImageFormat.Wmf;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据图片流获取图片的格式（根据图片流真实获取）
        /// </summary>
        /// <param name="imageStream">图片流</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(Stream imageStream)
        {
            if (null == imageStream || imageStream.Length == 0)
            {
                return null;
            }
            //设置图片流游标
            ImageFormat imgFormat = null;
            Image img = null;
            imageStream.Position = 0;
            try
            {
                img = Image.FromStream(imageStream);
                imgFormat = GetFormat(img.RawFormat.Guid);
            }
            catch
            {
                imgFormat = null;
            }
            finally
            {
                img.Dispose();

                //重置游标
                imageStream.Position = 0;
            }

            return imgFormat;
        }

        /// <summary>
        /// 根据图片的ImageFormat的GUID值获取图片的类型
        /// </summary>
        /// <param name="guid">ImageFormat类型中的GUID属性</param>
        /// <param name="isExistsDot">是否包含“.”</param>
        /// <returns></returns>
        public static string GetExtension(Guid guid, bool isExistsDot = true)
        {
            if (ImageFormat.Bmp.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".bmp" : "bmp");
            }
            else if (ImageFormat.Emf.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".emf" : "emf");
            }
            else if (ImageFormat.Exif.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".exif" : "exif");
            }
            else if (ImageFormat.Gif.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".gif" : "gif");
            }
            else if (ImageFormat.Icon.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".icon" : "icon");
            }
            else if (ImageFormat.Jpeg.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".jpg" : "jpg");
            }
            else if (ImageFormat.MemoryBmp.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".bmp" : "bmp");
            }
            else if (ImageFormat.Png.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".png" : "png");
            }
            else if (ImageFormat.Tiff.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".tiff" : "tiff");
            }
            else if (ImageFormat.Wmf.Guid == guid)
            {
                return string.Format("{0}", isExistsDot ? ".wmf" : "wmf");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取图片的拓展名（根据图片流真实获取）
        /// </summary>
        /// <param name="imageStream">图片流</param>
        /// <param name="isExistsDot">是否包含“.”</param>
        /// <returns></returns>
        public static string GetExtension(Stream imageStream, bool isExistsDot = true)
        {
            if (null == imageStream || imageStream.Length == 0)
            {
                return null;
            }
            //设置图片流游标
            string ext = null;
            Image img = null;
            imageStream.Position = 0;

            try
            {
                img = Image.FromStream(imageStream);
                ext = GetExtension(img.RawFormat.Guid, isExistsDot);
            }
            catch
            {
                ext = null;
            }
            finally
            {
                img.Dispose();

                //重置游标
                imageStream.Position = 0;
            }

            return ext;
        }

        /// <summary>
        /// 侦测图片类型
        /// </summary>
        /// <param name="imgBytes">字节数组</param>
        /// <param name="returnFormat">返回格式</param>
        /// <returns></returns>
        public static string DetectFormat(byte[] imgBytes, string returnFormat = ".{0}")
        {
            Stream imgStream = imgBytes.ToStream();
            return DetectFormat(imgStream, returnFormat);
        }

        /// <summary>
        /// 侦测图片类型
        /// </summary>
        /// <param name="imgStream">图片流数组</param>
        /// <param name="returnFormat">返回格式</param>
        /// <returns></returns>
        public static string DetectFormat(Stream imgStream, string returnFormat = ".{0}")
        {
            if (Stream.Null == imgStream || 0 >= imgStream.Length)
            {
                return string.Empty;
            }

            string format = string.Empty;
            using (Image img = Image.FromStream(imgStream))
            {
                try
                {
                    format = string.Format(returnFormat, img.RawFormat.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    img.Dispose();
                }
            }

            return format;
        }

        #endregion

        #region 打水印

        /// <summary>
        /// 为图片附件图片水印
        /// </summary>
        /// <param name="targetStream">需要打水印的图片流</param>
        /// <param name="watermarkStream">水印文件的图片流</param>
        /// <param name="watermarkStatus">图片水印位置 
        /// 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="watermarkTransparency">水印的透明度 1--10 10为不透明</param>
        /// <returns></returns>
        public static Stream AttachWaterMark(Stream targetStream, Stream watermarkStream, int watermarkStatus = 9, int quality = 80, int watermarkTransparency = 5)
        {
            MemoryStream ms = null;

            if (null == targetStream || null == watermarkStream)
            {
                return ms;
            }

            //构造源图对象GDI
            Image img = Image.FromStream(targetStream);
            Graphics g = Graphics.FromImage(img);
            //设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkStream);

            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
            {
                return ms;
            }

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
												new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
											};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                default:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(ms, ici, encoderParams);
            else
                img.Save(ms, img.RawFormat);

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
            ms.Position = 0;

            return ms;
        }

        /// <summary>
        /// 为图片附件文字水印
        /// </summary>
        /// <param name="targetStream">需要打水印的图片流</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="fontname">文字字体</param>
        /// <param name="fontsize">字体尺寸</param>
        /// <param name="watermarkStatus">图片水印位置 
        /// 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <returns></returns>

        public static Stream AttachWaterMark(Stream targetStream, string watermarkText, string fontname = "Tahoma", int fontsize = 12, int watermarkStatus = 9, int quality = 80)
        {
            MemoryStream ms = null;

            if (null == targetStream)
            {
                return ms;
            }

            //构造源图对象GDI
            Image img = Image.FromStream(targetStream);
            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Black), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(ms, ici, encoderParams);
            else
                img.Save(ms, img.RawFormat);

            g.Dispose();
            img.Dispose();
            ms.Position = 0;

            return ms;
        }

        #endregion

        #region 获取远程图片流

        /// <summary>
        /// 获取图片流
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <returns></returns>
        private static Stream RemoteImage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 20000;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 缩略图方法

        /// <summary>
        /// 制作小正方形
        /// </summary>
        /// <param name="image">图片对象</param>
        /// <param name="newSize">长度或宽度</param>
        public static Stream MakeSquare(Image image, int newSize)
        {
            if (null == image || 0 >= newSize)
            {
                return null;
            }

            int i = 0;
            int width = image.Width;
            int height = image.Height;
            if (width > height)
                i = height;
            else
                i = width;

            Stream ms = new MemoryStream();
            Bitmap b = new Bitmap(newSize, newSize);

            try
            {
                Graphics g = Graphics.FromImage(b);
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);
                if (width < height)
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle(0, (height - width) / 2, width, width), GraphicsUnit.Pixel);
                else
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle((width - height) / 2, 0, height, height), GraphicsUnit.Pixel);

                ImageFormat actFormat = GetFormat(image.RawFormat.Guid);
                if (null == actFormat)
                {
                    throw new Exception("图片格式异常");
                }
                ImageCodecInfo ici = GetCodecInfo("image/" + actFormat.ToString().ToLower());
                ms = SaveImage(b, ici);
                ms.Position = 0;
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }

            return ms;
        }

        /// <summary>
        /// 制作小正方形
        /// </summary>
        /// <param name="fileStream">图片文件流</param>
        /// <param name="newFileName">新文件名称</param>
        /// <param name="newSize">长度或宽度</param>
        public static Stream MakeSquare(Stream fileStream, string newFileName, int newSize)
        {
            return MakeSquare(Image.FromStream(fileStream), newSize);
        }

        /// <summary>
        /// 制作远程小正方形
        /// </summary>
        /// <param name="url">图片url</param>
        /// <param name="newSize">长度或宽度</param>
        public static Stream MakeRemoteSquare(string url, int newSize)
        {
            Stream stream = RemoteImage(url);
            if (stream == null)
                return null;
            Image original = Image.FromStream(stream);
            stream.Close();
            return MakeSquare(original, newSize);
        }

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="fileStream">图片流</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static Stream MakeThumbnail(Stream fileStream, int maxWidth, int maxHeight)
        {
            //2012-02-05修改过，支持替换
            Image img = Image.FromStream(fileStream);
            return GenerateThumbnail(img, maxWidth, maxHeight);
        }

        /// <summary>
        /// 生成缩略图(选择Cute可保证图片不变形)
        /// </summary>
        /// <param name="fileStream">源图流</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">根据枚举类型确定压缩策略</param>    
        public static Stream MakeThumbnail(Stream fileStream, int width, int height, ImageThumbStrategy mode)
        {
            return MakeThumbnail(fileStream, width, height, Color.Transparent, mode);
        }

        /// <summary>
        /// 生成缩略图(选择Cute可保证图片不变形)
        /// </summary>
        /// <param name="fileStream">源图流</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="bgColor">缩略图背景填充色</param>
        /// <param name="mode">根据枚举类型确定压缩策略</param>    
        public static Stream MakeThumbnail(Stream fileStream, int width, int height, Color bgColor, ImageThumbStrategy mode)
        {
            if (null == fileStream)
            {
                return null;
            }

            Stream ms = null;
            Image originalImage = Image.FromStream(fileStream);
            int towidth = width;//指定的缩略图的宽度
            int toheight = height;//指定缩略图的高度

            int drawX = 0;//指定在新的画布上开始画图的左顶点左边X
            int drawY = 0;//指定在新的画布上开始画图的左顶点左边Y
            int drawW = width;//指定在画布中需要画的宽度
            int drawH = height;//指定在画布中需要画的高度

            int cuteX = 0;//在源图上进行截图的坐标X
            int cuteY = 0;//在源图上进行截图的坐标Y
            int cuteW = originalImage.Width;//在源图上进行截图的宽度
            int cuteH = originalImage.Height;//在源图上进行截图的高度

            Image ThumbImage = null;

            switch (mode)
            {
                case ImageThumbStrategy.Cut:
                    #region 指定高宽裁减（不变形）
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        cuteH = originalImage.Height;
                        cuteW = originalImage.Height * towidth / toheight;
                        cuteY = 0;
                        cuteX = (originalImage.Width - cuteW) / 2;
                    }
                    else
                    {
                        cuteW = originalImage.Width;
                        cuteH = originalImage.Width * toheight / towidth;
                        cuteX = 0;
                        cuteY = (originalImage.Height - cuteH) / 2;
                    }
                    break;
                    #endregion
                case ImageThumbStrategy.SmartCut://智能裁剪,小图周围填充色
                    #region 智能裁剪,取短板压缩到指定,长版从中间截取,都小则使用填充色
                    /*
                     * 缩略图的宽与高都比源图大,执行将源图放在新的图片的正中间策略
                     */
                    if (originalImage.Width <= width && originalImage.Height <= height)
                    {
                        //图中图的左顶点坐标
                        drawX = (width - originalImage.Width) / 2;
                        drawY = (height - originalImage.Height) / 2;
                        drawW = originalImage.Width;
                        drawH = originalImage.Height;
                    }
                    else
                    {
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            cuteH = originalImage.Height;
                            cuteW = originalImage.Height * towidth / toheight;
                            cuteY = 0;
                            cuteX = (originalImage.Width - cuteW) / 2;
                        }
                        else
                        {
                            cuteW = originalImage.Width;
                            cuteH = originalImage.Width * toheight / towidth;
                            cuteX = 0;
                            cuteY = (originalImage.Height - cuteH) / 2;
                        }
                    }
                    break;
                    #endregion
                case ImageThumbStrategy.SmartFill://智能填充,小图周围填充色
                    #region 智能裁剪,取长板压缩到指定,短版居中绘制,都小则使用填充色(只是复制了 尚未开发 研究中)

                    /*
                     * 缩略图的宽与高都比源图大,执行将源图放在新的图片的正中间策略
                     */
                    if (originalImage.Width <= width && originalImage.Height <= height)
                    {
                        #region 宽高均小，居中 四周填充

                        //如果源图宽和高都小于指定压缩的图片的宽高，将源图置于画布中间，四周填充色
                        drawX = (width - originalImage.Width) / 2;
                        drawY = (height - originalImage.Height) / 2;
                        drawW = originalImage.Width;
                        drawH = originalImage.Height;

                        //指定源图截取部分为全图
                        cuteW = originalImage.Width;
                        cuteH = originalImage.Height;
                        cuteX = 0;
                        cuteY = 0;

                        #endregion
                    }
                    else
                    {
                        double origRate_wh = (double)originalImage.Width / (double)originalImage.Height;
                        double toRate_Wh = (double)towidth / (double)toheight;

                        if (origRate_wh > toRate_Wh)
                        {
                            #region 按宽度进行等比缩放到指定压缩宽度

                            int thumb_w = towidth;
                            int thumb_h = (int)((double)originalImage.Height * (double)towidth / (double)originalImage.Width);
                            ThumbImage = originalImage.GetThumbnailImage(thumb_w, thumb_h, ThumbnailCallback, IntPtr.Zero);

                            //指定源图在画布中所要展示的左定点位置与大小（计算居中坐标）
                            drawW = ThumbImage.Width;
                            drawH = ThumbImage.Height;
                            drawX = 0;
                            drawY = Math.Abs(toheight - ThumbImage.Height) / 2;

                            //指定源图截取部分为全图
                            cuteW = ThumbImage.Width;
                            cuteH = ThumbImage.Height;
                            cuteX = 0;
                            cuteY = 0;

                            #endregion
                        }
                        else
                        {
                            #region 按高度进行等比缩放到指定压缩高度

                            int thumb_w = (int)((double)originalImage.Width * (double)toheight / (double)originalImage.Height);
                            int thumb_h = toheight;
                            ThumbImage = originalImage.GetThumbnailImage(thumb_w, thumb_h, ThumbnailCallback, IntPtr.Zero);

                            //指定源图在画布中所要展示的左定点位置与大小（计算居中坐标）
                            drawW = ThumbImage.Width;
                            drawH = ThumbImage.Height;
                            drawX = Math.Abs(towidth - ThumbImage.Width) / 2;
                            drawY = 0;

                            //指定源图截取部分为全图
                            cuteW = ThumbImage.Width;
                            cuteH = ThumbImage.Height;
                            cuteX = 0;
                            cuteY = 0;

                            #endregion
                        }
                    }
                    break;
                    #endregion
                case ImageThumbStrategy.Width://指定宽，高按比例  
                    #region 指定宽，高按比例
                    toheight = originalImage.Height * width / originalImage.Width;
                    drawH = toheight;
                    break;
                    #endregion
                case ImageThumbStrategy.Height://指定高，宽按比例
                    #region 指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    drawW = towidth;
                    break;
                    #endregion
                default:
                    break;
            }

            //新建一个bmp图片
            Bitmap b = new Bitmap(towidth, toheight);
            try
            {
                //新建一个画板
                Graphics g = Graphics.FromImage(b);
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //清空画布并以透明背景色填充
                g.Clear(bgColor);
                if (null != ThumbImage)
                {
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(
                        ThumbImage, //要绘制的源图
                        new Rectangle(drawX, drawY, drawW, drawH), //指定源图所在画布的位置与大小
                        new Rectangle(cuteX, cuteY, cuteW, cuteH), //指定需要绘制源图中的部分
                        GraphicsUnit.Pixel
                        );

                    ImageFormat actFormat = GetFormat(originalImage.RawFormat.Guid);
                    if (null == actFormat)
                    {
                        throw new Exception("图片格式异常");
                    }
                    ImageCodecInfo ici = GetCodecInfo("image/" + actFormat.ToString().ToLower());
                    ms = SaveImage(b, ici);
                    ms.Position = 0;
                }
                else
                {
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(
                        originalImage, //要绘制的源图
                        new Rectangle(drawX, drawY, drawW, drawH), //指定源图所在画布的位置与大小
                        new Rectangle(cuteX, cuteY, cuteW, cuteH), //指定需要绘制源图中的部分
                        GraphicsUnit.Pixel
                        );

                    ImageFormat actFormat = GetFormat(originalImage.RawFormat.Guid);
                    if (null == actFormat)
                    {
                        throw new Exception("图片格式异常");
                    }
                    ImageCodecInfo ici = GetCodecInfo("image/" + actFormat.ToString().ToLower());
                    ms = SaveImage(b, ici);
                    ms.Position = 0;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                if (null != ThumbImage)
                    ThumbImage.Dispose();

                originalImage.Dispose();
                b.Dispose();
                fileStream.Seek(0, SeekOrigin.Begin);
            }

            return ms;
        }

        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        /// <param name="fileStream">源图流</param>
        /// <param name="maxWidth">缩略图宽度</param>
        /// <param name="maxHeight">缩略图高度</param>
        /// <param name="cropWidth">裁剪宽度</param>
        /// <param name="cropHeight">裁剪高度</param>
        /// <param name="X">X轴</param>
        /// <param name="Y">Y轴</param>
        public static Stream MakeThumbnail(Stream fileStream, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            if (null == fileStream)
            {
                return null;
            }

            Stream ms = new MemoryStream();
            Image originalImage = Image.FromStream(fileStream);
            Bitmap b = new Bitmap(cropWidth, cropHeight);
            try
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    //设置高质量插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //设置高质量,低速度呈现平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, cropWidth, cropHeight), X, Y, cropWidth, cropHeight, GraphicsUnit.Pixel);
                    Image displayImage = new Bitmap(b, maxWidth, maxHeight);

                    ImageFormat actFormat = GetFormat(originalImage.RawFormat.Guid);
                    if (null == actFormat)
                    {
                        throw new Exception("图片格式异常");
                    }
                    ImageCodecInfo ici = GetCodecInfo("image/" + actFormat.ToString().ToLower());
                    ms = SaveImage(displayImage, ici);
                    ms.Position = 0;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                b.Dispose();
                fileStream.Position = 0;
            }

            return ms;
        }

        /// <summary>
        /// 制作远程缩略图
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static Stream MakeRemoteThumbnail(string url, int maxWidth, int maxHeight)
        {
            Stream stream = RemoteImage(url);
            if (stream == null)
                return null;
            Image original = Image.FromStream(stream);
            stream.Close();
            return GenerateThumbnail(original, maxWidth, maxHeight);
        }

        #endregion

        #region 私有方法


        /// <summary>
        /// 缩略图回调CallBack
        /// </summary>
        /// <returns></returns>
        private static bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="ici">指定格式的编解码参数</param>
        private static Stream SaveImage(Image image, ImageCodecInfo ici)
        {
            Stream ms = new MemoryStream();

            //设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            image.Save(ms, ici, parameters);
            parameters.Dispose();

            return ms;
        }

        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType)
                    return ici;
            }
            return null;
        }

        /// <summary>
        /// 计算新尺寸(maxWidth、maxHeight若为0,则各自与宽、高一致)
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;
            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="original">图片对象</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="isDisposeOrginalImage">是否析构原Image对象</param>
        private static Stream GenerateThumbnail(Image original, int maxWidth, int maxHeight, bool isDisposeOrginalImage = true)
        {
            if (null == original)
            {
                return null;
            }

            Stream ms = new MemoryStream();
            Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);

            using (Image displayImage = new Bitmap(original, _newSize))
            {
                try
                {
                    displayImage.Save(ms, original.RawFormat);
                }
                finally
                {
                    if (isDisposeOrginalImage)
                    {
                        original.Dispose();
                    }
                }
            }
            ms.Position = 0;

            return ms;
        }

        #endregion
    }
}
