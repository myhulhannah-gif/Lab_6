using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Lab_6
{
    public class StudentCatalog
    {
        private readonly IFileSaver _fileSaver;
        private readonly IFileLoader _fileLoader;

        // Публічна колекція для відображення в інтерфейсі
        public ObservableCollection<Student> Students { get; }

        // Приватний повний список
        private List<Student> _allStudents;

        public StudentCatalog(IFileSaver fileSaver, IFileLoader fileLoader)
        {
            _fileSaver = fileSaver;
            _fileLoader = fileLoader;
            Students = new ObservableCollection<Student>();
            _allStudents = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            Students.Add(student);
            _allStudents.Add(student);
        }

        public void DeleteStudent(Student student)
        {
            Students.Remove(student);
            _allStudents.Remove(student);
        }

        public void SortByGroup() // метод сортування
        {
            var sorted = Students
                .OrderBy(s => s.GroupNumber, // сортуємо за зростанням
                         StringComparer.OrdinalIgnoreCase)
                .ToList();
            UpdateVisibleCollection(sorted);
        }

        public void FilterByMinGrade(int minGrade)
        {
            var filtered = _allStudents.Where(s => s.HasPassedSession(minGrade)).ToList();
            UpdateVisibleCollection(filtered);
        }

        public void ResetFilter()
        {
            UpdateVisibleCollection(_allStudents);
        }

        // 1. Пошук за частиною ПІБ або групи
        public void SearchStudents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ResetFilter();
                return;
            }

            string lowerTerm = searchTerm.ToLowerInvariant(); // переводимо запит у нижній регістр

            var result = _allStudents
                .Where(s =>
                    s.FullName.ToLowerInvariant().Contains(lowerTerm) ||
                    s.GroupNumber.ToLowerInvariant().Contains(lowerTerm))
                .ToList();
            UpdateVisibleCollection(result);
        }

        // 2. Отримання списку унікальних груп
        public void ShowUniqueGroups()
        {
            var uniqueGroups = _allStudents
                .Select(s => s.GroupNumber) // з кожного студента залишаємо тільки назву групи
                .Distinct() // прибираємо повтори
                .OrderBy(g => g) // за алфавітом
                .ToList();

            var groupRows = uniqueGroups
                .Select(g => new Student("ПІБ", g, 0, 0, 0))
                .ToList();
            UpdateVisibleCollection(groupRows);
        }

        // 3. Три найкращі студенти за середнім балом
        public void FindThreeBestStudents()
        {
            var best = _allStudents
                .OrderByDescending(                                 // за спаданням
                    s => (s.Exam1 + s.Exam2 + s.Exam3) / 3.0)
                .Take(3)
                .ToList();
            UpdateVisibleCollection(best);
        }

        // 4. Середній бал усіх студентів
        public double CalculateAverageGrade()
        {
            if (_allStudents.Count == 0)
                return 0;

            return _allStudents
                .Average(s => (s.Exam1 + s.Exam2 + s.Exam3) / 3.0);
        }

        // 5. Групування студентів за номером групи
        public void GroupStudentsByGroup()
        {
            var grouped = _allStudents
                .GroupBy(s => s.GroupNumber)
                .OrderBy(g => g.Key)                                // сортуємо за назвою групи за алфавітом
                .SelectMany(g => g) 
                .ToList();
            UpdateVisibleCollection(grouped);
        }

        private void UpdateVisibleCollection(List<Student> newList)
        {
            Students.Clear();
            foreach (var student in newList)
            {
                Students.Add(student);
            }
        }

        public async Task SaveAsync(string filePath)
        {
            await _fileSaver.SaveAsync(filePath, _allStudents);
        }

        public async Task LoadAsync(string filePath)
        {
            var loaded = await _fileLoader.LoadAsync<Student>(filePath);
            if (loaded != null)
            {
                _allStudents = loaded;
                UpdateVisibleCollection(_allStudents);
            }
        }


    }
}
