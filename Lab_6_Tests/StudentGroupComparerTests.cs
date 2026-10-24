using Lab_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6_Tests
{
    [TestClass]
    public sealed class StudentGroupComparerTests
    {

        [TestMethod]
        public void Compare_WhenGroupsAreDifferent_ReturnsCorrectOrder()
        {
            var s1 = new Student("Іваненко", "АНД-31", 80, 80, 80);
            var s2 = new Student("Петренко", "КН-32", 80, 80, 80);
            var comparer = new StudentGroupComparer();

            Assert.IsTrue(comparer.Compare(s1, s2) < 0, "Група 10 має бути перед групою 12.");
            Assert.IsTrue(comparer.Compare(s2, s1) > 0, "Група 12 має бути після групи 10.");
        }

        // Однакові групи дають 0, навіть якщо ПІБ й оцінки різні
        [TestMethod]
        public void Compare_WhenGroupsAreIdentical_ReturnsZero()
        {
            var s1 = new Student("Іваненко", "КН-32", 80, 80, 80);
            var s2 = new Student("Петренко", "КН-32", 70, 70, 70);
            var comparer = new StudentGroupComparer();

            Assert.AreEqual(0, comparer.Compare(s1, s2));
        }

        // В порівнянні з null повертається =0
        [TestMethod]
        public void Compare_WhenFirstStudentIsNull_ReturnsZero()
        {
            var s2 = new Student("Петренко", "КН-32", 80, 80, 80);
            var comparer = new StudentGroupComparer();

            Assert.AreEqual(0, comparer.Compare(null, s2));
        }

        [TestMethod]
        public void Compare_WhenSecondStudentIsNull_ReturnsZero()
        {
            var s1 = new Student("Іваненко", "КН-32", 80, 80, 80);
            var comparer = new StudentGroupComparer();

            Assert.AreEqual(0, comparer.Compare(s1, null));
        }

        [TestMethod]
        public void Compare_WhenBothStudentsAreNull_ReturnsZero()
        {
            var comparer = new StudentGroupComparer();

            Assert.AreEqual(0, comparer.Compare(null, null));
        }
    }
}
