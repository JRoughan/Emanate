using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emanate.Api
{
    public static class EnumerableExtensions
    {
        public static T SingleOr404<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var item = items.SingleOrDefault(predicate);
            if (item == null)
                throw new HttpException(404, "Item does not exist");
            return item;
        }
    }
}