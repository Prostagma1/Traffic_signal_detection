using System;
using System.Drawing;
using System.Windows.Forms;

namespace LABA2
{
    public partial class Form2 : Form
    {
        public Form2(int[,] gridHistogram, int gridWidth, int gridHeight)
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisY.Maximum = gridWidth * gridHeight;
            for (int i = 0; i < 10; i++)
            {
                chart1.Series[i].Name = $"Ячейка {i}";
                for (int j = 0; j < 10; j++)
                {
                    chart1.Series[j].Points.AddXY(i, gridHistogram[i, j]);

                }
            }
        }

    }
}
