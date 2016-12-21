using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace bioinformatyka
{
    /// <summary>
    /// Interaction logic for Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        private string job_id;
        private const string URL = "http://www.ebi.ac.uk/Tools/services/rest/ncbiblast/result/";
        private Stream contentStream;
        public Result(string job_id)
        {
            this.job_id = job_id;
            InitializeComponent();
            var resultContent = new ObservableCollection<string>(new List<string> { "xml", "visual-jpg", "complete-visual-jpg", "ffdp-query-jpeg", "ffdp-subject-jpeg" });
            resulttype.ItemsSource = resultContent;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            string uri = URL + job_id + "/" + resulttype.Text;
            HttpResponseMessage result = client.GetAsync(uri).Result;
            if (result.IsSuccessStatusCode)
            {
                contentStream = result.Content.ReadAsStreamAsync().Result;
                if (resulttype.Text.Equals("xml"))
                {
                    StreamReader reader = new StreamReader(contentStream);
                    string text = reader.ReadToEnd();
                    TextBlock content = new TextBlock();
                    content.Text = text;
                    content.TextWrapping = TextWrapping.Wrap;
                    scroller.Content = content;
                }
                else
                {
                    Image image = new Image();
                    image.Height = scroller.Height;
                    image.Width = scroller.Width;
                    image.Margin = scroller.Margin;
                    image.Source = BitmapFrame.Create(contentStream,
                                          BitmapCreateOptions.None,
                                          BitmapCacheOption.OnLoad);
                    scroller.Content = image;
                }
            } 

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if(contentStream != null)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    var fileStream = File.Create(filename);
                    contentStream.Seek(0, SeekOrigin.Begin);
                    contentStream.CopyTo(fileStream);
                    fileStream.Close();
                }
            }
        }
    }
}
