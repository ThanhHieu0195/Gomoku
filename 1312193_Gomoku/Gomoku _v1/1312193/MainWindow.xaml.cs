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

namespace _1312193
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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
            flag_game = true; //1vs2
            string mes = "Server: 1 vs 1\nPlayer 1: Red - Player 2: Blue.";
            mes = mes + getTime();
            lvw_chat.Items.Add(mes);

            newGame();
            color1 = Brushes.Red;
            color2 = Brushes.Blue;
        }    
        public MainWindow()
        {
            InitializeComponent();
            contructorForm();
           
        }

        #region Biến
        //ô chứa cờ
        Rectangle rec;

        //Số lượng ô vuông. mặc đinh 12
        const int number_cell = 12;

        //Kích thước 1 ô
        int cell_height = 50, cell_width = 50;

        //Tên người chơi
        string _playername;

        /*Biến chứa màu cờ khi đánh.
         * color1 ứng vs người chơi thứ nhất
         * color2 ứng với người chơi thứ 2
         */
        Brush color1;
        Brush color2;
        /*Biến đánh dấu lượt chơi
        * player_flag = true => player1
        * player_flag = false => player2
        */
        public bool player_flag = true;

        /*Chế độ chơi
         * 1vs1 hoặc 1vscom
         */
        bool flag_game;

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

            }
            private void cvs_gomoku_MouseDown(object sender, MouseButtonEventArgs e)
            {


                //Xác định vị trí
                Point p = new Point();
                p = e.GetPosition(cvs_gomoku);
                int col, row;
                col = (int)(p.X / cell_width);
                row = (int)(p.Y / cell_height);

                if (matrix[col, row] != 0)
                {
                    //MessageBox.Show("Ô đã được chọn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    string mes = "Server: Not allow!!!";
                    mes = mes + getTime();
                    lvw_chat.Items.Add(mes);
                }
                else
                {
                    //Rectangle cell_rec = createRec(cell_width - 4, cell_height - 4, col, row, color1);
                    Ellipse cell_elip = new Ellipse();

                    if (player_flag)
                    {
                        //nguoi choi 1
                        cell_elip = createElip(cell_width - 4, cell_height - 4, col, row, color1);
                        matrix[col, row] = 1;
                        cvs_gomoku.Children.Add(cell_elip);
                        //Thay doi luot choi
                        player_flag = !player_flag;
                        //kiểm tra thắng thua.
                        if (checkWinner(!player_flag, col, row))
                        {
                            showWinner();
                            //MessageBox
                        }


                        //toi luot may neu choi 1vsCOM
                        if (!flag_game)
                        {
                            do
                            {
                                Random rd = new Random();
                                col = rd.Next(0, 11);
                                row = rd.Next(0, 11);
                            }
                            while (matrix[col, row] != 0);

                            cell_elip = createElip(cell_width - 4, cell_height - 4, col, row, color2);
                            matrix[col, row] = 2;
                            cvs_gomoku.Children.Add(cell_elip);
                            //Thay doi luot choi
                            player_flag = !player_flag;
                            //kiểm tra thắng thua.

                            if (checkWinner(!player_flag, col, row))
                            {
                                showWinner();
                                //MessageBox
                            }

                        }
                    }
                    else
                    {
                        if (flag_game)
                        {
                            //nguoi choi 2
                            cell_elip = createElip(cell_width - 4, cell_height - 4, col, row, color2);
                            matrix[col, row] = 2;
                            cvs_gomoku.Children.Add(cell_elip);
                            //Thay doi luot choi
                            player_flag = !player_flag;
                            //kiểm tra thắng thua.
                            if (checkWinner(!player_flag, col, row))
                            {
                                showWinner();
                                //MessageBox
                            }

                        }
                    }
                }
            }
            private void btn_name_Click(object sender, RoutedEventArgs e)
            {
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
                flag_game = false;
                string mes = "Server: 1 vs COM\nPlayer 1: Red - COM: Blue.";
                mes = mes + getTime();
                lvw_chat.Items.Add(mes);
                newGame();
            }

            private void btn_1vs1_Click(object sender, RoutedEventArgs e)
            {
                flag_game = true;
                string mes = "Server: 1 vs 1\nPlayer 1: Red - Player 2: Blue.";
                mes = mes + getTime();
                lvw_chat.Items.Add(mes);
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
                cvs_gomoku.Children.Clear();
                loadCellforRec();
                createMatrix(number_cell);
                string mes = "Server: First turn: ";
                if (player_flag)
                    mes = mes + "Player 1.";
                else
                    if (flag_game)
                        mes = mes + "Player 2.";
                    else
                        mes = mes + "COM.";
                mes = mes + getTime();
                lvw_chat.Items.Add(mes);
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

            private void showWinner()
            {
                string winner;
                if (flag_game)
                    if (!player_flag)
                        winner = "Player 1 win";
                    else
                        winner = "Player 2 win";
                else
                    if (!player_flag)
                        winner = "Player win";
                    else
                        winner = "COM win";
                MessageBox.Show(winner, "Winner", MessageBoxButton.OK);
                // player_flag = true;
                newGame();
            }

            //Xét người chơi thắng
            private bool checkWinner(bool _player_flag, int _col, int _row)
            {
                int kt = (_player_flag == true) ? 1 : 2;
                int count = 1;
                int x = _col, y = _row;
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
                while (x - 1 >= 0 && y + 1 >= 0 && matrix[x - 1, y + 1] == kt)
                {
                    count++;
                    x -= 1;
                    y += 1;
                }
                x = _col;
                y = _row;
                while (x + 1 < 12 && y - 1 < 12 && matrix[x + 1, y - 1] == kt)
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

           
    }
}
