namespace CheatNoteMaker.Models.Users
{
    public class UserPermissions
    {
        #region Generators

        public static UserPermissions GetAdminPermissions()
        {
            return new UserPermissions
            {
                Title = "Admin",
                AllowGenerateCheatNoteHtml = true,
                AllowManageUsers = true,

                AllowCreateCheatNotes = true,
                AllowEditCheatNotes = true,
                AllowDeleteCheatNotes = true,

                AllowCreateCheatNoteItems = true,
                AllowEditCheatNoteItems = true,
                AllowDeleteCheatNoteItems = true
            };
        }

        public static UserPermissions GetReaderPermissions()
        {
            return new UserPermissions
            {
                Title = "Reader",
                AllowGenerateCheatNoteHtml = false,
                AllowManageUsers = false,

                AllowCreateCheatNotes = false,
                AllowEditCheatNotes = false,
                AllowDeleteCheatNotes = false,

                AllowCreateCheatNoteItems = false,
                AllowEditCheatNoteItems = false,
                AllowDeleteCheatNoteItems = false
            };
        }

        public static UserPermissions GetUserPermissions()
        {
            return new UserPermissions
            {
                Title = "User",
                AllowGenerateCheatNoteHtml = false,
                AllowManageUsers = false,

                AllowCreateCheatNotes = true,
                AllowEditCheatNotes = true,
                AllowDeleteCheatNotes = true,

                AllowCreateCheatNoteItems = true,
                AllowEditCheatNoteItems = true,
                AllowDeleteCheatNoteItems = true
            };
        }

        public static UserPermissions GetDefaultPermissions()
        {
            return GetReaderPermissions();
        }

        #endregion

        public int Id { get; set; }

        public string Title { get; set; }
        //public virtual ICollection<UserInfo> Users { get; set; }

        public bool AllowCreateCheatNotes { get; set; }
        public bool AllowEditCheatNotes { get; set; }
        public bool AllowDeleteCheatNotes { get; set; }

        public bool AllowCreateCheatNoteItems { get; set; }
        public bool AllowEditCheatNoteItems { get; set; }
        public bool AllowDeleteCheatNoteItems { get; set; }

        public bool AllowGenerateCheatNoteHtml { get; set; }

        public bool AllowManageUsers { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public void UpdateData(UserPermissions value)
        {
            if (value == null)
            {
                return;
            }

            AllowCreateCheatNotes = value.AllowCreateCheatNotes;
            AllowEditCheatNotes = value.AllowEditCheatNotes;
            AllowDeleteCheatNotes = value.AllowDeleteCheatNotes;

            AllowCreateCheatNoteItems = value.AllowCreateCheatNoteItems;
            AllowEditCheatNoteItems = value.AllowEditCheatNoteItems;
            AllowDeleteCheatNoteItems = value.AllowDeleteCheatNoteItems;

            AllowGenerateCheatNoteHtml = value.AllowGenerateCheatNoteHtml;

            AllowManageUsers = value.AllowManageUsers;
        }
    }
}