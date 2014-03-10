using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using CheatNoteMaker.Models.Common;

namespace CheatNoteMaker.BusinessLogic.Extensions
{
    public static class IQueryableExtensions
    {
        internal static IQueryable<T> IncludeBase<T>(this IQueryable<T> obj) where T : ObjectBase
        {
            return obj.Include(x => x.CreatedBy).Include(x => x.ModifiedBy).Include(x => x.DeletedBy);
        }

        public static T Get<T>(this IQueryable<T> obj, int id) where T : ObjectBase
        {
            return obj.IncludeBase().Single(x => x.Id == id);
        }

        public static IList<T> List<T>(this IQueryable<T> obj, Func<IQueryable<T>,IQueryable<T>> additionalOperation = null) where T : ObjectBase
        {
            var list = obj.IncludeBase();

            if (additionalOperation != null)
            {
                list = additionalOperation(list);
            }

            return list.ToList();
        }
    }
}