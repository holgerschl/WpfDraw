﻿using WpfDraw.Model;
using WpfDraw.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace WpfDraw.ViewModel
{
    class DrawViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<FrameworkElement> _sprites = new ObservableCollection<FrameworkElement>();
        public INotifyCollectionChanged Sprites { get { return _sprites; } }

        public string Name { get; private set; }
        public FrameworkElement OriginalSource { get; private set; }
        private int cursorType;
        public int CursorType
        {
            get
            {
                return cursorType;
            }
            set
            {
                cursorType = value;

                while (cursorType < 0)
                    cursorType += 8;
                while (cursorType >= 8)
                    cursorType -= 8;

            }
        }

        private int modusType;
        public int ModusType
        {
            get
            {
                return modusType;
            }
            set
            {
                modusType = value;

                while (modusType < 0)
                    modusType += 8;
                while (modusType >= 8)
                    modusType -= 8;
                switch (modusType)
                {
                    case 0:
                        modusType = (int)Modus.TopLeft;
                        break;
                    case 1:
                        modusType = (int)Modus.TopCenter;
                        break;
                    case 2:
                        modusType = (int)Modus.TopRight;
                        break;
                    case 3:
                        modusType = (int)Modus.CenterRight;
                        break;
                    case 4:
                        modusType = (int)Modus.BottomRight;
                        break;
                    case 5:
                        modusType = (int)Modus.BottomCenter;
                        break;
                    case 6:
                        modusType = (int)Modus.BottomLeft;
                        break;
                    case 7:
                        modusType = (int)Modus.CenterLeft;
                        break;
                }
            }
        }
        public Modus Modus { get; set; }

        private readonly Dictionary<Model.Shape, FrameworkElement> _shapes = new Dictionary<Model.Shape, FrameworkElement>();

        private KeyValuePair<Model.Adorner, FrameworkElement> adorner = new KeyValuePair<Model.Adorner, FrameworkElement>();

        private readonly DrawModel _model = new DrawModel();
        private bool resize;

        public DrawViewModel()
        {
            _model.ShapeChanged += ModelShapeChanged;
            _model.ModusChanged += ModelModusChanged;
        }

        private void ModelModusChanged(object sender, ModusChangedEventArgs e)
        {
            Modus = e.Modus;
        }

        private void ModelShapeChanged(object sender, ShapeChangedEventArgs e)
        {
            Model.Shape shape = e.Shape;
            if (e.Removed)
            {
                if (_shapes.ContainsKey(shape))
                {
                    _sprites.Remove(_shapes[shape]);
                    _shapes.Remove(shape);
                    if (shape is Model.Adorner)
                        adorner = new KeyValuePair<Model.Adorner, FrameworkElement>();
                }
                else
                {
                    throw new ElementNotFoundException();
                }
            }
            else
            {

                if (!_shapes.ContainsKey(shape))
                {
                    FrameworkElement shapeControl = null;
                    switch (e.Type)
                    {
                        case ShapeType.Rectangle:
                            shapeControl = DrawHelper.RectangleControlFactory(shape as Model.Rectangle);
                            break;
                        case ShapeType.Line:
                            shapeControl = DrawHelper.LineControlFactory(shape as Model.Line);
                            break;
                        case ShapeType.Ellipse:
                            shapeControl = DrawHelper.EllipseControlFactory(shape as Model.Ellipse);
                            break;
                        case ShapeType.Adorner:
                            shapeControl = DrawHelper.AdornerControlFactory(shape as Model.Adorner);
                            adorner = new KeyValuePair<Model.Adorner, FrameworkElement>((Model.Adorner)shape, shapeControl);
                            break;
                    }
                    _shapes[shape] = shapeControl;
                    _sprites.Add(shapeControl);
                }
                else
                {
                    FrameworkElement shapeControl = _shapes[shape];

                    if (shape is Model.Line)
                    {
                        DrawHelper.SetCanvasLocation(shapeControl, 0, 0, 100);
                        ((System.Windows.Shapes.Line)shapeControl).X1 = shape.Start.X;
                        ((System.Windows.Shapes.Line)shapeControl).X2 = ((Model.Line)shape).End2.X;
                        ((System.Windows.Shapes.Line)shapeControl).Y1 = shape.Start.Y;
                        ((System.Windows.Shapes.Line)shapeControl).Y2 = ((Model.Line)shape).End2.Y;
                    }
                    else
                    {
                        DrawHelper.SetCanvasLocation(shapeControl, shape.Start.X, shape.Start.Y, 100);
                        shapeControl.Width = shape.Area.Width;
                        shapeControl.Height = shape.Area.Height;
                    }
                    RotateShape(shape, shapeControl);
                }
            }
        }

        private void RotateShape(Model.Shape shape, FrameworkElement shapeControl)
        {
            if (!(shape is Model.Adorner))
            {
                RotateTransform shapeControlRotateTransform = new RotateTransform();
                shapeControlRotateTransform.Angle = shape.Rotation;
                TransformGroup shapeControlTransformGroup = new TransformGroup();
                shapeControlTransformGroup.Children.Add(shapeControlRotateTransform);
                if (shape is Model.Line)
                {
                    shapeControlRotateTransform.CenterX = shape.RotationCenter.X;
                    shapeControlRotateTransform.CenterY = shape.RotationCenter.Y;
                }
                else
                {
                    shapeControlRotateTransform.CenterX = shape.RotationCenter.X - shape.Start.X;
                    shapeControlRotateTransform.CenterY = shape.RotationCenter.Y - shape.Start.Y;
                }
                shapeControl.RenderTransform = shapeControlTransformGroup;
            }

            FrameworkElement adornerControl = null;
            Model.Adorner adorner = null;
            var adornerList = (from s in _shapes where s.Key is Model.Adorner select s).ToList();
            if (adornerList.Count() > 0)
            {
                adornerControl = adornerList[0].Value;
                adorner = (Model.Adorner)adornerList[0].Key;
            }
            if (adornerControl != null)
            {
                RotateTransform adornerRotateTransform = new RotateTransform();
                adornerRotateTransform.Angle = shape.Rotation;
                TransformGroup adornerTransformGroup = new TransformGroup();
                adornerTransformGroup.Children.Add(adornerRotateTransform);
                adornerControl.RenderTransform = adornerTransformGroup;
                adornerRotateTransform.CenterX = adorner.RotationCenter.X - adorner.Start.X - DrawHelper.adornerThickness;
                adornerRotateTransform.CenterY = adorner.RotationCenter.Y - adorner.Start.Y - DrawHelper.turnHandleDistance;
            }
        }

        internal void RemoveAdorner()
        {
            _model.RemoveAdorner();
        }

        internal void SelectShape(MouseButtonEventArgs e, Point position)
        {
            Model.Shape shape = null;
            try
            {
                shape = _shapes.First(x => (x.Value == (FrameworkElement)e.OriginalSource)).Key;
                Mouse.OverrideCursor = Cursors.SizeAll;
            }
            catch (InvalidOperationException ex)
            {
            }
            switch (Modus)
            {
                case Modus.Move:
                    _model.SelectShape(shape, position);
                    break;
                case Modus.BottomCenter:
                case Modus.BottomLeft:
                case Modus.BottomRight:
                case Modus.CenterLeft:
                case Modus.CenterRight:
                case Modus.TopCenter:
                case Modus.TopLeft:
                case Modus.TopRight:
                    _model.SetProjectedWidthAndHeight(position);
                    break;
                case Modus.TurnHandle:
                    _model.TurnShape(position);
                    break;
            }
        }

        internal void MoveShape(MouseEventArgs e, Point position)
        {
            Model.Shape shape = null;
            Name = ((FrameworkElement)e.OriginalSource).Name;
            OriginalSource = ((FrameworkElement)e.OriginalSource);
            if (Modus == Modus.TurnHandle && e.LeftButton == MouseButtonState.Pressed)
            {
                _model.TurnShape(position);

            }
            else if ((Modus >= (Modus)0 && Modus <= (Modus)7) && e.LeftButton == MouseButtonState.Pressed)
            {
                _model.ResizeShape(position, Modus);
            }
            else
            {
                try
                {
                    shape = _shapes.First(x => (x.Value == OriginalSource)).Key;
                    Mouse.OverrideCursor = Cursors.SizeAll;
                }
                catch (InvalidOperationException ex)
                {
                    resize = true;
                    switch (Name)
                    {

                        case "TopLeft":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 45) / 90));
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation - 22.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "TopCenter":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 45) / 90)) + 1;
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation + 22.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "TopRight":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 135) / 90));
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation + 67.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "CenterRight":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 135) / 90)) + 1;
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation + 112.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "BottomRight":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 225) / 90));
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation + 157.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "BottomCenter":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 225) / 90)) + 1;
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation + 202.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "BottomLeft":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 315) / 90));
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation + 247.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "CenterLeft":
                            ModusType = (int)(2 * Math.Floor((adorner.Key.Rotation + 315) / 90)) + 1;
                            CursorType = (int)(Math.Floor((adorner.Key.Rotation + 292.5) / 45));
                            Modus = (Modus)ModusType;
                            break;
                        case "TurnHandle":
                            Modus = Modus.TurnHandle;
                            Mouse.OverrideCursor = Cursors.Cross;
                            resize = false;
                            break;
                        default:
                            Modus = Modus.Move;
                            Mouse.OverrideCursor = Cursors.SizeAll;
                            resize = false;
                            break;
                    }
                    if (resize)
                    {
                        switch (CursorType)
                        {
                            case 0:
                            case 4:
                                Mouse.OverrideCursor = Cursors.SizeNWSE;
                                break;
                            case 1:
                            case 5:
                                Mouse.OverrideCursor = Cursors.SizeNS;
                                break;
                            case 2:
                            case 6:
                                Mouse.OverrideCursor = Cursors.SizeNESW;
                                break;
                            case 3:
                            case 7:
                                Mouse.OverrideCursor = Cursors.SizeWE;
                                break;
                        }

                    }
                    OnPropertyChanged("ModusType");
                }
            }
            _model.MoveShape(position);
        }

        internal void NewShape(Point position, ShapeType shapeType)
        {
            _model.NewShape(position, shapeType);
        }

        internal void DrawShape(Point position)
        {
            _model.DrawShape(position);
        }

        internal void DropShape(Point position)
        {
            _model.DropShape(position);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        internal void SetShape(Point position)
        {
            _model.SetShape(position);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
