using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageService.Models
{

    public class Status {

        List<SenderStatus> StatusList;
        private string com;
        private MySqlConnection conn;
        private MySqlCommand cmd;

        private static String TaskConnection = "server=localhost;port=9001;user id=admin;pwd=rak20adm/n16;database=task_service;";
        private static String PriorityConnection = "server=localhost;port=9001;user id=admin;pwd=rak20adm/n16;database=task_service";

        public Status() {
            StatusList = new List<SenderStatus>();

            com = "";
            conn = new MySqlConnection();
            cmd = new MySqlCommand();
        }

        public void Read() {
            com = "SELECT sender, priority, COUNT(*) as amount FROM tbl_tasks m1 GROUP BY sender ORDER BY priority ASC;";
            
            try
            {
                conn = new MySqlConnection(TaskConnection);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(com, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    StatusList.Add(new SenderStatus(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2)));
                }
                reader.Close();
            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally {
                conn.Close();
            }

        }

        public Int64 CheckAmount(string department)
        {
            long amount = 0;
            Int64 amount64 = new Int64();
            Int32 amount32;

            System.Data.SqlTypes.SqlInt64 sint64 = new System.Data.SqlTypes.SqlInt64();
            
            //var amount64;
            com = "SELECT DISTINCT COUNT(*) as amount FROM tbl_tasks WHERE channel='" + department + "' AND isLocked=False;";

            try
            {
                conn = new MySqlConnection(TaskConnection);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(com, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                //object result = cmd.ExecuteScalar();
                //if (Convert.IsDBNull(result))
                //{
                //    amount64 = 0;
                //    amount = 0;
                //    sint64 = 0;
                //}
                //else
                //{
                //    //UInt64? result64 = (UInt64?)(!Convert.IsDBNull(result) ? result : null);
                //    //amount64 = Convert.ToUInt64(result64);
                //    sint64 = (System.Data.SqlTypes.SqlInt64)result;
                //    amount64 = (!Convert.IsDBNull(sint64)) ? (Int64)sint64 : -1;
                //}
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        object val = reader[0];
                        amount = Convert.ToInt64(val);
                        amount64 = (Int64)reader.GetInt64(0);
                        //sint64 = (System.Data.SqlTypes.SqlInt64)reader[0];
                    }
                }
                else
                {
                    amount = 0;
                }
                reader.Close();

                amount32 = Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }
            return amount64;

        }

        public bool Update(List<StatusUpdate> statusUpdateList)
        {
            com = "";

            foreach (var statusupdate in statusUpdateList)
            {
                com += "UPDATE tbl_messages SET priority=" + statusupdate.Priority + " WHERE sender LIKE '%" + statusupdate.Sender + "%';"; 
            }

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(TaskConnection);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(com, conn);

                cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();

            }
            return true;
        }


        public override string ToString() {
            String status = "{'statuslist': [";
            foreach (SenderStatus su in StatusList) {
                status += "{";
                status += "'Sender': '" + su.Sender + "' , 'Priority': '" + su.Priority + "' , 'Amount':'"+su.Amount+"'";
                status += "},";
            }
            status.TrimEnd(',');
            status += "]}";

            return status;
        }

    }



    public class StatusUpdate{
        public string Sender;
        public int Priority;

        public StatusUpdate(string sender, int priority) {
            Sender = sender;
            Priority = priority;
        }
    }

    public class SenderStatus
    {

        public string Sender;
        public int Priority;
        public int Amount;

        public SenderStatus(string sender, int priority, int amount) {
            Sender = sender;
            Priority = priority;
            Amount = amount;
        }
    }

}