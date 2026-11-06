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

        public void SortByGroup()                                   // метод сортування; public, щоб викликати з вікна; void, бо нічого не повертає
        {
            var sorted = Students                                   // беремо поточну видиму колекцію студентів
                .OrderBy(s => s.GroupNumber,                        // сортуємо за зростанням; s - один студент, ключ сортування - його група
                         StringComparer.OrdinalIgnoreCase)          // порівняння рядків без урахування регістру, як було в StudentGroupComparer
                .ToList();                                          // перетворюємо результат на новий список List<Student>
            UpdateVisibleCollection(sorted);                        // очищаємо Students і заповнюємо відсортованим списком, щоб таблиця оновилась
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
        public void SearchStudents(string searchTerm)               // searchTerm - текст, який ввів користувач у полі пошуку
        {
            if (string.IsNullOrWhiteSpace(searchTerm))              // перевіряємо, чи рядок порожній або складається лише з пробілів
            {
                ResetFilter();                                      // якщо порожній, показуємо всіх студентів
                return;                                             // виходимо з методу, далі код не виконується
            }

            string lowerTerm = searchTerm.ToLowerInvariant();       // переводимо запит у нижній регістр, щоб пошук не залежав від великих літер

            var result = _allStudents                               // шукаємо в повному списку, а не лише у видимому
                .Where(s =>                                         // Where залишає тільки тих студентів, для яких умова дає true
                    s.FullName.ToLowerInvariant().Contains(lowerTerm) ||     // ПІБ у нижньому регістрі містить запит
                    s.GroupNumber.ToLowerInvariant().Contains(lowerTerm))    // АБО група у нижньому регістрі містить запит
                .ToList();                                          // збираємо знайдених у список
            UpdateVisibleCollection(result);                        // показуємо знайдених у таблиці
        }

        // 2. Отримання списку унікальних груп
        public void ShowUniqueGroups()                              // метод без параметрів: працює з усім каталогом
        {
            var uniqueGroups = _allStudents                         // беремо всіх студентів
                .Select(s => s.GroupNumber)                         // Select (проекція): з кожного студента залишаємо тільки назву групи, виходить 10 рядків
                .Distinct()                                         // прибираємо повтори, залишається 3 різні групи
                .OrderBy(g => g)                                    // сортуємо назви груп за алфавітом
                .ToList();                                          // збираємо в List<string>

            var groupRows = uniqueGroups                            // тепер для кожної групи створюємо тимчасового студента, бо DataGrid показує лише Student
                .Select(g => new Student("ПІБ", g, 0, 0, 0))          // ПІБ "—", група g, оцінки 0; це лише для відображення
                .ToList();                                          // збираємо в List<Student>
            UpdateVisibleCollection(groupRows);                     // виводимо в таблицю; _allStudents не змінюється
        }

        // 3. Три найкращі студенти за середнім балом
        public void FindThreeBestStudents()
        {
            var best = _allStudents                                 // беремо всіх студентів
                .OrderByDescending(                                 // сортуємо від більшого до меншого
                    s => (s.Exam1 + s.Exam2 + s.Exam3) / 3.0)       // ключ сортування - середній бал; ділимо на 3.0, щоб результат був дробовий, а не цілий
                .Take(3)                                            // беремо перші 3 елементи після сортування
                .ToList();                                          // збираємо в список
            UpdateVisibleCollection(best);                          // показуємо в таблиці
        }

        // 4. Середній бал усіх студентів
        public double CalculateAverageGrade()                       // повертає число (double), а не змінює таблицю
        {
            if (_allStudents.Count == 0)                            // якщо студентів немає
                return 0;                                           // повертаємо 0, бо Average() на порожньому списку кидає виняток

            return _allStudents                                     // беремо всіх студентів
                .Average(s => (s.Exam1 + s.Exam2 + s.Exam3) / 3.0); // Average (агрегація): для кожного студента рахуємо середній бал, потім середнє з цих значень
        }

        // 5. Групування студентів за номером групи
        public void GroupStudentsByGroup()
        {
            var grouped = _allStudents                              // беремо всіх студентів
                .GroupBy(s => s.GroupNumber)                        // розкладаємо по купках: ключ купки - номер групи
                .OrderBy(g => g.Key)                                // сортуємо самі купки за ключем (назвою групи) за алфавітом
                .SelectMany(g => g)                                 // розгруповуємо: викладаємо студентів усіх купок в один плоский список
                .ToList();                                          // збираємо в List<Student>
            UpdateVisibleCollection(grouped);                       // показуємо в таблиці; студенти йдуть підряд за групами
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
