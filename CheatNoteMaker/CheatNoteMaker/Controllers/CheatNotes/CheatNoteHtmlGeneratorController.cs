using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using CheatNoteMaker.BusinessLogic.CustomAttributes;
using CheatNoteMaker.BusinessLogic.Extensions;
using CheatNoteMaker.BusinessLogic.HtmlGenerator;
using CheatNoteMaker.Models;
using CheatNoteMaker.Models.CheatNotes;

using Ionic.Zip;

namespace CheatNoteMaker.Controllers.CheatNotes
{
    public class CheatNoteHtmlGeneratorController : BaseController
    {
        private const float FileSizeMultiplier = 2.1f;

        [Authorize]
        [PermissionRequired]
        public ActionResult GenerateHtmlFiles(int id)
        {
            using (var db = new CheatNotesContext())
            {
                var cheatNote = db.CheatNotes.Get(id);

                var model = new GenerateHtmlFilesModel(cheatNote)
                                {
                                    PartSize = 250,
                                    HtmlInfos = GetGeneratedHtmls(db, id)
                                };

                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        [PermissionRequired]
        public ActionResult GenerateHtmlFiles(GenerateHtmlFilesModel model)
        {
            using (var db = new CheatNotesContext())
            {
                var cheatNote = db.CheatNotes.Get(model.CheatNoteId);
                var partSize = !model.PartSize.HasValue || model.PartSize.Value <= 100 ? null : (long?)(model.PartSize * 1024 / FileSizeMultiplier);

                model.HtmlInfos = UpdateCheatNoteHtmls(db, model.CheatNoteId, CheatNoteHtmlGenerator.GenerateHtmlFiles(model.FileName, cheatNote, partSize));
                
                return View(model);
            }
        }
        
        public ActionResult Download(int id)
        {
            using (var db = new CheatNotesContext())
            {
                var html = db.CheatNoteHtmls.FirstOrDefault(x => x.Id == id);

                if (html == null)
                {
                    return null;
                }

                return File(TextToBytes(html.Html), "text/html", html.FileName);
            }
        }

        public ActionResult DownloadZip(int cnid)
        {
            using (var db = new CheatNotesContext())
            {
                var htmls = GetGeneratedHtmls(db, cnid);

                if (!htmls.Any())
                {
                    return null;
                }

                return File(CreateZip(htmls), "application/zip", string.Format("CheatNote_{0}.zip", cnid));
            }
        }

        private static byte[] CreateZip(IEnumerable<CheatNoteGeneratedHtml> htmls)
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    foreach (var html in htmls)
                    {
                        zip.AddEntry(html.FileName, TextToBytes(html.Html));
                    }

                    zip.Save(ms);
                }

                return ms.ToArray();
            }
        }

        private static IEnumerable<CheatNoteGeneratedHtml> UpdateCheatNoteHtmls(CheatNotesContext db, int cheatNoteId, IEnumerable<CheatNoteGeneratedHtml> htmls)
        {
            foreach (var html in htmls)
            {
                html.CheatNoteId = cheatNoteId;
                html.Size = TextToBytes(html.Html).Length;
            }

            var existing = GetGeneratedHtmls(db, cheatNoteId);

            if (existing.Any())
            {
                db.CheatNoteHtmls.RemoveRange(existing);
            }

            var result = db.CheatNoteHtmls.AddRange(htmls);

            db.SaveChanges();

            return result;
        }

        private static IEnumerable<CheatNoteGeneratedHtml> GetGeneratedHtmls(CheatNotesContext db, int cheatNoteId)
        {
            return db.CheatNoteHtmls.Where(html => html.CheatNoteId == cheatNoteId).ToList();
        }

        private static byte[] TextToBytes(string text)
        {
            return new UTF8Encoding().GetBytes(text);
        }

        //private static byte[] TextToBytes(string text)
        //{
        //    if (string.IsNullOrWhiteSpace(text))
        //    {
        //        return new byte[0];
        //    }

        //    using (var stream = new MemoryStream())
        //    {
        //        using (var sw = new StreamWriter(stream, Encoding.UTF8))
        //        {
        //            sw.Write(text);
        //            sw.Close();

        //            return stream.ToArray();
        //        }
        //    }
        //}
    }
}
