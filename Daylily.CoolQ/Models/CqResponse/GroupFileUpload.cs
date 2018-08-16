using Newtonsoft.Json;

namespace Daylily.CoolQ.Models.CqResponse
{
    /// <summary>
    /// 群文件上传。
    /// </summary>
    public class GroupFileUpload : Base
    {
        /// <summary>
        /// 事件名。
        /// </summary>
        [JsonProperty(PropertyName = "notice_type")]
        public string NoticeType { get; set; }

        /// <summary>
        /// 群号。
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }

        /// <summary>
        /// 发送者 QQ 号。
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// 文件信息。
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public FileObj File { get; set; }

        /// <summary>
        /// 文件信息。
        /// </summary>
        public class FileObj
        {
            /// <summary>
            /// 文件 ID。
            /// </summary>
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            /// <summary>
            /// 文件名。
            /// </summary>
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            /// <summary>
            /// 文件大小（字节数）。
            /// </summary>
            [JsonProperty(PropertyName = "size")]
            public long Size { get; set; }

            /// <summary>
            /// busid（目前不清楚有什么作用）。
            /// </summary>
            [JsonProperty(PropertyName = "busid")]
            public int BusId { get; set; }

        }
    }
}
