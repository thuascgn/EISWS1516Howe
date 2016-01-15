using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlobalClassLibrary;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Timers;

namespace ClientB2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string resourcePost;
        private string resourceGet;
        //public string response;
        private RuleTask ruleTask;
        private bool sendAndGetNext;
        public bool isInitialized = false;
        public bool isLoaded = false;
        MasterData masterData;
        HttpClient client;
        HttpResponseMessage response;

        public string TaskServiceBaseUri;
        public string TaskServiceEndpoint;
        public string RuleServiceBaseUri;
        public string RuleServiceEndpoint;
        public string department;
        public String currentStatus;

        public MainWindow()
        {
            
            InitializeComponent();
            InitMasterData();

            department = "logistik";
            currentStatus = "Client lädt...";

            TaskServiceBaseUri = @"http://localhost:55121/";
            TaskServiceEndpoint = @"api/tasks";
            RuleServiceBaseUri = @"http://localhost:55122/";
            RuleServiceEndpoint = @"api/rules";

            tbAccountingText.Text = tbDocumentNumber.Text = tbSender.Text = tbKeycharsDocumentNumber.Text = tbKeywordAccountingText.Text = @"";

            isInitialized = true;

            //var _timer = new System.Timers.Timer();
            //_timer.Elapsed += new ElapsedEventHandler(_timer_elapsed);
            //_timer.Enabled = true;
            
            //System.Timers.Timer _timer = new System.Timers.Timer();
            //_timer.Interval = 30000;
            //_timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            //InitCommunication();
            //_timer.Enabled = true;

            isInitialized = true;

            GuiAccess();
        }


        //public static Task WaitSleep() {
        //    return Task.Run(() =>
        //    {
        //        Thread.Sleep(15000);
        //    });
        //}

        //private async Task PollAsnyc(MainWindow main)
        //{
        //    Thread.Sleep(10000);
        //    client = new HttpClient();
        //    Task<HttpResponseMessage> TaskReponse;
        //    HttpResponseMessage ResponseTask = new HttpResponseMessage();

        //    while (true) {
        //        Thread.Sleep(10000);
        //        TaskReponse = ProcessAsync(TaskServiceBaseUri + "api/status/" + department, client,  main);

        //        ResponseTask = await TaskReponse;

        //        //await WaitSleep();
                
        //    }  


        //}
        //async Task<HttpResponseMessage> ProcessAsync(string uri, HttpClient client, MainWindow main) {
        //    //HttpResponseMessage response = await client.GetAsync(uri);
        //    HttpResponseMessage response = GetSync(TaskServiceBaseUri, "api/status/" + department);
        //    DisplayAsyncResult(response, main);
        //    return response;
        //}

        //private void DisplayAsyncResult(HttpResponseMessage response, MainWindow main)
        //{

        //    if (response != null)
        //    {
        //        currentStatus = response.Content.ToString();

        //        if (response.StatusCode.Equals(HttpStatusCode.NotFound))
        //        {
        //            currentStatus += " Neuer Versuch in 30 sekunden";
        //        }
        //    }
        //    else {
        //        main.tbStatus.Text = currentStatus = "kein status repsonse";
        //    }

        //    if (!IsLoaded) {
        //        main.tbStatus.Text = currentStatus;
        //    }

        //}




        #region GUI Interaction

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = "Laden";

            HttpResponseMessage response = GetSync(TaskServiceBaseUri, TaskServiceEndpoint + "/" + department );

            if (response != null)
            {
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    ruleTask = new RuleTask().FromJson(response.Content.ReadAsStringAsync().Result);

                    tbSender.Text = ruleTask.Document.Sender;
                    tbDocumentNumber.Text = ruleTask.Document.Number;
                    tbAccountingText.Text = ruleTask.Document.AccountingText;
                    var dep = from item in masterData.Departments where item.Name.Equals(ruleTask.Rule.Attribution.Department) select item;
                    combDepartment.SelectedItem = dep;

                    combProject.SelectedItem = from item in masterData.Projects where item.Id.Equals(ruleTask.Rule.Attribution.ProjectNumber) select item;
                    combProjectContact.SelectedItem = from item in masterData.Projects where item.Name.Equals(ruleTask.Rule.Attribution.ContactPerson) select item;
                    combAccount.SelectedItem = from item in masterData.Projects where item.Name.Equals(ruleTask.Rule.Attribution.Account) select item;
                    combCostCenter.SelectedItem = from item in masterData.CostCenters where item.Name.Equals(ruleTask.Rule.Attribution.CostCenter) select item;
                    tbStatus.Text = "Dokument geladen - ";
                    tbStatus.Text += response.ReasonPhrase;

                    isLoaded = true;
                    GuiAccess();

                } else if(response.StatusCode.Equals(HttpStatusCode.InternalServerError)){
                    tbStatus.Text = "Die Rechnung konnte nicht geladen werden.";
                }
                else {
                    ClearInputs();
                    tbStatus.Text = response.Content.ToString();
                }
            }
            else
            {
                tbStatus.Text = "Dokument konnte nicht geladen werden.";
            }
        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ruleTask.IsLocked = false;
            HttpResponseMessage httpResponse = PutSync(TaskServiceBaseUri, TaskServiceEndpoint + "/" + ruleTask.Id, ruleTask.ToJson());

            if (httpResponse != null)
            {
                if (httpResponse.StatusCode.Equals(HttpStatusCode.OK))
                {
                    tbStatus.Text = "Bearbeitung von Dokument " + ruleTask.Document.Number + " abgebrochen.";
                }
                else if (httpResponse.StatusCode.Equals(HttpStatusCode.NotModified)) {
                    tbStatus.Text = "Bearbeitung von Dokument wurde nicht abgebrochen. Bitte noch einmal versuchen";
                }
                else {
                    //
                }
            }
            else
            {
                tbStatus.Text = "Bearbeitung von Dokument " + ruleTask.Document.Number + " wurde nicht abgebrochen.";
            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            GatherInputs();

            ruleTask.IsLocked = false;
            ruleTask.Department = ruleTask.Rule.Attribution.Department;
            HttpResponseMessage httpResponse = PutSync(TaskServiceBaseUri, TaskServiceEndpoint + "/" + ruleTask.Id, ruleTask.ToJson());

            if (httpResponse != null)
            {
                if (httpResponse.StatusCode.Equals(HttpStatusCode.OK))
                {
                    tbStatus.Text = "An " + ruleTask.Department + " weitergeleitet.";
                    ClearInputs();
                } else if (httpResponse.StatusCode.Equals(HttpStatusCode.NotModified)) {
                    tbStatus.Text = "Nicht an " + ruleTask.Department + " weitergeleitet.";
                }
                else {
                    //
                }
            }
            else {
                tbStatus.Text = "An " + ruleTask.Department + " weitergeleitet.";
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GatherInputs();

            HttpResponseMessage httpResponse = PostSync(RuleServiceBaseUri, RuleServiceEndpoint, ruleTask.ToJson());

            //string ruleJson = await httpResponse.Content.ReadAsStringAsync();
            

            if (httpResponse != null)
            {
                if (httpResponse.StatusCode.Equals(HttpStatusCode.Created))
                {
                    Rule createdRule = new Rule().FromJson(httpResponse.Content.ReadAsStringAsync().Result);

                    tbStatus.Text = "Regel für " + ruleTask.Document.Number + " gespeichert.";
                    httpResponse = null;

                    //while (httpResponse == null)
                    //{
                        httpResponse = DeleteSync(TaskServiceBaseUri, TaskServiceEndpoint + "/" + ruleTask.Id);

                        if (httpResponse != null)
                        {
                            if (httpResponse.StatusCode.Equals(HttpStatusCode.OK))
                            {
                                tbStatus.Text = httpResponse.Content.ReadAsStringAsync().Result;
                                ClearInputs();
                            }
                            else if (httpResponse.StatusCode.Equals(HttpStatusCode.NotModified))
                            {
                                //httpResponse = null;
                                //ruleTask.Id = 0;
                                //httpResponse = PostSync(RuleServiceBaseUri, RuleServiceEndpoint, ruleTask.ToJson());
                                Console.WriteLine("Dokument " + ruleTask.Document.Number + " nicht gelöscht.");
                            }
                            else {
                            httpResponse = DeleteSync(RuleServiceBaseUri, RuleServiceEndpoint + "/" + );
                            }
                        }
                        else
                        {
                            Console.WriteLine("Dokument " + ruleTask.Document.Number + " nicht gelöscht.");
                        }
                }
                else if (httpResponse.StatusCode.Equals(HttpStatusCode.Conflict)){
                    //ClearInputs();
                    tbStatus.Text = httpResponse.Content.ReadAsStringAsync().Result;
                } else {
                    tbStatus.Text = "Die Regel konnte nicht gespeichert werden.";                
                 }
            }
            else
            {
                tbStatus.Text = "Regel " + ruleTask.Document.Number + " nicht gespeichert.";
            }
        }


        private void pollStatus() {
            HttpResponseMessage httpResponse;
            //string response;

            httpResponse = GetSync(TaskServiceBaseUri, "api/status/" + department);
            //HttpClient client = new HttpClient();
            //httpResponse = client.GetASync(TaskServiceBaseUri, "api/status" + department);

            if (httpResponse != null)
            {
                    if (httpResponse.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        string amount = httpResponse.Content.ReadAsStringAsync().Result;
                        tbStatus.Text = "Es liegen " + amount + " Dokumente zur Bearbeitung vor.";
                    }
                    else
                    {
                        tbStatus.Text = "Es liegen keine Dokumente zur Bearbeitung vor.";
                    }
            } else {
                tbStatus.Text = "Status konnte nicht ermittelt werden.";
            }
        }

        #endregion


        #region Requests

        public HttpResponseMessage GetSync(string baseUri, string resource)
        {
            //string response = String.Empty;
            HttpResponseMessage httpResponse;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    httpResponse = client.GetAsync(resource).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return httpResponse;
        }
        
        static HttpResponseMessage PostSync(string baseUri, string resource, string content)
        {
            HttpResponseMessage httpResponse;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    httpResponse = client.PostAsync(resource, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return httpResponse;
        }

        static HttpResponseMessage PutSync(string baseUri, string resource, string content)
        {
            //string response = "";
            HttpResponseMessage httpResponse;

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    httpResponse = client.PutAsync(resource, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }
            return httpResponse;
        }

        static HttpResponseMessage DeleteSync(string baseUri, string resource)
        {
            HttpResponseMessage httpResponse;

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    httpResponse = client.DeleteAsync(resource).Result;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                
            }
            return httpResponse;
        }

        private void InitCommunication()
        {
            Thread.Sleep(10000);
        }

        #endregion


        #region Gui Helper
        private void showProgress()
        {
            if (isLoaded)
            {
                double f = 100;
                double v = 0;
                double a = 7;

                v += (tbKeycharsDocumentNumber.Text != "") ? 1 : 0;
                v += (tbKeywordAccountingText.Text != "") ? 1 : 0;
                v += (combAccount.SelectedIndex != 0) ? 1 : 0;
                v += (combCostCenter.SelectedIndex != 0) ? 1 : 0;
                v += (combDepartment.SelectedIndex != 0) ? 1 : 0;
                v += (combProject.SelectedIndex != 0) ? 1 : 0;
                v += (combProjectContact.SelectedIndex != 0) ? 1 : 0;

                progressBar.Value = (f / a) * v;
            }
            else
            {
                progressBar.Value = 0;
            }
        }

        private void ClearInputs() {
            tbSender.Text = tbDocumentNumber.Text = tbAccountingText.Text = tbKeycharsDocumentNumber.Text =
                tbKeywordAccountingText.Text = @"";
            combAccount.SelectedIndex = combCostCenter.SelectedIndex = combDepartment.SelectedIndex =
                combProject.SelectedIndex = combProjectContact.SelectedIndex = 0;
            btnForward.Content = @"Weiterleiten";
        }

        private void GatherInputs() {
            ruleTask.Rule.Condition.Sender = tbSender.Text;
            ruleTask.Rule.Condition.KeycharsDocumentNumber = tbKeycharsDocumentNumber.Text;
            ruleTask.Rule.Condition.KeywordAccountingText = tbKeywordAccountingText.Text;
            ruleTask.Rule.Attribution.Department = masterData.Departments.ElementAt(combDepartment.SelectedIndex).Name;
            ruleTask.Rule.Attribution.ProjectNumber = masterData.Projects.ElementAt(combProject.SelectedIndex).Name;
            ruleTask.Rule.Attribution.ContactPerson = masterData.Persons.ElementAt(combProjectContact.SelectedIndex).Name;
            ruleTask.Rule.Attribution.Account = masterData.Persons.ElementAt(combAccount.SelectedIndex).Name;
            ruleTask.Rule.Attribution.CostCenter = masterData.CostCenters.ElementAt(combCostCenter.SelectedIndex).Name;
        }

        private void ShowRuleTask()
        {
            tbSender.Text = ruleTask.Document.Sender;
            tbDocumentNumber.Text = ruleTask.Document.Number;
            tbKeycharsDocumentNumber.Text = ruleTask.Rule.Condition.KeycharsDocumentNumber;

            tbAccountingText.Text = ruleTask.Document.AccountingText;
            tbKeywordAccountingText.Text = ruleTask.Rule.Condition.KeywordAccountingText;

            var dep = from item in masterData.Departments where item.Name.Equals(ruleTask.Rule.Attribution.Department) select item;
            combDepartment.SelectedItem = dep;
            
            combProject.SelectedItem = from item in masterData.Projects where item.Id.Equals(ruleTask.Rule.Attribution.ProjectNumber) select item;
            combProjectContact.SelectedItem = from item in masterData.Projects where item.Name.Equals(ruleTask.Rule.Attribution.ContactPerson) select item;
            combAccount.SelectedItem = from item in masterData.Projects where item.Name.Equals(ruleTask.Rule.Attribution.Account) select item;
            combCostCenter.SelectedItem = from item in masterData.CostCenters where item.Name.Equals(ruleTask.Rule.Attribution.CostCenter) select item;
            tbStatus.Text = "Dokument geladen";
        }


        private void GuiAccess()
        {
            tbDocumentNumber.IsEnabled = tbAccountingText.IsEnabled = tbKeycharsDocumentNumber.IsEnabled = tbKeywordAccountingText.IsEnabled
                    = tbSender.IsEnabled = combAccount.IsEnabled = combCostCenter.IsEnabled = combDepartment.IsEnabled = combProject.IsEnabled =
                    combProjectContact.IsEnabled = btnCancel.IsEnabled = btnForward.IsEnabled = btnSave.IsEnabled = isLoaded;
            btnLoad.IsEnabled = !isLoaded;

            tbStatus.Text = currentStatus;
        }

        #endregion

        #region GUI eventListeners, GUI Interaction / Selection

        private void tbAll_GotFocus(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = "Focus: " + sender;
            DropShadowEffect dse = new DropShadowEffect();
            dse.BlurRadius = 6;
            dse.Opacity = 0.3;
            dse.Direction = 0;
            var shadCol = new Color();
            shadCol.A = 100;
            shadCol.R = shadCol.G = shadCol.B = 180;
            dse.Color = shadCol;
            TextBox ctb = (TextBox)sender;
            ctb.Effect = dse;
            ctb.UpdateLayout();
        }

        private void tbAll_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox ctb = (TextBox)sender;
            ctb.Effect = null;
            ctb.UpdateLayout();
        }

        private void combDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                MasterData.DataItem di = (MasterData.DataItem)combDepartment.SelectedItem;
                if (di.Id > 0)
                {
                    btnForward.IsEnabled = true;
                    btnForward.Content = "An " + combDepartment.SelectedItem.ToString() + " weiterleiten";
                }
                else{
                    btnForward.IsEnabled = false;
                    btnForward.Content = "Weiterleiten";
                    btnForward.ToolTip = "Bitte wählen Sie eine Abteilung aus um dieses Dokument weiterzuleiten.";
                }
            }
        }
        
        private void combProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showProgress();
        }

        private void combProjectContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showProgress();
        }

        private void combAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showProgress();
        }

        private void combCostCenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showProgress();
        }

        private void tbKeycharsDocumentNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (validateKeyCharDocumentNumber())
            {
                 
            }
            showProgress();
        }

        private void tbKeywordAccountingText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (validateKeyWordAccountingText())
            {

            }

            showProgress();
        }

        public bool validateKeyCharDocumentNumber() {
            return tbDocumentNumber.Text.Contains(tbKeycharsDocumentNumber.Text);
        }

        public bool validateKeyWordAccountingText() {
            return tbKeywordAccountingText.Text.Contains(tbKeycharsDocumentNumber.Text);
        }

        #endregion

        #region Timer EventListener
        //public void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    pollStatus();
        //}
        #endregion



        #region Stammdaten Dummy

        /// <summary>
        /// Loads MasterData into Gui
        /// </summary>
        private void InitMasterData()
        {
            masterData = new MasterData();

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
        }

        #endregion

        private void Window_Initialized(object sender, EventArgs e)
        {
            //IsInitialized = true;
            Console.WriteLine("initialised");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Loaded");
            pollStatus();
        }

        private void btnLoadStatus_Click(object sender, RoutedEventArgs e)
        {
            pollStatus();
        }
    }
}
