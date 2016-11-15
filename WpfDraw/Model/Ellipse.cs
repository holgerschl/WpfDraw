using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;

namespace WpfDraw.Model
{
    class Ellipse:Shape
    {
        public Ellipse(Point start, Point end, double rotation, Point rotationCenter) : base(start, end, rotation, rotationCenter)
        {
        }
    }
}
