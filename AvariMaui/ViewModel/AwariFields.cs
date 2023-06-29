using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariMaui.ViewModel
{
    public class AwariFields : ViewModelBase
    {
        private bool _isEnabled;
        private Boolean _isFirstRow;
        private Boolean _isSecondRow;
        private Boolean _isThirdRow;
        private String _text = String.Empty;

        /// <summary>
        /// Zároltság lekérdezése, vagy beállítása.
        /// </summary>
        public Boolean IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsFirstRow
        {
            get { return _isFirstRow; }
            set
            {
                if (_isFirstRow != value)
                {
                    _isFirstRow = value;
                    OnPropertyChanged();
                }
            }
        }
        public Boolean IsSecondRow
        {
            get { return _isSecondRow; }
            set
            {
                if (_isSecondRow != value)
                {
                    _isSecondRow = value;
                    OnPropertyChanged();
                }
            }
        }
        public Boolean IsThirdRow
        {
            get { return _isThirdRow; }
            set
            {
                if (_isThirdRow != value)
                {
                    _isThirdRow = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Felirat lekérdezése, vagy beállítása.
        /// </summary>
        public String Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }

        /// <summary>
        /// Sorszám lekérdezése.
        /// </summary>
        public Int32 Number { get; set; }

        /// <summary>
        /// Lépés parancs lekérdezése, vagy beállítása.
        /// </summary>
        public DelegateCommand StepCommand { get; set; }
    }
}
