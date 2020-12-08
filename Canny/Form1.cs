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
using BitmapLibrary;
using NeuralNet;

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
            StateLbl.Refresh();
        }

        public Color GradientToColor(GradientVector gv)
        {
            byte lambda = Convert.ToByte(gv.Length > 255 ? 255 : gv.Length);
            return Color.FromArgb(255, lambda, lambda, lambda);
        }

        public Bitmap CannyProcessing(SmoothMatrixType type, int size)
        {
            Invoke(new Action(() => StateLbl.Text = "Начато преобразование"));
            var bitmap = new Bitmap(originalPic, 64, 64).GetBWPicture();
            Invoke(new Action(() => 
            {
                StateLbl.Text = "Получено чернобелое изображение";
                resultPicBox.Image = bitmap;
                UpdateForm();
            }));
            
            var smoothedBWPicture = bitmap.SmoothBWPicture(type, size);
            Invoke(new Action(() => 
            {
                StateLbl.Text = "Получено размытое изображение";
                resultPic = bitmap;
                UpdateForm();
            }));

            var gradients = smoothedBWPicture.FindGradients();
            Invoke(new Action(() => 
            {
                StateLbl.Text = "Найдены градиенты";
                StateLbl.Refresh();
            }));

            var gradientsWithSuppressedMaximums = gradients.SuppressMaximums();
            Invoke(new Action(() => 
            {
                StateLbl.Text = "Удалены немаксимумы";
                StateLbl.Refresh();
            }));

            var cuttedGradients = gradientsWithSuppressedMaximums.BlackEdge(size / 2 + 1);
            Invoke(new Action(() => 
            {
                StateLbl.Text = "Закрашены края";
                StateLbl.Refresh();
            }));
                
            var filteredGradients = cuttedGradients;//.Filtering();
            //Invoke(new Action(() => 
            //{
            //    StateLbl.Text = "Произведена фильтрация. Готово!";
            //    StateLbl.Refresh();
            //}));

            //Invoke(new Action(() => 
            //{
            //    StateLbl.Text = "Нейросеть вычисляет есть ли человек на фотографии!";
            //    StateLbl.Refresh();
            //    BNPNet net = new BNPNet(@"C:\Users\vladb\Desktop\somaset\network.json");

            //    var output = net.GetResult(filteredGradients.LengthsToArray().ToVector());
            //    StateLbl.Text = "Человек " + ((1 - output[0]) < 0.2 ? "присутствует" : "отсутствует") + " на фотографии";
            //    StateLbl.Refresh();
            //}));

            return filteredGradients.ToBitmap();
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
                originalPic = new Bitmap(Image.FromFile(openPictureDialog.FileName));
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
            var type = (SmoothMatrixType)SmoothTypeListBox.SelectedItem;
            var size = (int)MatrixSizeNUD.Value;
            Task.Run(new Action(() => 
            {
                resultPic = CannyProcessing(type, size);
                resultPic.Save(path + "res.bmp");
                resultPicBox.Image = resultPic;
                Invoke(new Action( () => UpdateForm()));
            }));
        }
    }
}
