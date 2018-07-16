using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Assist;
using Daylily.Common.Models;
using Daylily.Common.Models.Attributes;
using Daylily.Common.Models.Enum;
using Daylily.Common.Models.Interface;
using Daylily.Common.Utils;

namespace Daylily.Common.Function.Application
{
    [Name("关键词触发")]
    [Author("yf_extension")]
    [Version(0, 0, 1, PluginVersion.Alpha)]
    [Help("收到已给的关键词时，根据已给几率返回一张熊猫图")]
    public class KeywordTrigger : ApplicationApp
    {
        private static readonly string PandaDir = Path.Combine(Domain.CurrentDirectory, "panda");
        private static readonly string[] Me = { "我" };
        private static readonly string[] MeOut = { "me1.jpg" };

        private static readonly string[] You = { "你" };
        private static readonly string[] YouOut = { "you1.jpg", "you2.jpg" };

        private static readonly string[] Why = { "为啥", "为什么", "为毛", "为嘛", "why " };
        private static readonly string[] WhyOut = { "why1.jpg" };

        private static readonly string[] Kanlai = { "看来", "原来" };
        private static readonly string[] KanlaiOut = { "kanlai1.jpg", "kanlai2.jpg" };

        private static readonly string[] Sb = { "黄花菜" };
        private static readonly string[] SbOut = { "sb1.jpg", "sb2.jpg", "sb3.jpg", "sb4.jpg", "sb5.jpg", "sb6.jpg", "sb7.jpg", "sb8.jpg", "sb9.jpg" };

        public override CommonMessageResponse Message_Received(in CommonMessage messageObj)
        {
            //概率从小到大排序，更科学
            string msg = messageObj.Message;
            if (Trig(msg, Me, MeOut, out string img, 0.005)) // 0.05%
                return new CommonMessageResponse(new FileImage(Path.Combine(PandaDir, img)).ToString(), messageObj);
            if (Trig(msg, You, YouOut, out img, 0.02)) // 2%
                return new CommonMessageResponse(new FileImage(Path.Combine(PandaDir, img)).ToString(), messageObj);
            if (Trig(msg, Why, WhyOut, out img, 0.2)) // 20%
                return new CommonMessageResponse(new FileImage(Path.Combine(PandaDir, img)).ToString(), messageObj);
            if (Trig(msg, Kanlai, KanlaiOut, out img, 0.3)) // 30%
                return new CommonMessageResponse(new FileImage(Path.Combine(PandaDir, img)).ToString(), messageObj);
            if (Trig(msg, Sb, SbOut, out img, 0.5)) // 50%
                return new CommonMessageResponse(new FileImage(Path.Combine(PandaDir, img)).ToString(), messageObj);
            return null;
        }

        private static bool Trig(string message, IEnumerable<string> keywords, IReadOnlyList<string> pics,
            out string imgP, double hold = 0.1)
        {
            string msg = message.ToLower();
            if (keywords.Any(msg.Contains))
            {
                imgP = pics[Rnd.Next(pics.Count)];
                return Rnd.NextDouble() < hold;
            }

            imgP = null;
            return false;
        }
    }
}
