using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

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
    }
}