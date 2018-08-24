using System.Collections.Generic;
using System.Linq;
using Daylily.Common.Utils.SocketUtils;

namespace Daylily.Common.Utils.LoggerUtils
{
    public class LogLists
    {
        public LogList RawList { get; internal set; } = new LogList("RawList");
        public LogList OriginList { get; internal set; } = new LogList("OriginList");
        public LogList MessageList { get; internal set; } = new LogList("MessageList");
        public LogList DebugList { get; internal set; } = new LogList("DebugList");
        public LogList InfoList { get; internal set; } = new LogList("InfoList");
        public LogList WarnList { get; internal set; } = new LogList("WarnList");
        public LogList ErrorList { get; internal set; } = new LogList("ErrorList");
        public LogList SuccessList { get; internal set; } = new LogList("SuccessList");

        public LogLists Cut(int maxCount)
        {
            return new LogLists
            {
                RawList =
                {
                    Data = RawList.Data.Count > maxCount
                        ? RawList.Data.Skip(RawList.Data.Count - maxCount).ToList()
                        : RawList.Data
                },
                OriginList =
                {
                    Data = OriginList.Data.Count > maxCount
                        ? OriginList.Data.Skip(OriginList.Data.Count - maxCount).ToList()
                        : OriginList.Data
                },
                MessageList =
                {
                    Data = MessageList.Data.Count > maxCount
                        ? MessageList.Data.Skip(MessageList.Data.Count - maxCount).ToList()
                        : MessageList.Data
                },
                DebugList =
                {
                    Data = DebugList.Data.Count > maxCount
                        ? DebugList.Data.Skip(DebugList.Data.Count - maxCount).ToList()
                        : DebugList.Data
                },
                InfoList =
                {
                    Data = InfoList.Data.Count > maxCount
                        ? InfoList.Data.Skip(InfoList.Data.Count - maxCount).ToList()
                        : InfoList.Data
                },
                WarnList =
                {
                    Data = WarnList.Data.Count > maxCount
                        ? WarnList.Data.Skip(WarnList.Data.Count - maxCount).ToList()
                        : WarnList.Data
                },
                ErrorList =
                {
                    Data = ErrorList.Data.Count > maxCount
                        ? ErrorList.Data.Skip(ErrorList.Data.Count - maxCount).ToList()
                        : ErrorList.Data
                },
                SuccessList =
                {
                    Data = SuccessList.Data.Count > maxCount
                        ? SuccessList.Data.Skip(SuccessList.Data.Count - maxCount).ToList()
                        : SuccessList.Data
                }
            };
        }
    }
    public class LogList
    {
        public List<LogListObject> Data { get; set; } = new List<LogListObject>();

        public event WsMessageSendEventHandler MessageSend;
        public LogList(string friendlyName)
        {
            _friendlyName = friendlyName;
            MessageSend += SocketLogger.Ws_MessageSend;
        }

        public LogListObject this[int i] => Data[i];

        public void Add(string source, string info)
        {
            Data.Add(new LogListObject
            {
                Source = source,
                Info = info
            });

            MessageSend(this, new WsMessageSendEventArgs
            {
                FriendlyName = _friendlyName,
                Source = source,
                Info = info
            });
        }

        private readonly string _friendlyName;
    }

    public class LogListObject
    {
        public string Source { get; set; }
        public string Info { get; set; }
    }

    public class NewLogList
    {
        public List<LogListObject> Data { get; set; } = new List<LogListObject>();

        public void Add(string source, string info) => Data.Add(new LogListObject { Source = source, Info = info });
    }
}
