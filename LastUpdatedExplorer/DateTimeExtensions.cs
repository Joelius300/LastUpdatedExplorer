using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastUpdatedExplorer
{
    public static class DateTimeExtensions
    {
        public static bool Between(this DateTime input, DateTime inclStart, DateTime exclEnd)
        {
            return (input >= inclStart && input < exclEnd);
        }
    }
}
