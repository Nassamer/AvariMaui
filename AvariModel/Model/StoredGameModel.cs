﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariModel.Model
{
    public class StoredGameModel
    {
        /// <summary>
        /// Név lekérdezése, vagy beállítása.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Módosítás idejének lekérdezése, vagy beállítása.
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
