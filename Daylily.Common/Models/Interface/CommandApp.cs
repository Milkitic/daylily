using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Utils.LogUtils;

namespace Daylily.Common.Models.Interface
{
    public abstract class CommandApp : AppConstruct
    {
        public sealed override AppType AppType => AppType.Command;
        public string[] Commands { get; internal set; }

        public abstract void Initialize(string[] args);
        public abstract CommonMessageResponse Message_Received(in CommonMessage messageObj);

        protected CommandApp()
        {
            Type t = GetType();
            if (!t.IsDefined(typeof(CommandAttribute), false))
            {
                Logger.Warn($"\"{Name}\"尚未设置命令，因此无法被用户激活。");
            }
            else
            {
                var attrs = (CommandAttribute[])t.GetCustomAttributes(typeof(CommandAttribute), false);
                Commands = attrs[0].Commands;
            }
        }
    }
}
