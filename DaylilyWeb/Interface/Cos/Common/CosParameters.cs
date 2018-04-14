/*
 * Created by SharpDevelop.
 * User: tencent
 * Date: 2016/5/24
 * Time: 15:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace QCloud.CosApi.Common
{
	static class CosParameters
	{
		public const string Authorization = "Authorization";
		
		public const string PARA_OP = "op";
		
		public const string PARA_BIZ_ATTR = "biz_attr";

        public const string PARA_FORBID = "forbid";

        public const string PARA_LIST_FLAG = "list_flag";

		public const string PARA_INSERT_ONLY = "insertOnly";
		
		public const string PARA_AUTHORITY = "authority";

		public const string PARA_SESSION = "session";
		
		public const string PARA_NUM = "num";
		
		public const string PARA_ORDER = "order";
		
		public const string PARA_CONTEXT = "context";

		public const string PARA_PREFIX = "prefix";

		public const string PARA_PATTERN = "pattern";		
		
		public const string PARA_SLICE_SIZE = "slice_size";
		
		public const string PARA_CUSTOM_HEADERS = "custom_headers";
		
		public const string PARA_CACHE_CONTROL = "Cache-Control";
		
		public const string PARA_CONTENT_TYPE = "Content-Type";
		
		public const string PARA_CONTENT_DISPOSITION = "Content-Disposition";

		public const string PARA_CONTENT_LANGUAGE = "Content-Language";

		public const string PARA_X_COS_META_PREFIX = "x-cos-meta-";
		
		public const string PARA_FLAG = "flag";

        public const string PARA_CONTENT_ENCODING = "Content-Encoding";

    }

    public static class CosDefaultValue
    {
        public const string USER_AGENT_VERSION = "cos-dotnet-sdk-v4.2";

        public const string ACCEPT = "*/*";
    }

	public static class FolderPattern
	{
		public const string PATTERN_FILE = "eListFileOnly";
		public const string PATTERN_DIR = "eListDirOnly";
		public const string PATTERN_BOTH = "eListBoth";
	}
	
	internal static class InsertOnly
	{
		public const string INSERTONLY_0 = "0";
		public const string INSERTONLY_1 = "1";
	}

	internal static class Flag
	{
		public const int FLAG_BIZ_ATTR = 0x01;
		public const int FLAG_CUSTOMER_HEADER = 0x40;
		public const int FLAG_AUTHORITY = 0x80;
		public const int FLAG_FORBID = 0x100;
	}
	
	internal static class SLICE_SIZE
	{
        public const int SLIZE_SIZE_64K = 64 * 1024;
		public const int SLIZE_SIZE_512K = 512*1024;
		public const int SLIZE_SIZE_1M = 1*1024*1024;
		public const int SLIZE_SIZE_2M = 2*1024*1024;
		public const int SLIZE_SIZE_3M = 3*1024*1024;
	}
	
	internal static class AUTHORITY
	{
		public const string AUTHORITY_INVALID = "eInvalid";
		public const string AUTHORITY_PRIVATE = "eWRPrivate";
		public const string AUTHORITY_PRIVATEPUBLIC = "eWPrivateRPublic";
	}
	
	internal static class ERRORCode
	{
		public const int ERROR_CODE_FILE_NOT_EXIST = -1;
		public const int ERROR_CODE_NETWORK_ERROR = -2;
		public const int ERROR_CODE_PARAMETER_ERROE = -3;
	}
}
