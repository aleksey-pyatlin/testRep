using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheatNoteMaker.Models.CheatNotes
{
    public class CheatNoteGeneratedHtml : CheatNoteGeneratedHtmlInfo
    {
        [Required]
        [Column(TypeName = "ntext")]
        public string Html { get; set; }
    }
}