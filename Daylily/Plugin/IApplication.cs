using System;

namespace Daylily.Plugin
{
    public interface IApplication
    {
        string Execute(string @params, string user, string group, bool isRoot);
    }
}
