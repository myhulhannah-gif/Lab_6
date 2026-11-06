using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab_6
{
    // Клас для запису списку об'єктів у JSON-файл
    public class JsonFileWriter : IFileSaver
    {
        public async Task SaveAsync<T>(string filePath, List<T> data)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Перетворення даних на JSON-формат
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            // Запис тексту у файл
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
