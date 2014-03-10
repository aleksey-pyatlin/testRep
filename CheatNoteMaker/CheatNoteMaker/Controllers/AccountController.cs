using System;
using System.Web.Mvc;
using System.Web.Security;

using CheatNoteMaker.Controllers.CheatNotes;
using CheatNoteMaker.Models;
using CheatNoteMaker.OAuth.Vkontakte;
using CheatNoteMaker.OAuth.Vkontakte.DataContracts;

namespace CheatNoteMaker.Controllers
{
    public class AccountController : BaseController
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        public ActionResult VkAuth()
        {
            var vk = new Vkontakte();

            var auth = vk.Authorize(new Uri(string.Format("{0}://{1}/Account/VkLoggedIn", Request.Url.Scheme, Request.Url.Authority)));

            return RedirectToAction("Error", "Home", new { errorId = "VkAuthDenied" });
        }

        public ActionResult VkLoggedIn()
        {
            var vk = new Vkontakte();

            if (vk.UserInfo != null && !string.IsNullOrWhiteSpace(vk.UserInfo.Id))
            {
                return LogonWithVkUser(vk.UserInfo);
            }

            return RedirectToAction("Error", "Home", new { errorId = vk.UserInfo.FullName });
        }

        private ActionResult LogonWithVkUser(VkontakteUserInfo userInfo)
        {
            var userName = string.Format("VK{0}", userInfo.Id);

            var existing = Membership.GetUser(userName);

            if (existing != null)
            {
                // Logon with User
                FormsAuthentication.SetAuthCookie(userName, false);
            }
            else
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(userName, Guid.NewGuid().ToString(), string.Format("id{0}@vk.com", userInfo.Id), null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    var errorMsg = CreateUserInfo(userName);

                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        FormsAuthentication.SetAuthCookie(userName, true /* createPersistentCookie */);

                        return RedirectToAction("Index", "CheatNotes");
                    }

                    Membership.DeleteUser(userName, true);
                }
            }

            return RedirectToAction("Index", "CheatNotes");
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    //Membership.DeleteUser(model.UserName, true);
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "CheatNotes");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный пароль или имя пользователя.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "CheatNotes");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    var errorMsg = CreateUserInfo(model.UserName);
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true /* createPersistentCookie */);
                        return RedirectToAction("Index", "CheatNotes");
                    }

                    Membership.DeleteUser(model.UserName, true);
                    ModelState.AddModelError("", errorMsg);
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Текущий пароль неправильный или новый пароль некорретный.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Пользователь с таким именем уже существует.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Пользователь с таким e-mail адресом уже существует.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Некорректный пароль.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Некорректный адрес e-mail.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Ответ на секретный вопрос неправильный.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Некорректный секретный вопрос.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Некорректное имя пользователя.";

                case MembershipCreateStatus.ProviderError:
                    return "Не удалось войти в систему. Свяжитесь с администратором.";

                case MembershipCreateStatus.UserRejected:
                    return "Отказано в регистрации пользователя.";

                default:
                    return "Неизвестная ошибка. Свяжитесь с администратором.";
            }
        }
        #endregion
    }
}
