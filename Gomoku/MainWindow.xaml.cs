using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _1312189_Gomoku.Models;
using _1312189_Gomoku.ViewModels;
using System.Threading;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System.ComponentModel;

namespace _1312189_Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Player player;
        private bool nameChanged;
        private BoardViewModel boardViewModel;
        private bool playWithAI = false;
        public static bool endGame = false;
        public Socket socket;
        public BackgroundWorker worker;
        public MainWindow()
        {
            nameChanged = false;
            player = new Player();
            InitializeComponent();
            SetButtonClick();
            boardViewModel = new BoardViewModel();
            boardViewModel.CurrentBoard.OnPlayerWin += CurrentBoard_OnPlayerWin;
            //endGame = true;
        }

        void CurrentBoard_OnPlayerWin(CellValues player)
        {
            ChatMessage chatMessage = new ChatMessage("Server", DateTime.Now.ToString("hh:mm:ss tt"), player.ToString() + " win!");
            chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            chatBox.Items.Add(chatMessage);
            endGame = true;
            
        }

        private void SetButtonClick()   //Set click event handler for all cells in grid board.
        {
            foreach (Control control in board.Children)
            {
                Button cell = (Button)control;
                cell.Click += this.Cell_Click;
            }
        }
        
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (endGame)
            {
                return;
            }
            Button cell = (Button)sender;
            //Show the coordinate of the clicked cell.
            //MessageBox.Show("You clicked cell: " + "(" + Grid.GetRow(cell) + ", " + Grid.GetColumn(cell) +")", "", MessageBoxButton.OK, MessageBoxImage.Information);
            if (playWithAI == true)
            {
                //boardViewModel.CurrentBoard.ActivePlayer = CellValues.Player1;
                if (cell.Background is SolidColorBrush)
                {


                    if ((((SolidColorBrush)cell.Background).Color != Brushes.Red.Color) && ((SolidColorBrush)cell.Background).Color != Brushes.Blue.Color)
                    {
                        cell.Background = new SolidColorBrush(Colors.Red);

                    }
                }
                else
                    cell.Background = new SolidColorBrush(Colors.Red);
                
                //boardViewModel.CurrentBoard.AutoPlay(Grid.GetRow(cell), Grid.GetColumn(cell));
                int rowPlayer = 0, colPlayer = 0;
                //Thread t = new Thread(boardViewModel.CurrentBoard.AutoAI(Grid.GetRow(cell), Grid.GetColumn(cell)));
                if (Grid.GetColumn(cell) <= 12 - 2)
                {
                    colPlayer = Grid.GetColumn(cell) + 1;
                    rowPlayer = Grid.GetRow(cell);
                }
                else if (Grid.GetRow(cell) <= 12 - 2 && Grid.GetRow(cell) >= 1)
                {
                    rowPlayer = Grid.GetRow(cell) - 1;
                    colPlayer = Grid.GetRow(cell);
                }

                if (!endGame)
                    boardViewModel.CurrentBoard.AutoPlay(Grid.GetRow(cell), Grid.GetColumn(cell));
                    
                if(endGame)
                {
                    return;
                }
                

                foreach (Control control in board.Children)
                {
                    cell = (Button)control;
                    //cell.Click += this.Cell_Click;
                    if (Grid.GetRow(cell) == rowPlayer && Grid.GetColumn(cell) == colPlayer)
                    {
         
                            if (cell.Background is SolidColorBrush)
                            {


                                if ((((SolidColorBrush)cell.Background).Color != Brushes.Red.Color) && ((SolidColorBrush)cell.Background).Color != Brushes.Blue.Color)
                                {
                                    cell.Background = new SolidColorBrush(Colors.Blue);

                                }
                            }
                        
                        else
                            cell.Background = new SolidColorBrush(Colors.Blue);
                        boardViewModel.CurrentBoard.ActivePlayer = CellValues.AI;
                        if(!endGame)
                            boardViewModel.CurrentBoard.AutoPlay(Grid.GetRow(cell), Grid.GetColumn(cell));
                        if (endGame)
                        {
                            return;
                        }
                    }
                }
                
            }
            else
            {
                if (boardViewModel.CurrentBoard.ActivePlayer == CellValues.Player1)
                {
                    if (cell.Background is SolidColorBrush)
                    {


                        if ((((SolidColorBrush)cell.Background).Color != Brushes.Red.Color) && ((SolidColorBrush)cell.Background).Color != Brushes.Blue.Color)
                        {
                            cell.Background = new SolidColorBrush(Colors.Red);

                        }
                    } 
                    else
                        cell.Background = new SolidColorBrush(Colors.Red);

                    //cell.Background = new SolidColorBrush(Colors.Red);
                }
                else
                {

                    if (cell.Background is SolidColorBrush)
                    {


                        if ((((SolidColorBrush)cell.Background).Color != Brushes.Red.Color) && ((SolidColorBrush)cell.Background).Color != Brushes.Blue.Color)
                        {
                            cell.Background = new SolidColorBrush(Colors.Blue);

                        }
                    }
                    else
                        cell.Background = new SolidColorBrush(Colors.Blue);

                }
                    //cell.Background = new SolidColorBrush(Colors.Blue);
                boardViewModel.CurrentBoard.PlayAt(Grid.GetRow(cell), Grid.GetColumn(cell));
            }
            
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)    //Send message to chatbox
        {
            if (txtMessage.Text != "" && nameChanged)
            {
                ChatMessage chatMessage = new ChatMessage(player.Name, DateTime.Now.ToString("hh:mm:ss tt"), txtMessage.Text);
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(chatMessage);
            }         
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)  //Change name of Player
        {
            if (txtYourName.Text == "")
            {
                txtYourName.Text = "Guest";
            }
            else
            {
                player.Name = txtYourName.Text;
            }
        }


        private void txtMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMessage.Text == "Type your message here...")
            {
                txtMessage.Text = "";
                txtMessage.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtMessage_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMessage.Text == "")
            {
                nameChanged = false;
                txtMessage.Foreground = new SolidColorBrush(Color.FromRgb(135, 135, 135));
                txtMessage.Text = "Type your message here...";
            }
            else
            {
                nameChanged = true;
            }
        }

        private void txtYourName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtYourName.Text == "")
            {
                txtYourName.Text = "Guest";
            }
        }

        private void btnPlayAI_Click(object sender, RoutedEventArgs e)
        {
            playWithAI = true;
        }

        private void btnPlayOnline_Click(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
            bool firstConnect = true;
            object oldData = new object();
            while(true)
            {
                Thread.Sleep(100);
                socket.On("ChatMessage", (data) =>
                {
                    if(firstConnect || oldData != data)
                    {
                        firstConnect = false;
                        string chatSender;
                        string message = ((JObject)data)["message"].ToString();

                        if (((JObject)data)["from"] != null)
                        {
                            chatSender = ((JObject)data)["from"].ToString();
                        }
                        else
                        {
                            chatSender = "Server";
                        }
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            ChatMessage chatMessage = new ChatMessage(chatSender, DateTime.Now.ToString("hh:mm:ss tt"), message);
                            chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                            chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            chatBox.Items.Add(chatMessage);
                            if (((JObject)data)["message"].ToString() == "Welcome!")
                            {
                                
                              
                                socket.Emit("MyNameIs", txtYourName.Text);
                                socket.Emit("ConnectToOtherPlayer");
                            }
                            
                        }));
                        oldData = data;
                    }
                });
            }
        }
    }


}
