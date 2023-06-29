using AvariModel.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariModel.Model
{
    public enum MapSize { Small, Medium, Large }
    public class AwariGameModel
    {
        private const int GeneratedFieldCountSmall = 4;
        private const int GeneratedFieldCountMedium = 8;
        private const int GeneratedFieldCountLarge = 12;

        private AwariDataAcces dataAccess;
        private AwariTable table; // tábla
        private MapSize mapSize; // méret

        int counter;
        bool doublesteps;
        bool triplesteps;



        public AwariTable Table { get { return table; } }

        public Boolean IsGameOver { get { return (table.Sum()); } }

        public MapSize MapSize { get { return mapSize; } set { mapSize = value; } }


        public event EventHandler<AwariEventArgs>? GameOver;

        public event EventHandler<AwariEventArgs>? MapCreated;

        public event EventHandler<AwariEventArgs>? GameAdvanced;


        public AwariGameModel(AwariDataAcces dataAccess)
        {
            this.dataAccess = dataAccess;
            table = new AwariTable(GeneratedFieldCountMedium);
            //table = new HeatingControllTable();
            mapSize = MapSize.Medium;
            counter = 0;
            doublesteps = false;
            triplesteps = false;
        }

        public void NewGame()
        {

            counter = 0;
            doublesteps = false;
            triplesteps = false;
            switch (mapSize) // pályaméret állítás
            {
                case MapSize.Small:
                    GenerateFields(GeneratedFieldCountSmall);
                    break;
                case MapSize.Medium:
                    GenerateFields(GeneratedFieldCountMedium);
                    break;
                case MapSize.Large:
                    GenerateFields(GeneratedFieldCountLarge);
                    break;
            }

            OnGameCreated();
        }

        public void Step(int x, int y)
        {
            //doublesteps = table.GetDoubleSteps;
            //triplesteps = table.GetTripleSteps;
            if (IsGameOver) // ha már vége a játéknak, nem játszhatunk
                return;
            if (table.IsLocked(x, y)) // ha a mező zárolva van, nem léthatünk
                return;

            table.StepValue(x, y);


            if (doublesteps == true && triplesteps == false)
            {
                triplesteps = true;
            }
            else if (doublesteps == true && triplesteps == true)
            {
                triplesteps = false;
                doublesteps = false;
                counter++;
            }
            else
            {
                counter++;
            }


            if (counter % 2 == 0)
            {
                OnGameAdvanced("blue");
                if (IsGameOver) // ha vége a játéknak, jelezzük, hogy győztünk
                {
                    if (table.FieldValues[1, 0] < table.FieldValues[1, (table.FieldValues.GetLength(1) - 1)])
                    {
                        OnGameOver("red");
                    }
                    else if (table.FieldValues[1, 0] > table.FieldValues[1, (table.FieldValues.GetLength(1) - 1)])
                    {
                        OnGameOver("blue");
                    }
                    else
                    {
                        OnGameOver("draw");
                    }

                }
            }
            else
            {
                OnGameAdvanced("red");
                if (IsGameOver) // ha vége a játéknak, jelezzük, hogy győztünk
                {
                    if (table.FieldValues[1, 0] > table.FieldValues[1, (table.FieldValues.GetLength(1) - 1)])
                    {
                        OnGameOver("blue");
                    }
                    else if (table.FieldValues[1, 0] < table.FieldValues[1, (table.FieldValues.GetLength(1) - 1)])
                    {
                        OnGameOver("red");
                    }
                    else
                    {
                        OnGameOver("draw");
                    }
                }
            }



        }

        public async Task LoadGameAsync(String path)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            table = await dataAccess.LoadAsync(path);

            switch (mapSize) // játékméret beállítása
            {
                case MapSize.Small:
                    break;
                case MapSize.Medium:
                    break;
                case MapSize.Large:
                    break;
            }
        }

        public async Task SaveGameAsync(String path)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await dataAccess.SaveAsync(path, table);
        }
        private void GenerateFields(int count)
        {
            Random random = new Random();
            table = new AwariTable(count);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (i != 1)
                    {
                        table.FieldValues[i, j] = 6;

                    }
                    else
                    {
                        table.FieldValues[i, j] = 0;
                        table.SetLock(i, j);
                    }
                }

            }
        }

        private void OnGameAdvanced(string who)
        {
            GameAdvanced?.Invoke(this, new AwariEventArgs(who));
            if (who == "red")
            {
                for (int i = 0; i < table.FieldLocks.GetLength(0); i++)
                {
                    for (int j = 0; j < table.FieldLocks.GetLength(1); j++)
                    {
                        if (i == 0)
                        {
                            table.FieldLocks[i, j] = true;
                        }
                        else if (i == 2)
                        {
                            table.FieldLocks[i, j] = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < table.FieldLocks.GetLength(0); i++)
                {
                    for (int j = 0; j < table.FieldLocks.GetLength(1); j++)
                    {
                        if (i == 0)
                        {
                            table.FieldLocks[i, j] = false;
                        }
                        else if (i == 2)
                        {
                            table.FieldLocks[i, j] = true;
                        }
                    }
                }
            }
        }

        private void OnGameCreated()
        {
            MapCreated?.Invoke(this, new AwariEventArgs(""));
        }

        /// <summary>
        /// Játék vége eseményének kiváltása.
        /// </summary>
        /// <param name="whoWon">Ki nyert a játékban.</param>
        private void OnGameOver(string whoWhon)
        {
            GameOver?.Invoke(this, new AwariEventArgs(whoWhon));
        }
    }
}
