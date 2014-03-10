using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CheatNoteMaker.Models.Common;

namespace CheatNoteMaker.Models.CheatNotes
{
    public class CheatNoteItem : ObjectBase
    {
        [Required]
        public int CheatNoteId { get; set; }

        [Required]
        [Display(Name = "Номер вопроса")]
        public int Position { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Текст вопроса")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Текст ответа")]
        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        public virtual new string ToString()
        {
            return string.Format("{0}. {1}", Position, Title);
        }
    }
}
