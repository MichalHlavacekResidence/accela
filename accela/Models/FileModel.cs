using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace UploadMultipleFilesInMVC.Models
{
    public class FileModel
    {
        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBaseModelBinder[] files { get; set; }
       // public HttpPostedFileBase[] files { get; set; }

    }
}