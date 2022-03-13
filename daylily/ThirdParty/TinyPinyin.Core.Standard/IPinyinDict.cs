namespace daylily.ThirdParty.TinyPinyin.Core.Standard
{
    public interface IPinyinDict
    {
        /// <summary>
        /// 转换文本为到拼音
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        string[] ToPinyin(string word);

        List<string> Words();
    }
}
