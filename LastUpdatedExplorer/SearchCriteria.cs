using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastUpdatedExplorer
{
    [Flags]
    enum SearchCriteria
    {
        None            = 0,
        CreationTime    = 1 << 0,
        LastModified    = 1 << 1,
        LastAccess      = 1 << 2
    }
}
