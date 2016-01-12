using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1312193.Model
{
    class broad
    {
        #region varible
        public int[,] matrix;
        private int cell_number;
        #endregion
        #region process
        public int Cell_number
        {
            get { return cell_number; }
            set { cell_number = value; }
        }
        public broad()
        {
            Cell_number = 0;
        }
        public broad(int number)
        {
            Cell_number = number;
        }
        //tạo matrix
        public void createMatrix()
        {
            int size = Cell_number;
            matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = 0;
                }
            }
        }
        #endregion
    }
}
