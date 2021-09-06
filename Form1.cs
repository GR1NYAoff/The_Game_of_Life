using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace idz_Tsyapa
{
    public partial class Form1 : Form
    {
        //Для подсчета поколений создадим переменную
        private int numberGeneration;
        //Створюємо графіку
        private Graphics gr;
        //Змінна для подальшого використання
        private int resolution;
        //Створемо двумірний булевий масив
        private bool [,] field;
        //Для розрахунку строк і колонок створемо дві змінні
        private int rows;
        private int cols;
        public Form1()
        {
            InitializeComponent();
        }
        private void StartGame()
        {
            numberGeneration = 0;
            Text = $"Generation{numberGeneration}";

            if (timer1.Enabled)
            {
                return;
            } 
            //Блокировка элементов управления
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            resolution = Convert.ToInt32(nudResolution.Value);
            //Розрахунок строк і колонок(реалізація масштабу)
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;
            field = new bool[cols, rows];
            //Реалізація плотності населення поля
            Random rnd = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] =rnd.Next(50-Convert.ToInt32(nudDensity.Value))==0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
            /*//Малювання прямокутника (квадрат)
            gr.FillRectangle(Brushes.Crimson, 0, 0, resolution, resolution);*/
        }
        //Реализация отрисовки
        private void NextGeneration()
        {
            var newField= new bool[cols, rows];
            gr.Clear(Color.Black);

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    //Вызываем метод
                    var neighboursCount = CountNeighbours(x, y);
                    //Информация о живой клетке по текущим координтам
                    var Life = field[x, y];
                    if (!Life && neighboursCount==3)
                    {
                        newField[x, y] = true;
                    }
                    else if(Life && (neighboursCount < 2||neighboursCount>3))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    
                            
                    if (Life)
                    {
                        //Отрисовка клетки
                        gr.FillEllipse(Brushes.Azure,x*resolution, y*resolution, resolution, resolution);
                    }
                }
            }
            //Следующее поколениестановится текущим
            field = newField;
            pictureBox1.Refresh();
            Text = $"Generation{++numberGeneration}";
        }
        //Реализация подсчета соседей "клетки"
        private int CountNeighbours(int x, int y)
        {
            int count = 0;
            //Проверка соседей
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    //Выполним операцию остатка от деления для исключения крайних соседей на поле
                    //Позволит заглянуть за край прямоугольного поля
                    var col = (x + i+cols) % cols;
                    var row = (y + j+rows) % rows;
                    //Исключим проверку в самой "клетке", для этого создадим массив
                    var isSelfChecking = col == x && row == y;
                    var Life= field[col, row];
                    if (Life && !isSelfChecking)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
            {
                return;
            }
            timer1.Stop();
            //Разблокировка элементов управления
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }
        //Реализация интерактивного удаления "клеток" на поле
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(!timer1.Enabled)
            {
                return;
            }
            if(e.Button==MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var zonapassed = zona(x, y);
                if(zonapassed)
                {
                    field[x, y] = true;
                }                
            }
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var zonapassed = zona(x, y);
                if (zonapassed)
                {
                    field[x, y] = false;
                }
            }
        }
        //Проверка на выход за границу нашего поя с зажатой кнопкой мыши
        private bool zona(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = $"Generation {numberGeneration}";
        }
    }
}
