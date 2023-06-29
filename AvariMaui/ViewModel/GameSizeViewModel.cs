using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvariModel.Model;

namespace AvariMaui.ViewModel
{
    public class GameSizeViewModel : ViewModelBase
    {
        public MapSize mapSize;

        public MapSize Size
        {
            get => mapSize;
            set
            {
                mapSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SizeText));
            }
        }

        public string SizeText => mapSize.ToString();
    
    }
}
