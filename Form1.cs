using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnionPolygons
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        private PointF[] UNION = new PointF[0];
        Pen pen_union = new Pen(Color.Red, 2);
        private Graphics g;
        private bool isDraw;    //переменные для событий
        private PointF startPoint, endPoint = PointF.Empty;     //отрисовка линий
        private PointF mainPoint = new PointF(0, 0);
        private PointF minPolyPoint, maxPolyPoint;
        private PointF[] polygon = new PointF[0];
        private PointF[] general = new PointF[0];
        bool flag = false;
        PointF intersection = new PointF(-13, -13);
        public Form1()
        {
            /*InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = (Bitmap)pictureBox1.Image;
            Clear();
            g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;
            g.TranslateTransform(1, -1);*/

            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = (Bitmap)pictureBox1.Image;
            g = Graphics.FromImage(bmp);
            isDraw = true;
        }

        private void Clear()
        {
            g.Clear(Color.White);
            startPoint = endPoint = Point.Empty;
            Array.Clear(polygon, 0, polygon.Length);
            Array.Resize(ref polygon, 0);
            intersection = new PointF(-1, -1);
            general = new PointF[0];
            polygon = new PointF[0];
            UNION = new PointF[0];
            //twoPolygons = false;
            flag = false;

            pictureBox1.Invalidate();
        }

        //List<Point> general = new List<Point>();
        //List<Point> newPolygon = new List<Point>();
        //bool twoPolygons = false;


        #region Рисование полигонов
        private void pictureBox1_MouseDown1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDraw)
                {
                    if (polygon.Length == 0)
                    {
                        minPolyPoint = maxPolyPoint = e.Location;
                        startPoint = e.Location;
                        Array.Resize(ref polygon, 1);
                        polygon[0] = startPoint;
                    }
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDraw)
                {
                    Array.Resize(ref polygon, polygon.Length + 1);

                    if (endPoint.X < minPolyPoint.X)
                    {
                        minPolyPoint.X = endPoint.X;
                    }
                    if (endPoint.Y < minPolyPoint.Y)
                    {
                        minPolyPoint.Y = endPoint.Y;
                    }
                    if (endPoint.X > maxPolyPoint.X)
                    {
                        maxPolyPoint.X = endPoint.X;
                    }
                    if (endPoint.Y > maxPolyPoint.Y)
                    {
                        maxPolyPoint.Y = endPoint.Y;
                    }

                    polygon[polygon.Length - 1] = endPoint;
                    startPoint = endPoint;
                }
            }
            else
            {
                mainPoint.X = e.Location.X;
                mainPoint.Y = e.Location.Y;
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDraw)
                {
                    endPoint = e.Location;
                    pictureBox1.Invalidate();
                    
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (general.Length > 1)
                e.Graphics.DrawPolygon(Pens.Black, general);
            if (polygon.Length > 1)
                e.Graphics.DrawPolygon(Pens.Black, polygon);
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        { 
            if (general.Count() == 0){
                general = polygon;
                polygon = new PointF[0];
                //flag = false;
            }
        }

        //Принадлежит ли точка отрезку
        private bool pointBelongsToLine(PointF p, PointF a, PointF b)
        {
            var x = Math.Round((p.Y - a.Y) / (b.Y - a.Y), 3);
            var y = Math.Round((p.X - a.X) / (b.X - a.X), 3);
            return  x == y;
        }


        //Функция берет список, стартовую точку в нем, ближайшее пересечение
        //Действие - добавляет точки в список 
        private void add_points(ref PointF[] src, PointF start, PointF intersection, bool direction) { 
            PointF temp = start;
            while (temp != intersection) {
                Array.Resize(ref UNION, UNION.Length + 1);
                UNION[UNION.Length - 1] = temp;
                var nxt = next(ref src, temp);            
                if (!pointBelongsToLine(intersection,temp,nxt))
                    temp = nxt;
                else temp = intersection;
            }
            Array.Resize(ref UNION, UNION.Length + 1);
            UNION[UNION.Length - 1] = intersection;
        }
        
        //Функция возвращает следущую точку массива, если конец -> возвращает начало
        private PointF next(ref PointF[] lst, PointF p) {
            int index = Array.IndexOf(lst, p);
            PointF result = new PointF(0,0);

            if (index == -1)
            {
                int n = lst.Length;
                int i = 0;
                do
                {
                    int next = (i + 1) % n;
                    if (pointBelongsToLine(p, lst[i], lst[next]))
                        return lst[next];

                    i = next;
                } while (i != 0);
            }
            else
            {
                if (index + 1 == lst.Count())
                    result = lst[0];
                else
                    result = lst[index + 1];
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var general_min = general.OrderBy(x => x.X).ThenBy(x => x.Y).First();
            var newPol_min = polygon.OrderBy(x => x.X).ThenBy(x => x.Y).First();
            bool flag_pol = true;
            var intersect = IntersectPoints(general,polygon);
            //Array.Resize(ref general, intersect.Count());
            //for (var i = general.Length - intersect.Count(); i < general.Length; i++ )
             //   general[i]

            PointF start_point = general.First();
            PointF[] work_lst;

            if (flag_pol)
                work_lst = general;
            else
                work_lst = polygon;
            PointF t = start_point;
            bool direction = true;
            foreach (var i in intersect){
                add_points(ref work_lst, t, i, direction);
                //PointF current_point;
                if (flag_pol){
                    PointF check_now = next(ref polygon, i);
                    work_lst = polygon;
                    flag_pol = false;
                    t = check_now;
                }
                else {
                    PointF check_now = next(ref general, i);
                    work_lst = general;
                    flag_pol = true;
                    t = check_now;
                }
            }

            if (flag_pol)
            {
                if (UNION.Count() == 0)
                {
                    Array.Resize(ref UNION, UNION.Length + 1);
                    UNION[UNION.Length - 1] = next(ref general, general.First());
                }
                add_points(ref general, UNION.Last(), general.First(), true);
            }
            else
            {
                if (UNION.Count() == 0)
                {
                    Array.Resize(ref UNION, UNION.Length + 1);
                    UNION[UNION.Length - 1] = polygon.First();
                }
                add_points(ref polygon, UNION.Last(), polygon.First(), true);
            }
            foreach (var i in UNION)
                textBox1.Text += "(" + i.X + "; " + i.Y + ")   ";

            g.DrawLines(pen_union, UNION.ToArray());
            pictureBox1.Image = bmp;

		}

        private List<PointF> IntersectPoints(PointF[] general, PointF[] polygon)
        {
            var res = new List<PointF>();
            int n = general.Length;
            int m = polygon.Length;
            int j = 0, i = 0;

            do
            {
                int nextI = (i + 1) % n;
                do
                {
                    int nextJ = (j + 1) % m;
                    PointF intersection = Intersection(general[i], general[nextI],polygon[j], polygon[nextJ]);
                    if (intersection.X != -1 && intersection.Y != -1)
                    {
                        Math.Round(intersection.X,0);
                        Math.Round(intersection.Y, 0);
                        res.Add(intersection);
                    }
                    j = nextJ;
                } while (j != 0);

                i = nextI;
            } while (i != 0);



            return res;
        }
        private PointF Intersection(PointF p0, PointF p1, PointF p2, PointF p3)
        {
            PointF i = new PointF(-1, -1);
            PointF s1 = new PointF();
            PointF s2 = new PointF();
            s1.X = p1.X - p0.X;
            s1.Y = p1.Y - p0.Y;
            s2.X = p3.X - p2.X;
            s2.Y = p3.Y - p2.Y;
            float s, t;
            s = (-s1.Y * (p0.X - p2.X) + s1.X * (p0.Y - p2.Y)) / (-s2.X * s1.Y + s1.X * s2.Y);
            t = (s2.X * (p0.Y - p2.Y) - s2.Y * (p0.X - p2.X)) / (-s2.X * s1.Y + s1.X * s2.Y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                i.X = p0.X + (t * s1.X);
                i.Y = p0.Y + (t * s1.Y);

            }
            if (i == p0)
                i = new PointF(-1, -1);
            return i;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();            
        }
    }
}














/*textBox1.Text = "";
           foreach (var i in general)
               textBox1.Text += "(" + i.X + "; " + i.Y + ")    ";

           textBox2.Text = "";
           foreach (var i in newPolygon)
               textBox2.Text += "(" + i.X + "; " + i.Y + ")    ";

           textBox3.Text = "";
           foreach (var i in intersect)
               textBox3.Text += "(" + i.X + "; " + i.Y + ")    ";*/