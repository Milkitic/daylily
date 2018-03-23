using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Functions.Applications
{
    public interface IApplication
    {
        string Execute(string @params);
    }
}
