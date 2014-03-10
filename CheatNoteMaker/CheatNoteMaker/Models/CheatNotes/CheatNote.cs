using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CheatNoteMaker.Models.Common;

namespace CheatNoteMaker.Models.CheatNotes
{
    public class CheatNote : ObjectBase
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        public virtual ICollection<CheatNoteItem> Items { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
