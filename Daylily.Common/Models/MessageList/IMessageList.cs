﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Daylily.Common.Models.CQResponse;

namespace Daylily.Common.Models.MessageList
{
    public interface IMessageList
    {
        void Add(long id);
        Type Type { get; }
    }

    public interface IMessageSettings<T>
    {
        Queue<T> MsgQueue { get; set; }
        Task Task { get; set; }
        int MsgLimit { get; set; }
        bool LockMsg { get; set; } // 用于判断是否超出消息阀值
    }
}
