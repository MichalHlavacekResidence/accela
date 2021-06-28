using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace accela.Extensions
{
    public class Uploader
    {
        private DateTime _timeStamp { get; set; }
        public string FileName { get; set; }
        public string FilePath {get;set;}
        public string Status { get; set; }
        public string AlertStatus { get; set; }
        public string FileType {get;set;}


        public Uploader()
        {
            FileName = "Název souboru";
            Status = "Čekám na upload";
            AlertStatus = "alert-info";
        }

        public Uploader(string filename)
        {
            FileName = filename;
            FilePath = "wwwroot/uploads/" + FileName;
            Status = "Čekám na upload";
            AlertStatus = "alert-info";
        }

        public bool CheckFileExistence(string filename, string type)
        {
            this.FileName = filename;
            string path = "wwwroot/file/uploaded/" + type + "/" + this.FileName;
            bool existenceOfFile = File.Exists(path);
            if (existenceOfFile == true)
            {
                FileName = this.FileName;
                AlertStatus = "alert-success";
                return true;
            }
            else
            {
                Status = "Soubor nebyl nalezen";
                AlertStatus = "alert-danger";
                return false;
            }

        }

        public bool Delete()
        {
            if(this.FileName == null)
            {
                return false;
            }

            string SavePath = null;
            string Folder = null;

            switch(Path.GetExtension(this.FileName))
            {
                case ".pdf":
                    SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/uploaded/pdfs/", FileName);
                    Folder = "pdfs";
                break;

                case ".jpg":
                case ".png":
                case ".gif":
                case ".jpeg":
                    SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/uploaded/images/", FileName);
                    Folder = "images";
                break;

                default: 
                    SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/file/uploaded/unknown/", FileName);
                    Folder = "unknown";
                break;
            }

            this.FilePath = "wwwroot/file/uploaded/" + this.FileName;
            File.Delete(this.FilePath);
            if (this.CheckFileExistence(this.FileName, Folder) == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}