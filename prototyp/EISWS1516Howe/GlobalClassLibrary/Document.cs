using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace GlobalClassLibrary
{
    public class Document
    {
        public int Id;
        public Guid DocumentGuid;
        public String Path;
        public String Sender;
        public String Number;
        public String AccountingText;

        public Document() { }

        public override string ToString()
        {
            return "{ Id: " + Id.ToString() + ", DocumentGuid: " + DocumentGuid +
                ", Path: " + Path + ", Sender: " + Sender + ", Number: " + Number + ", 'AccountingText': '" + AccountingText + "'}";
        }


        public string ToJson() {
            MemoryStream memstr = new MemoryStream();
            var jser = new DataContractJsonSerializer(typeof(Document));
            jser.WriteObject(memstr, this);
            memstr.Position = 0;
            var sr = new StreamReader(memstr);
            string json = sr.ReadToEnd();

            return json;
        }

        public Document FromJson() {
            return new Document();
        }

        public Document FromXml(string file) {

            Document document = new Document();
            document.DocumentGuid = Guid.NewGuid();
            XmlTextReader xmlReader;
            try
            {
                xmlReader = new XmlTextReader(file);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            string currentKey = "";
            string currentValue = "";
            string lastKey = "";
            string lastValue = "";
            bool keyChecked = false;
            bool valChecked = false;
            bool nodeChecked = false;

            if (xmlReader != null)
            {
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentKey = xmlReader.Name;
                            keyChecked = true;
                            break;
                        case XmlNodeType.Text:
                            currentValue = xmlReader.Value;
                            valChecked = true;
                            break;
                        case XmlNodeType.EndElement:
                            nodeChecked = keyChecked = valChecked = true;
                            break;
                        default: break;
                    }

                    nodeChecked = keyChecked && valChecked;

                    if (nodeChecked)
                    {
                        if (!currentKey.Equals("") && !currentValue.Equals(""))
                        {
                            if (!currentKey.Equals(lastKey))
                            {
                                switch (currentKey)
                                {
                                    case "Sender":
                                        document.Sender = currentValue;
                                        break;
                                    case "DocumentNumber":
                                        document.Number = currentValue;
                                        break;
                                    case "AccountingText":
                                        document.AccountingText = currentValue;
                                        break;
                                    default: break;
                                }
                            }

                            lastKey = currentKey;
                            lastValue = currentValue;
                            currentKey = currentValue = "";
                        }
                        else if (!currentKey.Equals("Data") && !currentKey.Equals("") && currentValue.Equals(""))
                        {
                            lastKey = currentKey;
                            lastValue = currentValue;
                            currentKey = currentValue = "";
                        }
                        nodeChecked = keyChecked = valChecked = false;
                    }
                }
            }
            else {
                document = null;
            }

            xmlReader.Close();
            xmlReader.Dispose();

            return document;
        }
    }
}