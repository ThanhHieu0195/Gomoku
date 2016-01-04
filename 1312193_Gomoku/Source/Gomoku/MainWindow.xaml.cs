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

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         //ô caro
        Rectangle rec;
        double cost_rec_height = 0;
        double cost_rec_width = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //chiều cao của 1 ô
            cost_rec_height = cs_gomoku.ActualHeight/12;
            //chiều rộng của 1 ô
            cost_rec_width = cs_gomoku.ActualWidth / 12;
            //số ô
            int Max_square = 12;

            for (int i = 0; i < Max_square; i++)
                for (int j = 0; j < Max_square; j++)
                {
                    rec = new Rectangle();
                    rec.Height = cost_rec_height;
                    rec.Width = cost_rec_width;
                    if ((i+j) % 2 == 0)
                        rec.Fill = Brushes.White;
                    else
                        rec.Fill = Brushes.Tomato;
                    Canvas.SetLeft(rec, i * cost_rec_width);
                    Canvas.SetTop(rec, j * cost_rec_height);
                    cs_gomoku.Children.Add(rec);

                }

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cs_gomoku_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = new Point();
            p = e.GetPosition(cs_gomoku);
            int Column = (int)(p.X / cost_rec_height) + 1;
            int Row = (int)(p.Y / cost_rec_width) + 1;
            MessageBox.Show("Column: " + Column + "\n" + "Row: " + Row, "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;
                case Key.Enter:
                         Button_Click(btn_send, null);
                    break;
            }

               
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String content = tb_chat.Text;
            String time = DateTime.Now.Date.ToShortTimeString();
            
            content = "User \t time: " + time +" : "+DateTime.Now.Minute+ "\n \t content: " +content; 
            tb_chat.Clear();
            lv_chat.Items.Add(content);
        }


        private void tb_chat_TextChanged(object sender, TextChangedEventArgs e)
        {
            btn_send.KeyDown += Window_KeyDown;
            
        }

        private void tb_chat_TouchEnter(object sender, TouchEventArgs e)
        {

        }

        private void cs_gomoku_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cs_gomoku.Children.Clear();
            Window_Loaded(null, null);
        }
    }
  
}
