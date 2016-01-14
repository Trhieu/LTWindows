using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1312189_Gomoku.Properties;

namespace _1312189_Gomoku.Models
{
    class Board
    {
        public int BoardSize { get; set; }
        public CellValues[,] Cells { get; set; }
        public CellValues ActivePlayer { get; set; }

        public event PlayerWinHandler OnPlayerWin;
        //public event PlayerAtHandler OnPlayerAt;

        public Board()
        {
            BoardSize = Settings.Default.BOARD_SIZE;
            Cells = new CellValues[BoardSize, BoardSize];
            ActivePlayer = CellValues.Player1;
        }

        public void PlayAt(int row, int col)
        {
            Cells[row, col] = ActivePlayer;
            // Check win state
            // Vertiacal check
            if (CountPlayerItem(row, col, 1, 0) >= 5
                || CountPlayerItem(row, col, 0, 1) >= 5
                || CountPlayerItem(row, col, 1, 1) >= 5
                || CountPlayerItem(row, col, 1, -1) >= 5)
            {
                if (OnPlayerWin != null)
                    OnPlayerWin(player: ActivePlayer);
                return;
            }
            if (ActivePlayer == CellValues.Player1)
                ActivePlayer = CellValues.Player2;
            else 
            {
                ActivePlayer = CellValues.Player1;
            }

        }

        private bool IsInBoard(int row, int col)
        {
            return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
        }

        public void AutoPlay(int row, int col)
        {
            if (MainWindow.endGame)
            {
                return;
            }
            Cells[row, col] = ActivePlayer;
            // Check win state
            // Vertiacal check
            if (CountPlayerItem(row, col, 1, 0) >= 5
                || CountPlayerItem(row, col, 0, 1) >= 5
                || CountPlayerItem(row, col, 1, 1) >= 5
                || CountPlayerItem(row, col, 1, -1) >= 5)
            {
                MainWindow.endGame = true;
                if (OnPlayerWin != null)
                {
                    OnPlayerWin(player: ActivePlayer);
                    return;
                }
                return;
            }
            
            if (ActivePlayer == CellValues.AI)
                ActivePlayer = CellValues.Player1;
            else
            {
                ActivePlayer = CellValues.AI;
            }
        }

        private int CountPlayerItem(int row, int col, int drow, int dcol)
        {
            int crow = row + drow;
            int ccol = col + dcol;
            int count = 1;
            while (IsInBoard(crow, ccol) && Cells[crow, ccol] == ActivePlayer)
            {
                count++;
                crow = crow + drow;
                ccol = ccol + dcol;
            }
            crow = row - drow;
            ccol = col - dcol;
            while (IsInBoard(crow, ccol) && Cells[crow, ccol] == ActivePlayer)
            {
                count++;
                crow = crow - drow;
                ccol = ccol - dcol;
            }
            return count;
        }

        public void AutoAI(int rowPlayer, int colPlayer)
        {
            if (colPlayer <= BoardSize - 2)
                Cells[rowPlayer, colPlayer + 1] = CellValues.AI;
            else if (rowPlayer <= BoardSize - 2 && rowPlayer >= 1)
                Cells[rowPlayer - 1, rowPlayer] = CellValues.AI;
        }
    }
    
    public delegate void PlayerWinHandler(CellValues player);
    public enum CellValues { None = 0, Player1 = 1, Player2= 2, AI = 3}
}
