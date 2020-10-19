using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Canny
{
    public struct GradientVector
    {
        public double Length;
        public double Angle;

        public Color GradientToColor()
        {
            byte lambda = Convert.ToByte(Length > 255 ? 255 : Length);
            return Color.FromArgb(255, lambda, lambda, lambda);
        }
    }
}
