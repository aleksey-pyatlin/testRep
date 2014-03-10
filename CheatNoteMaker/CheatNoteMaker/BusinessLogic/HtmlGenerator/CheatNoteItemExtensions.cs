using System.Collections.Generic;
using System.Linq;

using CheatNoteMaker.Models.CheatNotes;

namespace CheatNoteMaker.BusinessLogic.HtmlGenerator
{
    internal static class CheatNoteItemExtensions
    {
        public static long GetLength(this CheatNoteItem item)
        {
            if (item == null)
            {
                return 0;
            }

            var length = 0;

            if (!string.IsNullOrEmpty(item.Title))
            {
                length += item.Title.Length;
            }

            if (!string.IsNullOrEmpty(item.Content))
            {
                length += item.Content.Length;
            }

            return length;
        }

        public static long GetLength(this IEnumerable<CheatNoteItem> items)
        {
            return items == null ? 0 : items.Sum(item => item.GetLength());
        }

        public static long GetLength(this IEnumerable<TaggedCheatNoteItem> items)
        {
            return items == null ? 0 : items.Sum(item => item.Item.GetLength());
        }
    }
}