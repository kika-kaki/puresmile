using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PureSmileUI.App_Start
{
    public class FileHelper
    {
        public static string SaveFile(ControllerContext context)
        {
            try
            {
                if (context.HttpContext.Request.Files.Count > 0)
                {
                    var file = context.HttpContext.Request.Files[0];
                    var filepath = GetFilePath(file.FileName);
                    file.SaveAs(filepath);
                    var filename = Path.GetFileName(filepath);
                    return UrlHelper.GenerateContentUrl(string.Format("{0}{1}", ConfigurationManager.ImageStoragePath, filename), context.HttpContext);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex, "SaveFile. Error occured while saving file.");
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteFile(string filepath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filepath))
                {
                    File.Delete(HttpContext.Current.Server.MapPath("~" + filepath));
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex, "DeleteFile. Error occured while deleting file.");
            }
        }

        public static bool ValidateIfImage(HttpFileCollectionBase files)
        {
            var extensions = ConfigurationManager.AllowedImageFormats.Split(' ');
            if (files.Count > 0)
            {
                foreach (var key in files.AllKeys)
                {
                    var file = files[key];
                    if (!string.IsNullOrEmpty(file.FileName) && !extensions.Contains(Path.GetExtension(file.FileName)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static string GetFilePath(string fileName)
        {
            var storagePath = HttpContext.Current.Server.MapPath($"~{ConfigurationManager.ImageStoragePath}");
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
            return string.Format("{0}{1}{2}", storagePath, Guid.NewGuid(), Path.GetExtension(fileName));
        }
    }
}