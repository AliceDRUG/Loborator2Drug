using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using org.mariuszgromada.math.mxparser;

namespace Loborator2Drug

{
    public partial class Form1 : Form
    {
        List<List<PointPairList>> minList = new List<List<PointPairList>>();
        PointPairList list = new PointPairList();
        PointPairList list2 = new PointPairList();
        double x1, x2;
        int index;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void zedGraphControl2_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async void рToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            if (pane.CurveList.Count > 0)
            {
                MessageBox.Show("Очистите граф перед постройкой нового!.", "Ошибка.");
            }
            else { 
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            list2.Clear();
            list.Clear();
            minList.Clear();
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();
            x1 = 0;
            x2 = 0;


            if (textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Введите обе границы");
            }
            if (textBox2.Text.Length >= 4 || textBox3.Text.Length >= 4)
            {
                DialogResult err = MessageBox.Show("Слишком большие границы!", "Внимание!");
            }
                if (textBox4.Text.Length > 9)//проверка на заполненность данных
                {
                    DialogResult err = MessageBox.Show("Уменьшите Е", "Ошибка!");
                }
                else if (double.Parse(textBox2.Text) >= double.Parse(textBox3.Text))
                {
                    MessageBox.Show("Граница a должна быть меньше границы b");
                }
                else
                {
                    await buildasync();
                    string expression = textBox1.Text;

                    double localMinX = await method(double.Parse(textBox2.Text), double.Parse(textBox3.Text), double.Parse(textBox4.Text));
                    list2.Add(localMinX, func(localMinX));

                    index = minList.Count;
                    addpoint();
                    zedGraphControl1.AxisChange();
                    zedGraphControl1.Invalidate();
                }
            }
        }
        public void addpoint()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            LineItem minimum = pane.AddCurve("Минимум", list2, Color.Red, SymbolType.Diamond);
        }

        private void buildgraph()
        {
            double xmin, xmax;
            GraphPane pane = zedGraphControl1.GraphPane;
            list.Clear();

            xmin = Convert.ToDouble(textBox2.Text);
            xmax = Convert.ToDouble(textBox3.Text);

            for (double x = xmin; x <= xmax; x += 0.1)
            {
                list.Add(x, func(x));
            }
            pane.AddCurve(textBox1.Text, list, Color.Blue, SymbolType.None);
            Action action = () => addpoint();
            Invoke(action);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private double goldenRatio(double a, double b, double e)
        {
            double d = (-1 + Math.Sqrt(5)) / 2;
            while (Math.Abs(b - a) > e)
            {
                PointPairList xa = new PointPairList();
                PointPairList xb = new PointPairList();
                List<PointPairList> x1x2 = new List<PointPairList>();

                xa.Add(a, func(a));
                xb.Add(b, func(b));
                x1x2.Add(xa);
                x1x2.Add(xb);

                minList.Add(x1x2);

                x1 = b - (b - a) * d;
                x2 = a + (b - a) * d;

                if (func(x1) >= func(x2))
                {
                    a = x1;
                }
                else
                {
                    b = x2;
                }

            }

            return (a + b) / 2;
        }

        async Task<Double> method(double a, double b, double e)//асинхроним расчеты метода
        {
            var result = await Task.Run(() => goldenRatio(a, b, e));
            return result;
        }
        private async Task buildasync()
        {
            await Task.Run(() => buildgraph());
        }
        private double func(double x)
        {
            Argument xmain = new Argument("x");
            Expression y = new Expression(textBox1.Text.Replace(',', '.'), xmain);

            xmain.setArgumentValue(x);
            return y.calculate();
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            list.Clear();
            list2.Clear();
            minList.Clear();
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void шагВпередToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (list.Count == 0)
            {
                buildgraph();
            }
            else
            {
                GraphPane pane = zedGraphControl1.GraphPane;

                if (pane.CurveList.Count == 2)
                {
                    pane.CurveList.RemoveAt(1);
                }
                else if (pane.CurveList.Count == 3)
                {
                    pane.CurveList.RemoveAt(1);
                    pane.CurveList.RemoveAt(1);
                }
                if (index > 0)
                {
                    index--;



                    LineItem aMin = pane.AddCurve("x1", minList[index][0], Color.Red, SymbolType.Diamond);
                    LineItem bMin = pane.AddCurve("x2", minList[index][1], Color.Red, SymbolType.Diamond);


                }
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                //0,00000000000000000000001
            }
        }

        private void шагНазадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (list.Count == 0)
            {
                buildgraph();
            }
            else
            {
                GraphPane pane = zedGraphControl1.GraphPane;

                if (pane.CurveList.Count == 2)
                {
                    pane.CurveList.RemoveAt(1);
                }
                else if (pane.CurveList.Count == 3)
                {
                    pane.CurveList.RemoveAt(1);
                    pane.CurveList.RemoveAt(1);
                }

                if (index < minList.Count - 1)
                {
                    index++;
                    LineItem aMin = pane.AddCurve("x1", minList[index][0], Color.Red, SymbolType.Diamond);
                    LineItem bMin = pane.AddCurve("x2", minList[index][1], Color.Red, SymbolType.Diamond);
                }
                else if (index == minList.Count - 1)
                {
                    LineItem minimum = pane.AddCurve("Минимум", list2, Color.Red, SymbolType.Diamond);
                }
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char comma = e.KeyChar;

            if (comma == 46)
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 && number != 44 && number != 45)
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 && number != 44 && number != 45)
            {
                e.Handled = true;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (textBox4.Text.Length == 2)
            {
                if (number == 8 && number == 45)
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (!Char.IsDigit(number) && number != 8)
                {
                    e.Handled = true;
                }
            }
        }
    }
}
