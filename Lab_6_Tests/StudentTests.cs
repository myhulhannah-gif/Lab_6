using Lab_6;

namespace Lab_6_Tests
{
    [TestClass]
    public sealed class StudentTests
    {
        // Перевірка конструктора
        [TestMethod]
        public void Constructor_WithValidArguments_CreatesStudentObject()
        {
            var student = new Student("Іваненко Марія Петрівна", "КН-32", 85, 90, 78);

            Assert.AreEqual("Іваненко Марія Петрівна", student.FullName);
            Assert.AreEqual("КН-32", student.GroupNumber);
            Assert.AreEqual(85, student.Exam1);
            Assert.AreEqual(90, student.Exam2);
            Assert.AreEqual(78, student.Exam3);
        }

        // Валідація ПІБ: null, порожній рядок і рядок з пробілу
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Constructor_WithInvalidFullName_ThrowsArgumentException(string invalidName)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student(invalidName, "КН-32", 80, 80, 80));
        }

        // Ввалідація номера групи: нуль і від'ємне число
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Constructor_WithInvalidGroupNumber_ThrowsArgumentException(string invalidGroup)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student("Іваненко", invalidGroup, 80, 80, 80));
        }

        // Валідація першої оцінки: менше 0 і більше 100
        [TestMethod]
        [DataRow(-1)]
        [DataRow(101)]
        public void Constructor_WithInvalidExam1_ThrowsArgumentException(int invalidGrade)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student("Іваненко", "КН-31", invalidGrade, 80, 80));
        }

        // Валідація другої оцінки: менше 0 і більше 100
        [TestMethod]
        [DataRow(-1)]
        [DataRow(101)]
        public void Constructor_WithInvalidExam2_ThrowsArgumentException(int invalidGrade)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student("Іваненко", "КН-31", 80, invalidGrade, 80));
        }

        // Валідація третьої оцінки: менше 0 і більше 100
        [TestMethod]
        [DataRow(-1)]
        [DataRow(101)]
        public void Constructor_WithInvalidExam3_ThrowsArgumentException(int invalidGrade)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student("Іваненко", "КН-31", 80, 80, invalidGrade));
        }

        // Перевірка IComparable (порівняння за ПІБ)
        [TestMethod]
        public void CompareTo_WithOtherStudent_ReturnsCorrectValue()
        {
            var s1 = new Student("Андрієнко Олена Сергіївна", "КН-31", 80, 80, 80);
            var s2 = new Student("Борисенко Іван Петрович", "КН-32", 85, 80, 80);
            var s3 = new Student("Андрієнко Олена Сергіївна", "АНД-31", 70, 70, 70);

            Assert.IsTrue(s1.CompareTo(s2) < 0, "Андрієнко має бути перед Борисенко.");
            Assert.IsTrue(s2.CompareTo(s1) > 0, "Борисенко має бути після Андрієнко.");
            Assert.AreEqual(0, s1.CompareTo(s3), "Однакові ПІБ мають бути рівними.");
        }

        // Порівняння не залежить від регістру
        [TestMethod]
        public void CompareTo_WithDifferentCase_ReturnsZero()
        {
            var s1 = new Student("іваненко", "КН-31", 80, 80, 80);
            var s2 = new Student("Іваненко", "КН-31", 80, 80, 80);

            Assert.AreEqual(0, s1.CompareTo(s2), "Порівняння не залежить від регістру.");
        }

        // В порівнянні з null повертається >0
        [TestMethod]
        public void CompareTo_WithNull_ReturnsPositiveValue()
        {
            var student = new Student("Іваненко", "КН-31", 80, 80, 80);

            Assert.IsTrue(student.CompareTo(null) > 0, "Порівняння з null має повертати додатне число.");
        }

        // Усі три оцінки вищі за 60, тож сесія складена
        [TestMethod]
        public void HasPassedSession_AllGradesAboveMin_ReturnsTrue()
        {
            var student = new Student("Іваненко", "КН-31", 85, 90, 78);

            Assert.IsTrue(student.HasPassedSession(60));
        }

        // Усі три оцінки = 60, тож сесія складена
        [TestMethod]
        public void HasPassedSession_GradeEqualToMin_ReturnsTrue()
        {
            var student = new Student("Іваненко", "КН-31", 60, 60, 60);

            Assert.IsTrue(student.HasPassedSession(60));
        }

        // Хоча б одна оцінка менша за 60, тож сесія не складена
        [TestMethod]
        public void HasPassedSession_OneGradeBelowMin_ReturnsFalse()
        {
            var student = new Student("Іваненко", "КН-31", 85, 59, 90);

            Assert.IsFalse(student.HasPassedSession(60));
        }

        // Метод ToString
        [TestMethod]
        public void ToString_ReturnsCorrectlyFormattedString()
        {
            var student = new Student("Іваненко Марія", "КН-31", 85, 90, 78);

            string expected = "ПІБ: Іваненко Марія, Група: КН-31, Оцінки: 85/90/78";

            Assert.AreEqual(expected, student.ToString());
        }
    }
}