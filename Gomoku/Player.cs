using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1312189_Gomoku
{
    class Player
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private bool canPlay;
        public bool CanPlay
        {
            get { return canPlay; }
            set { canPlay = value; }
        }

        public Player()
        {
            name = "Guest";
            canPlay = false;
        }

        public Player(string name)
        {
            this.name = name;
            canPlay = false;
        }

        public void Chat(String msg)
        {
            
        }
    }
}
