using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Windows.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Project1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static readonly RoutedCommand LoadCommand = new RoutedUICommand("Load Image", "LoadCommand", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
            {
                new KeyGesture(Key.L, ModifierKeys.Control)
            }));

        public static readonly RoutedCommand SaveCommand = new RoutedUICommand("Save Image", "SaveCommand", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
           {
                new KeyGesture(Key.S, ModifierKeys.Control)
           }));

        public static readonly RoutedCommand ReloadCommand = new RoutedUICommand("Reload Image", "ReloadCommand", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
          {
                new KeyGesture(Key.R, ModifierKeys.Control)
          }));

        public Bitmap OriginalImage { get; set; }
        public Bitmap FilteredImage { get; set; }
        public BitmapImage FilteredImageConverted { get; set; }
        public BitmapImage OriginalImageConverted { get; set; }
        public List<Kernel> FilterKernels { get; set; }
        public PointCollection ListOfPoints { get; set; }
        public PointCollection CirclePoints { get; set; }
        public List<Ellipse> ListOfEllipses { get; set; }
        public PointCollection SavedListOfPoints { get; set; }
        public PointCollection SavedListOfPoints2 { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            FilterKernels = LoadDefaultKernels();
            ListOfPoints = new PointCollection();
            CirclePoints = new PointCollection();
            ListOfEllipses = new List<Ellipse>();
            SavedListOfPoints = new PointCollection();
            SavedListOfPoints2 = new PointCollection();
        }
        private List<Kernel> LoadDefaultKernels()
        {
            List<Kernel> tmp = new List<Kernel>();
            tmp.Add(new Kernel(Constants.BlurKernel, "Blur"));
            tmp.Add(new Kernel(Constants.GaussianBlurKernel, "Gaussian Blur"));
            tmp.Add(new Kernel(Constants.SharpenKernel, "Sharpen"));
            tmp.Add(new Kernel(Constants.EdgeDetectionKernel, "Edge Detection"));
            tmp.Add(new Kernel(Constants.EmbossKernel, "Emboss"));
            tmp.Add(new Kernel(Constants.Identity, "Custom1"));
            tmp.Add(new Kernel(Constants.Identity, "Custom2"));
            return tmp;
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png";
            openFileDialog.Title = "Load an Image File";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                OriginalImage = new Bitmap(openFileDialog.FileName);
                FilteredImage = OriginalImage;
                Uri fileUri = new Uri(openFileDialog.FileName);
                OriginalImageConverted = new BitmapImage(fileUri);
                FilteredImageConverted = OriginalImageConverted;
                Image.Source = FilteredImageConverted;
                EnableAllButtons();
            }
        }
        private void EnableAllButtons()
        {
            FuncInversion.IsEnabled = true;
            FuncBrightnessCorrection.IsEnabled = true;
            FuncContrastEnhancement.IsEnabled = true;
            FuncGammaCorrection.IsEnabled = true;
            ConvBlur.IsEnabled = true;
            ConvGaussianBlur.IsEnabled = true;
            ConvSharpen.IsEnabled = true;
            ConvEdgeDetection.IsEnabled = true;
            ConvEmboss.IsEnabled = true;
            ReloadImage.IsEnabled = true;
            OriginalImageButton.IsEnabled = true;
            FilteredImageButton.IsEnabled = true;
            Grayscale.IsEnabled = true;
            AverageDitheringGray.IsEnabled = true;
            AverageDitheringColor.IsEnabled = true;
            MedianCut.IsEnabled = true;
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        FilteredImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        FilteredImage.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        FilteredImage.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Done By\nMarcin Świerkot");
        }

        private void FuncInversion_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = FunctionalFilters.Filter(FilteredImage, FunctionalFilters.Invert);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void FuncBrightnessCorrection_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = FunctionalFilters.Filter(FilteredImage, FunctionalFilters.CorrectBrightness);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void FuncContrastEnhancement_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = FunctionalFilters.Filter(FilteredImage, FunctionalFilters.EnhanceContrast);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void FuncGammaCorrection_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = FunctionalFilters.Filter(FilteredImage, FunctionalFilters.CorrectGamma);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }
        private void FuncCustom1_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = FunctionalFilters.Custom1(FilteredImage, SavedListOfPoints);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void FuncCustom2_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = FunctionalFilters.Custom2(FilteredImage, SavedListOfPoints2);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void ReloadImage_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = OriginalImage;
            FilteredImageConverted = OriginalImageConverted;
            Image.Source = FilteredImageConverted;
            ChooseBetweenGrayAndColor.SelectedIndex = 1;
            sliderDitheringGray.Value = 2;
            sliderDitheringColorRed.Value = 2;
            sliderDitheringColorGreen.Value = 2;
            sliderDitheringColorBlue.Value = 2;
            sliderMedian.Value = 2;
        }

        private void OriginalImageButton_Click(object sender, RoutedEventArgs e)
        {
            Image.Source = OriginalImageConverted;
        }

        private void FilteredImageButton_Click(object sender, RoutedEventArgs e)
        {
            Image.Source = FilteredImageConverted;
        }

        private void ConvBlur_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = ConvolutionalFilters.ConvolutionFilter(FilteredImage, ConvolutionalFilters.FindKernelByName(FilterKernels, "Blur"));
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void ConvGaussianBlur_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = ConvolutionalFilters.ConvolutionFilter(FilteredImage, ConvolutionalFilters.FindKernelByName(FilterKernels, "Gaussian Blur"));
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void ConvSharpen_Click(object sender, RoutedEventArgs e)
        { 
            FilteredImage = ConvolutionalFilters.ConvolutionFilter(FilteredImage, ConvolutionalFilters.FindKernelByName(FilterKernels, "Sharpen"));
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void ConvEdgeDetection_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = ConvolutionalFilters.ConvolutionFilter(FilteredImage, ConvolutionalFilters.FindKernelByName(FilterKernels, "Edge Detection"));
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void ConvEmboss_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = ConvolutionalFilters.ConvolutionFilter(FilteredImage, ConvolutionalFilters.FindKernelByName(FilterKernels, "Emboss"));
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetEditorToDefault();
        }

        private void DrawCirclesAroundPoints()
        {
            const float width = 8;
            const float radius = width / 2;
            foreach (System.Windows.Point point in CirclePoints)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.SetValue(Canvas.LeftProperty, point.X - radius);
                ellipse.SetValue(Canvas.TopProperty, point.Y - radius);
                ellipse.Fill = System.Windows.Media.Brushes.Black;
                ellipse.Stroke = System.Windows.Media.Brushes.Black;
                ellipse.StrokeThickness = 1;
                ellipse.Width = width;
                ellipse.Height = width;
                ListOfEllipses.Add(ellipse);
                CanvasEditor.Children.Add(ellipse);
            }
        }
        private void ResetEditorToDefault()
        {
            ClearEllipses();
            CirclePoints.Clear();
            ListOfPoints.Clear();
            CirclePoints.Add(new System.Windows.Point(0, 255));
            CirclePoints.Add(new System.Windows.Point(255, 0));
            for (int i = 0; i < 256; i++)
            {
                ListOfPoints.Add(new System.Windows.Point(i, 255 - i));
            }
            MyPolyline.Points = CirclePoints;
            DrawCirclesAroundPoints();
        }

        private void CanvasEditor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                System.Windows.Point ClickedPlace = e.GetPosition(CanvasEditor);
                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    if (ListOfPoints[i].X == (int)ClickedPlace.X)
                    {
                        System.Windows.Point point = new System.Windows.Point((int)ClickedPlace.X, (int)ClickedPlace.Y);
                        for (int j = 0; j < CirclePoints.Count - 1; j++)
                        {
                            if (point.X > CirclePoints[j].X && point.X < CirclePoints[j+1].X)
                            {
                                CirclePoints.Insert(j + 1, point);
                                break;
                            }
                        }           
                        MyPolyline.Points = CirclePoints;
                        UpdateListOfPoints();
                    }
                }
                DrawCirclesAroundPoints();
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                System.Windows.Point ClickedPlace = e.GetPosition(CanvasEditor);
                if ((int)ClickedPlace.X <= 2)
                {
                    //MessageBox.Show("test");
                    CirclePoints.RemoveAt(0);
                    CirclePoints.Insert(0, new System.Windows.Point(0, (int)ClickedPlace.Y));
                    ClearEllipses();
                    DrawCirclesAroundPoints();
                    UpdateListOfPoints();
                }
                else if ((int)ClickedPlace.X >= 251)
                {
                    //MessageBox.Show("test2");
                    CirclePoints.RemoveAt(CirclePoints.Count - 1);
                    CirclePoints.Add(new System.Windows.Point(255, (int)ClickedPlace.Y));
                    ClearEllipses();
                    DrawCirclesAroundPoints();
                    UpdateListOfPoints();
                }
                else
                {
                    //MessageBox.Show("test3");
                    for (int i = 1; i < CirclePoints.Count - 1; i++)
                    {
                        if (Math.Abs(CirclePoints[i].X - (int)ClickedPlace.X) <= 4 && Math.Abs(CirclePoints[i].Y - (int)ClickedPlace.Y) <= 4)
                        {            
                            CirclePoints.Remove(CirclePoints[i]);
                            ClearEllipses();
                            DrawCirclesAroundPoints();
                            UpdateListOfPoints();
                        }
                    }
                }
            
            }
        }

        private void UpdateListOfPoints()
        {
            ListOfPoints.Clear();
            for (int i = 0; i < CirclePoints.Count - 1; i++)
            {
                double x1 = CirclePoints[i].X;
                double x2 = CirclePoints[i + 1].X;
                double y1 = CirclePoints[i].Y;
                double y2 = CirclePoints[i + 1].Y;
                double slope = (y2 - y1) / (x2 - x1);

                ListOfPoints.Add(new System.Windows.Point(x1, y1));
                for (int j = 1; j < x2 - x1; j++)
                {
                    ListOfPoints.Add(new System.Windows.Point(x1 + j, (int)(y1 + (j * slope))));
                }

                if (i == CirclePoints.Count - 2)
                {
                    ListOfPoints.Add(new System.Windows.Point(x2, y2));
                }
            }
        }
        private void ResetFilterEditor_Click(object sender, RoutedEventArgs e)
        {
            ResetEditorToDefault();
        }

        private void SaveFilterEditor_Click(object sender, RoutedEventArgs e)
        {
            if (SavedListOfPoints.Count == 0)
            {
                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    SavedListOfPoints.Add(new System.Windows.Point(ListOfPoints[i].X, ListOfPoints[i].Y));
                }
                FuncCustom1.IsEnabled = true;
                DeleteFuncCustom1.Visibility = Visibility;
                DeleteFuncCustom1.IsEnabled = true;
                MessageBox.Show($"Saved under {FuncCustom1.Content}");
            }
            else if (SavedListOfPoints2.Count == 0)
            {
                for (int i = 0; i < ListOfPoints.Count; i++)
                {
                    SavedListOfPoints2.Add(new System.Windows.Point(ListOfPoints[i].X, ListOfPoints[i].Y));
                }
                FuncCustom2.IsEnabled = true;
                DeleteFuncCustom2.Visibility = Visibility;
                DeleteFuncCustom2.IsEnabled = true;
                MessageBox.Show($"Saved under {FuncCustom2.Content}");
            }
            else
            {
                MessageBox.Show("You can create only up to 2 custom filters.\nDelete one of them to create new one");
            }


        }

        private void EditFuncInversion_Click(object sender, RoutedEventArgs e)
        {
            ClearEllipses();
            ClearListOfPoints();
            CirclePoints.Add(new System.Windows.Point(0, 0));
            CirclePoints.Add(new System.Windows.Point(255, 255));
            for (int i = 0; i < 256; i++)
            {
                ListOfPoints.Add(new System.Windows.Point(i, 255 - (255 - i)));
            }
            MyPolyline.Points = ListOfPoints;
            DrawCirclesAroundPoints();
        }

        private void EditFuncBrightnessCorrection_Click(object sender, RoutedEventArgs e)
        {
            ClearEllipses();
            ClearListOfPoints();
            CirclePoints.Add(new System.Windows.Point(0, 255 - FunctionalFilters.Truncate(Constants.BrightnessCoefficient + 0)));
            CirclePoints.Add(new System.Windows.Point(255, 255 - FunctionalFilters.Truncate(Constants.BrightnessCoefficient + 255)));
            for (int i = 0; i < 256; i++)
            {
                ListOfPoints.Add(new System.Windows.Point(i, 255 - FunctionalFilters.Truncate(Constants.BrightnessCoefficient + i)));
            }
            MyPolyline.Points = ListOfPoints;
            DrawCirclesAroundPoints();
        }

        private void EditFuncContrastEnhancement_Click(object sender, RoutedEventArgs e)
        {
            ClearEllipses();
            ClearListOfPoints();
            float Factor = (259 * (255 + Constants.ContrastCoefficient)) / (255 * (259 - Constants.ContrastCoefficient));
            CirclePoints.Add(new System.Windows.Point(0, 255 - FunctionalFilters.Truncate((int)(Factor * (0 - 128)) + 128)));
            CirclePoints.Add(new System.Windows.Point(255, 255 - FunctionalFilters.Truncate((int)(Factor * (255 - 128)) + 128)));
            for (int i = 0; i < 256; i++)
            {
                ListOfPoints.Add(new System.Windows.Point(i, 255 - FunctionalFilters.Truncate((int)(Factor * (i - 128)) + 128)));
            }
            MyPolyline.Points = ListOfPoints;
            DrawCirclesAroundPoints();
        }

        private void EditFuncGammaCorrection_Click(object sender, RoutedEventArgs e)
        {
            ClearEllipses();
            ClearListOfPoints();
            CirclePoints.Add(new System.Windows.Point(0, 255 - Math.Pow(0 / 255, Constants.GammaCoefficient) * 255));
            CirclePoints.Add(new System.Windows.Point(255, 255 - Math.Pow(255 / 255, Constants.GammaCoefficient) * 255));
            for (double i = 0; i < 256; i++)
            {
                double y = 255 - Math.Pow(i / 255, Constants.GammaCoefficient) * 255;
                ListOfPoints.Add(new System.Windows.Point(i, (int)y));
            }
            MyPolyline.Points = ListOfPoints;
            DrawCirclesAroundPoints();
        }

        private void DeleteFuncCustom1_Click(object sender, RoutedEventArgs e)
        {
            SavedListOfPoints.Clear();
            FuncCustom1.IsEnabled = false;
            DeleteFuncCustom1.Visibility = Visibility.Hidden;
            DeleteFuncCustom1.IsEnabled = false;

        }

        private void DeleteFuncCustom2_Click(object sender, RoutedEventArgs e)
        {
            SavedListOfPoints2.Clear();
            FuncCustom2.IsEnabled = false;
            DeleteFuncCustom2.Visibility = Visibility.Hidden;
            DeleteFuncCustom2.IsEnabled = false;
        }
        private void ClearListOfPoints()
        {
            ListOfPoints.Clear();
            CirclePoints.Clear();
        }
        private void ClearEllipses()
        {
            if (ListOfEllipses.Count > 0)
            {
                foreach (Ellipse ellipse in ListOfEllipses)
                {
                    CanvasEditor.Children.Remove(ellipse);
                }
            }
        }

        private void Grayscale_Click(object sender, RoutedEventArgs e)
        {
            FilteredImage = Project1.Grayscale.ApplyGrayscale(FilteredImage);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
            ChooseBetweenGrayAndColor.SelectedIndex = 0;
        }

        private void MedianCut_Click(object sender, RoutedEventArgs e)
        {
            int numColors = (int)sliderMedian.Value;
            MedianCutAlgorithm medianCut = new MedianCutAlgorithm(numColors);
            FilteredImage = medianCut.ApplyMedianCut(FilteredImage);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
            //MessageBox.Show("Test");
        }

        private void AverageDitheringGray_Click(object sender, RoutedEventArgs e)
        {
            int numColors = (int)sliderDitheringGray.Value;
            AverageDitheringAlgorithmGray averageDithering = new AverageDitheringAlgorithmGray(numColors);
            FilteredImage = averageDithering.ApplyAverageDithering(FilteredImage);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
            //MessageBox.Show("Test");
        }

        private void AverageDitheringColor_Click(object sender, RoutedEventArgs e)
        {
            int numRedColors = (int)sliderDitheringColorRed.Value;
            int numGreenColors = (int)sliderDitheringColorGreen.Value;
            int numBlueColors = (int)sliderDitheringColorBlue.Value;
            AverageDitheringAlgorithm averageDithering = new AverageDitheringAlgorithm(numRedColors, numGreenColors, numBlueColors);
            FilteredImage = averageDithering.ApplyAverageDithering(FilteredImage);
            FilteredImageConverted = ConvertBitmapToBitmapImage.Convert(FilteredImage);
            Image.Source = FilteredImageConverted;
            //MessageBox.Show("Test");
        }
    }
    public class ConvertBitmapToBitmapImage
    {
        public static BitmapImage Convert(Bitmap src)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                src.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }

}