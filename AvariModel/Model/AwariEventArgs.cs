using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariModel.Model
{
    public class AwariEventArgs : EventArgs
    {
        private string whoWon;

        public string WhoWon
        {
            get { return whoWon; }
        }


        public AwariEventArgs(string whoWon2)
        {
            this.whoWon = whoWon2;
        }
    }
}
