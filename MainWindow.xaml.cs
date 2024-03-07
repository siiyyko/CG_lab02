using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace lab02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int gridSize = 30;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawAxes();
            transformCanvas();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawAxes();
            transformCanvas();

        }

        private void DrawAxes()
        {
            axisCanvas.Children.Clear();

            double width = axisCanvas.ActualWidth;
            double height = axisCanvas.ActualHeight;

            Line xAxis = new Line();
            Line yAxis = new Line();


            xAxis.Stroke = Brushes.Black;
            yAxis.Stroke = Brushes.Black;

            xAxis.StrokeThickness = 2;
            yAxis.StrokeThickness = 2;

            xAxis.X1 = 0;
            xAxis.Y1 = height / 2;
            xAxis.X2 = width;
            xAxis.Y2 = height / 2;

            yAxis.X1 = width / 2;
            yAxis.Y1 = 0;
            yAxis.X2 = width / 2;
            yAxis.Y2 = height;

            axisCanvas.Children.Add(xAxis);
            axisCanvas.Children.Add(yAxis);

            GenerateAxisNumbers();

            DrawGrid();
        }

        private void DrawGrid()
        {
            double width = axisCanvas.ActualWidth;
            double height = axisCanvas.ActualHeight;

            for (double i = -height / 2; i <= 0; i += gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = 0,
                    X2 = width,
                    Y1 = i + height,
                    Y2 = i + height
                };

                axisCanvas.Children.Add(line);
            }

            for (double i = height / 2; i >= 0; i -= gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = 0,
                    X2 = width,
                    Y1 = i,
                    Y2 = i
                };

                axisCanvas.Children.Add(line);
            }

            for (double i = -width / 2; i <= 0; i += gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = i + width,
                    X2 = i + width,
                    Y1 = 0,
                    Y2 = height
                };

                axisCanvas.Children.Add(line);
            }

            for (double i = width / 2; i >= 0; i -= gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = i,
                    X2 = i,
                    Y1 = 0,
                    Y2 = height
                };

                axisCanvas.Children.Add(line);
            }
        }

        private void GenerateAxisNumbers()
        {
            double width = axisCanvas.ActualWidth;
            double height = axisCanvas.ActualHeight;

            double quantOfX = width / gridSize;
            double quantOfY = height / gridSize;

            for (int i = (int)(-(quantOfX / 2)); i <= (quantOfX / 2); ++i)
            {
                TextBlock xText = new TextBlock()
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness((i + (quantOfX / 2)) * gridSize, height / 2, 0, 0)
                };
                axisCanvas.Children.Add(xText);
            }

            for (int i = (int)((quantOfY / 2)); i >= (-quantOfY / 2); --i)
            {
                TextBlock yText = new TextBlock()
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(width / 2, (-i + (quantOfY / 2)) * gridSize, 0, 0)
                };
                axisCanvas.Children.Add(yText);
            }
        }

        private void transformCanvas()
        {
            double centerX = mainCanvas.ActualWidth / 2;
            double centerY = mainCanvas.ActualHeight / 2;

            TranslateTransform transform = new TranslateTransform(centerX, centerY);
            mainCanvas.RenderTransform = transform;
        }

        private void displayPoints(Point[] points)
        {
            foreach (Point point in points)
            {
                Ellipse pointEllipse = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.Red
                };

                Canvas.SetLeft(pointEllipse, point.X - pointEllipse.Width / 2);
                Canvas.SetTop(pointEllipse, point.Y - pointEllipse.Height / 2);

                mainCanvas.Children.Add(pointEllipse);
            }
        }

        private Point calculateBezierPoint(Point[] controlPoints, double t)
        {
            int n = controlPoints.Length - 1;

            if(n == 0)
            {
                return controlPoints[0];
            }

            Point[] intermediatePoints = new Point[n];
            for (int i = 0; i < n; i++)
            {
                intermediatePoints[i] = new Point(
                    (1 - t) * controlPoints[i].X + t * controlPoints[i + 1].X,
                    (1 - t) * controlPoints[i].Y + t * controlPoints[i + 1].Y
                );
            }


            return calculateBezierPoint(intermediatePoints, t);
        }

        private Point[]? getPointsFromTextBox()
        {
            string[] pointsText = pointsTextBox.Text.Split(',');

            for(int i = 0; i < pointsText.Length; ++i)
            {
                pointsText[i] = pointsText[i].TrimStart();
            }

            List<Point> points = new List<Point>();
            
            foreach(string point in pointsText)
            {
                string[] coords = point.Split(' ');
                if(!(double.TryParse(coords[0], out double x) &&
                     double.TryParse(coords[1], out double y))  )
                {
                    MessageBox.Show("Please, enter correct values!");
                    return null;
                }
                Point temp = new Point()
                {
                    X = x*gridSize,
                    Y = -y*gridSize
                };

                points.Add(temp);
            }

            return points.ToArray();
        }

        private List<Point> bezierPoints;

        private void DrawBezierCurve(object sender, RoutedEventArgs e)
        {
            Point[] points = getPointsFromTextBox();
            if( points == null ) return;

            DrawDirectionalLines(points);

            bezierPoints = new List<Point>();

            for (double t = 0; t <= 1; t += 0.001)
            {
                bezierPoints.Add(calculateBezierPoint(points, t));
            }

            displayPoints(bezierPoints.ToArray());
        }

        private void DrawDirectionalLines(Point[] points)
        {
            for(int i = 0; i < points.Length - 1; ++i)
            {
                Line line = new Line()
                {
                    X1 = points[i].X,
                    Y1 = points[i].Y,
                    X2 = points[i + 1].X,
                    Y2 = points[i + 1].Y,

                    Stroke = Brushes.Gray,
                    StrokeDashArray = new DoubleCollection { 5, 5},
                    StrokeDashOffset = 6
                };

                mainCanvas.Children.Add(line);
            }
            foreach(Point point in points)
            {
                Ellipse ellipse = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Fill = Brushes.Gray
                };

                Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                mainCanvas.Children.Add(ellipse);
            }
        }

        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            mainCanvas.Children.Clear();
        }

        private int calculateBinomial(int n, int k)
        {
            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= n - (k - i);
                result /= i;
            }
            return result;
        }

        string FormString(List<double> list)
        {
            string res = "";
            for(int i = 0; i < list.Count; ++i)
            {
                res += $"Bern{i+1}: {list[i]}\n";
            }
            return res;
        }

        double[] getBernsteinPoints(Point[] points, double t)
        {
            List<double> results = new List<double>();

            int n = points.Length;

            for (int i = 0; i < n; i++)
            {
                double res = calculateBinomial(n, i) * Math.Pow(t, i) * Math.Pow((1 - t), n - i);
                results.Add(res);
            }

            return results.ToArray();
        }

        private void CalculatePolynoms(object sender, RoutedEventArgs e)
        {
            Point[] points = getPointsFromTextBox();

            if(points == null || !double.TryParse(ParameterTextBox.Text, out double t))
            {
                MessageBox.Show("Please, enter correct values in 't' textbox!");
                return;
            }
            else if(t < 0 || t > 1)
            {
                MessageBox.Show("t must be in range [0;1]");
                return;
            }

            double[] bernstein = getBernsteinPoints(points, t);

            MessageBox.Show(FormString(bernstein.ToList()));

        }

        private void CalculateCoords(object sender, RoutedEventArgs e)
        {
            if(!int.TryParse(PointsNumberTextBox.Text, out int n))
            {
                MessageBox.Show("Please, enter correct values!");
                return;
            }

            if(bezierPoints.Count == 0)
            {
                MessageBox.Show("Please, enter some points");
                return;
            }

            int step = bezierPoints.Count / n;
            string message = "";
            double x;
            double y;

            for (int i = 1; i <= bezierPoints.Count; i+=step)
            {
                x = Math.Round(bezierPoints[i-1].X / 30, 4);
                y = Math.Round(bezierPoints[i-1].Y / 30, 4);
                message += $"Point{i}: X = {x}, Y = {-y}\n";
            }

            MessageBox.Show(message);
        }

        private void DrawParameterical(object sender, RoutedEventArgs e)
        {
            Point[] points = getPointsFromTextBox();

            if (points == null)
                return;

            int n = points.Length;
            List<Point> bezierPoints = new List<Point>();

            for (double t = 0; t <= 1; t += 0.001)
            {
                double x = 0;
                double y = 0;
                //double[] bernstein = getBernsteinPoints(points, t);
                for (int i = 0; i < n; ++i)
                {
                    double bernstein = calculateBinomial(n-1, i) * Math.Pow(t, i) * Math.Pow((1 - t), n - i - 1);
                    x += points[i].X * bernstein;
                    y += points[i].Y * bernstein;
                }

                Point point = new Point(x, y);
                bezierPoints.Add(point);
            }

            displayPoints(bezierPoints.ToArray());
            DrawDirectionalLines(points);
        }
    }
}

/*
 
Nice test: 0 0, 7 1, 8 4, -3 -3, 10 2, 11 -1, 0 -5
 
 */