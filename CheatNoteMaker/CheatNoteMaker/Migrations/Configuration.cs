using CheatNoteMaker.Models.Users;

namespace CheatNoteMaker.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : CreateDatabaseIfNotExists<CheatNoteMaker.Models.CheatNotesContext>
    {
        protected override void Seed(CheatNoteMaker.Models.CheatNotesContext context)
        {
            if (context.UserInfos.Any())
            {
                return;
            }

            var predefinedUsers = new[]
                                      {
                                          new UserInfo
                                              {
                                                  UserName = "Admin",
                                                  Permissions = UserPermissions.GetAdminPermissions()
                                              },
                                          new UserInfo
                                              {
                                                  UserName = "Chill",
                                                  Permissions = UserPermissions.GetAdminPermissions()
                                              },
                                          new UserInfo
                                              {
                                                  UserName = "Ksu",
                                                  Permissions = UserPermissions.GetUserPermissions()
                                              },
                                          new UserInfo
                                              {
                                                  UserName = "Nina",
                                                  Permissions = UserPermissions.GetUserPermissions()
                                              }
                                      };

            context.UserInfos.AddOrUpdate(predefinedUsers);
        }
    }
}
