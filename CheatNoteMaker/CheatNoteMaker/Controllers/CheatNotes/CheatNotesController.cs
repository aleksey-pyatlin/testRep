using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using CheatNoteMaker.BusinessLogic.CustomAttributes;
using CheatNoteMaker.BusinessLogic.Extensions;
using CheatNoteMaker.Models;
using CheatNoteMaker.Models.CheatNotes;

namespace CheatNoteMaker.Controllers.CheatNotes
{
    public class CheatNotesController : BaseController
    {
        //
        // GET: /CheatNote/
        [Authorize]
        public ActionResult Index(bool? showDeleted)
        {
            using (var db = new CheatNotesContext())
            {
                ViewBag.CreatePermitted = IsPermitted(o => o.AllowCreateCheatNotes);
                //return View(db.CheatNotes.IncludeBase().Where(CheatNotesContextHelper.GetListConditions(new CheatNotesContextOptions(showDeleted ?? false))).OrderByDescending(x => x.DateModified).ToList());
                return View(db.CheatNotes.List(l => l.Where(CheatNotesContextHelper.GetListConditions(new CheatNotesContextOptions(showDeleted ?? false))).OrderByDescending(x => x.DateModified)));
            }
        }

        //
        // GET: /CheatNote/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            ViewBag.GeneratorPermitted = IsPermitted(p => p.AllowGenerateCheatNoteHtml);
            ViewBag.EditPermitted = IsPermitted(o => o.AllowEditCheatNotes);
            ViewBag.DeletePermitted = IsPermitted(o => o.AllowDeleteCheatNotes);

            using (var db = new CheatNotesContext())
            {
                return View(db.CheatNotes.Get(id));
            }
        }

        //
        // GET: /CheatNote/Create
        [Authorize]
        [PermissionRequired]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /CheatNote/Create

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult Create(CheatNote cheatNote, string questionsPlainText)
        {
            try
            {
                var cheatNoteItems = ParseCheatNoteItems(questionsPlainText);

                if (cheatNoteItems != null && cheatNoteItems.Count > 0)
                {
                    foreach (var item in cheatNoteItems)
                    {
                        item.CreatedById = GetCurrentUserId();
                    }

                    cheatNote.Items = cheatNoteItems;
                }

                using (var db = new CheatNotesContext())
                {
                    cheatNote.CreatedById = GetCurrentUserId();

                    db.CheatNotes.Add(cheatNote);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View(cheatNote);
            }
        }

        [Authorize]
        [PermissionRequired]
        public ActionResult Edit(int id)
        {
            using (var db = new CheatNotesContext())
            {
                return View(db.CheatNotes.Get(id));
            }
        }

        //
        // POST: /CheatNote/Edit/5

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult Edit(int id, CheatNote cheatNote)
        {
            try
            {
                using (var db = new CheatNotesContext())
                {
                    var instance = db.CheatNotes.Get(id);

                    if (instance != null)
                    {
                        instance.Name = cheatNote.Name;
                        instance.Description = cheatNote.Description;
                        instance.DateModified = DateTime.Now;
                        instance.ModifiedById = GetCurrentUserId();
                    }

                    db.Entry(instance).State = EntityState.Modified;
                    db.SaveChanges();
                }
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // POST: /CheatNote/Delete/5

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult Delete(int id)
        {
            return SetInstanceDeleted(id);
        }

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult Restore(int id)
        {
            return SetInstanceDeleted(id, false);
        }

        #region Private Helpers

        private ActionResult SetInstanceDeleted(int id, bool isDeleted = true)
        {
            try
            {
                SetInstanceDeleted(db => db.CheatNotes, id, isDeleted);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        private static ICollection<CheatNoteItem> ParseCheatNoteItems(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0 || !file.ContentType.Equals("text/plain"))
            {
                return null;
            }

            using (var sr = new StreamReader(file.InputStream, Encoding.Default))
            {
                return ParseCheatNoteItems(sr.ReadToEnd());
            }
        }

        private static ICollection<CheatNoteItem> ParseCheatNoteItems(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            var lines = content.Replace("\r", string.Empty).Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (!lines.Any())
            {
                return null;
            }

            var items = new List<CheatNoteItem>();

            var globalNumber = 1;

            var splittingChars = new[] { ' ', '.', ',', ':', ';', '/', '?', ':', '#', '№', '!', '*', '^', '%', '$', '-', '=', ')' };

            foreach (var line in lines)
            {
                var numberAndText = line.TrimStart(splittingChars).Trim().Split(splittingChars, 2, StringSplitOptions.RemoveEmptyEntries);

                var number = 0;
                var numberParsed = false;
                var text = string.Empty;

                if (numberAndText.Length > 1)
                {
                    numberParsed = int.TryParse(numberAndText[0], out number);

                    text = numberParsed ? numberAndText[1].Trim() : line.Trim();
                }
                else if (numberAndText.Length > 0)
                {
                    text = line.Trim();
                }

                items.Add(new CheatNoteItem
                            {
                                Position = numberParsed ? number : globalNumber,
                                Title = text
                            });

                globalNumber++;
            }

            return items;
        }

        #endregion
    }
}
