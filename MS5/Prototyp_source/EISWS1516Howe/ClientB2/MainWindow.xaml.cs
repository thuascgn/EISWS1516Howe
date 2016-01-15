using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlobalClassLibrary;

namespace ClientB2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isInitialized = false;
        public MainWindow()
        {
            InitializeComponent();

            MasterData masterData = new MasterData();

            for (int i = 0; i < masterData.Departments.Count; i++)
            {
                combDepartment.Items.Add(masterData.Departments.ElementAt(i));

            }
            combDepartment.SelectedIndex = 0;

            for (int i = 0; i < masterData.Projects.Count; i++)
            {
                combProject.Items.Add(masterData.Projects.ElementAt(i));
            }
            combProject.SelectedIndex = 0;

            for (int i = 0; i < masterData.Persons.Count; i++)
            {
                combProjectContact.Items.Add(masterData.Persons.ElementAt(i));
            }
            combProjectContact.SelectedIndex = 0;

            for (int i = 0; i < masterData.Accounts.Count; i++)
            {
                combAccount.Items.Add(masterData.Accounts.ElementAt(i));
            }
            combAccount.SelectedIndex = 0;

            for (int i = 0; i < masterData.CostCenters.Count; i++)
            {
                combCostCenter.Items.Add(masterData.CostCenters.ElementAt(i));
            }
            combCostCenter.SelectedIndex = 0;

            isInitialized = true;
        }

        

        private void combDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                MasterData.DataItem di = (MasterData.DataItem)combDepartment.SelectedItem;
                if (di.Id > 0)
                {
                    btnForward.Content = "An " + combDepartment.SelectedItem.ToString() + " weiterleiten";
                }
            }
        }
    }
}
