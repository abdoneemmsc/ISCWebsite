using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;


namespace CMS.Web.Classes
{
    public class UploadingFiles
    {
        private string[] _Extensions;
        public List<string> ErrorMessage = new List<string>();
        public string[] Extensions { get { return _Extensions; } set { _Extensions = value; } }
        public long MaxFileSize { get; set; }
        //private string _FolderName;
        public string FolderName { get; set; }
        private IFormFile _FileToUpload;
        private int _Width;
        [DefaultValue(0)]
        public int Width { get { return _Width; } set { _Width = value; } }
        private int _Height;
        public int Height { get { return _Height; } set { _Height = value; } }
        public string newFileName { get; set; }
        public IFormFile FileToUpload
        {
            get
            {
                return _FileToUpload;
            }
            set
            {
                _FileToUpload = value;
            }
        }
        public string upload()
        {
            string targetPath = "";
            if (FileToUpload != null)
            {
                // Check the extension of image  
                string extension = Path.GetExtension(FileToUpload.FileName);
                bool isInExt = false;
                foreach (string ext in Extensions)
                {
                    if (extension.ToLower() == ext)
                    {
                        isInExt = true;
                        break;
                    }
                }
                if (FileToUpload.Length > MaxFileSize)
                {
                    ErrorMessage.Add("حجم الملف كبير");
                }
                else if (!isInExt)
                {
                    ErrorMessage.Add("امتداد الملف غير صالح");
                }
                else
                {
                    Stream strm = FileToUpload.OpenReadStream();
                    try
                    {
                        using (var image = Image.FromStream(strm))
                        {
                            var heightWidthPercentage = decimal.Divide(image.Height, image.Width);
                            var widthHeightPercentage = decimal.Divide(image.Width, image.Height);

                            int newHeight = Height;
                            int newWidth = Width;
                            if (Width == 0)
                            {
                                newHeight = Height; newWidth = Convert.ToInt32(Height * widthHeightPercentage);
                            }
                            else if (Height == 0)
                            {
                                newWidth = Width; newHeight = Convert.ToInt32(Width * heightWidthPercentage);
                            }
                            else if (Width == 0 && Height == 0)
                            {
                                newWidth = image.Width;
                                newHeight = image.Height;
                            }
                            var thumbImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            var imgRectangle = new Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image, imgRectangle);
                            targetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", newFileName);
                            thumbImg.Save(targetPath, image.RawFormat);
                        }

                    }
                    catch
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", newFileName);
                        using (Stream stream = new FileStream(path, FileMode.Create))
                        {
                            FileToUpload.CopyTo(stream);
                        }
                    }
                }

            }
            return targetPath;
        }

        internal void removerFile(string oldFilePath)
        {
            try { File.Delete(oldFilePath); } catch { }
        }
    }

}
