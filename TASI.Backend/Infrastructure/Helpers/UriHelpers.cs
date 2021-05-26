using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TASI.Backend.Infrastructure.Helpers
{
    public static class UriHelpers
    {
        public static string ToQueryString(this IDictionary<string, string> dict)
        {
            return dict.Aggregate("?", (s, pair) => $"{s}{HttpUtility.UrlEncode(pair.Key)}={HttpUtility.UrlEncode(pair.Value)}&")[..^1];
        }
    }
}
