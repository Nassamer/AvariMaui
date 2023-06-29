using AvariModel.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariMaui.Persistence
{
    public class AwariDataAccesser : AwariDataAcces
    {
        
        public async Task<AwariTable> LoadAsync(string path)
        {
            // a betöltés a személyes könyvtárból
            var filePath = Path.Combine(FileSystem.AppDataDirectory, path);

            // a fájlműveletet taszk segítségével végezzük (aszinkron módon)
            var values = (await Task.Run(() => File.ReadAllText(filePath))).Split(' ');

            var tableSize = int.Parse(values[0]);
            
            var table = new AwariTable(tableSize);

            var valueIndex = 2;
            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
                for (var columnIndex = 0; columnIndex < tableSize; columnIndex++)
                {
                    table.SetValue(rowIndex, columnIndex, int.Parse(values[valueIndex]));
                    valueIndex++;
                }

            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
                for (var columnIndex = 0; columnIndex < tableSize; columnIndex++)
                {
                    if (values[valueIndex] == "1")
                        table.SetLock(rowIndex, columnIndex);
                    valueIndex++;
                }

            return table;
        }

        /// <summary>
        ///     Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        public async Task SaveAsync(string path, AwariTable table)
        {
            var text = table.Size + " " ; // méret

            for (var i = 0; i < 3; i++)
                for (var j = 0; j < table.Size; j++)
                    text += " " + table[i, j]; // mezőértékek

            for (var i = 0; i < 3; i++)
                for (var j = 0; j < table.Size; j++)
                    text += " " + (table.IsLocked(i, j) ? "1" : "0");

            // fájl létrehozása
            var filePath = Path.Combine(FileSystem.AppDataDirectory, path);

            // kiírás (aszinkron mdon)
            await Task.Run(() => File.WriteAllText(filePath, text));
        }
    }
}
