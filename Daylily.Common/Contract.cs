using System;
using System.Collections.Generic;
using System.Text;

namespace Daylily.Common
{
    public static class Contract
    {
        public static void Requires<TException>(bool predicate) where TException : Exception, new()
        {
            if (!predicate)
                throw new TException();
        }
    }
}
