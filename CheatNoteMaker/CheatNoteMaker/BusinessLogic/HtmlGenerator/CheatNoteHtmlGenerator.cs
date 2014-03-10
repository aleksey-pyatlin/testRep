using System.Collections.Generic;
using System.Linq;
using System.Text;

using CheatNoteMaker.Models.CheatNotes;

namespace CheatNoteMaker.BusinessLogic.HtmlGenerator
{
    public static class CheatNoteHtmlGenerator
    {
        private static readonly string AnchorPattern = "cni{0}";

        private static readonly string LinkToTop = "<br/><p><a href=\"#top\">[ наверх ]</a></p>";

        public static IEnumerable<CheatNoteGeneratedHtml> GenerateHtmlFiles(string fileName, CheatNote cheatNote, long? partSize)
        {
            if (string.IsNullOrWhiteSpace(fileName) || cheatNote == null || cheatNote.Items == null)
            {
                return Enumerable.Empty<CheatNoteGeneratedHtml>();
            }

            var itemsTree = SplitCheatNoteItems(cheatNote.Items.Where(x => !x.DateDeleted.HasValue).OrderBy(x => x.Position), partSize);

            return itemsTree.Any() ? GenerateHtmlCollection(fileName, cheatNote.Name, itemsTree) : Enumerable.Empty<CheatNoteGeneratedHtml>();
        }

        private static IEnumerable<CheatNoteGeneratedHtml> GenerateHtmlCollection(string fileName, string title, IList<IList<TaggedCheatNoteItem>> itemTree)
        {
            var tableOfContents = GenerateTableOfContents(fileName, itemTree);

            if (string.IsNullOrWhiteSpace(tableOfContents))
            {
                return Enumerable.Empty<CheatNoteGeneratedHtml>();
            }

            var result = new List<CheatNoteGeneratedHtml>();

            foreach (var part in itemTree)
            {
                var html = WrapToHtml(title, tableOfContents, GenerateContent(part));

                var partNo = part.First().PartNo;

                var partFileName = partNo.HasValue ? GetPartFileName(fileName, partNo) : string.Format("{0}.html", fileName);

                result.Add(new CheatNoteGeneratedHtml
                               {
                                   FileName = partFileName,
                                   Size = html.Length,
                                   Html = html
                               });
            }

            return result;
        }

        private static string GenerateContent(IEnumerable<TaggedCheatNoteItem> items)
        {
            if (items == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var item in items)
            {
                sb.Append(GenerateContentItemHtml(item));
            }

            return sb.ToString();
        }

        

        private static string GenerateTableOfContents(string fileName, IList<IList<TaggedCheatNoteItem>> itemTree)
        {
            if (itemTree == null)
            {
                return string.Empty;
            }

            var singlePart = itemTree.Count <= 1;
            var sb = new StringBuilder();

            for (var i = 0; i < itemTree.Count; i++)
            {
                foreach (var item in itemTree[i])
                {
                    sb.Append(GenerateTableOfContentsItemHtml(item, fileName, singlePart ? null : (int?)i));
                }
            }

            return sb.ToString();
        }

        private static string GenerateTableOfContentsItemHtml(TaggedCheatNoteItem item, string fileName, int? partNo)
        {
            if (item == null)
            {
                return string.Empty;
            }

            // EXAMPLE:
            // <p><a href="{fileName}{partNo}.html#{anchor}">{Position}. {Title}</a></p>
            var anchor = GetAnchor(item.Item.Position);
            var href = string.Format("{0}#{1}", GetPartFileName(fileName, partNo), anchor);

            return string.Format("<p><a href=\"{0}\">{1}. {2}</a></p>", href, item.Item.Position, item.Item.Title);
        }

        private static string GetPartFileName(string fileName, int? partNo)
        {
            return partNo.HasValue ? string.Format("{0}{1}.html", fileName, partNo.Value) : string.Empty;
        }

        private static string GetAnchor(int itemPosition)
        {
            return string.Format(AnchorPattern, itemPosition);
        }

        private static string GenerateContentItemHtml(TaggedCheatNoteItem item)
        {
            // EXAMPLE:
            // <br/><p><a href="#top">[наверх]</a></p><h1><a name="{anchor}">{Position}. {Title}</h1><p>{Content}</p>
            var anchor = GetAnchor(item.Item.Position);

            return string.Format("{0}<h3><a name=\"{1}\">{2}. {3}</h3><p>{4}</p>", LinkToTop, anchor, item.Item.Position, item.Item.Title, item.Item.Content);
        }

        private static string WrapToHtml(string title, string tableOfContents, string content)
        {
            return string.Format(
                        "<html>"
                        +   "<head>"
                        +       "<meta http-equiv=Content-Type content=\"text/html; charset=UTF-8\" />"
                        +       "<title>{0}</title>"
                        +   "</head>"
                        +   "<body>"
                        +       "<div>{1}</div>"
                        +       "<div>{2}</div>"
                        +       LinkToTop
                        +   "</body>"
                        + "</html>", title, tableOfContents, content);
        }

        private static IList<IList<TaggedCheatNoteItem>> SplitCheatNoteItems(IEnumerable<CheatNoteItem> items, long? partLength)
        {
            var list = new List<IList<TaggedCheatNoteItem>>();

            if (items == null)
            {
                return list;
            }

            list.Add(new List<TaggedCheatNoteItem>());

            foreach (var item in items)
            {
                var currentCollection = list.Last();

                if (partLength.HasValue && currentCollection.GetLength() + item.GetLength() >= partLength.Value && currentCollection.Any())
                {
                    list.Add(new List<TaggedCheatNoteItem>());
                    currentCollection = list.Last();
                }

                currentCollection.Add(new TaggedCheatNoteItem(item, partLength.HasValue ? (int?)list.Count - 1 : null));
            }

            if (list.Count == 1)
            {
                foreach (var i in list.SelectMany(item => item))
                {
                    i.PartNo = null;
                }
            }

            return list;
        }
    }
}