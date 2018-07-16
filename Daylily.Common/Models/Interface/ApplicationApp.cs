using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Interface
{
    public abstract class ApplicationApp : AppConstruct
    {
        public sealed override AppType AppType => AppType.Application;

        public abstract CommonMessageResponse Message_Received(in CommonMessage messageObj);
    }
}
