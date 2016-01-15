using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using GlobalClassLibrary;

namespace RuleService.Providers
{
    public class RuleProvider
    {
        
        private string SqlCmd;
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private static String RuleConnection = "";
        private String db = "rule_service";
        private String db_table_rules = "tbl_rules";
        

        public RuleProvider() {
            RuleConnection = "server=localhost;port=9001;user id=admin;pwd=rak20adm/n16;database="+ db +";";
        }

        /// <summary>
        /// Creates persistent Rule from setted variables
        /// </summary>
        /// <returns></returns>
        public long Create(Rule rule)
        {
            long lastInsertedId = -1; ;
            SqlCmd = "INSERT INTO "+ db_table_rules +" (id, condition_Sender, condition_KeywordAccoutingText, condition_KeycharsDocumentNumber,"
                    + " attribution_Department, attribution_ContactPerson, attribution_ProjectNumber, attribution_Account, attribution_CostCenter) "
                    + "values (null, '" + rule.Condition.Sender + "' , '" + rule.Condition.KeywordAccountingText + "' , '" + rule.Condition.KeycharsDocumentNumber 
                    + "' , '" + rule.Attribution.Department + "' , " + rule.Attribution.ContactPerson + " , '" + rule.Attribution.ProjectNumber 
                    + "' , '" + rule.Attribution.Account + "' , '" + rule.Attribution.CostCenter + "');";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(RuleConnection);
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

        public Rule Check(Document document)
        {
            Rule rule = new Rule();

            SqlCmd = "SELECT * FROM "+ db_table_rules +" WHERE condition_sender LIKE \""+ document.Sender +"\" AND attribution_department NOT LIKE \"\" "
                +"AND INSTR(\""+ document.AccountingText + "\", condition_KeywordAccountingText) > 0 "
                +"AND INSTR(\"" + document.Number + "\", condition_KeycharsDocumentNumber ) > 0 ;";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(RuleConnection);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(SqlCmd, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        rule.Id = (int)reader[0];
                        rule.Condition.Sender = (string)reader[1];
                        rule.Condition.KeywordAccountingText = (string)reader[2];
                        rule.Condition.KeycharsDocumentNumber = (string)reader[3];
                        rule.Attribution.Department = (string)reader[4];
                        rule.Attribution.ContactPerson = (string)reader[5];
                        rule.Attribution.ProjectNumber = (string)reader[6];
                        rule.Attribution.Account = (string)reader[7];
                        rule.Attribution.CostCenter = (string)reader[8];
                    }
                    reader.Close();
                }
                else
                {
                    rule = null;
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
            return rule;
        }

        /// <summary>
        /// Reads a Rule for a given Rule Id
        /// </summary>
        /// <param name="channel"></param>
        public Rule Read(int Id)
        {
            Rule rule = new Rule();

            SqlCmd = "SELECT * FROM " + db_table_rules + " WHERE id =" + Id + ";";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(RuleConnection);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(SqlCmd, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        rule.Id = (int)reader[0];
                        rule.Condition.Sender = (string)reader[1];
                        rule.Condition.KeywordAccountingText = (string)reader[2];
                        rule.Condition.KeycharsDocumentNumber = (string)reader[3];
                        rule.Attribution.Department = (string)reader[4];
                        rule.Attribution.ContactPerson = (string)reader[5];
                        rule.Attribution.ProjectNumber = (string)reader[6];
                        rule.Attribution.Account = (string)reader[7];
                        rule.Attribution.CostCenter = (string)reader[8];
                    }
                    reader.Close();
                }
                else {
                    rule = null;
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
            return rule;
        }

        /// <summary>
        /// Updates the current Message from Database
        /// id, guid nicht aktualisieren :: guid="+mGuid+", 
        /// </summary>
        internal Boolean Update(Rule rule)
        {
            int success = -1;
            SqlCmd = "UPDATE " + db_table_rules + " SET condition_Sender='" + rule.Condition.Sender + "', condition_KeywordAccountingText='" + rule.Condition.KeywordAccountingText 
                    + "', condition_KeycharsDocumentNumber='" + rule.Condition.KeycharsDocumentNumber 
                    + "', attribution_Department= " + rule.Attribution.Department + ", attribution_ContactPerson='" + rule.Attribution.ContactPerson 
                    + "', attribution_ProjectNumber='" + rule.Attribution.ProjectNumber + "', attribution_Account='" + rule.Attribution.Account 
                    + "', attribution_CostCenter='" + rule.Attribution.CostCenter 
                    + "' WHERE id = " + rule.Id + ";";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(RuleConnection);
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
        /// Deletes a Rule with given Id from Database
        /// </summary>
        /// <param name="id"></param>
        internal Boolean Delete(int id)
        {
            int success = -1;
            SqlCmd = "DELETE FROM " + db_table_rules + " WHERE id = " + id + ";";

            conn = null;
            cmd = null;

            try
            {
                conn = new MySqlConnection(RuleConnection);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(SqlCmd, conn);
                success = cmd.ExecuteNonQuery();
                
            }
            catch (MySqlException e)
            {
                //throw e;
                return false;
            }
            finally
            {
                conn.Close();
            }

            return (success > 0) ? true : false;
        }

    }
}