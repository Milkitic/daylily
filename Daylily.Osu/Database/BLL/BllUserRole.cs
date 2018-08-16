using System;
using System.Collections.Generic;
using System.Data;
using Daylily.Osu.Database.Model;
using MySql.Data.MySqlClient;

namespace Daylily.Osu.Database.BLL
{
    public class BllUserRole
    {
        public int InsertUserRole(TblUserRole role)
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

        public List<TblUserRole> GetUserRoleByQq(long qq) =>
            _GetUserRole("SELECT * FROM userrole WHERE qq = @qq",
                new MySqlParameter("@qq", qq));

        private List<TblUserRole> _GetUserRole(string queryString, params MySqlParameter[] param)
        {
            DbHelper dbCabbage = new DbHelper("cabbage");
            List<TblUserRole> parsedList = new List<TblUserRole>();
            DataTable dataTable = dbCabbage.FillTable(queryString, param);
            foreach (DataRow item in dataTable.Rows)
            {
                parsedList.Add(new TblUserRole
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

            return parsedList;
        }
    }
}
