namespace CheatNoteMaker.Models.Users
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public virtual UserPermissions Permissions { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}