using _1312189_Gomoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1312189_Gomoku.ViewModels
{
    class BoardViewModel
    {
        public Board CurrentBoard { get; set; }

        public BoardViewModel()
        {
            CurrentBoard = new Board();
        }
    }
}
