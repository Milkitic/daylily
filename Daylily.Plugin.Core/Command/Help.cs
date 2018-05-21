using Daylily.Common.Models;

namespace Daylily.Plugin.Core.Command
{
    public class Help : AppConstruct
    {
        public override CommonMessageResponse Execute(CommonMessage message)
        {
            return new CommonMessageResponse("太多了哇..都在这里：https://www.zybuluo.com/milkitic/note/1130078", message);
        }
    }
}
