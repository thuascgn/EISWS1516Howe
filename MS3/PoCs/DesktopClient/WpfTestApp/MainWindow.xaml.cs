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

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<String> items;
        public string entryResult;
        public Boolean withRadio;
        public Boolean withCheck;

        public MainWindow()
        {
            InitializeComponent();

            items = new List<string>();
            items.Add("Eins");
            items.Add("Zwei");
            items.Add("Drei");

            LoadData();
        }

        private void LoadData()
        {
            button.Content = "Speichern";
            entryResult = "";

            comboBox.ItemsSource = items;
            comboBox.UpdateLayout();

            tbProjektnummer.Text = "";
            tbVerantwortlicher.Text = "";
            pgBar.Value = 0;
            pgBar.UpdateLayout();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Dér buto^n geklickt");
            gatherResult();
            updateProgressBar();
        }

        private void gatherResult()
        {
            textBlock.Text = "";
            
            entryResult += "{ ";
            entryResult += "Anzahl: " + comboBox.SelectedItem + ", ";
            entryResult += lblVerwantwortlicher.Content + ": " + tbVerantwortlicher.Text + ",";
            entryResult += lblProjektnummer.Content + ": " + tbProjektnummer.Text + ", ";
            entryResult += "mitRadio " + ": " + withRadio.ToString() + ", ";
            entryResult += "includeCheck" + ": " + withCheck.ToString();
            entryResult += " }";texmaker


            textBlock.Text = entryResult;
            textBlock.UpdateLayout();
        }

        private void radioButton_Clicked(object sender, RoutedEventArgs e)
        {
            withRadio = (bool)radioButton.IsChecked;
        }

        private void cbOneBool_Clicked(object sender, RoutedEventArgs e)
        {
            withCheck = (bool)cbOneBool.IsChecked;
        }

        private void tb_OnTouchEnter(object sender, TouchEventArgs e)
        {
            updateProgressBar();
        }

        private void updateProgressBar() {
            int k = 0;
            k += (tbProjektnummer.Text != "") ? 50 : 0;
            k += (tbVerantwortlicher.Text != "") ? 50 : 0;
            pgBar.Value = k;
            pgBar.UpdateLayout();
        }
    }
}
