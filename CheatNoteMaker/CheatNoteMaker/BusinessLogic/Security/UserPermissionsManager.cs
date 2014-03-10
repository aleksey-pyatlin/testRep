using System;
using System.Data.Entity;
using System.Linq;

using CheatNoteMaker.Models;
using CheatNoteMaker.Models.Users;

namespace CheatNoteMaker.BusinessLogic.Security
{
    public class UserPermissionsManager
    {
        public static UserInfo GetUserInfo(string userName)
        {
            using (var db = new CheatNotesContext())
            {
                var userInfo = db.UserInfos.Include(x => x.Permissions).FirstOrDefault(x => x.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

                return userInfo;
            }
        }

        public static UserPermissions GetUserPermissions(string userName)
        {
            var userInfo = GetUserInfo(userName);

            return userInfo != null && userInfo.Permissions != null ? userInfo.Permissions : UserPermissions.GetDefaultPermissions();
        }

        public static bool IsPermitted(string userName, Func<UserPermissions, bool> getOperation)
        {
            return getOperation(GetUserPermissions(userName));
        }

        public static string CreateUserInfo(string userName)
        {
            try
            {
                using (var db = new CheatNotesContext())
                {
                    if (db.UserInfos.Any(x => x.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return null;
                    }

                    var userInfo = new UserInfo
                    {
                        UserName = userName,
                        Permissions = UserPermissions.GetDefaultPermissions()
                    };

                    db.UserInfos.Add(userInfo);
                    db.SaveChanges();
                }

                return null;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}