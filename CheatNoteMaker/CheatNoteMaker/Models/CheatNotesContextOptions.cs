namespace CheatNoteMaker.Models
{
    public class CheatNotesContextOptions
    {
        public CheatNotesContextOptions(bool showDeleted)
        {
            ShowDeletedInstances = showDeleted;
        }

        public bool ShowDeletedInstances { get; set; }
    }
}