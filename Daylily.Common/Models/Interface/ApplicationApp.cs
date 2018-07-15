using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Interface
{
    public abstract class ApplicationApp : AppConstruct
    {
        public sealed override AppType AppType => AppType.Application;

        public abstract CommonMessageResponse OnExecute(in CommonMessage messageObj);

    }
}
