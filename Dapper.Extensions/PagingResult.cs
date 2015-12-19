using System.Collections.Generic;

namespace Dapper.Extensions
{
    public class PagingResult<T> where T : class
    {
        public IList<T> List { get; set; }

        public int TotalPages { get; set; }

        public int TotalRecords { get; set; }
    }
}
