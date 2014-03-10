using System;
using System.Web.Mvc;
using System.Web.Routing;

using CheatNoteMaker.BusinessLogic.Security;
using CheatNoteMaker.Models.Users;

namespace CheatNoteMaker.BusinessLogic.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public sealed class PermissionRequiredAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var action = filterContext.ActionDescriptor.ActionName;
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var userName = filterContext.HttpContext.User.Identity.Name;

            var operation = GetOperation(action, controller);

            if (operation == null || !UserPermissionsManager.IsPermitted(userName, operation))
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary
                            {
                                { "action", "Error" },
                                { "controller", "Home" },
                                { "errorId", "PermissionDenied"}
                            });
                //throw new UnauthorizedAccessException();
            }
        }

        private static Func<UserPermissions, bool> GetOperation(string action, string controller)
        {
            if (action.Equals("Create"))
            {
                if (controller.Equals("CheatNotes"))
                {
                    return x => x.AllowCreateCheatNotes;
                }
                if (controller.Equals("CheatNoteItems"))
                {
                    return x => x.AllowCreateCheatNoteItems;
                }
            }

            if (action.Equals("Edit"))
            {
                if (controller.Equals("CheatNotes"))
                {
                    return x => x.AllowEditCheatNotes;
                }
                if (controller.Equals("CheatNoteItems"))
                {
                    return x => x.AllowEditCheatNoteItems;
                }
            }

            if (action.Equals("Delete") || action.Equals("Restore"))
            {
                if (controller.Equals("CheatNotes"))
                {
                    return x => x.AllowDeleteCheatNotes;
                }
                if (controller.Equals("CheatNoteItems"))
                {
                    return x => x.AllowDeleteCheatNoteItems;
                }
            }

            if (controller.Equals("UserManagement"))
            {
                return x => x.AllowManageUsers;
            }

            if (controller.Equals("CheatNoteHtmlGenerator"))
            {
                return x => x.AllowGenerateCheatNoteHtml;
            }

            return null;
        }
    }
}