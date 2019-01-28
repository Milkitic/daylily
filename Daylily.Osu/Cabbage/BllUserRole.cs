using Daylily.Common;
using Daylily.Common.IO;
using Daylily.Osu.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Daylily.Osu.Cabbage
{
    public class BllUserRole
    {
        private static readonly string CachePath = Path.Combine(Domain.CurrentPath, "osuDb.json");
        private static readonly ConcurrentDictionary<long, long> UserDictionary;

        static BllUserRole()
        {
            try
            {
                UserDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<ConcurrentDictionary<long, long>>(
                    File.ReadAllText(CachePath)
                );
            }
            catch (Exception e)
            {
                UserDictionary = new ConcurrentDictionary<long, long>();
            }
        }

        public int InsertUserRole(TableUserRole role)
        {
            DbHelper dbCabbage = new DbHelper("cabbage");

            return dbCabbage.ExecuteNonQuery(@"INSERT INTO userrole
(user_id,role,qq,legacy_uname,current_uname,is_banned,repeat_count,speaking_count,mode)
VALUES(@user_id,@role,@qq,@legacy_uname,@current_uname,@is_banned,@repeat_count,@speaking_count,@mode)",
                new MySqlParameter("@user_id", role.UserId),
                new MySqlParameter("@role", role.Role),
                new MySqlParameter("@qq", role.QQ),
                new MySqlParameter("@legacy_uname", role.LegacyUname),
                new MySqlParameter("@current_uname", role.CurrentUname),
                new MySqlParameter("@is_banned", role.IsBanned),
                new MySqlParameter("@repeat_count", role.RepeatCount),
                new MySqlParameter("@speaking_count", role.SpeakingCount),
                new MySqlParameter("@mode", role.Mode));
        }

        public List<TableUserRole> GetUserRoleByQq(long qq) =>
            _GetUserRole("SELECT * FROM userrole WHERE qq = @qq",
                new MySqlParameter("@qq", qq));

        public List<TableUserRole> GetUserRoleByUid(long uid) =>
            _GetUserRole("SELECT * FROM userrole WHERE user_id = @uid",
                new MySqlParameter("@uid", uid));


        private List<TableUserRole> _GetUserRole(string queryString, params MySqlParameter[] param)
        {
            DbHelper dbCabbage = new DbHelper("cabbage");
            List<TableUserRole> parsedList = new List<TableUserRole>();
            DataTable dataTable = dbCabbage.FillTable(queryString, param);
            foreach (DataRow item in dataTable.Rows)
            {
                parsedList.Add(new TableUserRole
                {
                    Id = Convert.ToInt32(item["id"]),
                    UserId = Convert.ToInt64(item["user_id"]),
                    Role = Convert.ToString(item["role"]),
                    QQ = Convert.ToInt64(item["qq"]),
                    LegacyUname = Convert.ToString(item["legacy_uname"]),
                    CurrentUname = Convert.ToString(item["current_uname"]),
                    IsBanned = Convert.ToBoolean(item["is_banned"].ToString()),
                    RepeatCount = Convert.ToInt64(item["repeat_count"]),
                    SpeakingCount = Convert.ToInt64(item["speaking_count"]),
                    Mode = Convert.ToInt32(item["mode"])
                });
            }

            if (parsedList.Count > 0)
            {
                var first = parsedList[0];
                if (!UserDictionary.ContainsKey(first.QQ))
                {
                    UserDictionary.TryAdd(first.QQ, first.UserId);
                    SaveCache();
                }
                else
                {
                    if (UserDictionary[first.QQ] != first.UserId)
                    {
                        UserDictionary[first.QQ] = first.UserId;
                        SaveCache();
                    }
                }
            }

            return parsedList;
        }

        private static void SaveCache()
        {
            ConcurrentFile.WriteAllText(CachePath, Newtonsoft.Json.JsonConvert.SerializeObject(UserDictionary, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
