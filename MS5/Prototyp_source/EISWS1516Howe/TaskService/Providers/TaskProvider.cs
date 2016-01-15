using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using GlobalClassLibrary;

namespace TaskService.Providers
{
    public class TaskProvider
    {
        //private Task task;

        private string SqlCmd;
        private MySqlConnection conn;
        private MySqlCommand cmd;

        private String db_task = "tasks";
        private String db_table_task = "tbl_tasks";

        private static String TaskConnection = "";


        public TaskProvider() {
            TaskConnection = "server=localhost;port=9001;user id=admin;pwd=rak20adm/n16;database=" + db_task + ";";
        }


        /// <summary>
        /// Creates persistent Tasks
        /// </summary>
        /// <returns></returns>
        public long Create(RuleTask task)
        {
            long lastInsertedId;
            SqlCmd = "INSERT INTO "+ db_table_task +" (id, priority, channel, isLocked, " + 
                "document_Id, document_Guid, document_Path, document_Sender, document_Number, document_AccountingText," + 
                "ruleCondition_Sender, ruleCondition_KeywordAccountingText, ruleCondition_KeycharsDocumentNumber, " +  
                "ruleAttribution_Department, ruleAttribution_ProjectNumber, ruleAttribution_ContactPerson, " + 
                "ruleAttribution_Account, ruleAttribution_CostCenter) " +
                "values (null," + task.Priority + ", '" + task.Channel.ToLower() + "', " + task.IsLocked +
                " , '" + task.Document.Id + "' , '" + task.Document.DocumentGuid + "' , '" + task.Document.Path + 
                "' , '" + task.Document.Sender + "' , '" + task.Document.Number + "' , '" + task.Document.AccountingText + 
                "' , '" + task.Rule.Condition.Sender + "', '" + task.Rule.Condition.KeywordAccountingText + "' , '" + task.Rule.Condition.KeycharsDocumentNumber + 
                "', '" + task.Rule.Attribution.Department + "', '" + task.Rule.Attribution.ProjectNumber + "', '" + task.Rule.Attribution.ContactPerson + 
                "', '" + task.Rule.Attribution.Account + "', '" + task.Rule.Attribution.CostCenter +
                "');";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(TaskConnection);
                conn.Open();
                cmd = new MySqlCommand(SqlCmd, conn);
                cmd.ExecuteNonQuery();
                lastInsertedId = cmd.LastInsertedId;
            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }

            return lastInsertedId;
        }

        /// <summary>
        /// Reads the next Message for a given Channel
        /// </summary>
        /// <param name="channel"></param>
        public RuleTask Read(String channel)
        {
            RuleTask task = new RuleTask();

            SqlCmd = "SELECT * FROM "+ db_table_task +" WHERE channel ='" + channel.ToLower() + "' AND isLocked=false ORDER BY priority ASC LIMIT 1;";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(TaskConnection);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(SqlCmd, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        task.Id = (int)reader[0];
                        task.Priority = (int)reader[1];
                        task.Channel = (string)reader[2];
                        task.IsLocked = (bool)reader[3];
                        task.Document.Id = (int)reader[4];
                        task.Document.DocumentGuid = new Guid((string)reader[5]);
                        task.Document.Path = (string)reader[6];
                        task.Document.Sender = (string)reader[7];
                        task.Document.Number = (string)reader[8];
                        task.Document.AccountingText = (string)reader[9];
                        task.Rule.Condition.Sender = (string)reader[10];
                        task.Rule.Condition.KeywordAccountingText = (string)reader[11];
                        task.Rule.Condition.KeycharsDocumentNumber = (string)reader[12];
                        task.Rule.Attribution.Department = (string)reader[13];
                        task.Rule.Attribution.ProjectNumber = (int)reader[14];
                        task.Rule.Attribution.ContactPerson = (string)reader[15];
                        task.Rule.Attribution.Account = (string)reader[16];
                        task.Rule.Attribution.CostCenter = (string)reader[17];
                    }
                    reader.Close();
                }
                else {
                    task = null;
                }
                
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return task;
        }

        /// <summary>
        /// Updates the current Message from Database
        /// id, guid nicht aktualisieren :: guid="+mGuid+", 
        /// </summary>
        internal Boolean Update(RuleTask task)
        {
            int success = -1;
            SqlCmd = "UPDATE tbl_messages SET priority=" + task.Priority + ", channel='" + task.Channel.ToLower() + "', isLocked=" + task.IsLocked +
                ", document_Id=" + task.Document.Id + ", document_Guid='" + task.Document.DocumentGuid+ "', document_Path='" + task.Document.Path +
                "', document_Sender='" + task.Document.Sender + "', document_Number='" + task.Document.Number + "', document_PostingText='" + task.Document.AccountingText + 
                "', ruleCondition_Sender='"+ task.Rule.Condition.Sender +"', ruleCondition_KeywordAccountingText='" + task.Rule.Condition.KeywordAccountingText +
                "', ruleCondition_KeycharsDocumentNumber='"+ task.Rule.Condition.KeycharsDocumentNumber + 
                "', ruleAttribution_Department='" + task.Rule.Attribution.Department + "', ruleAttribution_ProjectNumber='" + task.Rule.Attribution.ProjectNumber + 
                "', ruleAttribution_ContactPerson='" + task.Rule.Attribution.ContactPerson + "', ruleAttribution_Account='" + task.Rule.Attribution.Account + 
                "', ruleAttribution_CostCenter='" + task.Rule.Attribution.CostCenter +
                "' WHERE id = " + task.Id + ";";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(TaskConnection);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SqlCmd, conn);

                success = cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }

            return (success > 0) ? true : false;
        }

        /// <summary>
        /// Deletes the current Message from Database
        /// </summary>
        /// <param name="id"></param>
        internal Boolean Delete(int id)
        {
            int success = -1;
            SqlCmd = "DELETE FROM tbl_tasks WHERE id = " + id + ";";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(TaskConnection);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SqlCmd, conn);
                success = cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }

            return (success > 0) ? true : false;
        }
    }
}