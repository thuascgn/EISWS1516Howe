using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace GlobalClassLibrary
{
    public class Rule
    {
        public int Id;

        public Condition Condition;
        public Attribution Attribution;


        public Rule()
        {
            Id = 0;
            Condition = new Condition();
            Attribution = new Attribution();
        }

        public Rule(int id, string conditionSender, string conditionKeywordAccountingText, string conditionKeycharsDocumentNumber,
                    string attributionDepartment, string attributionContactPerson, int attributionProjectNumber, 
                    string attributionAccount, string attributionCostcenter)
        {
            Id = id;
            Condition = new Condition(conditionSender, conditionKeywordAccountingText, conditionKeycharsDocumentNumber);
            Attribution = new Attribution(attributionDepartment, attributionContactPerson, attributionProjectNumber, 
                attributionAccount, attributionCostcenter);
            
        }
        
        public override string ToString()
        {
            return "{ rule: [{ 'id':'" + Id 
                        + "', '" + Condition.ToString()
                        + "', '" + Attribution.ToString()
                        + "}] }";
        }

        public String ToJson()
        {
            MemoryStream memstr = new MemoryStream();
            var jser = new DataContractJsonSerializer(typeof(Rule));
            jser.WriteObject(memstr, this);
            memstr.Position = 0;
            var sr = new StreamReader(memstr);
            string rt_json = sr.ReadToEnd();

            return rt_json;
        }

        public Rule FromJson(string json)
        {
            var memstr = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var jser = new DataContractJsonSerializer(typeof(Rule));

            return jser.ReadObject(memstr) as Rule;
        }

    }

    #region Condition
    public class Condition {
        public string Sender;
        public string KeywordAccountingText;
        public string KeycharsDocumentNumber;

        public Condition() {
            Sender = KeywordAccountingText = KeycharsDocumentNumber = @"";
        }

        public Condition(string sender, string keywordAccountingText, string keycharsDocumentNumber)
        {
            Sender = sender;
            KeywordAccountingText = keywordAccountingText;
            KeycharsDocumentNumber = keycharsDocumentNumber;
        }

        public override string ToString() {
            return "'condition': {"
                    + "'Sender':'" + Sender + "',"
                    + "'KeywordAccountingText':'" + KeywordAccountingText +"',"
                    + "'KeycharsDocumentNumber':'" + KeycharsDocumentNumber + "'"
                    + "}";
        }


    }
    #endregion
    
    #region Attribution
    public class Attribution {
        public string Department;
        public string ContactPerson;
        public int ProjectNumber;
        public string Account;
        public string CostCenter;

        public Attribution()
        {
            Department = ContactPerson = Account = CostCenter = @"";
            ProjectNumber = 0;
        }

        public Attribution(string department, string contactPerson, int projectNumber, string account, string costCenter) {
            Department = department;
            ContactPerson = contactPerson;
            ProjectNumber = projectNumber;
            Account = account;
            CostCenter = costCenter;
        }   

        public override string ToString() {
            return "'attribution': {"
                + "'Department':'" + Department + "',"
                + "'ContactPerson':'" + ContactPerson + "',"
                + "'ProjectNumber':'" + ProjectNumber + "',"
                + "'Account':'" + Account + "',"
                + "'CostCenter':'" + CostCenter + "',"
                + "}";
        }
    }
    #endregion
}