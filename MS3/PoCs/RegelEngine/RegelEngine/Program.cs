using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml;


namespace SimpleImportTest
{
    class Program
    {

        static void Main(string[] args)
        {
            string import = "C:\\Users\\Import";
            string export = "C:\\Users\\Export";
            string newPath = "";
            string[] dirs;
            string[] files;
            string filename;
            string extension;
            string currentKey;
            string currentValue;
            bool fileIsData = false;
            bool fileIsCondition = false;
            bool fileIsAttribution = false;
            bool ruleMatch = false;
            XmlTextReader xmlReader;

            //@todo ggf bessere datenstruktur, zuviele explizite iterationen
            List<RulePair> dataSet = new List<RulePair>();
            List<RulePair> ruleConditions = new List<RulePair>();
            List<RulePair> ruleAttribution = new List<RulePair>();
            List<RulePair> dataSetExport = new List<RulePair>();

            //
            //@Todo: in eigene Methode auslagern, bei exception > neustart 
            try {
                while (true)
                {
                    if (Directory.Exists(import))
                    {
                        Console.WriteLine("exists: " + import);
                        dirs = Directory.GetDirectories(import);

                        if (dirs.Length > 0)
                        {
                            foreach (string dir in dirs)
                            {
                                files = Directory.GetFiles(dir);
                                if (files != null)
                                {
                                    Guid guid = Guid.NewGuid();

                                    #region iterate files, build data structures
                                    foreach (string file in files)
                                    {
                                        filename = Path.GetFileNameWithoutExtension(file);
                                        extension = Path.GetExtension(file);
                                        currentKey = currentValue = "";

                                        //Console.WriteLine(file);

                                        fileIsData = filename.Equals("data");
                                        fileIsCondition = filename.Equals("conditions");
                                        fileIsAttribution = filename.Equals("attributions");
                                        String lastKey = "";
                                        String lastValue = "";
                                        bool keyChecked = false;
                                        bool valChecked = false;
                                        bool nodeChecked = false;

                                        if ( extension.Equals(".xml") && (fileIsData || fileIsCondition || fileIsAttribution) ) {
                                            xmlReader = new XmlTextReader(file);
                                            

                                            #region xmlReader
                                            while (xmlReader.Read()) {
                                                switch (xmlReader.NodeType) {
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
                                                        if (fileIsData && !currentKey.Equals(lastKey))
                                                        {
                                                            //Console.WriteLine("data        :   Key / Value :  " + currentKey + " : " + currentValue);
                                                            dataSet.Add(new RulePair(currentKey, currentValue));
                                                        }
                                                        else if (fileIsCondition)
                                                        {
                                                            //Console.WriteLine("condition   :   Key / Value :  " + currentKey + " : " + currentValue);
                                                            ruleConditions.Add(new RulePair(currentKey, currentValue));
                                                        }
                                                        else if (fileIsAttribution)
                                                        {
                                                            //Console.WriteLine("attribution :   Key / Value :  " + currentKey + " : " + currentValue);
                                                            ruleAttribution.Add(new RulePair(currentKey, currentValue));
                                                        }
                                                        lastKey = currentKey;
                                                        lastValue = currentValue;
                                                        currentKey = currentValue = "";
                                                    }
                                                    else if (fileIsData && !currentKey.Equals("Data") && !currentKey.Equals("") && currentValue.Equals(""))
                                                    {
                                                        // lastKey != "" && lastValue != "" &&
                                                        //leere felder aus data behalten
                                                        dataSet.Add(new RulePair(currentKey, currentValue));
                                                        lastKey = currentKey;
                                                        lastValue = currentValue;
                                                        currentKey = currentValue = "";
                                                    }
                                                    nodeChecked = keyChecked = valChecked = false;
                                                }

                                                
                                            }

                                            #endregion xmlReader
                                        }
                                    }
                                    #endregion iterate files
                                    #region check
                                    //@todo hier nur test: verschieben an vorgelagerte stelle 

                                    //@todo erst in conditions dann in data, weil condition immer kürzer
                                    foreach (RulePair dataRp in dataSet) {
                                        foreach (RulePair conditionRp in ruleConditions) {
                                            if (dataRp.Attribute.Equals(conditionRp.Attribute)) {
                                                // Volltext mit Contains
                                                // andere Attribute mit Equals
                                                if (!dataRp.Attribute.Equals("Volltext"))
                                                {
                                                    if (dataRp.Assign.Equals(conditionRp.Assign))
                                                    {
                                                        //Console.WriteLine("Equals");
                                                        ruleMatch = true; 
                                                    }
                                                } else if (dataRp.Attribute.Equals("Volltext"))
                                                {
                                                    //Console.WriteLine("Volltext");
                                                    //Console.WriteLine("data:");
                                                    //Console.WriteLine(dataRp.Assign);
                                                    //Console.WriteLine("condition:");
                                                    //Console.WriteLine(conditionRp.Assign);
                                                    if (dataRp.Assign.Contains(conditionRp.Assign))
                                                    {
                                                        Console.WriteLine("Contains");
                                                        ruleMatch = true; 
                                                    }
                                                }
                                                else {
                                                    ruleMatch = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion check
                                    #region match
                                    // Zuweisen der Regelattribute zur exportierenden Struktur
                                    //
                                    if (ruleMatch) {
                                        foreach (RulePair dataPairs in dataSet) {
                                            if (dataPairs.Assign == "")
                                            {
                                                foreach (RulePair rulePairs in ruleAttribution)
                                                {
                                                    if (dataPairs.Attribute.Equals(rulePairs.Attribute))
                                                    {
                                                        dataSetExport.Add(new RulePair(dataPairs.Attribute, rulePairs.Assign));
                                                    }
                                                }
                                            }
                                            else {
                                                dataSetExport.Add(new RulePair(dataPairs.Attribute, dataPairs.Assign));
                                            }
                                        }
                                    }

                                    #endregion

                                    Console.WriteLine("Export: ");
                                    foreach (RulePair rp in dataSetExport) {
                                        Console.WriteLine(rp.Attribute + ", " + rp.Assign);
                                    }



                                    #region xmlWriter
                                    
                                    newPath = export + "\\" + guid.ToString();
                                    Directory.CreateDirectory(newPath);

                                    

                                    XmlWriter writer = XmlWriter.Create((newPath + "\\" + "dataexport.xml"));

                                    writer.WriteStartDocument();
                                    writer.WriteStartElement("Data");
                                    foreach (RulePair dR in dataSetExport)
                                    {
                                        //writer.WriteStartElement(dR.Attribute);
                                        writer.WriteElementString(dR.Attribute , dR.Assign);
                                        //writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                    writer.WriteEndDocument();
                                    writer.Flush();
                                    writer.Close();
                                    #endregion

                                    //Console.Read();

                                    Directory.Delete(dir, true);
                                }
                                else
                                {
                                    //leerer Ordner
                                    //
                                    Console.WriteLine("empty folder");
                                    Thread.Sleep(2500);
                                    if (Directory.GetFiles(dir) == null) {
                                        Console.Write("Delete Directory: " + dir);
                                        Directory.Delete(dir, true);
                                    }

                                }
                            } 
                        }
                    }
                    Thread.Sleep(2500);
                }
                //Console.Read();
            }
            catch(Exception e) {
                throw e;
            }  
        }
    }

    #region Helper Class
    //@todo: auslagern
    public class RulePair {
        public string Attribute;
        public string Assign;
        public RulePair(string att, string ass) {
            Attribute = att;
            Assign = ass;
        }
    }
    #endregion
}
