using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public class Student: IComparable<Student>
    {
        private string fullName;
        private string groupNumber;
        private int exam1;
        private int exam2;
        private int exam3;

        public string FullName
        {
            get { return fullName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("ПІБ не може бути порожнім.", nameof(FullName));
                }
                fullName = value;
            }
        }

        public string GroupNumber
        {
            get { return groupNumber; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Група не може бути порожньою.", nameof(GroupNumber));
                }
                groupNumber = value;
            }
        }

        public int Exam1
        {
            get { return exam1; }
            set
            {
                ValidateGrade(value, nameof(Exam1));
                exam1 = value;
            }
        }

        public int Exam2
        {
            get { return exam2; }
            set
            {
                ValidateGrade(value, nameof(Exam2));
                exam2 = value;
            }
        }

        public int Exam3
        {
            get { return exam3; }
            set
            {
                ValidateGrade(value, nameof(Exam3));
                exam3 = value;
            }
        }

        private static void ValidateGrade(int value, string paramName)
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentException("Оцінка має бути від 0 до 100.", paramName);
            }
        }

        public Student(string fullName, string groupNumber, int exam1, int exam2, int exam3)
        {
            FullName = fullName;
            GroupNumber = groupNumber;
            Exam1 = exam1;
            Exam2 = exam2;
            Exam3 = exam3;
        }

        // Реалізація IComparable<Student>: природний порядок за ПІБ
        public int CompareTo(Student other)
        {
            if (other == null)
            {
                return 1;
            }
            return string.Compare(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
        }

        // Сесія успішна, якщо всі три оцінки не нижчі за minGrade
        public bool HasPassedSession(int minGrade)
        {
            return Exam1 >= minGrade && Exam2 >= minGrade && Exam3 >= minGrade;
        }

        public override string ToString()
        {
            return $"ПІБ: {FullName}, Група: {GroupNumber}, Оцінки: {Exam1}/{Exam2}/{Exam3}";
        }
    }
}
