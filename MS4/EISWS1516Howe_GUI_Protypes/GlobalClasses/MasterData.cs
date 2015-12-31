using System;
using System.Collections.Generic;

namespace GlobalClasses
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

            Projects.Add(new DataItem(0000, "..."));
            Projects.Add(new DataItem(2001, "Anuga Köln 2016"));
            Projects.Add(new DataItem(2002, "Aufbau Zweigstelle Hamburg"));
            Projects.Add(new DataItem(2003, "Integration Fliessband C"));

            Accounts.Add(new DataItem(0000, "..."));
            Accounts.Add(new DataItem(3001, "Personalaufwand"));
            Accounts.Add(new DataItem(3002, "Materialaufwand"));
            Accounts.Add(new DataItem(3003, "Lieferungen & Leistungen"));

            CostCenters.Add(new DataItem(0000, "..."));
            CostCenters.Add(new DataItem(4001, "Transport"));
            CostCenters.Add(new DataItem(4002, "Dienstleistungen"));
            CostCenters.Add(new DataItem(4003, "Produktion"));

            Persons.Add(new DataItem(0000, "..."));
            Persons.Add(new DataItem(5001, "Kiesewetter, Susanne"));
            Persons.Add(new DataItem(5002, "Ünel, Birol"));
            Persons.Add(new DataItem(5003, "Januzaj, Svenja"));
            Persons.Add(new DataItem(5004, "Fillipi, Thomas"));
            Persons.Add(new DataItem(5005, "Chen, Katie"));
            Persons.Add(new DataItem(5006, "Hübecker, Julia"));

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
