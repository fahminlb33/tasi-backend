using System;

namespace TASI.Backend.Infrastructure.Database
{
    public interface IDaoEntity
    {
        DateTime ModifiedDate { get; set; }
    }
}
