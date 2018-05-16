using QCloud.CosApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daylily.Common.Models
{
    public static class Signature
    {
        public static int appId;
        public static string secretId;
        public static string secretKey;
        public static string bucketName;

        public static string Get()
        {
            return Sign.Signature(appId, secretId, secretKey, DateTime.Now.AddMonths(1).ToUnixTime() / 1000, bucketName);
        }

    }
}
