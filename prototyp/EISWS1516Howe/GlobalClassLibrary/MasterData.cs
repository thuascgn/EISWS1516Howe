using System;
using System.Collections.Generic;

namespace GlobalClassLibrary
{
    public class MasterData
    {
        public List<DataItem> Departments = new List<DataItem>();

        public List<DataItem> Projects = new List<DataItem>();
        public List<DataItem> Accounts = new List<DataItem>();
        public List<DataItem> CostCenters = new List<DataItem>();
        public List<DataItem> Persons = new List<DataItem>();

        public MasterData()
        {
            Departments.Add(new DataItem(0000, "..."));
            Departments.Add(new DataItem(1001, "Vetrieb"));
            Departments.Add(new DataItem(1002, "Marketing"));
            Departments.Add(new DataItem(1003, "Logistik"));
            Departments.Add(new DataItem(1004, "Produktion"));
            Departments.Add(new DataItem(1005, "Verwaltung"));
            Departments.Add(new DataItem(1999, "Sonstige"));

            Projects.Add(new DataItem(0000, "..."));
            Projects.Add(new DataItem(2001, "Marketing Messe"));
            Projects.Add(new DataItem(2002, "Marketing externe Redaktionen"));
            Projects.Add(new DataItem(2003, "Marketing/Vetrieb Promotion"));
            Projects.Add(new DataItem(2004, "Vetrieb Aufbau Zweigstelle Hamburg"));
            Projects.Add(new DataItem(2005, "Produktion Integration Fliessband C"));
            Projects.Add(new DataItem(2006, "Produktion Rohstoffe"));
            Projects.Add(new DataItem(2007, "Produktion Ersatzteile Allgemein"));
            Projects.Add(new DataItem(2008, "Logistik Lieferungen Allgemein"));
            Projects.Add(new DataItem(2009, "Logistik Ersatzteile Allgemein"));
            Projects.Add(new DataItem(2999, "Sonstige"));


            Accounts.Add(new DataItem(0000, "..."));
            Accounts.Add(new DataItem(3001, "Personalaufwand"));
            Accounts.Add(new DataItem(3002, "Materialaufwand"));
            Accounts.Add(new DataItem(3003, "Lieferungen & Leistungen"));
            Accounts.Add(new DataItem(3999, "Sonstige"));

            CostCenters.Add(new DataItem(0000, "..."));
            CostCenters.Add(new DataItem(4001, "Transport"));
            CostCenters.Add(new DataItem(4002, "Dienstleistungen"));
            CostCenters.Add(new DataItem(4003, "Produktion"));
            CostCenters.Add(new DataItem(4999, "Sonstige"));

            Persons.Add(new DataItem(0000, "..."));
            Persons.Add(new DataItem(5001, "Kiesewetter, Susanne"));
            Persons.Add(new DataItem(5002, "Ünel, Birol"));
            Persons.Add(new DataItem(5003, "Januzaj, Svenja"));
            Persons.Add(new DataItem(5004, "Fillipi, Thomas"));
            Persons.Add(new DataItem(5005, "Chen, Katie"));
            Persons.Add(new DataItem(5006, "Hambecker, Julia"));
            
        }

        /// <summary>
        /// Abteilungen
        /// </summary>
        public class DataItem
        {
            public int Id;
            public string Name;

            public DataItem(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    } 
}
