﻿using Daylily.Common.Models.CQCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Daylily.Common.Assist
{
    /// <summary>
    /// CQ码帮助类，可用于编码成CQ码、将CQ码解码以及从文本中提取CQ码。
    /// </summary>
    public static class CqCode
    {
        /// <summary>
        /// CoolQ所在的目录，由appsettings.json配置，若CQ与本程序在同一平台，此属性有效，可节省网络请求。
        /// </summary>
        public static string CqRoot { get; set; }

        #region Decode

        /// <summary>
        /// 将消息中所有可能出现的CQ码转换为可读形式。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Decode(string source)
        {
            source = DecodeImageToText(source);
            // TODO
            return source;
        }

        /// <summary>
        /// 将消息中出现的图片类型CQ码转换为可读形式。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string DecodeImageToText(string source)
        {
            StringBuilder sb = new StringBuilder(source);
            int index = 0;
            string str1 = "[CQ:image,";
            while ((index = sb.ToString().IndexOf(str1, index, StringComparison.Ordinal)) != -1)
            {
                int length = source.IndexOf("]", index, StringComparison.Ordinal) - index + 1;
                sb.Remove(index, length);
                sb.Insert(index, "[图片]");
            }

            return sb.ToString();
        }

        #endregion

        #region Get

        /// <summary>
        /// 提取消息中被At的QQ号。返回一个string数组。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string[] GetAt(string source)
        {
            List<string> infoList = new List<string>();
            int index, index2 = 0;
            const string str1 = "[CQ:at,qq=";

            while ((index = source.IndexOf(str1, index2, StringComparison.Ordinal) + str1.Length) != -1 + str1.Length)
            {
                int length = source.IndexOf("]", index, StringComparison.Ordinal) - index;
                infoList.Add(source.Substring(index, length));
                index2 = index + length;
            }

            return infoList.Count == 0 ? null : infoList.ToArray();
        }

        /// <summary>
        /// 提取消息中的图片。返回一个对象数组。
        /// </summary>
        /// <param name="source">消息文本</param>
        /// <returns></returns>
        public static ImageInfo[] GetImageInfo(string source)
        {
            List<ImageInfo> infoList = new List<ImageInfo>();
            int index, index2 = 0;
            while ((index = source.IndexOf("[CQ:image", index2, StringComparison.Ordinal)) != -1)
            {
                int length = source.IndexOf("]", index, StringComparison.Ordinal) - index;
                infoList.Add(new ImageInfo(source.Substring(index, length + 1)));
                index2 = index + length;
            }

            return infoList.Count == 0 ? null : infoList.ToArray();
        }

        #endregion

        private static string Escape(string text)
        {
            return text.Replace("&", "&amp").Replace("[", "&#91").Replace("]", "&#93").Replace(",", "&#44");
        }
    }
}
