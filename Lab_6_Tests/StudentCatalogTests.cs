using Lab_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6_Tests
{
    public class FakeSaver : IFileSaver
    {
        public string? SavedPath;
        public object? SavedData;

        public Task SaveAsync<T>(string filePath, List<T> data)
        {
            SavedPath = filePath;
            SavedData = data;
            return Task.CompletedTask;
        }
    }

    // Повертає заздалегідь заданий список
    public class FakeLoader : IFileLoader
    {
        public object? DataToReturn;

        public Task<List<T>?> LoadAsync<T>(string filePath)
        {
            return Task.FromResult(DataToReturn as List<T>);
        }
    }

    [TestClass]
    public class StudentCatalogTests
    {
        private StudentCatalog CreateCatalog()
        {
            return new StudentCatalog(new FakeSaver(), new FakeLoader());
        }

        // Після додавання студент є в Students
        [TestMethod]
        public void AddStudent_AddsStudentToVisibleList()
        {
            var catalog = CreateCatalog();
            var student = new Student("Іваненко", "КН-32", 80, 80, 80);

            catalog.AddStudent(student);

            Assert.AreEqual(1, catalog.Students.Count);
            Assert.AreSame(student, catalog.Students[0]);
        }

        // Після видалення студента в списку немає
        [TestMethod]
        public void DeleteStudent_RemovesStudent()
        {
            var catalog = CreateCatalog();
            var student = new Student("Іваненко", "КН-32", 80, 80, 80);
            catalog.AddStudent(student);

            catalog.DeleteStudent(student);

            Assert.AreEqual(0, catalog.Students.Count);
        }

        // Сортування розставляє студентів за зростанням групи
        [TestMethod]
        public void SortByGroup_OrdersStudentsByGroup()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Сидоренко", "КН-32", 80, 80, 80));
            catalog.AddStudent(new Student("Іваненко", "КН-31", 80, 80, 80));
            catalog.AddStudent(new Student("Петренко", "АНД-31", 80, 80, 80));

            catalog.SortByGroup();

            Assert.AreEqual("АНД-31", catalog.Students[0].GroupNumber);
            Assert.AreEqual("КН-31", catalog.Students[1].GroupNumber);
            Assert.AreEqual("КН-32", catalog.Students[2].GroupNumber);
        }

        // Фільтр залишає в Students лише тих, хто склав іспити
        [TestMethod]
        public void FilterByMinGrade_ShowsOnlyPassedStudents()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Сидоров", "КН-31", 85, 90, 78));
            catalog.AddStudent(new Student("Петров", "КН-32", 85, 50, 90));

            catalog.FilterByMinGrade(60);

            Assert.AreEqual(1, catalog.Students.Count);
            Assert.AreEqual("Сидоров", catalog.Students[0].FullName);
        }

        // Зміна фільтра змінює список
        [TestMethod]
        public void FilterByMinGrade_AfterStricterFilter_UsesFullList()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Сидоров", "КН-31", 95, 92, 90));
            catalog.AddStudent(new Student("Петров", "КН-32", 85, 90, 78));

            catalog.FilterByMinGrade(90);
            Assert.AreEqual(1, catalog.Students.Count);

            catalog.FilterByMinGrade(60);
            Assert.AreEqual(2, catalog.Students.Count);
        }

        // ResetFilter повертає всіх студентів
        [TestMethod]
        public void ResetFilter_ShowsAllStudents()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Сидоров", "КН-31", 85, 90, 78));
            catalog.AddStudent(new Student("Петров", "КН-32", 85, 50, 90));
            catalog.FilterByMinGrade(60);

            catalog.ResetFilter();

            Assert.AreEqual(2, catalog.Students.Count);
        }
    }
}
