using System;
using System.Linq.Expressions;
using CheatNoteMaker.Models.CheatNotes;

namespace CheatNoteMaker.Models
{
    public static class CheatNotesContextHelper
    {
        #region Constants (Options)

        private static readonly CheatNotesContextOptions DefaultOptions = new CheatNotesContextOptions(false);

        #endregion

        #region CheatNote conditions

        public static Expression<Func<CheatNote, bool>> GetListConditions(CheatNotesContextOptions options = null)
        {
            var opt = options ?? DefaultOptions;

            return x => (opt.ShowDeletedInstances || !x.DateDeleted.HasValue);
        }

        #endregion

        #region CheatNoteItem conditions

        public static Expression<Func<CheatNoteItem, bool>> GetListConditions(int cnid, CheatNotesContextOptions options = null)
        {
            var opt = options ?? DefaultOptions;

            return x => x.CheatNoteId == cnid
                && (opt.ShowDeletedInstances || !x.DateDeleted.HasValue);
        }

        #endregion
    }
}