using DaylilyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Function.Application
{
    public abstract class Application
    {
        protected PermissionLevel CurrentLevel { get; set; }

        public Random rnd = new Random();

        public abstract string Execute(string @params, string user, string group, PermissionLevel currentLevel, ref bool ifAt, long messageId);

    }
}
