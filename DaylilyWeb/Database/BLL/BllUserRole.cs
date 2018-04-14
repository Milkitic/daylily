using DaylilyWeb.Database.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Database.BLL
{
    public class BllUserRole
    {
        public List<TblUserRole> GetUserRoleByQQ(long qq)
        {
            return _GetUserRole("SELECT * FROM userrole WHERE qq = @qq",
                new MySqlParameter("@qq", qq));
        }
        private List<TblUserRole> _GetUserRole(string queryString, params MySqlParameter[] param)
        {
            DbHelper dbCabbage = new DbHelper("cabbage");
            List<TblUserRole> parsed_list = new List<TblUserRole>();
            DataTable dataTable = dbCabbage.FillTable(queryString, param);
            foreach (DataRow item in dataTable.Rows)
            {
                parsed_list.Add(new TblUserRole
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
            return parsed_list;
        }
    }
}
