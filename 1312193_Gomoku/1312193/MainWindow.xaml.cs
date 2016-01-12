using _1312193.Model;
using _1312193.ModelProcess;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace _1312193
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
 
        //Hàm khởi tạo khung chơi
        private void contructorForm()
        {
            /*Chỉnh kích thước form phụ thuộc vào kích thước ô cờ
             */
            cvs_gomoku.Width = number_cell * cell_width;
            cvs_gomoku.Height = number_cell * cell_height;
          
            border.Height = cvs_gomoku.Height + 4;
            gomoku_form.MinHeight = gomoku_form.Height = border.Height + 70;
            gomoku_form.MinWidth = gomoku_form.Width = border.Width + lvw_chat.Width + 60;
            _playername = tbx_name.Text;

            //Tạo biến luôn phiên người chơi
            
            string mes = "Server: 1 vs 1\nPlayer 1: Red - Player 2: Blue.";
            mes = mes + getTime();
            lvw_chat.Items.Add(mes);
            newGame();
        }    
        public MainWindow()
        {
            
            //chế độ chơi mặc đinh
            mybroad.User.state = true;

            InitializeComponent();
            contructorForm();
        }

        #region Biến
        //
        object synch = new object();
        object synch_pc_ol = new object();
        //tầng sử lí:
        process_broad mybroad = new process_broad(number_cell);

        int testshot = 2;
        int testshot_online = 1;

        Socket socket;
       //ô chứa cờ
        Rectangle rec;

        //Số lượng ô vuông. mặc đinh 12
        const int number_cell = 12;

        //Kích thước 1 ô
        int cell_height = 50, cell_width = 50;

        //Tên người chơi
        string _playername;

      

        /*Chế độ chơi
         * 1vs1 hoặc 1vscom
         */
        #endregion
        #region Functions Edit to form 
        //Tạo quân cờ
        private Ellipse createElip(int _width, int _height, int _col, int _row, Brush color)
        {
            var elip = new Ellipse
            {
                Height = _height,
                Width = _width,
                Fill = color
            };
            Canvas.SetLeft(elip, _col * cell_width + 2);
            Canvas.SetTop(elip, _row * cell_height + 2);
            return elip;
        }

        // tạo ô cờ
        private Rectangle createRec(int _width, int _height, int _col, int _row, Brush color)
        {
            var rec = new Rectangle
            {
                Height = _height,
                Width = _width,
                Fill = color
            };
            Canvas.SetLeft(rec, _col * cell_width);
            Canvas.SetTop(rec, _row * cell_height);
            return rec;
        }
        //Hàm thay đổi kích thước của cả form
        private void resize()
        {
            border.Height = gomoku_form.Height - 60;
            cell_height = (int.Parse(border.Height.ToString()) - 4) / number_cell;
            cell_width = (int.Parse(border.Width.ToString()) - 4) / number_cell;
            cvs_gomoku.Height = cell_height * number_cell;
            cvs_gomoku.Width = cell_width * number_cell;
            border.Width = cvs_gomoku.Width + 4;
            border.Height = cvs_gomoku.Height + 4;
        }
        #endregion EndFuntion
        #region Functions event process
            private void btn_playService_Click(object sender, RoutedEventArgs e)
            {
                
                mybroad.Server.state = true;

                newGame();
                connectServer();
           
            }
            private void showNote_ErrorChot()
            {
                string mes = "Server: Not allow!!!";
                mes = mes + getTime();
                lvw_chat.Items.Add(mes);
            }
            private bool testLocal(int row, int col)
            {
                if (mybroad.mbroad.matrix[col, row] != 0)
                {
                    return false;
                }
                return true;
            }
            //Hàm vẽ cờ
            private bool drawGomoku(player p_object, int color_player)
            {
                int col = p_object.column;
                int row = p_object.row;
                //Xét ô đã được đánh
                testLocal(row, col);
                Ellipse cell_elip = new Ellipse();
                cell_elip = createElip(cell_width - 4, cell_height - 4, col, row, switchPlayerColor(color_player));
                mybroad.mbroad.matrix[col, row] = color_player;
                cvs_gomoku.Children.Add(cell_elip);
                return true;
                
            }
            private Brush switchPlayerColor(int color)
            {
                Brush cl;
                switch (color)
                {
                    case 1:
                        cl = Brushes.Red;
                        break;
                    case 2:
                        cl = Brushes.Blue;
                        break;
                    case 3:
                        cl =  Brushes.Yellow;
                        break;
                    default:
                        cl = Brushes.Black;
                        break;
                }
                return cl;
            }

            private void cvs_gomoku_MouseDown(object sender, MouseButtonEventArgs e)
            {
                lock (synch)
                {
                    int col = -1, row = -1;
              
                    Point p = new Point();
                    p = e.GetPosition(cvs_gomoku);
                    col = (int)(p.X / cell_width);
                    row = (int)(p.Y / cell_height);
                
                    //xác định vị trí người chơi nhấp vào:
                   



                    if (testLocal(row, col))    //Kiểm tra ví trí chọn có hợp lệ
                    {
                        //xét trường hợp: người chơi nhấp vào bàn cờ
                        /*TH1: Người chơi có chọn chế độ chơi
                         * TH1.1 Người chơi chơi offline
                        */
                        if (mybroad.User.state == true)
                        {
                            if (mybroad.Server.state == false)
                            {

                                //chế độ người chơi offine 1vs 1
                                mybroad.User = new player(row, col, true);
                                //kiểm tra chơi người vs người
                                if (mybroad.Pc.state == false)
                                {

                                    testshot = 3 - testshot;
                                    drawGomoku(mybroad.User, testshot);
                                    if (checkWinner(mybroad.User, testshot))
                                        showWinner(testshot);
                                }
                                else //chơi vs máy
                                {
                                    mybroad.User = new player(row, col, true);
                                    drawGomoku(mybroad.User, 1);
                                    if (checkWinner(mybroad.User, 1))
                                        showWinner(1);
                                    else
                                    {
                                      
                                        mybroad.Pc = mybroad.findWayforPC(1, 2);
                                        drawGomoku(mybroad.Pc, 2);
                                        if (checkWinner(mybroad.Pc, 2))
                                            showWinner(2);
                                    }
                                    testshot = 2;
                                }
                            }
                            else //chế độ online
                            {
                                if (mybroad.Pc.state == false)
                                {
                                    if (testshot_online != 1)
                                    {
                                        testshot_online = 4 - testshot_online;
                                        mybroad.User = new player(row, col, true);
                                        drawGomoku(mybroad.User, 1);
                                        socket.Emit("MyStepIs", JObject.FromObject(new { row = row, col = col }));
                                        if (checkWinner(mybroad.User, 1))
                                            showWinner(1);
                                    }
                                    else
                                    {
                                        lvw_chat.Items.Add("Waiting plays");
                                    }

                                }
                            
                             
                            }
                        }
                    }
                }
            }

            private void btn_name_Click(object sender, RoutedEventArgs e)
            {
                if (mybroad.Server.state == true)
                {
                    try
                    {
                        socket.Emit("MyNameIs", tbx_name.Text);
                       // socket.Emit("ConnectToOtherPlayer");
                    }
                    catch
                    {
                        String mes = "You not connect to server";
                        lvw_chat.Items.Add(mes);
                    }
                }
                else if (_playername != tbx_name.Text)
                {
                    string mes;
                    mes = "Server: " + _playername;
                    _playername = tbx_name.Text;
                    mes = mes + " is now called " + _playername + getTime();
                    lvw_chat.Items.Add(mes);
                }
            }

            private void btn_1vsCOM_Click(object sender, RoutedEventArgs e)
            {
               
                if (mybroad.Server.state == false)
                {
                    mybroad.User.state = true;
                    mybroad.Pc.state = true;
                }
                else
                {
                    mybroad.User.state = false;
                    mybroad.Pc.state = true;
                }
                newGame();
                
            }

            private void btn_1vs1_Click(object sender, RoutedEventArgs e)
            {
                
                mybroad.User.state = true;
                mybroad.Pc.state = false;
                newGame();
            }
            //Sự kiện thay đổi form
            private void gomoku_form_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                resize();
                cvs_gomoku.Children.Clear();
                loadCellforRec();
            }

            //bắt sự kiện sử lí gửi mess
            private void btn_sendmes_Click(object sender, RoutedEventArgs e)
            {
                string mes = tbx_mes.Text;

                if (mes == "")
                    mes = "Type something!!!";
                mes = _playername + ": " + mes + getTime();
                tbx_mes.Clear();
                if (mybroad.Server.state == true)
                { 
                    socket.Emit("ChatMessage", mes);
                }
                else
                {
                   
                  
                    lvw_chat.Items.Add(mes);
                }
              
            }

            //Sự kiện load form
            private void gomoku_form_load(object sender, RoutedEventArgs e)
            {
                loadCellforRec();
            }
        #endregion
        #region Functions orther
           
            private void newGame()
            {
                //Xóa sạch khung chat
                lvw_chat.Items.Clear();
                //Xóa sạch bản
                cvs_gomoku.Children.Clear();
                //tạo ô cờ
                loadCellforRec();
                //tạo ma trận
                mybroad.mbroad.createMatrix();
                 string mes = "";
                if (mybroad.Server.state == false)
                {
                    mes += "Sever: chế độ chơi offline. \n";
                    if (mybroad.Pc.state == false)
                    {
                        mes += "1 vs 1 \n";
                        mes += "Turn first: ";
                        mes += (testshot==1) ? "player2" : "player1";
                        mes = mes + getTime();
                        lvw_chat.Items.Add(mes);
                    }
                    else
                    {
                        mes += "1 vs com \n";
                        mes += "Turn first: ";
                        mes += (testshot == 1) ? "Com" : "player1";
                        mes = mes + getTime();
                        lvw_chat.Items.Add(mes);
                    }
                }
                else
                {
                    mes += "Sever: chế độ chơi online \n";
                    lvw_chat.Items.Add(mes);
                }
            }

            //Hàm lấy thời gian
            private string getTime()
            {
                string time = "\n(" +
                           DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "-" +
                           DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year +
                           ")";
                return time;
            }

            /*cập nhật dữ liệu ban đầu cho cho bảng cờ
         * màu nền ô ví trí
         */
            private void loadCellforRec()
            {
                for (int i = 0; i < number_cell; i++)
                {
                    for (int j = 0; j < number_cell; j++)
                    {
                        Brush color;
                        if (i % 2 == j % 2)
                            color = Brushes.Lavender;
                        else
                            color = Brushes.Gray;
                        rec = createRec(cell_width, cell_height, i, j, color);
                        cvs_gomoku.Children.Add(rec);
                    }
                }
            }
            //xuất người chiến thắng
            private void showWinner(int player_color)
            {
                string winner="";
                switch (player_color)
                {
                    case 1:
                        winner = "User1 win";
                        break;
                    case 2:
                        winner = "User2 win";
                        break;
                    case 3:
                        winner = "Server win";
                        break;
                }
                MessageBox.Show(winner, "Winner", MessageBoxButton.OK);
                newGame();
            }
            //kiểm tra người chơi thắng
            private bool checkWinner(player pl, int player_color)
            {
                int kt = player_color;
                int count = 1;
                int _row = pl.row, _col = pl.column;
                int x = pl.column, y = pl.row;
                //kiểm tra theo hàng
                while (y - 1 >= 0 && mybroad.mbroad.matrix[x, y - 1] == kt)
                {
                    count++;
                    y -= 1;
                }
                y = _row;
                while (y + 1 < 12 && mybroad.mbroad.matrix[x, y + 1] == kt)
                {
                    count++;
                    y += 1;
                }
                if (count == 5)
                    return true;
                //kiểm tra theo cột:
                x = _col;
                y = _row;
                count = 1;
                while (x - 1 >= 0 && mybroad.mbroad.matrix[x - 1, y] == kt)
                {
                    count++;
                    x -= 1;
                }
                x = _col;
                while (x + 1 < 12 && mybroad.mbroad.matrix[x + 1, y] == kt)
                {
                    count++;
                    x += 1;
                }
                if (count == 5)
                    return true;
                //theo chiều chéo trên xuống
                x = _col;
                y = _row;
                count = 1;
                while (x - 1 >= 0 && y - 1 >= 0 && mybroad.mbroad.matrix[x - 1, y - 1] == kt)
                {
                    count++;
                    x -= 1;
                    y -= 1;
                }
                x = _col;
                y = _row;
                while (x + 1 < 12 && y + 1 < 12 && mybroad.mbroad.matrix[x + 1, y + 1] == kt)
                {
                    count++;
                    x += 1;
                    y += 1;
                }
                if (count == 5)
                    return true;
                //theo chiều chéo dưới lên
                x = _col;
                y = _row;
                count = 1;
                while (x - 1 >= 0 && y + 1 <12 && mybroad.mbroad.matrix[x - 1, y + 1] == kt)
                {
                    count++;
                    x -= 1;
                    y += 1;
                }
                x = _col;
                y = _row;
                while (x + 1 < 12 && y - 1 > 0 && mybroad.mbroad.matrix[x + 1, y - 1] == kt)
                {
                    count++;
                    x += 1;
                    y -= 1;
                }
                if (count == 5)
                    return true;
                return false;
            }
        #endregion 
        #region Function Socket
            //kết nối server
            private void connectServer()
            {
                String cnt = System.Configuration.ConfigurationSettings.AppSettings["IPCONNECT"];
                socket =  IO.Socket(cnt);
             //Sự kiện kết nối thành công
                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        lvw_chat.Items.Add("Connected to Server");
                    }));
                });

               //Sư kiên tin nhắn
                socket.On(Socket.EVENT_MESSAGE, (data) =>
                {
                    
                    this.Dispatcher.Invoke((Action)(() =>
                    {

                        lvw_chat.Items.Add(data + "");
                    }));
                });
                socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                      //  String s = ((Newtonsoft.Json.Linq.JObject)data)["message"].ToString();
                        lvw_chat.Items.Add("connect Error");
                    }));
                });


                socket.On("ChatMessage", (data) =>
                {

                    if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {

                            socket.Emit("MyNameIs", tbx_name.Text);
                            socket.Emit("ConnectToOtherPlayer");

                        }));

                    }
                    else
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            String s = ((Newtonsoft.Json.Linq.JObject)data)["message"].ToString();
                            s = strimMess(s);
                            lvw_chat.Items.Add(s + "");
                            //socket.Emit("ChatMessage", "Start");
                        }));
                    }

                });
                socket.On(Socket.EVENT_ERROR, (data) =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        lvw_chat.Items.Add(data + "event error");
                    }));
                });
                socket.On("NextStepIs", (data) =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        lock (synch)
                        {
                            int row = (int)((JObject)data).GetValue("row");
                            int col = (int)((JObject)data).GetValue("col");
                            if (testLocal(row, col))
                            {
                                testshot_online = 3;
                                mybroad.Server = new player(row, col, true);
                                drawGomoku(mybroad.Server, 3);
                                if (checkWinner(mybroad.Server, 3))
                                {
                                    showWinner(3);
                                    socket.Emit("ConnectToOtherPlayer");
                                }
                              
                                if (mybroad.Pc.state == true)
                                {
                                    lock (synch_pc_ol)
                                    {
                                        mybroad.User = mybroad.findWayforPC(3, 2);
                                        mybroad.User.state = true;
                                        drawGomoku(mybroad.User, 2);
                                        socket.Emit("MyStepIs", JObject.FromObject(new { row = mybroad.User.row, col = mybroad.User.column }));
                                        if (checkWinner(mybroad.User, 2))
                                        {
                                            showWinner(2);
                                            newGame();
                                        }
                                    }
                                    
                                }
                             
                            }
                        }
                       
                      
                    }));

                });
            }
            //Xử lí mess đánh trước sau
            private String strimMess(string s)
           {
               String kq = s;
               if (s.IndexOf("You are the first player!") != -1)
               {
                   testshot_online = 3;
                   if (mybroad.Pc.state == true)
                   {
                       Random rd = new Random();
                       mybroad.Pc = new player(rd.Next(0, 11), rd.Next(0, 11), true);
                       drawGomoku(mybroad.Pc, 2);
                       socket.Emit("MyStepIs", JObject.FromObject(new { row = mybroad.Pc.row, col = mybroad.Pc.column }));
                       testshot_online = 2;
                   }
               }
               else if (s.IndexOf("You are the second player!") != -1)
               {
                   testshot_online = 1;
               }
               int test = kq.IndexOf("<br />");
               while (test != -1)
               {
                   kq = kq.Remove(test, 6);
                   test = kq.IndexOf("<br />");
               }
               return kq;
                //for (int i = 34; i < s.Length-6; i++)
                //{
                //    if (s[i] == '<' && s[i + 1] == 'b' && s[i + 2] == 'r' && s[i + 4] == '/' && s[i +5 ] == '>')
                //    {
                       
                //        string s1 = s.Substring(40);
                     
                //        if (s1.Equals("You are the first player!"))
                //        {
                //            testshot_online = 3;
                //            if (mybroad.Pc.state == true)
                //            {
                //                Random rd = new Random();
                //                mybroad.Pc = new player(rd.Next(0,11), rd.Next(0,11), true);
                //                drawGomoku(mybroad.Pc, 2);
                //                socket.Emit("MyStepIs", JObject.FromObject(new { row = mybroad.Pc.row , col = mybroad.Pc.column }));
                //                testshot_online = 2;
                //            }
                //        }
                //        else
                //        {
                //            testshot_online = 1;
                //        }
                //        return s = (s.Remove(i, 6)).Insert(i, "\n");
                //    }
                //}
                //return s;
            }
           
        #endregion
        #region SettingFunction
            private void rd_playeruser_Click(object sender, RoutedEventArgs e)
            {
              
                mybroad.User.state = true;
                mybroad.Pc.state = false;
            }

            private void rd_playerpc_Click(object sender, RoutedEventArgs e)
            {
                if (mybroad.Server.state == false)
                {
                    mybroad.User.state = true;
                    mybroad.Pc.state = true;
                }
                else
                {
                    mybroad.User.state = false;
                    mybroad.Pc.state = true;
                }
            }

            private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                ComboBox cb = (ComboBox)sender;
                if (cb.SelectedIndex == 0)
                {
                    mybroad.Server.state = false;
                }
                else
                {
                    mybroad.Server.state = true;
                }
            }

            private void Button_Click(object sender, RoutedEventArgs e)
            {
                newGame();
                if (mybroad.Server.state == true)
                {
                    connectServer();
                }
            }
            #endregion
    }
}
