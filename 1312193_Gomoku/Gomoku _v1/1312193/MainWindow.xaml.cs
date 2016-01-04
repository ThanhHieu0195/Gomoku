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
       
        //ô caro
        Rectangle rec;
        const int cost_rec = 50;
        int Max_square = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           

            Max_square = (int)cs_gomoku.ActualHeight / 50;

            for (int i = 0; i < Max_square; i++)
                for (int j = 0; j < Max_square; j++)
                {
                    rec = new Rectangle();
                    rec.Height = rec.Width = cost_rec;
                    if ((i+j) % 2 == 0)
                        rec.Fill = Brushes.White;
                    else
                        rec.Fill = Brushes.Tomato;
                    Canvas.SetLeft(rec, i*50);
                    Canvas.SetTop(rec, j*50);
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
            int Column = (int)(p.X / cost_rec) + 1;
            int Row = (int)(p.Y / cost_rec)+1;
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
            
            content = "User \t time: " + DateTime.Now.Hour +" : "+DateTime.Now.Minute+ "\n \t content: " +content; 
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
    }
}
