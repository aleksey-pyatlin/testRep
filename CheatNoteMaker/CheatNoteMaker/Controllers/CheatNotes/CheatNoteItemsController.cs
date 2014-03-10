using System;
using System.Linq;
using System.Web.Mvc;

using CheatNoteMaker.BusinessLogic.CustomAttributes;
using CheatNoteMaker.Models;
using CheatNoteMaker.Models.CheatNotes;
using CheatNoteMaker.BusinessLogic.Extensions;

namespace CheatNoteMaker.Controllers.CheatNotes
{
    public class CheatNoteItemsController : BaseController
    {
        //
        // GET: /CheatNoteItems/List
        [Authorize]
        public ActionResult List(int? cnid, bool? showDeleted)
        {
            if (!cnid.HasValue)
            {
                return RedirectToAction("Error", "Home");
            }

            using (var db = new CheatNotesContext())
            {
                if (!db.CheatNotes.Any(x => x.Id == cnid.Value))
                {
                    return RedirectToAction("Error", "Home");
                }

                ViewBag.CreatePermitted = IsPermitted(o => o.AllowCreateCheatNoteItems);
                ViewBag.EditPermitted = IsPermitted(o => o.AllowEditCheatNotes);

                return View(db.CheatNoteItems.List( l => l.Where(CheatNotesContextHelper.GetListConditions(cnid.Value, new CheatNotesContextOptions(showDeleted ?? false))).OrderBy(x => x.Position)));
            }
        }

        //
        // GET: /CheatNoteItems/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            using (var db = new CheatNotesContext())
            {
                ViewBag.EditPermitted = IsPermitted(o => o.AllowEditCheatNoteItems);
                ViewBag.DeletePermitted = IsPermitted(o => o.AllowDeleteCheatNoteItems);

                return View(db.CheatNoteItems.Get(id));
            }
        }

        //
        // GET: /CheatNoteItems/Create
        [Authorize]
        [PermissionRequired]
        public ActionResult Create(int cnid)
        {
            return View(new CheatNoteItem
                            {
                                CheatNoteId = cnid,
                                Position = GetDefaultItemPosition(cnid)
                            });
        } 

        //
        // POST: /CheatNoteItems/Create

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        [ValidateInput(false)]
        public ActionResult Create(CheatNoteItem cheatNoteItem)
        {
            try
            {
                using (var db = new CheatNotesContext())
                {
                    cheatNoteItem.CreatedById = GetCurrentUserId();

                    cheatNoteItem.Title = cheatNoteItem.Title.AntiCodeInjection();
                    cheatNoteItem.Content = cheatNoteItem.Content.AntiCodeInjection();

                    db.CheatNoteItems.Add(cheatNoteItem);
                    db.SaveChanges();
                }

                return RedirectToItemList(cheatNoteItem.CheatNoteId, cheatNoteItem.Position);
                //return RedirectToAction("List", new { cnid = cheatNoteItem.CheatNoteId });
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /CheatNoteItems/Edit/5

        [Authorize]
        [PermissionRequired]
        public ActionResult Edit(int id)
        {
            using (var db = new CheatNotesContext())
            {
                var item = db.CheatNoteItems.Get(id);

                var nextItem = db.CheatNoteItems.OrderBy(x => x.Position).FirstOrDefault(x => x.CheatNoteId == item.CheatNoteId && x.Position > item.Position);

                ViewBag.NextItemId = nextItem == null ? null : (int?)nextItem.Id;

                return View(item);
            }
        }

        //
        // POST: /CheatNoteItems/Edit/5

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        [ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "Edit")]
        public ActionResult Edit(int id, CheatNoteItem cheatNoteItem, int? nextItemId)
        {
            return Save(id, cheatNoteItem, null);
        }

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        [ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "Next")]
        public ActionResult Next(int id, CheatNoteItem cheatNoteItem, int? nextItemId)
        {
            return Save(id, cheatNoteItem, nextItemId);
        }

        private ActionResult Save(int id, CheatNoteItem cheatNoteItem, int? nextItemId)
        {
            try
            {
                using (var db = new CheatNotesContext())
                {
                    var instance = db.CheatNoteItems.Get(id);

                    instance.Position = cheatNoteItem.Position;
                    instance.Title = cheatNoteItem.Title.AntiCodeInjection();
                    instance.Content = cheatNoteItem.Content.AntiCodeInjection();
                    instance.DateModified = DateTime.Now;
                    instance.ModifiedById = GetCurrentUserId();

                    db.SaveChanges();
                }

                return nextItemId.HasValue
                    ? RedirectToAction("Edit", new { id = nextItemId.Value })
                    : RedirectToItemList(cheatNoteItem.CheatNoteId, cheatNoteItem.Position);
                //return RedirectToAction("List", new { cnid = cheatNoteItem.CheatNoteId });
            }
            catch
            {
                return View();
            }
        }

        //
        // POST: /CheatNoteItems/Delete/5

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult Delete(int id, CheatNoteItem cheatNoteItem)
        {
            return SetInstanceDeleted(id, cheatNoteItem);
        }

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult Restore(int id, CheatNoteItem cheatNoteItem)
        {
            return SetInstanceDeleted(id, cheatNoteItem, false);
        }

        [Authorize]
        public ActionResult CheatNoteInfo(int cnid, bool? toDetails)
        {
            using (var db = new CheatNotesContext())
            {
                return PartialView(toDetails.HasValue && toDetails.Value ? "_CheatNoteInfoToDetailsControl" : "_CheatNoteInfoToListControl", db.CheatNotes.Find(cnid));
            }
        }

        #region Private Helpers

        private ActionResult SetInstanceDeleted(int id, CheatNoteItem cheatNoteItem, bool isDeleted = true)
        {
            try
            {
                SetInstanceDeleted(db => db.CheatNoteItems, id, isDeleted);

                return RedirectToItemList(cheatNoteItem.CheatNoteId, cheatNoteItem.Position);
                //return RedirectToAction("List", new { cnid = cheatNoteItem.CheatNoteId });
            }
            catch
            {
                return RedirectToItemList(cheatNoteItem.CheatNoteId, cheatNoteItem.Position);
                //return RedirectToAction("List", new { cnid = cheatNoteItem.CheatNoteId });
            }
        }

        private static int GetDefaultItemPosition(int cnid)
        {
            using (var db = new CheatNotesContext())
            {
                var maxPos = db.CheatNoteItems.Where(x => !x.DateDeleted.HasValue && x.CheatNoteId == cnid).Max(x => (int?)x.Position);

                return (maxPos.HasValue && maxPos.Value > 0) ? maxPos.Value + 1 : 1;
            }
        }

        private ActionResult RedirectToItemList(int cnid, int? position)
        {
            var positionAnchor = position.HasValue
                                     ? string.Format("#{0}", position.Value)
                                     : string.Empty;

            //return RedirectToAction("List", new { cnid });
            return new RedirectResult(Url.Action("List", new { cnid }) + positionAnchor);
        }

        #endregion
    }
}
