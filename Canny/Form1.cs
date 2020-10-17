using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canny
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SmoothTypeListBox.Items.Add(SmoothMatrixType.Simple);
        }

        private Bitmap originalPic;
        private Bitmap resultPic;

        public void UpdateForm()
        {
            originalPicBox.Refresh();
            resultPicBox.Refresh();
        }

        public Color GradientToColor(GradientVector gv)
        {
            byte lambda = Convert.ToByte(gv.Length);
            return Color.FromArgb(255, lambda, lambda, lambda);
        }

        public Bitmap Process(SmoothMatrixType type, int size)
        {
            Console.WriteLine("Process started");
            var resGrad = originalPic
                .GetBWPicture()
                .SmoothPicture(type, size)
                .FindGradients();

            Bitmap result = new Bitmap(resGrad.GetLength(0), resGrad.GetLength(1));
            for (int i = 0; i < resGrad.GetLength(0); i++)
            {
                for (int j = 0; j < resGrad.GetLength(1); j++)
                {
                    result.SetPixel(i, j, GradientToColor(resGrad[i,j]));
                }
            }
            Console.WriteLine("Process ended");
            return result;
        }

        public void OpenFile()
        {
            if (openPictureDialog.ShowDialog() == DialogResult.OK)
            {
                originalPic = new Bitmap(openPictureDialog.FileName);
                originalPicBox.Image = originalPic;
            }
        }

        private void ChoosePicBtn_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void ExecuteBtn_Click(object sender, EventArgs e)
        {
            resultPic = Process((SmoothMatrixType)SmoothTypeListBox.SelectedItem, (int)MatrixSizeNUD.Value);
            resultPicBox.Image = resultPic;
            UpdateForm();
        }
    }
}
