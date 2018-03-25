using System;

namespace Daylily.Plugin
{
    internal interface IApplication
    {
        string Execute(string @params, string user, string group, bool isRoot);
    }
}
