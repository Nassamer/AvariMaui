using AvariModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariMaui.ViewModel
{
    public class AwariViewModel : ViewModelBase
    {
        private readonly AwariGameModel model;
        private GameSizeViewModel mapSize;
        private RowDefinitionCollection _gameTableRows;
        private ColumnDefinitionCollection _gameTableColumns;
        private int gridSize;


        public AwariViewModel(AwariGameModel model)
        {
            this.model = model;
            this.model.MapCreated += Model_MapCreated;
            this.model.GameOver += Model_GameOver;
            this.model.GameAdvanced += Model_GameAdvanced;

            var mediumMap = new GameSizeViewModel { Size = AvariModel.Model.MapSize.Medium };
            MapSize = mediumMap;
            MapSizes.Add(new GameSizeViewModel { Size = AvariModel.Model.MapSize.Small });         
            MapSizes.Add(mediumMap);
            MapSizes.Add(new GameSizeViewModel { Size = AvariModel.Model.MapSize.Large });

            NewMapCommand = new DelegateCommand(param => OnNewMap());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            
            GridSize = this.model.Table.Size;
            Fields = new ObservableCollection<AwariFields>();
            
            //IsEnbaled amúgy pont fordítva van használva

            Fields.Clear();
            for (Int32 i = 0; i < 3; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < model.Table.Size; j++)
                {
                    Fields.Add(new AwariFields
                    {
                        IsEnabled = false,
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        Number = i * model.Table.Size + j, // a gomb sorszáma, amelyet felhasználunk az azonosításhoz
                        StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                        // ha egy mezőre léptek, akkor jelezzük a léptetést, változtatjuk a lépésszámot
                    });
                }
            }
            
            RefreshTable();
        }


        public DelegateCommand NewMapCommand { get;  }

        public DelegateCommand LoadGameCommand { get;  }

        public DelegateCommand SaveGameCommand { get; }

        public DelegateCommand ExitCommand { get; }

        public ObservableCollection<AwariFields> Fields { get; set; }

        public ObservableCollection<GameSizeViewModel> MapSizes { get; set; } = new();


        public GameSizeViewModel MapSize
        {
            get => mapSize;
            set
            {
                mapSize = value;
                model.MapSize = value.mapSize;
                OnPropertyChanged();
            }
        }

        public int GridSize
        {
            get => gridSize;
            set
            {
                gridSize = value;
                OnPropertyChanged();
                GenerateGridDefinitions(gridSize);
            }
        }

        public RowDefinitionCollection GameTableRows
        {
            get => _gameTableRows;
            set
            {
                _gameTableRows = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Segédproperty a tábla méretezéséhez
        /// </summary>
        public ColumnDefinitionCollection GameTableColumns
        {
            get => _gameTableColumns;
            set
            {
                _gameTableColumns = value;
                OnPropertyChanged();
            }
        }


        public event EventHandler NewMap;

        public event EventHandler LoadGame;

        public event EventHandler SaveGame;

        public event EventHandler ExitGame;

        private void GenerateGridDefinitions(int size)
        {
            var rowDefinitions = new RowDefinition[size];
            var columnDefinitions = new ColumnDefinition[size];

            for (var i = 0; i < size; i++)
            {
                rowDefinitions[i] = new RowDefinition
                {
                    Height = GridLength.Star
                };
                columnDefinitions[i] = new ColumnDefinition
                {
                    Width = GridLength.Star
                };
            }

            GameTableRows = new RowDefinitionCollection(rowDefinitions);
            GameTableColumns = new ColumnDefinitionCollection(columnDefinitions);
        }

        public void RefreshTable()
        {
            Fields.Clear();
            GridSize = model.Table.Size;
            for (int i = 0; i < 3; i++) // inicializáljuk a mezőket
            {
                for (int j = 0; j < model.Table.Size; j++)
                {
                    if (model.Table.GetValue(i, j) == 0)
                    {
                        Fields.Add(new AwariFields
                        {
                            //Ide még vissza kell térni

                            Text = "0",
                            X = i,
                            Y = j,
                            Number = i * model.Table.Size + j, // a gomb sorszáma, amelyet felhasználunk az azonosításhoz
                            StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                            // ha egy mezőre léptek, akkor jelezzük a léptetést, változtatjuk a lépésszámot

                        });
                    }
                    else
                    {
                        Fields.Add(new AwariFields
                        {
                            //Ide még vissza kell térni

                            Text = model.Table.GetValue(i, j).ToString(),
                            X = i,
                            Y = j,
                            Number = i * model.Table.Size + j, // a gomb sorszáma, amelyet felhasználunk az azonosításhoz
                            StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                            // ha egy mezőre léptek, akkor jelezzük a léptetést, változtatjuk a lépésszámot
                        });
                    }
                    foreach (AwariFields field in Fields)
                    {
                        field.IsEnabled = !(model.Table.IsLocked(field.X, field.Y));
                        field.IsFirstRow = model.Table.IsFirstRow(field.X);
                        field.IsSecondRow = model.Table.IsSecondRow(field.X);
                        field.IsThirdRow = model.Table.IsThirdRow(field.X);
                    }


                }
            }

            OnPropertyChanged();
        }

        private void StepGame(int index)
        {
            ;
            var field = Fields[index];

            model.Step(field.X, field.Y);

            field.Text = model.Table[field.X, field.Y] > 0 ? model.Table[field.X, field.Y].ToString() : "0"; // visszaírjuk a szöveget
            OnPropertyChanged(); // jelezzük a lépésszám változást
            RefreshTable();

            field.Text = !model.Table.IsEmpty(field.X, field.Y) ? model.Table[field.X, field.Y].ToString() : "0";
        }

        private void Model_GameAdvanced(object sender, AwariEventArgs e)
        {
            OnPropertyChanged();
        }


        private void Model_MapCreated(object sender, AwariEventArgs e)
        {
            RefreshTable();
        }

        private void OnNewMap()
        {
            NewMap?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
            RefreshTable();
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        private void Model_GameOver(object sender, AwariEventArgs e)
        {
            foreach (AwariFields field in Fields)
            {
                field.IsEnabled = true; // minden mezőt lezárunk
            }
        }
    }
}
