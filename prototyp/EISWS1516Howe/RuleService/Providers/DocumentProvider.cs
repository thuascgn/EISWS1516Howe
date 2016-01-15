using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using GlobalClassLibrary;
using System.Xml;
using System.IO;

namespace RuleService.Providers
{
    public class DocumentProvider
    {
        private string SqlCmd;
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private static String RuleConnection = "";
        private String db = "rule_service";
        private String db_table_documents = "tbl_documents";


        public DocumentProvider()
        {
            RuleConnection = "server=localhost;port=9001;user id=admin;pwd=rak20adm/n16;database=" + db + ";";
        }

        /// <summary>
        /// Creates persistent Rule from setted variables
        /// </summary>
        /// <returns></returns>
        public long Create(Document document)
        {
            long lastInsertedId = -1; ;
            SqlCmd = "INSERT INTO " + db_table_documents + " (id, documentGuid, path, sender, documentNumber, accountingText)"
                    + "values (null, '" + document.DocumentGuid.ToString() + "' , '" + document.Path + "' , '" + document.Sender
                    + "', '" + document.Number + "', '" + document.AccountingText + "');";

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

        #region Check
        internal Rule Check(Document document)
        {
            Rule rule = new Rule();

            SqlCmd = "SELECT * FROM " + db_table_documents + " WHERE condition_sender LIKE \"" + document.Sender + "\" AND attribution_department NOT LIKE \"\" "
                + "AND INSTR(\"" + document.AccountingText + "\", condition_KeywordAccountingText) > 0 "
                + "AND INSTR(\"" + document.Number + "\", condition_KeycharsDocumentNumber ) > 0 ;";

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

        #endregion

        /// <summary>
        /// Reads a Rule for a given Rule Id
        /// </summary>
        /// <param name="channel"></param>
        public Document Read(int Id)
        {
            Document document = new Document();

            SqlCmd = "SELECT * FROM "+ db_table_documents +" WHERE id =" + Id + ";";

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
                        document.Id = (int)reader[0];
                        document.DocumentGuid = (Guid)reader[1];
                        document.Path = (string)reader[2];
                        document.Sender = (string)reader[3];
                        document.Number = (string)reader[4];
                        document.AccountingText = (string)reader[5];
                    }
                    reader.Close();
                }
                else
                {
                    document = null;
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
            return document;
        }

        /// <summary>
        /// Updates the current Message from Database
        /// id, guid nicht aktualisieren :: guid="+mGuid+", 
        /// </summary>
        internal Boolean Update(Document document)
        {
            int success = -1;
            SqlCmd = "UPDATE "+ db_table_documents +" SET documentGuid='" + document.DocumentGuid + "', path='" + document.Path
                    + "', sender='" + document.Sender + "', documentNumber= " + document.Number + ", accountingText='" + document.AccountingText
                    + "' WHERE id = " + document.Id + ";";

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
            SqlCmd = "DELETE FROM "+ db_table_documents + " WHERE id = " + id + ";";

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


        internal bool Export(Document document, Rule rule)
        {
            string export = "C:\\Users\\Export";
            string filename = export + "\\" + document.DocumentGuid.ToString() + ".xml";
            XmlWriter writer = XmlWriter.Create((export + "\\" + document.DocumentGuid.ToString() + ".xml"));

            try
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Metadata");
                //foreach (RulePair dR in dataSetExport)
                //{
                //    //writer.WriteStartElement(dR.Attribute);
                //    writer.WriteElementString(dR.Attribute, dR.Assign);
                //    //writer.WriteEndElement();
                //}

                writer.WriteElementString("Guid", document.DocumentGuid.ToString());
                writer.WriteElementString("Path", document.Path);
                writer.WriteEndElement();

                writer.WriteStartElement("Document");
                writer.WriteElementString("Sender", document.Sender);
                writer.WriteElementString("Number", document.Number);
                writer.WriteElementString("AccountingText", document.AccountingText);
                writer.WriteEndElement();

                writer.WriteStartElement("Attribution");
                writer.WriteElementString("Department", rule.Attribution.Department);
                writer.WriteElementString("Project", rule.Attribution.ProjectNumber.ToString());
                writer.WriteElementString("ContactPerson", rule.Attribution.ContactPerson);
                writer.WriteElementString("Account", rule.Attribution.Account);
                writer.WriteElementString("CostCenter", rule.Attribution.CostCenter);
                writer.WriteEndElement();

                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                writer.Close();
            }


            if (File.Exists(filename))
            {
                return true;
            }
            else {
                return false;
            }
        }
    }
}