using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public class StudentGroupComparer : IComparer<Student>
    {
        public int Compare(Student x, Student y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            // Порівняння груп
            return string.Compare(x.GroupNumber, y.GroupNumber, StringComparison.OrdinalIgnoreCase);
        }
    }
}
