﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;

namespace WpfDraw.Model
{
    class Adorner : Shape
    {
        public Adorner(Point start, Point end, int rotation, Point rotationCenter) : base(start, end, rotation, rotationCenter)
        {
        }
    }
}
