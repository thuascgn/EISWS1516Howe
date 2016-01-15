using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GlobalClassLibrary;
using System.IO;
using System.Threading;
using System.Net.Http;
using System.Net;

namespace DocumentImportSimulation
{
    class Program
    {

        public bool run;

        static void Main(string[] args)
        {
            string import = "C:\\Users\\Import";
            string export = "C:\\Users\\Export";
            string temp = "C:\\Users\\tempDocs";
            string newPath = "";
            string[] dirs;
            string[] files;
            string filename;
            string extension;
            //string currentKey;
            //string currentValue;
            bool fileIsData = false;
            //bool fileIsCondition = false;
            //bool fileIsAttribution = false;
            //bool ruleMatch = false;
            //XmlTextReader xmlReader;
            String ruleServiceBaseUri = "http://localhost:55122";
            Document document = new Document(); ;

            bool run = true;
            Console.WriteLine("Warten...");
            Thread.Sleep(40000);

            try
            {
                while (run)
                {
                    if (Directory.Exists(import))
                    {
                        Console.WriteLine("exists: " + import);
                        //dirs = Directory.GetDirectories(import);
                        files = Directory.GetFiles(import);
                        //
                        // Es stehen Dokumente zum Import bereit
                        Console.WriteLine("files " + files.Length);
                        if (files != null && files.Length > 0)
                        {
                            #region iterate files, build data structures
                            foreach (string file in files)
                            {
                                filename = Path.GetFileNameWithoutExtension(file);
                                extension = Path.GetExtension(file);
                                fileIsData = filename.Contains("data");
                                
                                if (extension.Equals(".xml") && (fileIsData))
                                {
                                    try
                                    {
                                        document = new Document().FromXml(file);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }

                                if (document!=null) {
                                    Console.WriteLine(document.ToString());

                                    Request req = new Request();
                                    HttpResponseMessage res = new HttpResponseMessage();
                                    res = req.PostSync(ruleServiceBaseUri, "api/documents", document.ToJson());
                                    if (res.StatusCode.Equals(HttpStatusCode.Created) || res.StatusCode.Equals(HttpStatusCode.OK) ) {
                                        //File.Delete(file);
                                        File.Move(file, temp + "\\" + document.DocumentGuid + "_" + filename + extension);
                                    }
                                    else {
                                        Console.WriteLine("Import wiederholen, " + document.Number);
                                    }

                                }

                                Thread.Sleep(15000);
                            }
                            #endregion iterate files
                        } else {
                            //leerer Ordner
                            //
                            Console.WriteLine("empty folder");
                            Thread.Sleep(5000);
                            //if (Directory.GetFiles(dir) == null)
                            //{
                            //    Console.Write("Delete Directory: " + dir);
                            //    Directory.Delete(dir, true);
                            //}
                        }
                        Thread.Sleep(15000);
                    }
                    else { 
                        Console.Write("No Import Directory");
                        Thread.Sleep(30000);
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }


        }
    }
}
