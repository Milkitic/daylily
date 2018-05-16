using Daylily.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Web.Function.Application
{
    public class Test : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage commonMessage)
        {
            var abc = CQApi.GetGroupList();
            return null;
        }
    }
}
