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

        // SearchStudents знаходить студентів за частиною ПІБ
        [TestMethod]
        public void SearchStudents_FindsByPartOfName()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Іваненко Марія", "КН-32", 80, 80, 80));
            catalog.AddStudent(new Student("Петренко Олег", "КН-31", 80, 80, 80));
            catalog.AddStudent(new Student("Коваленко Анна", "АНД-31", 80, 80, 80));

            catalog.SearchStudents("петр");

            Assert.AreEqual(1, catalog.Students.Count);
            Assert.AreEqual("Петренко Олег", catalog.Students[0].FullName);
        }

        // SearchStudents знаходить студентів за частиною групи, без урахування регістру
        [TestMethod]
        public void SearchStudents_FindsByPartOfGroup()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Іваненко Марія", "КН-32", 80, 80, 80));
            catalog.AddStudent(new Student("Петренко Олег", "КН-31", 80, 80, 80));
            catalog.AddStudent(new Student("Коваленко Анна", "АНД-31", 80, 80, 80));

            catalog.SearchStudents("анд");

            Assert.AreEqual(1, catalog.Students.Count);
            Assert.AreEqual("Коваленко Анна", catalog.Students[0].FullName);
        }

        // SearchStudents порожній запит показує всіх студентів
        [TestMethod]
        public void SearchStudents_WhenTermIsEmpty_ShowsAllStudents()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Іваненко Марія", "КН-32", 80, 80, 80));
            catalog.AddStudent(new Student("Петренко Олег", "КН-31", 80, 80, 80));
            catalog.SearchStudents("петр");

            catalog.SearchStudents("");

            Assert.AreEqual(2, catalog.Students.Count);
        }

        // ShowUniqueGroups: кожна група зустрічається один раз, відсортовано за алфавітом
        [TestMethod]
        public void ShowUniqueGroups_ReturnsEachGroupOnce()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("Іваненко", "КН-32", 80, 80, 80));
            catalog.AddStudent(new Student("Петренко", "КН-32", 80, 80, 80));
            catalog.AddStudent(new Student("Сидоренко", "КН-31", 80, 80, 80));
            catalog.AddStudent(new Student("Коваленко", "АНД-31", 80, 80, 80));

            catalog.ShowUniqueGroups();

            Assert.AreEqual(3, catalog.Students.Count);
            Assert.AreEqual("АНД-31", catalog.Students[0].GroupNumber);
            Assert.AreEqual("КН-31", catalog.Students[1].GroupNumber);
            Assert.AreEqual("КН-32", catalog.Students[2].GroupNumber);
        }

        // FindThreeBestStudents залишає трьох студентів з найвищим середнім балом, від більшого до меншого
        [TestMethod]
        public void FindThreeBestStudents_ReturnsTopThreeByAverage()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("А", "КН-31", 60, 60, 60));
            catalog.AddStudent(new Student("Б", "КН-31", 90, 90, 90));
            catalog.AddStudent(new Student("В", "КН-31", 70, 70, 70));
            catalog.AddStudent(new Student("Г", "КН-31", 80, 80, 80));

            catalog.FindThreeBestStudents();

            Assert.AreEqual(3, catalog.Students.Count);
            Assert.AreEqual("Б", catalog.Students[0].FullName);
            Assert.AreEqual("Г", catalog.Students[1].FullName);
            Assert.AreEqual("В", catalog.Students[2].FullName);
        }

        // CalculateAverageGrade: середнє з 60 і 90 дорівнює 75
        [TestMethod]
        public void CalculateAverageGrade_ReturnsCorrectValue()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("А", "КН-31", 60, 60, 60));
            catalog.AddStudent(new Student("Б", "КН-31", 90, 90, 90));

            Assert.AreEqual(75.0, catalog.CalculateAverageGrade(), 0.001);
        }

        // CalculateAverageGrade для порожнього каталогу повертає 0
        [TestMethod]
        public void CalculateAverageGrade_WhenCatalogIsEmpty_ReturnsZero()
        {
            var catalog = CreateCatalog();

            Assert.AreEqual(0, catalog.CalculateAverageGrade());
        }

        // GroupStudentsByGroup: студенти однієї групи йдуть підряд, групи за алфавітом
        [TestMethod]
        public void GroupStudentsByGroup_PlacesSameGroupTogether()
        {
            var catalog = CreateCatalog();
            catalog.AddStudent(new Student("А", "КН-32", 80, 80, 80));
            catalog.AddStudent(new Student("Б", "КН-31", 80, 80, 80));
            catalog.AddStudent(new Student("В", "КН-32", 80, 80, 80));

            catalog.GroupStudentsByGroup();

            Assert.AreEqual(3, catalog.Students.Count);
            Assert.AreEqual("КН-31", catalog.Students[0].GroupNumber);
            Assert.AreEqual("КН-32", catalog.Students[1].GroupNumber);
            Assert.AreEqual("КН-32", catalog.Students[2].GroupNumber);
        }
    }
}
