using MovieSearch.MovieDTO;
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

namespace MovieSearch
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Repository repository = new Repository();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void buttonSearch_Click(object sender, RoutedEventArgs e)
        {

            var results = await repository.GetResults(textBoxQuery.Text);
            listBoxResults.ItemsSource = results;
        }

        private async void listBoxResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var item = listBoxResults.SelectedItem as Movie;
            var results = await repository.GetProperResults(item.Title);

            listBoxSecond.Items.Add(String.Format("{0}: Year: {1}, Runtime {2}, Plot {3}", results.Title, results.Year, results.Runtime, results.Plot));
         
        }
    }
}
