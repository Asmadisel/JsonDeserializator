using System.ComponentModel.DataAnnotations;

namespace JsonDeserializator.Models
{
    public class SingleFileModel : ReponseModel
    {
        [Required(ErrorMessage = "Please enter file name")]
        public string FileName { get; set; }
        [Required(ErrorMessage = "Please select file")]
        public IFormFile File { get; set; }
    }
}
