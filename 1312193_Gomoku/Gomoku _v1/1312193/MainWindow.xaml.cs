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
        double width_rec=0;
        double height_rec = 0;
        const int number_rec = 12;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //detele data of canvas
            cs_gomoku.Children.Clear();
            //set value for canvas and border
            double height, width;
            width = height = (grid_fram.ActualHeight+ grid_fram.ActualWidth)/ 3;

            if (height < grid_fram.ActualHeight)
                border_canvas.Height = height;
            if (width < grid_fram.ActualHeight)
                border_canvas.Width = width; 
            
            //cs_gomoku.Height = cs_gomoku.Width = border_canvas.ActualHeight - border_canvas.ActualHeight / 10;
       
            height_rec = cs_gomoku.ActualHeight / number_rec;
            width_rec = cs_gomoku.ActualWidth / number_rec;
            //Draw 
            for (int i = 0; i < number_rec; i++)
            {
                for (int j = 0; j < number_rec; j++)
                {
                    rec = new Rectangle();
                    rec.Height = height_rec;
                    rec.Width = width_rec;
                    if ((i + j) % 2 == 0)
                    {
                        rec.Fill = Brushes.White;
                    }
                    else
                    {
                        rec.Fill = Brushes.Black;
                    }
                    Canvas.SetLeft(rec, width_rec * i);
                    Canvas.SetTop(rec, height_rec * j);
                    cs_gomoku.Children.Add(rec);
                }
            }
           

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cs_gomoku_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = new Point();
            p = e.GetPosition(cs_gomoku);
            int Column = (int)(p.X / width_rec) + 1;
            int Row = (int)(p.Y / height_rec)+1;
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
                         MessageBox.Show(ActualHeight + " " + ActualWidth, "");
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

        private void frm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window_Loaded(sender, e);   
        }
    }
}
