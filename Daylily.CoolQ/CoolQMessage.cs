using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Daylily.CoolQ
{
    public class CoolQMessage
    {
        private readonly CoolQCode _assembledCqCodes;

        public CoolQMessage(params CoolQCode[] cqCodes)
        {
            var cqCodeList = new List<CoolQCode>(cqCodes.Length);
            cqCodeList.AddRange(cqCodes);
            _assembledCqCodes = new Assemblage(cqCodeList);
        }

        public string Compose() => InnerCompose(_assembledCqCodes);

        public static string Compose(CoolQCode cqCode) => InnerCompose(cqCode);

        private static string InnerCompose(CoolQCode cqCode)
        {
            switch (cqCode)
            {
                case Text text:
                    return text.Content;
                case At at:
                    return $"[CQ:at,qq={at.UserId}]";
                case Face face:
                    return $"[CQ:face,id={face.FaceId}]";
                case Emoji emoji:
                    return $"[CQ:emoji,id={emoji.EmojiId}]";
                case BFace bface:
                    return $"[CQ:bface,id={bface.BFaceId}]";
                case SFace sface:
                    return $"[CQ:sface,id={sface.SFaceId}]";
                case CustomMusic music:
                    return string.Format("[CQ:music,type=custom,url={0},audio={1},title={2},content={3},image={4}]",
                        music.LinkUrl, music.AudioUrl, music.Title, music.Content, music.ImageUrl);
                case Share share:
                    return string.Format("[CQ:share,url={0},title={1},content={2},image={3}]",
                        share.LinkUrl, share.Title, share.Content, share.ImageUrl);
                case FileImage image:
                    {
                        switch (image.FileType)
                        {
                            case FileCoolQCode.FileTypeEnum.Local:
                                return image.UseBase64
                                    ? $"[CQ:image,file=base64://{EncodeFileToBase64(image.Path)}]"
                                    : $"[CQ:image,file=file://{image.Path}]";
                            // ReSharper disable once RedundantCaseLabel
                            case FileCoolQCode.FileTypeEnum.Url:
                            default:
                                return $"[CQ:image,file=http://{image.Url.Replace("http://", "").Replace("https://", "")}]";
                        }
                    }
                case FileRecord record:
                    {
                        switch (record.FileType)
                        {
                            case FileCoolQCode.FileTypeEnum.Local:
                                return record.UseBase64
                                    ? $"[CQ:record,file=base64://{EncodeFileToBase64(record.Path)}]"
                                    : $"[CQ:record,file=file://{record.Path}]";
                            // ReSharper disable once RedundantCaseLabel
                            case FileCoolQCode.FileTypeEnum.Url:
                            default:
                                return $"[CQ:record,file=http://{record.Url.Replace("http://", "")}]";
                        }
                    }
                case Assemblage assemblage:
                    var codes = assemblage.CoolQCodes;
                    StringBuilder sb = new StringBuilder();
                    foreach (var code in codes)
                        sb.Append(InnerCompose(code));
                    return sb.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string EncodeFileToBase64(string path)
        {
            using (FileStream filestream = new FileStream(path, FileMode.Open))
            {
                byte[] bt = new byte[filestream.Length];
                filestream.Read(bt, 0, bt.Length);
                return Convert.ToBase64String(bt);
            }
        }
    }

    public abstract class Template
    {
        private readonly CoolQMessage _inner;
        protected abstract CoolQCode[] CoolQCodes { get; }

        protected Template()
        {
            _inner = new CoolQMessage(CoolQCodes);
        }

        protected CoolQCode At(string userId) => new At(userId);
        protected CoolQCode Face(string faceId) => new Face(faceId);
        protected CoolQCode Emoji(string emojiId) => new Emoji(emojiId);
        protected CoolQCode BFace(string bfaceId) => new BFace(bfaceId);
        protected CoolQCode SFace(string sfaceId) => new SFace(sfaceId);
        protected CoolQCode Music(string linkUrl, string audioUrl, string title, string content = null,
            string imageUrl = null) => new CustomMusic(linkUrl, audioUrl, title, content, imageUrl);
        protected CoolQCode Share(string linkUrl, string title, string content, string imageUrl) =>
            new Share(linkUrl, title, content, imageUrl);
        protected CoolQCode Image(string path) => new FileImage(path);
        protected CoolQCode UImage(string url) => new FileImage(new Uri(url));
        protected CoolQCode Image(Image image) => new FileImage(image);
        protected CoolQCode Record(string path) => new FileImage(path);
        protected CoolQCode URecord(string url) => new FileRecord(url, true);

        public string Compose() => _inner.Compose();
    }
}
