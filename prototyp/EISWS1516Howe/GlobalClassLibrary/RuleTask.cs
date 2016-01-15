using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace GlobalClassLibrary
{
    [Serializable]
    public class RuleTask
    {
        public int Id;
        public int Priority;
        public string Department;
        public Boolean IsLocked;
        public Document Document;
        public Rule Rule;

        public RuleTask()
        {
            Id = Priority = 0;
            Department = "";
            IsLocked = false;
            Document = new Document();
            Rule = new Rule();
            //MGuid = Channel = Sender = Department = ContactPerson = FullText = @"";
        }

        public RuleTask(int id, int priority, string channel, Boolean isLocked, Document document, Rule rule)
        {
            Id = id;
            Priority = priority;
            Department = channel.ToLower();
            IsLocked = isLocked;
            Document = document;
            Rule = rule;
        }

        public RuleTask(Document document) {
            Document = document;     
        }
        
        //public override String ToString() {
        //    return "Id: " + Id + ", Priority: " + Priority.ToString() + ", Department: " + Department + ", isLocked: " + IsLocked.ToString() +
        //        ", Document: " + Document.ToString() + ", Rule: " + Rule.ToString();
        //}

        public String ToJson() {
            MemoryStream memstr = new MemoryStream();
            var jser = new DataContractJsonSerializer(typeof(RuleTask));
            jser.WriteObject(memstr, this);
            memstr.Position = 0;
            var sr = new StreamReader(memstr);
            string rt_json = sr.ReadToEnd();

            return rt_json;
        }

        public RuleTask FromJson(string json)
        {
            var memstr = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var jser = new DataContractJsonSerializer(typeof(RuleTask));

            return jser.ReadObject(memstr) as RuleTask;
        }


    }
}
