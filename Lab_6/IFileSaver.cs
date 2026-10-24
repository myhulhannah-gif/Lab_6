using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public interface IFileSaver
    {
        // Зберігає список об'єктів у файл
        Task SaveAsync<T>(string filePath, List<T> data);
    }
}
