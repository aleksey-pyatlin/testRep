using CheatNoteMaker.Models.CheatNotes;

namespace CheatNoteMaker.BusinessLogic.HtmlGenerator
{
    public class TaggedCheatNoteItem
    {
        public TaggedCheatNoteItem(CheatNoteItem item, int? partNo)
        {
            Item = item;
            PartNo = partNo;
        }

        public CheatNoteItem Item { get; private set; }

        public int? PartNo { get; set; }
    }
}