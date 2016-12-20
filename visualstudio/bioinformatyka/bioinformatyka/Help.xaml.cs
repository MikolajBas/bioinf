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

namespace bioinformatyka
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        Dictionary<string, string> ParametersDescriptions;
        public Help()
        {
            InitializeComponent();
            ParametersDescriptions = new Dictionary<string, string>();
            ParametersDescriptions["email"] = "jest konieczny aby zapytanie zostało dodane do listy zadań.";
            ParametersDescriptions["stype"] = "dostępne możliwości: dna, rna, protein.";
            ParametersDescriptions["sequence"] = "sekwencja jaką chcemy badać.";
            ParametersDescriptions["program"] = "program blastowy, który posłuży nam do porównania zadanej sekwencji.";
            var cmbContent = new ObservableCollection<string>(ParametersDescriptions.Keys);
            ParametersComboBox.ItemsSource = cmbContent;
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            string param = comboBox.SelectedItem as string;
            Description.Content = new TextBlock() { Text = "Description for " + param + ": " + ParametersDescriptions[param], TextWrapping = TextWrapping.Wrap };
        }
    }
}
