using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariModel.Persistence
{
    public interface AwariDataAcces
    {
        /// <summary>
        ///     Erre az útvonalra mentjük a félbehagyott játékokat
        /// </summary>
        public const string SuspendedGameSavePath = "SuspendedGame";

        Task<AwariTable> LoadAsync(String path);


        Task SaveAsync(String path, AwariTable table);
    }
}
