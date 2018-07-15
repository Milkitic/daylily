using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Interface
{
    public abstract class ServiceApp : AppConstruct
    {
        public sealed override AppType AppType => AppType.Service;
    }
}
