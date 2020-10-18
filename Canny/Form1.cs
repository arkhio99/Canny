using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
            //SmoothTypeListBox.Items.Add(SmoothMatrixType.Gauss);
            SmoothTypeListBox.SelectedIndex = 0;
            ExecuteBtn.Enabled = false;
        }

        private Bitmap originalPic;
        private string path;
        private Bitmap resultPic;

        public void UpdateForm()
        {
            originalPicBox.Refresh();
            resultPicBox.Refresh();
        }

        public Color GradientToColor(GradientVector gv)
        {
            byte lambda = Convert.ToByte(gv.Length > 255 ? 255 : gv.Length);
            return Color.FromArgb(255, lambda, lambda, lambda);
        }

        public Bitmap Process(SmoothMatrixType type, int size)
        {
            var resGrad = originalPic
               // .GetBWPicture()
               // .SmoothPicture(type, size)
                .FindGradients()
                .SuppressMaximums();

            Bitmap result = new Bitmap(resGrad.GetLength(1), resGrad.GetLength(0));
            for (int i = 0; i < resGrad.GetLength(0); i++)
            {
                for (int j = 0; j < resGrad.GetLength(1); j++)
                {
                    result.SetPixel(j, i, GradientToColor(resGrad[i, j]));
                }
            }

            return result;
        }

        public void OpenFile()
        {
            if (openPictureDialog.ShowDialog() == DialogResult.OK)
            {
                path = openPictureDialog.FileName;
                string[] pathSplitted = path.Split(@"\");
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < pathSplitted.Length - 1; i++)
                {
                    sb.Append(pathSplitted[i] + @"\");
                }
                path = sb.ToString();
                originalPic = new Bitmap(openPictureDialog.FileName);
                originalPicBox.Image = originalPic;
                ExecuteBtn.Enabled = true;
            }
        }

        private void ChoosePicBtn_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void ExecuteBtn_Click(object sender, EventArgs e)
        {
            resultPic = Process((SmoothMatrixType)SmoothTypeListBox.SelectedItem, (int)MatrixSizeNUD.Value);
            resultPic.Save(path + "res.bmp");
            resultPicBox.Image = resultPic;
            UpdateForm();
        }
    }
}
