using Lab_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6_Tests
{
    [TestClass]
    public class FileHandlerTests
    {
        private string _testFilePath = "";

        // Унікальний шлях до тимчасового файлу
        [TestInitialize]
        public void TestInitialize()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
        }

        // Видаляється файл
        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        // Після збереження файл існує, не порожній і з нього можна прочитати назад ті самі дані
        [TestMethod]
        public async Task SaveAsync_Should_WriteDataToFile()
        {
            var students = new List<Student>
            {
                new Student("Іваненко Марія Петрівна", "КН-32", 85, 90, 78),
                new Student("Петренко Олег Іванович", "КН-31", 60, 70, 65)
            };

            var saver = new JsonFileWriter();
            await saver.SaveAsync(_testFilePath, students);

            Assert.IsTrue(File.Exists(_testFilePath), "Файл не було створено.");
            string content = await File.ReadAllTextAsync(_testFilePath);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content), "Файл порожній.");

            var reader = new JsonFileReader();
            var loaded = await reader.LoadAsync<Student>(_testFilePath);

            Assert.IsNotNull(loaded, "Десеріалізація не вдалася.");
            Assert.AreEqual(2, loaded.Count, "Кількість об'єктів не збігається.");
            Assert.AreEqual("Іваненко Марія Петрівна", loaded[0].FullName);
            Assert.AreEqual(78, loaded[0].Exam3);
        }

        // Читання з вручну записаного JSON
        [TestMethod]
        public async Task LoadAsync_Should_ReadDataFromFile()
        {
            string json = "[{\"FullName\":\"Іваненко Марія\",\"GroupNumber\":\"КН-32\",\"Exam1\":85,\"Exam2\":90,\"Exam3\":78}]";
            await File.WriteAllTextAsync(_testFilePath, json);

            var reader = new JsonFileReader();
            var loaded = await reader.LoadAsync<Student>(_testFilePath);

            Assert.IsNotNull(loaded, "Об'єкти не були завантажені.");
            Assert.AreEqual(1, loaded.Count);
            Assert.AreEqual("Іваненко Марія", loaded[0].FullName);
            Assert.AreEqual("КН-32", loaded[0].GroupNumber);
        }

        // Якщо файлу не існує, повертається null
        [TestMethod]
        public async Task LoadAsync_WhenFileDoesNotExist_ReturnsNull()
        {
            var reader = new JsonFileReader();
            var loaded = await reader.LoadAsync<Student>(_testFilePath);

            Assert.IsNull(loaded, "Повинен повертати null, якщо файл не існує.");
        }
    }
}
