using System;
using System.Data.Entity;
using System.Linq;
using System.Security;
using System.Web.Mvc;

using CheatNoteMaker.BusinessLogic.Extensions;
using CheatNoteMaker.BusinessLogic.Security;
using CheatNoteMaker.Models;
using CheatNoteMaker.Models.Common;
using CheatNoteMaker.Models.Users;

namespace CheatNoteMaker.Controllers.CheatNotes
{
    public class BaseController : Controller
    {
        protected void SetInstanceDeleted<T>(Func<CheatNotesContext, IDbSet<T>> getInstances, int id, bool instanceDeleted)
            where T : ObjectBase
        {
            using (var db = new CheatNotesContext())
            {
                var instance = getInstances(db).Get(id);

                if (instance != null)
                {
                    if (instanceDeleted)
                    {
                        instance.DateDeleted = DateTime.Now;
                        instance.DeletedById = GetCurrentUserId();
                    }
                    else
                    {
                        instance.DateDeleted = null;
                        instance.DateModified = DateTime.Now;
                        instance.ModifiedById = GetCurrentUserId();
                    }
                }

                db.Entry(instance).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        protected UserInfo GetCurrentUser()
        {
            return UserPermissionsManager.GetUserInfo(User.Identity.Name);
        }

        protected int? GetCurrentUserId()
        {
            var user = UserPermissionsManager.GetUserInfo(User.Identity.Name);

            return user != null ? (int?)user.Id : null;
        }

        protected UserPermissions GetCurrentUserPermissions()
        {
            return UserPermissionsManager.GetUserPermissions(User.Identity.Name) ?? UserPermissions.GetReaderPermissions();
        }

        protected void ThrowIfNotPermitted(Func<UserPermissions, bool> getOperation)
        {
            if (!IsPermitted(getOperation))
            {
                throw new SecurityException("У Вас нет доступа к этой операции. Обратитесь к администратору.");
            }
        }

        protected bool IsPermitted(Func<UserPermissions, bool> getOperation)
        {
            return UserPermissionsManager.IsPermitted(User.Identity.Name, getOperation);
        }

        protected string CreateUserInfo(string userName)
        {
            return UserPermissionsManager.CreateUserInfo(userName);
        }
    }
}
