using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CheatNoteMaker.BusinessLogic.CustomAttributes;
using CheatNoteMaker.Controllers.CheatNotes;
using CheatNoteMaker.Models;
using CheatNoteMaker.Models.Users;
using System.Data.Entity;

namespace CheatNoteMaker.Controllers
{
    public class UserManagementController : BaseController
    {
        //
        // GET: /UserManagement/
        [Authorize]
        [PermissionRequired]
        public ActionResult Index()
        {
            using (var db = new CheatNotesContext())
            {
                ViewBag.CurrentUserId = GetCurrentUserId();

                return View(db.UserInfos.Include(x => x.Permissions).ToList());
            }
        }
        
        ////
        //// GET: /UserManagement/Create

        //public ActionResult Create()
        //{
        //    return View();
        //} 

        ////
        //// POST: /UserManagement/Create

        //[HttpPost]
        //public ActionResult Create(UserInfo userInfo)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //public ActionResult UserInfoPermissionsRow(int id)
        //{
        //    try
        //    {
        //        //var userPermissions = user.Permissions;
        //        using (var db = new CheatNotesContext())
        //        {
        //            var userInfo = db.UserInfos.Include(x => x.Permissions).FirstOrDefault(u => u.Id == id);

        //            return PartialView(userInfo);
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult Update(UserInfo userInfo)
        {
            try
            {
                var userPermissions = userInfo.Permissions;

                if (userInfo.Id == this.GetCurrentUserId())
                {
                    userPermissions.AllowManageUsers = true;
                }

                using (var db = new CheatNotesContext())
                {
                    var permissions = db.UserPermissions.FirstOrDefault(p => p.Id == userPermissions.Id);

                    if (permissions != null)
                    {
                        permissions.UpdateData(userPermissions);

                        db.Entry(permissions).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        ////
        //// GET: /UserManagement/Delete/5

        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /UserManagement/Delete/5

        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
