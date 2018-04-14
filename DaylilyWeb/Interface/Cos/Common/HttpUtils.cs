/*
 * Created by SharpDevelop.
 * User: jonnxu
 * Date: 2016/5/24
 * Time: 16:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Web;

namespace QCloud.CosApi.Util
{
	public static class HttpUtils
	{
        static char[] reserveChar = new char[] {'/','?','*',':','|','\\','<','>','\"'};

        /// <summary>
        /// 远程路径Encode处理,会保证开头是/，结尾也是/
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public static string EncodeRemotePath(string remotePath)
		{
			if (remotePath == "/") {
				return remotePath;
			}
			var endWith = remotePath.EndsWith("/");
			String[] part = remotePath.Split('/');
			remotePath = "";
			foreach (var s in part) {
				if (s != "") {
					if (remotePath != "") {
						remotePath += "/";
					}
					remotePath += HttpUtility.UrlEncode(s).Replace("+","%20");
				}
			}

			remotePath = (remotePath.StartsWith("/") ? "" : "/") + remotePath + (endWith ? "/" : "");
			return remotePath;
		}

        /// <summary>
        /// 标准化远程目录路径,会保证开头是/，结尾也是/ ,如果命名不规范，存在保留字符，会返回空字符
        /// </summary>
        /// <param name="remotePath">要标准化的远程路径</param>
        /// <returns></returns>
        public static string StandardizationRemotePath(string remotePath)
        {
            if(String.IsNullOrEmpty(remotePath))
            {
                return "";
            }

            if(!remotePath.StartsWith("/"))
            {
                remotePath = "/" + remotePath;
            }

            if(!remotePath.EndsWith("/"))
            {
                remotePath = remotePath + "/";
            }

            int index1 = 1;
            int index2 = 0;
            while(index1 < remotePath.Length)
            {
                index2 = remotePath.IndexOf('/', index1);
                if (index2 == index1)
                {
                    return "";
                }

                var folderName = remotePath.Substring(index1, index2 - index1);
                if(folderName.IndexOfAny(reserveChar) != -1)
                {
                    return "";
                }

                index1 = index2+1;

            }
            return remotePath;
		}
	}
}
