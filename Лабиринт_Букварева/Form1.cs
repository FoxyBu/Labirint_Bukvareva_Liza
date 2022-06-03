using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


namespace Лабиринт_Букварева
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int Cellw, Cellh;
        Labirint Lab = new Labirint(10, 10);
        Bitmap bit1 = new Bitmap(1, 1);

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        //Cтруктура клетки лабиринта
        public struct Cell
        {
            //координаты клетки
            public int x { get; set; }
            public int y { get; set; }
            public bool Cell_0 { get; set; }
            public bool Visited { get; set; }
            public Cell(int X, int Y, bool Visited_0 = false, bool Cell_1 = true)
            {
                x = X;
                y = Y;
                Cell_0 = Cell_1;
                Visited = Visited_0;
            }
        }


        class Labirint
        {
            public readonly Cell[,] cells;
            private int Width1;
            private int Height1;
            public Stack<Cell> path = new Stack<Cell>(); //память, предназначенная для сохранения пути 
            public List<Cell> neighbours = new List<Cell>(); //список соседей текущей клетки
            public Random rnd = new Random();
            public Cell start;
            public List<Cell> visited = new List<Cell>(); //список посещенных клеток
            public List<Cell> solve = new List<Cell>(); //список клеток, которые являются решением лабиринта
            public Cell finish;
            public Labirint(int width, int height)
            {
                start = new Cell(1, 1, true, true);//начальная клетка, расположенная в левом верхнем углу
                finish = new Cell(width - 3, height - 3, true, true);// финишная клетка, расположенная в правом нижнем углу
                Width1 = width;
                Height1 = height;
                cells = new Cell[width, height];
                for (var i = 0; i < width; i++)
                    for (var j = 0; j < height; j++)
                        if ((i % 2 != 0 && j % 2 != 0) && (i < Width1 - 1 && j < Height1 - 1)) //если ячейка нечетная по х и по у и не выходит за пределы лабиринта
                        {
                            cells[i, j] = new Cell(i, j); //то это нужная клетка
                        }
                        else
                        {
                            cells[i, j] = new Cell(i, j, false, false);
                        }
                path.Push(start); //добавим нужную нам клетку в стек (наверх)
                cells[start.x, start.y] = start; //эта клетка будет начальной
            }

            private void GetNeighbours2(Cell localcell)
            {
                //начальное положение клетки
                int xk = localcell.x;
                int yk = localcell.y;
                const int dist = 2; //растояние между клетками, равное двум, т.е. ищем соседей через клетку

                Cell[] posNeighbours = new[] // Все возможные соседи
                {
                new Cell(xk, yk - dist), // Вверх
                new Cell(xk, yk + dist), // Вниз
                new Cell(xk + dist, yk), // Вправо
                new Cell(xk - dist, yk) // Влево
                 };

                //проверяем соседей во всех направлениях
                for (int i = 0; i < 4; i++)
                {
                    Cell currentNeighbour = posNeighbours[i];//текущей клетке присваивается значение возможного соседа
                    if (currentNeighbour.x > 0 && currentNeighbour.x < Width1 && currentNeighbour.y > 0 && currentNeighbour.y < Height1)
                    {
                        if (cells[currentNeighbour.x, currentNeighbour.y].Cell_0 && !cells[currentNeighbour.x, currentNeighbour.y].Visited)
                        {
                            neighbours.Add(currentNeighbour);
                        }
                    }
                }
            }


            private Cell AddCell(List<Cell> neighbours) //выбор случайной клетки, которая находится поблизости
            {
                int r = rnd.Next(neighbours.Count);
                return neighbours[r];
            }


            private void RemoveWall(Cell first, Cell second)
            {
                int xd = second.x - first.x;
                int yd = second.y - first.y;
                // направление удаления стены
                int addx = (xd != 0) ? xd / Math.Abs(xd) : 0;
                int addy = (yd != 0) ? yd / Math.Abs(yd) : 0;
                // координаты удаленной стены
                cells[first.x + addx, first.y + addy].Cell_0 = true; //делаем стену клеткой
                cells[first.x + addx, first.y + addy].Visited = true;
                second.Visited = true; // клетка становится посещенной
                cells[second.x, second.y] = second;
            }

            public void LabirintCreate()
            {
                cells[start.x, start.y] = start;
                while (path.Count != 0) //пока в стеке есть клетки ищем соседей и строим путь
                {
                    neighbours.Clear();
                    GetNeighbours2(path.Peek());
                    if (neighbours.Count != 0)
                    {
                        Cell nextCell = AddCell(neighbours);
                        RemoveWall(path.Peek(), nextCell);
                        nextCell.Visited = true; //делаем текущую клетку посещенной
                        cells[nextCell.x, nextCell.y].Visited = true; //и в общем массиве
                        path.Push(nextCell); //затем добавляем её в стек
                    }
                    else
                    {
                        path.Pop(); //иначе удаляем ее
                    }
                }
            }

            public void LabirintSolve()
            {
                bool flag = false; //флаг, отвечающий за достижения финиша
                foreach (Cell c in cells)
                {
                    if (cells[c.x, c.y].Cell_0 == true)
                    {
                        cells[c.x, c.y].Visited = false;
                    }
                }

                path.Clear();
                path.Push(start);

                while (path.Count != 0) //пока у нас есть клетки, строим путь и находим соседние клетки
                {
                    if (path.Peek().x == finish.x && path.Peek().y == finish.y) //если это финиш, то конец игры
                    {
                        flag = true;
                    }

                    if (!flag) //иначе продолжаем искать путь
                    {
                        neighbours.Clear();
                        GetNeighbours1(path.Peek());
                        if (neighbours.Count != 0) //если количество соседей не равно нулю
                        {
                            Cell nextCell = AddCell(neighbours);
                            nextCell.Visited = true; // клетка, в которой мы находимся, становится посещенной
                            cells[nextCell.x, nextCell.y].Visited = true;
                            path.Push(nextCell); //добавим эту клетку в стек
                            visited.Add(path.Peek());
                        }
                        else
                        {
                            path.Pop(); //иначе удаляем эту клетку (последнюю)
                        }
                    }

                    else
                    {
                        solve.Add(path.Peek()); //
                        path.Pop(); //
                    }
                }
            }

            private void GetNeighbours1(Cell localcell) // Получаем соседа текущей клетки
            {

                int x2 = localcell.x;
                int y2 = localcell.y;
                const int distance = 1; //дистанция одна клетка
                Cell[] posNeighbours = new[] // все возможные соседи
                {
                new Cell(x2, y2 - distance), // Вверх
                new Cell(x2, y2 + distance), // Вниз
                new Cell(x2 + distance, y2), // Вправо
                new Cell(x2 - distance, y2) // Влево
            };
                for (int i = 0; i < 4; i++) // проверка на наличие клеток (соседей), которые находятся поблизости, во всех направлениях
                {
                    Cell curNeighbour = posNeighbours[i]; //текущей клетки присваивается значение возможного соседа
                    if (curNeighbour.x > 0 && curNeighbour.x < Width1 && curNeighbour.y > 0 && curNeighbour.y < Height1)
                    {
                        if (cells[curNeighbour.x, curNeighbour.y].Cell_0 && !cells[curNeighbour.x, curNeighbour.y].Visited) // Если сосед находится в пределах клетки и не является посещенной клеткой
                        {
                            neighbours.Add(curNeighbour); // добавляем соседа в список 
                        }
                    }
                }
            }
        }


        //кнопка, предназначенная для создания лабиринта
        private void button1_Click_1(object sender, EventArgs e)
        {
                // ввод ширины и высоты лабиринта в textbox и textbox2
                int w = int.Parse(textBox1.Text);
                int h = int.Parse(textBox2.Text);

            int w1 = 0;
            int h1 = 0;
            if(w % 2 != 0 && w != 0)
            {
                w1 = 1;
            }
            if (w % 2 != 0 && w != 0)
            {
                h1 = 1;
            }
            Cellw = pictureBox1.ClientSize.Width / (w + 2); // ширина одной клетки
                Cellh = pictureBox1.ClientSize.Height / (h + 2); // высота одной клетки

                const int mincell = 10; //минимальный размер ячейки
                                        //если ширина клетки меньше минимальной, 
                                        //то ширине клетки присваивается мин. значение, а высоте - значение ширины
                if (Cellw < mincell)
                {
                    Cellw = mincell;
                    Cellh = Cellw;
                }
                //аналогично и для высоты клетки
                else
                if (mincell > Cellh)
                {
                    Cellh = mincell;
                    Cellw = Cellh;
                }
                //если высота и ширина имеют разные размеры, то приравниваем их
                else
                if (Cellh < Cellw)
                    Cellw = Cellh;
                else Cellh = Cellw;

                Labirint labirint = new Labirint(w, h);

            labirint.finish.x = labirint.finish.x + w1;
            labirint.finish.y = labirint.finish.y + h1;
            labirint.LabirintCreate();
            Drawlab();
            Lab = labirint;



                void Drawlab() // построение лабиринта
                {

                    bit1.Dispose(); //bit1 - изображение решения лабиринта
                    Bitmap bit = new Bitmap(Cellw * (labirint.finish.x + 2), Cellh * (labirint.finish.y + 2),System.Drawing.Imaging.PixelFormat.Format16bppRgb555); //изображение самого лабиринта
                    Brush whiteBr = new SolidBrush(Color.White); //белым цветом будет заливка свободных клеток
                    Brush blackBr = new SolidBrush(Color.Black); //черным цветом - заливка стен лабиринта

                    using (Graphics gr = Graphics.FromImage(bit))
                    {

                        gr.SmoothingMode = SmoothingMode.AntiAlias;

                        //строим лабиринт
                        for (var i = 0; i < labirint.cells.GetUpperBound(0); i++)
                            for (var j = 0; j < labirint.cells.GetUpperBound(1); j++)
                            {
                                Point point = new Point(i * Cellw, j * Cellw);
                                Size size = new Size(Cellw, Cellw);
                                Rectangle rec = new Rectangle(point, size);
                                if (labirint.cells[i, j].Cell_0)
                                {
                                    gr.FillRectangle(whiteBr, rec);
                                }
                                else
                                {

                                    gr.FillRectangle(blackBr, rec);
                                }
                            }
                        //старт
                        gr.FillRectangle(new SolidBrush(Color.Green),
                            new Rectangle(new Point(labirint.start.x * Cellw, labirint.start.y * Cellw),
                            new Size(Cellw, Cellw)));
                        //финиш
                        gr.FillRectangle(new SolidBrush(Color.Red),
                            new Rectangle(new Point(labirint.finish.x * Cellw, labirint.finish.y * Cellw),
                            new Size(Cellw, Cellw)));
                    }
                    //вывод изображенея лабиринта на экран (в picturebox1)
                    pictureBox1.Image = bit;
                    bit1 = bit;
                }
        }


        //кнопка, отвечающая за поиск пути к финишу, т.е. за решение лабиринта
        private void button2_Click_1(object sender, EventArgs e)

        {
            Lab.LabirintSolve();
            Solution();

            //решение лабиринта, поиск маршрута
            void Solution()
            {

                Brush yellowgreenBr = new SolidBrush(Color.GreenYellow); //салатовым цветом будет обозначаться путь,
                //который ведет к финишу
                Brush pinkBr = new SolidBrush(Color.Pink); //розовым цветом будет обозначаться путь,
                //приводящий в тупик
                using (Graphics gr = Graphics.FromImage(bit1))
                {
                    gr.SmoothingMode = SmoothingMode.AntiAlias; // сгладим кривые и линии для более приятной картинки

                    foreach (Cell c in Lab.visited) // клетки, которые ведут в тупик и по которым нам уже не надо идти
                    {
                        Point point = new Point(c.x * Cellw, c.y * Cellw);
                        Size size = new Size(Cellw, Cellw);
                        Rectangle rec = new Rectangle(point, size);
                        gr.FillRectangle(pinkBr, rec);
                    }

                    foreach (Cell c in Lab.solve) // клетки, которые приведут к финишу
                    {
                        Point point = new Point(c.x * Cellw, c.y * Cellw);
                        Size size = new Size(Cellw, Cellw);
                        Rectangle rect = new Rectangle(point, size);
                        gr.FillRectangle(yellowgreenBr, rect);
                    }
                    //начальная точка, т.е. старт
                    gr.FillRectangle(new SolidBrush(Color.Green),
                       new Rectangle(new Point(Lab.start.x * Cellw, Lab.start.y * Cellw),
                       new Size(Cellw, Cellw)));
                    //конечная точка, т.е. финиш
                    gr.FillRectangle(new SolidBrush(Color.Red),
                        new Rectangle(new Point(Lab.finish.x * Cellw, Lab.finish.y * Cellw),
                        new Size(Cellw, Cellw)));
                }

                pictureBox1.Image = bit1; //вывод решенного лабиринта

            }
        }

    }
}
