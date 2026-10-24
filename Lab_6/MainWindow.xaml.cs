using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab_6
{
    public partial class MainWindow : Window
    {
        private readonly StudentCatalog _catalog;

        public MainWindow()
        {
            InitializeComponent();

            _catalog = new StudentCatalog(new JsonFileWriter(), new JsonFileReader());
            StudentsDataGrid.ItemsSource = _catalog.Students;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(Exam1TextBox.Text, out int exam1) ||
                    !int.TryParse(Exam2TextBox.Text, out int exam2) ||
                    !int.TryParse(Exam3TextBox.Text, out int exam3))
                {
                    MessageBox.Show("Оцінки мають бути цілими числами.");
                    return;
                }

                var student = new Student(FullNameTextBox.Text, GroupTextBox.Text, exam1, exam2, exam3);
                _catalog.AddStudent(student);

                FullNameTextBox.Clear();
                GroupTextBox.Clear();
                Exam1TextBox.Clear();
                Exam2TextBox.Clear();
                Exam3TextBox.Clear();
            }
            catch (ArgumentException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsDataGrid.SelectedItem is Student selected)
            {
                _catalog.DeleteStudent(selected);
            }
            else
            {
                MessageBox.Show("Оберіть студента в таблиці.");
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            _catalog.SortByGroup();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MinGradeTextBox.Text, out int minGrade))
            {
                _catalog.FilterByMinGrade(minGrade);
            }
            else
            {
                MessageBox.Show("Введіть мінімальний бал числом.");
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _catalog.ResetFilter();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                FileName = "students.json"
            };

            if (dialog.ShowDialog() == true)
            {
                await _catalog.SaveAsync(dialog.FileName);
                MessageBox.Show("Збережено.");
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _catalog.LoadAsync(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не вдалося прочитати файл: " + ex.Message);
                }
            }
        }
    }
}