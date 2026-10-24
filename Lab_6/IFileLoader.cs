using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public interface IFileLoader
    {
        // Завантажує список об'єктів із файлу
        Task<List<T>?> LoadAsync<T>(string filePath);
    }
}
