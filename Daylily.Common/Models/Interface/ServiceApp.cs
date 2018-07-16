using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Interface
{
    public abstract class ServiceApp : AppConstruct
    {
        public sealed override AppType AppType => AppType.Service;
        public abstract void RunTask(string[] args);
    }
}
