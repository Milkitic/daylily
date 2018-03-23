using System;

namespace DaylilyWeb.Functions.Applications
{
    public interface IApplication
    {
        string Execute(string @params, string user, string group);
    }
}
