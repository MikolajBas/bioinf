using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Diagnostics;

namespace bioinformatyka
{
    /// <summary>
    /// Interaction logic for Blast.xaml
    /// </summary>
    public partial class Blast : Window
    {
        private const string RUN_URL = "http://www.ebi.ac.uk/Tools/services/rest/ncbiblast/run";
        private const string STATUS_URL = "http://www.ebi.ac.uk/Tools/services/rest/ncbiblast/status/";

        public Blast()
        {
            InitializeComponent();
            var programContent = new ObservableCollection<string>(new List<string> { "blastp", "blastx", "blastn"});
            program.ItemsSource = programContent;
            var databaseContent = new ObservableCollection<string>(new List<string> { "uniprotkb", "uniref100", "epop", "nrpl2", "pdb", "em_rel" });
            database.ItemsSource = databaseContent;
            var stypeContent = new ObservableCollection<string>(new List<string> { "dna", "rna", "protein"});
            stype.ItemsSource = stypeContent;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(email.Text) && !String.IsNullOrWhiteSpace(database.Text) &&
                 !String.IsNullOrWhiteSpace(program.Text) && !String.IsNullOrWhiteSpace(sequence.Text) &&
                 !String.IsNullOrWhiteSpace(stype.Text)) {
                validation.Content = "Please wait...";
                var dict = new Dictionary<string, string>();
                dict.Add(email.Name, email.Text);
                dict.Add(database.Name, database.Text);
                dict.Add(program.Name, program.Text);
                dict.Add(stype.Name, stype.Text);
                dict.Add(sequence.Name, sequence.Text);
                var content = new FormUrlEncodedContent(dict);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.PostAsync(RUN_URL, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Stream stream = response.Content.ReadAsStreamAsync().Result;
                    StreamReader reader = new StreamReader(stream);
                    string job_id = reader.ReadToEnd();

                    string status = "";
                    HttpResponseMessage result;
                    while (!status.Equals("FINISHED"))
                    {
                        System.Threading.Thread.Sleep(20000);
                        result = client.GetAsync(STATUS_URL + job_id).Result;
                        Stream stream1 = result.Content.ReadAsStreamAsync().Result;
                        reader = new StreamReader(stream1);
                        status = reader.ReadToEnd();
                        Debug.WriteLine(status);
                    }
                    Result r = new Result(job_id);
                    //Result r = new Result("ncbiblast-R20161220-152022-0033-48463228-pg");

                    r.Show();
                 this.Close();
            }
        } else
            {
                validation.Content = "Please fill all fields before proceed";
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                Debug.WriteLine(filename);
                if (File.Exists(filename))
                {
                    string to;
                    to = File.ReadAllText(filename);
                    sequence.Text = to;
                }
            }
        }
    }
}
