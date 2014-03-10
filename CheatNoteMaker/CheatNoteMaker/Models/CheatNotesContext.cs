using System.Data.Entity;
using CheatNoteMaker.Models.CheatNotes;
using CheatNoteMaker.Models.Users;

namespace CheatNoteMaker.Models
{
    public class CheatNotesContext : DbContext
    {
        public DbSet<CheatNote> CheatNotes { get; set; }
        public DbSet<CheatNoteItem> CheatNoteItems { get; set; }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<UserPermissions> UserPermissions { get; set; }

        public DbSet<CheatNoteGeneratedHtml> CheatNoteHtmls { get; set; }
    }
}