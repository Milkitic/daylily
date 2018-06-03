using Daylily.Common.Interface.CQHttp;
using Daylily.Common.Models;
using Daylily.Common.Models.Interface;

namespace Daylily.Web.Function.Application
{
    public class Test : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage commonMessage)
        {
            var abc = CqApi.GetGroupList();
            return null;
        }
    }
}
