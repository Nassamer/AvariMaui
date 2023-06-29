using AvariModel.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariMaui.Persistence
{
    public class AwariStore : IStore
    {
        public Task<IEnumerable<String>> GetFiles() =>
        Task.Run(() => Directory.GetFiles(FileSystem.AppDataDirectory).Select(Path.GetFileName));

        /// <summary>
        ///     Módosítás idejének lekérdezése.
        /// </summary>
        /// <param name="name">A fájl neve.</param>
        /// <returns>Az utols módosítás ideje.</returns>
        public Task<DateTime> GetModifiedTime(String name)
        {
            var info = new FileInfo(Path.Combine(FileSystem.AppDataDirectory, name));

            return Task.Run(() => info.LastWriteTime);
        }
    }
}
