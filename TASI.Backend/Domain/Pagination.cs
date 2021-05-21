using System;
using System.Collections.Generic;

namespace TASI.Backend.Domain
{
    public record Pagination<T>(int Page, int Size, int TotalRecords, IList<T> Data)
    {
        public int TotalPages => (int) Math.Ceiling(TotalRecords / (double) Size);

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages - 1;
    }
}
