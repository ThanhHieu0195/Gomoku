using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1312193
{
    class player
    {
        public int row { get; set; }
        public int column { get; set; }
        public bool state { get; set; }
        public player()
        {
            row = column = -1;
            state = false;
        }
        public player(int row, int column)
        {
            this.row = row;
            this.column = column;
            state = false;
        }
        public player(int row, int column, bool state)
        {
            this.row = row;
            this.column = column;
            this.state = state;
        }
        public player(player pObject)
        {
            state = pObject.state;
            row = pObject.row;
            column = pObject.column;
        }
    }
}
