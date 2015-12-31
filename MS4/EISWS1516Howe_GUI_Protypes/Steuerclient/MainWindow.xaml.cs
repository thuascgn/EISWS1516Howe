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

namespace Steuerclient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> ListViewList;
        List<int> ListBoxList;


        public MainWindow()
        {
            InitializeComponent();

            ListViewList = new List<String>();
            ListViewList.Add("Reis");
            ListViewList.Add("Zucker");
            ListViewList.Add("Wasser");
            ListViewList.Add("Milch");
            ListViewList.Add("Apfel");
            ListViewList.Add("Zimt");

            listView.ItemsSource = ListViewList;

            ListBoxList = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                //ListBoxItem lbi = new ListBoxItem();
                //lbi.Resources = i;
                //lbi.Re
                ListBoxList.Add(i); 
            }
            
            listBox.ItemsSource = ListBoxList;

            listBox.UpdateLayout();
            listView.UpdateLayout();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("listView: " + listView.SelectedItem.ToString());
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("listView: " + listBox.SelectedItem.ToString());
        }
    }
}
