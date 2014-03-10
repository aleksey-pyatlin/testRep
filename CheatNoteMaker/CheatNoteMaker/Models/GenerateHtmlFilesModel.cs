using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using CheatNoteMaker.Models.CheatNotes;

namespace CheatNoteMaker.Models
{
    public class GenerateHtmlFilesModel
    {
        public GenerateHtmlFilesModel(){}

        public GenerateHtmlFilesModel(CheatNote cheatNote)
        {
            CheatNoteId = cheatNote.Id;
            CheatNoteName = cheatNote.Name;
            CheatNoteItemsTotal = cheatNote.Items.Count;
        }


        public int CheatNoteId { get; set; }

        [Display(Name = "Название")]
        public string CheatNoteName { get; set; }

        [Display(Name = "Количество вопросов")]
        public int CheatNoteItemsTotal { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Имя файла(ов)")]
        public string FileName { get; set; }

        [Display(Name = "Максимальный размер файла (КБ)")]
        public long? PartSize { get; set; }

        public IEnumerable<CheatNoteGeneratedHtmlInfo> HtmlInfos { get; set; }
    }
}