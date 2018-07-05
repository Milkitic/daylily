using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using Daylily.Common.Assist;

namespace Daylily.Common.Utils
{
    /// <summary>
    /// CQ码
    /// </summary>
    public abstract class CoolQCode
    {
        /// <summary>
        /// 纯文本。
        /// </summary>
        /// <param name="text">文本内容。</param>
        public static implicit operator CoolQCode(string text) =>
            new Text(text);

        /// <summary>
        /// 判断文本是否为数字。
        /// </summary>
        /// <param name="str">要判断的文本。</param>
        /// <returns></returns>
        protected static bool IsNum(string str) =>
            str.All(char.IsDigit);

        /// <summary>
        /// 判断文本是否在某一范围（包含界限）
        /// </summary>
        /// <param name="str">要判断的文本。</param>
        /// <param name="lBound">界限的下界。</param>
        /// <param name="uBound">界限的上界。</param>
        /// <returns></returns>
        protected static bool InRange(string str, int lBound, int uBound) =>
            int.Parse(str) >= lBound && int.Parse(str) <= uBound;
        protected static string Escape(string text)
        {
            return text.Replace("&", "&amp").Replace("[", "&#91").Replace("]", "&#93").Replace(",", "&#44");
        }
    }

    /// <summary>
    /// 包含文件信息的CQ码
    /// </summary>
    public abstract class FileCoolQCode : CoolQCode
    {
        /// <summary>
        /// 文件所在路径。
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// 文件所在网络路径。
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// 文件CQ码的类别
        /// </summary>
        public FileTypeEnum FileType { get; }
        public bool UseBase64 { get; set; } = true;

        /// <summary>
        /// 包含本地文件信息的CQ码。
        /// </summary>
        /// <param name="file">文件信息。</param>
        public FileCoolQCode(FileSystemInfo file)
        {
            string path = file.FullName;
            Contract.Requires<ArgumentException>(File.Exists(path));
            Path = Escape(path);
            FileType = FileTypeEnum.Local;
        }

        /// <summary>
        /// 包含网络文件信息的CQ码。
        /// </summary>
        /// <param name="url">网络地址。</param>
        public FileCoolQCode(string url)
        {
            Url = Escape(url);
            FileType = FileTypeEnum.Url;
        }

        /// <summary>
        /// 文件CQ码的类别
        /// </summary>
        public enum FileTypeEnum
        {
            /// <summary>
            /// 本地文件绝对路径形式。
            /// </summary>
            Local,
            /// <summary>
            /// 网络路径形式。
            /// </summary>
            Url,
        }

        /// <inheritdoc />
        protected FileCoolQCode() { }
    }

    /// <summary>
    /// 纯文本
    /// </summary>
    public class Text : CoolQCode
    {
        /// <summary>
        /// 文本内容。
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// 纯文本。
        /// </summary>
        /// <param name="content">文本内容。</param>
        public Text(string content) => Content = Escape(content);
    }

    /// <summary>
    /// @某人
    /// </summary>
    public class At : CoolQCode
    {
        /// <summary>
        /// 被@的群成员QQ。
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// @某人。
        /// </summary>
        /// <param name="userId">为被@的群成员QQ。若该参数为all，则@全体成员（次数用尽或权限不足则会转换为文本）。</param>
        public At(string userId)
        {
            Contract.Requires<ArgumentException>(IsNum(userId) || userId.ToLower() == "all");
            UserId = Escape(userId);
        }
    }

    /// <summary>
    /// QQ表情
    /// </summary>
    public class Face : CoolQCode
    {
        /// <summary>
        /// QQ表情ID。
        /// </summary>
        public string FaceId { get; }

        /// <summary>
        /// QQ表情。
        /// </summary>
        /// <param name="faceId">QQ表情ID，为0-170的数字。</param>
        public Face(string faceId)
        {
            Contract.Requires<ArgumentException>(IsNum(faceId));
            Contract.Requires<IndexOutOfRangeException>(InRange(faceId, 0, 170));

            FaceId = Escape(faceId);
        }
    }

    /// <summary>
    /// emoji表情
    /// </summary>
    public class Emoji : CoolQCode
    {
        /// <summary>
        /// emoji字符的unicode编号
        /// </summary>
        public string EmojiId { get; }

        /// <summary>
        /// emoji表情。
        /// </summary>
        /// <param name="emojiId">为emoji字符的unicode编号。</param>
        public Emoji(string emojiId)
        {
            Contract.Requires<ArgumentException>(IsNum(emojiId));
            EmojiId = Escape(emojiId);
        }
    }

    /// <summary>
    /// 原创表情
    /// </summary>
    public class BFace : CoolQCode
    {
        /// <summary>
        /// 原创表情的ID。
        /// </summary>
        public string BFaceId { get; }

        /// <summary>
        /// 为该原创表情的ID，存放在酷Q目录的data\bface\下。
        /// </summary>
        /// <param name="bFaceId">原创表情的ID，存放在酷Q目录的data\bface\下。</param>
        public BFace(string bFaceId)
        {
            Contract.Requires<ArgumentException>(IsNum(bFaceId));
            BFaceId = Escape(bFaceId);
        }
    }

    /// <summary>
    /// 小表情
    /// </summary>
    public class SFace : CoolQCode
    {
        /// <summary>
        /// 小表情的ID。
        /// </summary>
        public string SFaceId { get; }

        /// <summary>
        /// 小表情。
        /// </summary>
        /// <param name="sFaceId">为该小表情的ID。</param>
        public SFace(string sFaceId)
        {
            Contract.Requires<ArgumentException>(IsNum(sFaceId));
            SFaceId = Escape(sFaceId);
        }
    }

    /// <summary>
    /// 发送音乐自定义分享
    /// </summary>
    public class CustomMusic : CoolQCode
    {
        /// <summary>
        /// 分享链接。
        /// </summary>
        public string LinkUrl { get; }

        /// <summary>
        /// 音频链接。
        /// </summary>
        public string AudioUrl { get; }

        /// <summary>
        /// 音乐的标题。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 音乐的简介。
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// 音乐的封面图片链接。
        /// </summary>
        public string ImageUrl { get; }

        /// <summary>
        /// 发送音乐自定义分享。注意：音乐自定义分享只能作为单独的一条消息发送。
        /// </summary>
        /// <param name="linkUrl">为分享链接，即点击分享后进入的音乐页面（如歌曲介绍页）。</param>
        /// <param name="audioUrl">为音频链接（如mp3链接）。</param>
        /// <param name="title">为音乐的标题，建议12字以内。</param>
        /// <param name="content">为音乐的简介，建议30字以内。该参数可被忽略。</param>
        /// <param name="imageUrl">为音乐的封面图片链接。若参数为空或被忽略，则显示默认图片。</param>
        public CustomMusic(string linkUrl, string audioUrl, string title, string content = null, string imageUrl = null)
        {
            LinkUrl = Escape(linkUrl);
            AudioUrl = Escape(audioUrl);
            Title = Escape(title);
            Content = Escape(content);
            ImageUrl = Escape(imageUrl);
        }
    }

    /// <summary>
    /// 发送链接分享
    /// </summary>
    public class Share : CoolQCode
    {
        /// <summary>
        /// 分享链接。
        /// </summary>
        public string LinkUrl { get; }

        /// <summary>
        /// 分享的标题。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 分享的简介。
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// 分享的图片链接。
        /// </summary>
        public string ImageUrl { get; }

        /// <summary>
        /// 发送链接分享。注意：链接分享只能作为单独的一条消息发送。
        /// </summary>
        /// <param name="linkUrl">为分享链接。</param>
        /// <param name="title">为分享的标题，建议12字以内。</param>
        /// <param name="content">为分享的简介，建议30字以内。该参数可被忽略。</param>
        /// <param name="imageUrl">为分享的图片链接。若参数为空或被忽略，则显示默认图片。</param>
        public Share(string linkUrl, string title, string content, string imageUrl)
        {
            LinkUrl = Escape(linkUrl);
            Title = Escape(title);
            Content = Escape(content);
            ImageUrl = Escape(imageUrl);
        }
    }

    /// <summary>
    /// 发送自定义图片
    /// </summary>
    public class FileImage : FileCoolQCode
    {
        /// <summary>
        /// 发送本地已存在的图片。
        /// </summary>
        /// <param name="file">文件信息。</param>
        public FileImage(FileInfo file) : base(file) { }

        /// <summary>
        /// 发送网络中的图片。
        /// </summary>
        /// <param name="url">网络地址</param>
        public FileImage(string url) : base(url) { }

        /// <summary>
        /// 发送Image对象中的图片。
        /// </summary>
        /// <param name="img">为欲发送的图片</param>
        public FileImage(Image img)
        {
            string path = System.IO.Path.Combine(Domain.CurrentDirectory, "images", Guid.NewGuid() + ".png");
            img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            Path = Escape(path);
        }
    }

    /// <summary>
    /// 发送语音
    /// </summary>
    public class FileRecord : FileCoolQCode
    {
        /// <summary>
        /// 发送本地已存在的语音。
        /// </summary>
        /// <param name="file">文件信息。</param>
        public FileRecord(FileInfo file) : base(file) { }

        /// <summary>
        /// 发送网络中的语音。
        /// </summary>
        /// <param name="url">网络地址</param>
        public FileRecord(string url) : base(url) { }
    }

    public class Assemblage : CoolQCode
    {
        public CoolQCode[] CoolQCodes { get; }

        public Assemblage(CoolQCode[] cqCodes) => CoolQCodes = cqCodes;

        public Assemblage(IEnumerable<CoolQCode> cqCodes) => CoolQCodes = cqCodes.ToArray();
    }
}