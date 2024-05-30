using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using FileInfo = Projekat1.Models.FileInfo;


namespace Projekat1.Services
{
    public class FileService
    {
        public static FileInfo findFile(string name, string ext)
        {
            var list = new DirectoryInfo("./").GetFiles("*",SearchOption.AllDirectories).Where(b=>b.Name.EndsWith(".gif") || b.Name.EndsWith(".png") || b.Name.EndsWith(".jpg"));
            var file = list.FirstOrDefault(b => b.Name == name + ext);
            if (file!=null)
            {
                return new FileInfo(name,file.DirectoryName,ext);
            }

            var fileName = list.FirstOrDefault(b => b.Name.StartsWith(name+"."));
            if (fileName!=null)
            {
                throw new BadExtensionExcpetion();   
            }

            return null;
        }
        public static string GetFileExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            int lastDotIndex = filePath.LastIndexOf('.');
            if (lastDotIndex == -1 || lastDotIndex == filePath.Length - 1)
                return string.Empty;

            return filePath.Substring(lastDotIndex);
        }
    }
}