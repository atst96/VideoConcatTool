using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoConcatTool
{
    internal static class Extensions
    {
        public static IEnumerable<T> ExcludeDefault<T>(this IEnumerable<T> collection)
        {
            return collection.Where(item => item != default);
        }
    }
}
