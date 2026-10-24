using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void SortByGroup()
        {
            var sorted = new List<Student>(Students);
            sorted.Sort(new StudentGroupComparer());
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
