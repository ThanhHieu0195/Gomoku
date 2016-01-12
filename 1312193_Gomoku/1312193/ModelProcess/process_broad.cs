using _1312193.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _1312193.ModelProcess
{
    class process_broad
    {
        #region varible
        //Các biến người chơi:
        private player _user, _pc, _server;
        internal player Server
        {
            get { return _server; }
            set { _server = value; }
        }
        internal player Pc
        {
            get { return _pc; }
            set { _pc = value; }
        }
        internal player User
        {
            get { return _user; }
            set { _user = value; }
        }
        public broad mbroad;
        #endregion
        //contructor
        public process_broad(int cell_number)
        {
            User = new player();
            Pc = new player();
            Server = new player();
            Server.state = false;
            mbroad = new broad(cell_number);
            mbroad.createMatrix();
        }

        #region process
        //tìm nước đi:
        public player findWayforPC(int p1, int p2)
        {
            int col, row;
            do
            {
                Point p = TimKiemNuocDi(p1, p2);
                col = (int)p.X;
                row = (int)p.Y;
            }
            while (mbroad.matrix[col, row] != 0);
            player pc_new = new player(row, col, true);
            return pc_new;
        }
        //Thuật toán tìm đường đi
        #region AI

        private long[] MangDiemTanCong = new long[7] { 0, 9, 54, 162, 1458, 13112, 118008 };
        private long[] MangDiemPhongNgu = new long[7] { 0, 3, 27, 99, 729, 6561, 59049 };

        // p1: đối thủ
        //p2: 
        public Point TimKiemNuocDi(int p1, int p2)
        {
            Point oCoResult = new Point();
            long DiemMax = 0;
            for (int i = 0; i < mbroad.Cell_number; i++)
            {
                for (int j = 0; j < mbroad.Cell_number; j++)
                {
                    if (mbroad.matrix[i, j] == 0)
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
            for (int Dem = 1; Dem < 6 && currDong + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong + Dem, currCot] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong + Dem, currCot] == p2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }

            for (int Dem = 1; Dem < 6 && currDong - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong - Dem, currCot] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong - Dem, currCot] == p2)
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
            for (int Dem = 1; Dem < 6 && currCot + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong, currCot + Dem] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong, currCot + Dem] == p2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }

            for (int Dem = 1; Dem < 6 && currCot - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong, currCot - Dem] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong, currCot - Dem] == p2)
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
            for (int Dem = 1; Dem < 6 && currCot + Dem < mbroad.Cell_number && currDong - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong - Dem, currCot + Dem] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong - Dem, currCot + Dem] == p2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }
            
            for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong + Dem, currCot - Dem] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong + Dem, currCot - Dem] == p2)
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
            for (int Dem = 1; Dem < 6 && currCot + Dem < mbroad.Cell_number && currDong + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong + Dem, currCot + Dem] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong + Dem, currCot + Dem] == p2)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }

            for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong - Dem, currCot - Dem] == p1)
                    SoQuanTa++;
                else if (mbroad.matrix[currDong - Dem, currCot - Dem] == p2)
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
            for (int Dem = 1; Dem < 6 && currDong + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong + Dem, currCot] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong + Dem, currCot] == p2)
                {
                    SoQuanDich++;
                }
                else
                    break;
            }

            for (int Dem = 1; Dem < 6 && currDong - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong - Dem, currCot] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong - Dem, currCot] == p2)
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
            for (int Dem = 1; Dem < 6 && currCot + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong, currCot + Dem] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong, currCot + Dem] == p2)
                {
                    SoQuanDich++;
                }
                else
                    break;
            }

            for (int Dem = 1; Dem < 6 && currCot - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong, currCot - Dem] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong, currCot - Dem] == p2)
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
            for (int Dem = 1; Dem < 6 && currCot + Dem < mbroad.Cell_number && currDong - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong - Dem, currCot + Dem] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong - Dem, currCot + Dem] == p2)
                {
                    SoQuanDich++;
                }
                else
                    break;
            }

            for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong + Dem, currCot - Dem] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong + Dem, currCot - Dem] == p2)
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
            for (int Dem = 1; Dem < 6 && currCot + Dem < mbroad.Cell_number && currDong + Dem < mbroad.Cell_number; Dem++)
            {
                if (mbroad.matrix[currDong + Dem, currCot + Dem] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong + Dem, currCot + Dem] == p2)
                {
                    SoQuanDich++;
                }
                else
                    break;
            }

            for (int Dem = 1; Dem < 6 && currCot - Dem >= 0 && currDong - Dem >= 0; Dem++)
            {
                if (mbroad.matrix[currDong - Dem, currCot - Dem] == p1)
                {
                    SoQuanTa++;
                    break;
                }
                else if (mbroad.matrix[currDong - Dem, currCot - Dem] == p2)
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
        #endregion

    }
}