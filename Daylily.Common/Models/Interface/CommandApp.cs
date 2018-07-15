using System;
using System.Collections.Generic;
using System.Text;
using Daylily.Common.Models.Enum;

namespace Daylily.Common.Models.Interface
{
    public abstract class CommandApp : AppConstruct
    {
        public sealed override AppType AppType => AppType.Command;
        public abstract string Command { get; }

        public abstract CommonMessageResponse OnExecute(in CommonMessage messageObj);

    }
}
