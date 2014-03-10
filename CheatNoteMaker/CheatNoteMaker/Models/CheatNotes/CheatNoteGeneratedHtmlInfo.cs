using System;
using System.ComponentModel.DataAnnotations;

namespace CheatNoteMaker.Models.CheatNotes
{
    public class CheatNoteGeneratedHtmlInfo
    {
        public CheatNoteGeneratedHtmlInfo()
        {
            DateCreated = DateTime.Now;
        }

        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        public int CheatNoteId { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public long Size { get; set; }
    }
}