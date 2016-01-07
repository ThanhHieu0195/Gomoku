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
 
        //
        object synch = new object();
        object synch_pc_ol = new object();
        //
        player playeruser, player_pc, player_server;
        int testshot = 2;
        int testshot_online = 1;
        private void contructorForm()
        {
            /*Chỉnh kích thước form phụ thuộc vào kích thước ô cờ
             */
            cvs_gomoku.Width = number_cell * cell_width;
            cvs_gomoku.Height = number_cell * cell_height;
            menu.Width = border.Width = cvs_gomoku.Width + 4;
            border.Height = cvs_gomoku.Height + 4;
            gomoku_form.MinHeight = gomoku_form.Height = border.Height + 100;
            gomoku_form.MinWidth = gomoku_form.Width = border.Width + lvw_chat.Width + 60;
            _playername = tbx_name.Text;
            createMatrix(number_cell);

            //Tạo biến luôn phiên người chơi
            flag_player = true; //1vs2
            string mes = "Server: 1 vs 1\nPlayer 1: Red - Player 2: Blue.";
            mes = mes + getTime();
            lvw_chat.Items.Add(mes);

            //chế độ chơi mặc đinh
            playeruser = new player();
            playeruser.state = true;
            player_pc = new player();
            player_server = new player();

            newGame();
        }    
        public MainWindow()
        {
            InitializeComponent();
            contructorForm();
            //connectServer();
            //Event_Socket();
        }

        #region Biến
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
        bool flag_player;

        /*Ma trận
         * chứa các vị trí người chơi đã đánh
         */
        public int[,] matrix;
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
            menu.Width = border.Width = gomoku_form.Width - lvw_chat.Width - 60;
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
                
                player_server.state = true;
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
                if (matrix[col, row] != 0)
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
                matrix[col, row] = color_player;
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

            private player findWayforPC(int p1, int p2)
            {
                int col, row;
                do
                {
                    Point p = TimKiemNuocDi(p1, p2);
                    col = (int)p.X;
                    row = (int)p.Y;
                }
                while (matrix[col, row] != 0);
                player pc_new = new player(row, col, true);
                return pc_new;
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
                        if (playeruser.state == true)
                        {
                            if (player_server.state == false)
                            {

                                //chế độ người chơi offine 1vs 1
                                playeruser = new player(row, col, true);
                                //kiểm tra chơi người vs người
                                if (player_pc.state == false)
                                {

                                    testshot = 3 - testshot;
                                    drawGomoku(playeruser, testshot);
                                    if (checkWinner(playeruser, testshot))
                                        showWinner(testshot);
                                }
                                else //chơi vs máy
                                {
                                    playeruser = new player(row, col, true);
                                    drawGomoku(playeruser, 1);
                                    if (checkWinner(playeruser, 1))
                                        showWinner(1);
                                    else
                                    {
                                        player_pc = findWayforPC(1, 2);
                                        drawGomoku(player_pc, 2);
                                        if (checkWinner(player_pc, 2))
                                            showWinner(2);
                                    }
                                    testshot = 2;
                                }
                            }
                            else //chế độ online
                            {
                                if (player_pc.state == false)
                                {
                                    if (testshot_online != 1)
                                    {
                                        testshot_online = 4 - testshot_online;
                                        playeruser = new player(row, col, true);
                                        drawGomoku(playeruser, 1);
                                        socket.Emit("MyStepIs", JObject.FromObject(new { row = row, col = col }));
                                        if (checkWinner(playeruser, 1))
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
                if (player_server.state == true)
                {
                    socket.Emit("MyNameIs", tbx_name.Text);
                    socket.Emit("ConnectToOtherPlayer");
                }
                if (_playername != tbx_name.Text)
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
               
                if (player_server.state == false)
                {
                    playeruser.state = true;
                    player_pc.state = true;
                }
                else
                {
                    playeruser.state = false;
                    player_pc.state = true;
                }
                newGame();
                
            }

            private void btn_1vs1_Click(object sender, RoutedEventArgs e)
            {
                flag_player = true;
                playeruser.state = true;
                player_pc.state = false;
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
                if (player_server.state == true)
                { 
                    socket.Emit("ChatMessage", tbx_mes.Text);
                }
                string mes = tbx_mes.Text;

                if (mes == "")
                    mes = "Type something!!!";
                mes = _playername + ": " + mes + getTime();
                tbx_mes.Clear();
                lvw_chat.Items.Add(mes);
            }

            //Sự kiện load form
            private void gomoku_form_load(object sender, RoutedEventArgs e)
            {
                loadCellforRec();
            }
        #endregion
        #region Functions orther
            //Hàm khải tạo mãng Mảng matrix với giá trị mặc định là 0
            public void createMatrix(int size)
            {
                matrix = new int[size, size];
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        matrix[i, j] = 0;
                    }
                }
            }
            private void newGame()
            {
                //Xóa sạch khung chat
                lvw_chat.Items.Clear();
                //Xóa sạch bản
                cvs_gomoku.Children.Clear();
                //tạo ô cờ
                loadCellforRec();
                //tạo ma trận
                createMatrix(number_cell);
                 string mes = "";
                if (player_server.state == false)
                {
                    mes += "Sever: chế độ chơi offline. \n";
                    if (player_pc.state == false)
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

            //Xét người chơi thắng
            private bool checkWinner(player pl, int player_color)
            {
                int kt = player_color;
                int count = 1;
                int _row = pl.row, _col = pl.column;
                int x = pl.column, y = pl.row;
                //kiểm tra theo hàng
                while (y - 1 >= 0 && matrix[x, y - 1] == kt)
                {
                    count++;
                    y -= 1;
                }
                y = _row;
                while (y + 1 < 12 && matrix[x, y + 1] == kt)
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
                while (x - 1 >= 0 && matrix[x - 1, y] == kt)
                {
                    count++;
                    x -= 1;
                }
                x = _col;
                while (x + 1 < 12 && matrix[x + 1, y] == kt)
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
                while (x - 1 >= 0 && y - 1 >= 0 && matrix[x - 1, y - 1] == kt)
                {
                    count++;
                    x -= 1;
                    y -= 1;
                }
                x = _col;
                y = _row;
                while (x + 1 < 12 && y + 1 < 12 && matrix[x + 1, y + 1] == kt)
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
                while (x - 1 >= 0 && y + 1 <12 && matrix[x - 1, y + 1] == kt)
                {
                    count++;
                    x -= 1;
                    y += 1;
                }
                x = _col;
                y = _row;
                while (x + 1 < 12 && y - 1 > 0 && matrix[x + 1, y - 1] == kt)
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
            private void connectServer()
            {

                socket =  IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
             
                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        lvw_chat.Items.Add("Connected to Server");
                    }));
                    //lvw_chat.Items.Add("Enter to begin");
                    //lvw_chat.Items.Add("Enter to make your move");
                });
               
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
                                player_server = new player(row, col, true);
                                drawGomoku(player_server, 3);
                                if (checkWinner(player_server, 3))
                                {
                                    showWinner(3);
                                    newGame();
                                }
                                else
                                {
                                    String s = "Your turn!";
                                    lvw_chat.Items.Add(s);
                                }
                                if (player_pc.state == true)
                                {
                                    lock (synch_pc_ol)
                                    {
                                        playeruser = findWayforPC(3, 1);
                                        playeruser.state = true;
                                        drawGomoku(playeruser, 1);
                                        socket.Emit("MyStepIs", JObject.FromObject(new { row = playeruser.row, col = playeruser.column }));
                                        if (checkWinner(playeruser, 1))
                                        {
                                            showWinner(1);
                                            newGame();
                                        }
                                    }
                                    
                                }
                             
                            }
                        }
                       
                      
                    }));

                });
            }

            private String strimMess(string s)
           {
                for (int i = 34; i < s.Length-6; i++)
                {
                    if (s[i] == '<' && s[i + 1] == 'b' && s[i + 2] == 'r' && s[i + 4] == '/' && s[i +5 ] == '>')
                    {
                       
                        string s1 = s.Substring(40);
                        if (s1.Equals("You are the first player!"))
                        {
                            testshot_online = 3;
                            if (player_pc.state == true)
                            {
                                Random rd = new Random();
                                player_pc = new player(rd.Next(0,11), rd.Next(0,11), true);
                                drawGomoku(player_pc, 2);
                                socket.Emit("MyStepIs", JObject.FromObject(new { row = player_pc.row , col = player_pc.column }));
                                testshot_online = 2;
                            }
                        }
                        else
                        {
                            testshot_online = 1;
                        }
                        return s = (s.Remove(i, 6)).Insert(i, "\n");
                    }
                }
                return s;
            }
           
        #endregion

            private void mousemove_Item(object sender, MouseEventArgs e)
            {
                MenuItem mi = (MenuItem)sender;
                mi.FontStyle = FontStyles.Italic;
            }

            private void mouseleave_Item(object sender, MouseEventArgs e)
            {
                MenuItem mi = (MenuItem)sender;
                mi.FontStyle = FontStyles.Normal;
            }
            #region AI

            private long[] MangDiemTanCong = new long[7] { 0, 9, 54, 162, 1458, 13112, 118008 };
            private long[] MangDiemPhongNgu = new long[7] { 0, 3, 27, 99, 729, 6561, 59049 };

            // p1: đối thủ
        //p2: 
            private Point TimKiemNuocDi(int p1, int p2)
            {
                Point oCoResult = new Point();
                long DiemMax = 0;
                for (int i = 0; i < number_cell; i++)
                {
                    for (int j = 0; j < number_cell; j++)
                    {
                        if (matrix[i, j] == 0)
                        {
                            long DiemTanCong = DiemTanCong_DuyetDoc(i, j, p1, p2) + DiemTanCong_DuyetNgang(i, j, p1, p2) + DiemTanCong_DuyetCheoNguoc(i, j, p1, p2) + DiemTanCong_DuyetCheoXuoi(i, j, p1, p2);
                            long DiemPhongNgu = DiemPhongNgu_DuyetDoc(i, j, p1, p2) + DiemPhongNgu_DuyetNgang(i, j, p1, p2) + DiemPhongNgu_DuyetCheoNguoc(i, j, p1, p2) + DiemPhongNgu_DuyetCheoXuoi(i, j, p1, p2);
                            long DiemTam = DiemTanCong > DiemPhongNgu ? DiemTanCong : DiemPhongNgu;
                            if (DiemMax < DiemTam)
                            {
                                DiemMax = DiemTam;
                                oCoResult = new Point(i, j);

                            }
                        }
                    }
                }

                return oCoResult;
            }
            #region Tấn công
            private long DiemTanCong_DuyetDoc(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currDong + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong + Dem, currCot] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong + Dem, currCot] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currDong - Dem >= 0; Dem++)
                {
                    if (matrix[currDong - Dem, currCot] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong - Dem, currCot] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }
                if (SoQuanDich == 2)
                    return 0;
                DiemTong -= MangDiemPhongNgu[SoQuanDich + 1] * 2;
                DiemTong += MangDiemTanCong[SoQuanTa];
                return DiemTong;
            }
            private long DiemTanCong_DuyetNgang(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currCot + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong, currCot + Dem] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong, currCot + Dem] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currCot - Dem >= 0; Dem++)
                {
                    if (matrix[currDong, currCot - Dem] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong, currCot - Dem] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }
                if (SoQuanDich == 2)
                    return 0;
                DiemTong -= MangDiemPhongNgu[SoQuanDich + 1] * 2;
                DiemTong += MangDiemTanCong[SoQuanTa];
                return DiemTong;
            }
            private long DiemTanCong_DuyetCheoNguoc(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currCot + Dem < number_cell && currDong - Dem >= 0; Dem++)
                {
                    if (matrix[currDong - Dem, currCot + Dem] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong - Dem, currCot + Dem] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong + Dem, currCot - Dem] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong + Dem, currCot - Dem] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }
                if (SoQuanDich == 2)
                    return 0;
                DiemTong -= MangDiemPhongNgu[SoQuanDich + 1] * 2;
                DiemTong += MangDiemTanCong[SoQuanTa];
                return DiemTong;
            }
            private long DiemTanCong_DuyetCheoXuoi(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currCot + Dem < number_cell && currDong + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong + Dem, currCot + Dem] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong + Dem, currCot + Dem] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong - Dem >= 0; Dem++)
                {
                    if (matrix[currDong - Dem, currCot - Dem] == p1)
                        SoQuanTa++;
                    else if (matrix[currDong - Dem, currCot - Dem] == p2)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                        break;
                }
                if (SoQuanDich == 2)
                    return 0;
                DiemTong -= MangDiemPhongNgu[SoQuanDich + 1] * 2;
                DiemTong += MangDiemTanCong[SoQuanTa];
                return DiemTong;
            }
            #endregion
            #region Phòng ngự
            private long DiemPhongNgu_DuyetDoc(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currDong + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong + Dem, currCot] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong + Dem, currCot] == p2)
                    {
                        SoQuanDich++;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currDong - Dem >= 0; Dem++)
                {
                    if (matrix[currDong - Dem, currCot] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong - Dem, currCot] == p2)
                    {
                        SoQuanDich++;
                    }
                    else
                        break;
                }
                if (SoQuanTa == 2)
                    return 0;
                DiemTong += MangDiemPhongNgu[SoQuanDich];
                return DiemTong;
            }
            private long DiemPhongNgu_DuyetNgang(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currCot + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong, currCot + Dem] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong, currCot + Dem] == p2)
                    {
                        SoQuanDich++;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currCot - Dem >= 0; Dem++)
                {
                    if (matrix[currDong, currCot - Dem] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong, currCot - Dem] == p2)
                    {
                        SoQuanDich++;

                    }
                    else
                        break;
                }
                if (SoQuanTa == 2)
                    return 0;
                DiemTong += MangDiemPhongNgu[SoQuanDich];
                return DiemTong;
            }
            private long DiemPhongNgu_DuyetCheoNguoc(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currCot + Dem < number_cell && currDong - Dem >= 0; Dem++)
                {
                    if (matrix[currDong - Dem, currCot + Dem] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong - Dem, currCot + Dem] == p2)
                    {
                        SoQuanDich++;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong + Dem, currCot - Dem] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong + Dem, currCot - Dem] == p2)
                    {
                        SoQuanDich++;
                    }
                    else
                        break;
                }
                if (SoQuanTa == 2)
                    return 0;
                DiemTong += MangDiemPhongNgu[SoQuanTa];
                return DiemTong;
            }
            private long DiemPhongNgu_DuyetCheoXuoi(int currDong, int currCot, int p1, int p2)
            {
                long DiemTong = 0;
                int SoQuanTa = 0;
                int SoQuanDich = 0;
                for (int Dem = 1; Dem < 6 && currCot + Dem < number_cell && currDong + Dem < number_cell; Dem++)
                {
                    if (matrix[currDong + Dem, currCot + Dem] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong + Dem, currCot + Dem] == p2)
                    {
                        SoQuanDich++;
                    }
                    else
                        break;
                }

                for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong - Dem >= 0; Dem++)
                {
                    if (matrix[currDong - Dem, currCot - Dem] == p1)
                    {
                        SoQuanTa++;
                        break;
                    }
                    else if (matrix[currDong - Dem, currCot - Dem] == p2)
                    {
                        SoQuanDich++;
                    }
                    else
                        break;
                }
                if (SoQuanTa == 2)
                    return 0;

                DiemTong += MangDiemPhongNgu[SoQuanTa];
                return DiemTong;
            }
            #endregion
#endregion
    }
}
