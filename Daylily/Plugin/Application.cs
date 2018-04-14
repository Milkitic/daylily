using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Plugin
{
    public abstract class Application
    {
        protected AppType appType;
        protected bool isRoot;

        public abstract string Execute(string @params, string user, string group, bool isRoot, ref bool ifAt);
    }
}
