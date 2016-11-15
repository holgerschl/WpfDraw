using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfDraw.Model;
using WpfDraw.ViewModel;

namespace WpfDraw.View
{
    /// <summary>
    /// Interaction logic for DrawView.xaml
    /// </summary>
    public partial class DrawView : Window
    {
        private ShapeType shapeType;
        private DrawViewModel viewModel;
        public DrawView()
        {
            InitializeComponent();
            viewModel = new DrawViewModel();
            DataContext = viewModel;
        }

        private void itemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (shapeType == ShapeType.Selector)
            {
                viewModel.SelectShape(e, e.GetPosition(itemsControl));
            }
            else
                viewModel.NewShape(e.GetPosition(itemsControl), shapeType);
        }

        private void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (shapeType == ShapeType.Selector)
            {
                viewModel.MoveShape(e, e.GetPosition(itemsControl));
            }
            else

                viewModel.DrawShape(e.GetPosition(itemsControl));

        }

        private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (shapeType == ShapeType.Selector)
            {
                viewModel.DropShape(e.GetPosition(itemsControl));
            }
            else
                viewModel.SetShape(e.GetPosition(itemsControl));

        }

        private void line_Click(object sender, RoutedEventArgs e)
        {
            shapeType = ShapeType.Line;
            line.BorderBrush = new SolidColorBrush(Colors.Yellow);
            rectangle.BorderBrush = new SolidColorBrush(Colors.LightGray);
            circle.BorderBrush = new SolidColorBrush(Colors.LightGray);
            select.BorderBrush = new SolidColorBrush(Colors.LightGray);
            viewModel.RemoveAdorner();

        }

        private void rectangle_Click(object sender, RoutedEventArgs e)
        {
            shapeType = ShapeType.Rectangle;
            line.BorderBrush = new SolidColorBrush(Colors.LightGray);
            rectangle.BorderBrush = new SolidColorBrush(Colors.Yellow);
            circle.BorderBrush = new SolidColorBrush(Colors.LightGray);
            select.BorderBrush = new SolidColorBrush(Colors.LightGray);
            viewModel.RemoveAdorner();

        }

        private void circle_Click(object sender, RoutedEventArgs e)
        {
            shapeType = ShapeType.Ellipse;
            line.BorderBrush = new SolidColorBrush(Colors.LightGray);
            rectangle.BorderBrush = new SolidColorBrush(Colors.LightGray);
            circle.BorderBrush = new SolidColorBrush(Colors.Yellow);
            select.BorderBrush = new SolidColorBrush(Colors.LightGray);
            viewModel.RemoveAdorner();

        }

        private void select_Click(object sender, RoutedEventArgs e)
        {
            shapeType = ShapeType.Selector;
            line.BorderBrush = new SolidColorBrush(Colors.LightGray);
            rectangle.BorderBrush = new SolidColorBrush(Colors.LightGray);
            circle.BorderBrush = new SolidColorBrush(Colors.LightGray);
            select.BorderBrush = new SolidColorBrush(Colors.Yellow);

        }
    }
}
