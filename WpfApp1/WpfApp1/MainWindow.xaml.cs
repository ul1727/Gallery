using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Windows.Interop;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DocumentFormat.OpenXml.Office2010.Word;
using System.Runtime.Serialization;
using DocumentFormat.OpenXml.Bibliography;
using static System.Net.WebRequestMethods;
using Brushes = System.Drawing.Brushes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    class IMG : INotifyPropertyChanged
    {    
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private double _MposX;
        public double MposX
        {
            get { return _MposX; }
            set { _MposX = value; OnPropertyChanged("MposX"); }
        }

        private double _MposY;
        public double MposY
        {
            get { return _MposY; }
            set { _MposY = value; OnPropertyChanged("MposY"); }
        }

        private double _Scale = 1;
        public double Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value;
                OnPropertyChanged("Scale");
            }
        }

        private List<string> _Files;
        public List<string> Files
        {
            get { return _Files; }
            set
            {
                _Files = value;
                OnPropertyChanged("Files");
            }
        }
        private BitmapSource _ImgS;
        public BitmapSource ImgS
        {
            get { return _ImgS; }
            set
            {
                _ImgS = value;
                OnPropertyChanged("ImgS");
            }
        }
        private string _Chfile;
        public string Chfile
        {
            get { return _Chfile; }
            set
            {
                _Chfile = value;
                string[] photonames = new string[] { ".jpg", ".png", ".jpeg", ".tiff", ".gif", ".bmp", ".ico", ".webp", ".raw" };
                if (!string.IsNullOrEmpty(Chfile) && System.IO.File.Exists(Chfile) && Directory.Exists(System.IO.Path.GetDirectoryName(Chfile)))
                { //1
                    if (photonames.Contains(System.IO.Path.GetExtension(value)))
                    {
                        Scale = 1;
                        BitmapImage X = new BitmapImage();
                        X.BeginInit();
                        X.UriSource = new Uri(value, UriKind.RelativeOrAbsolute);
                        X.EndInit();
                        ImgS = X;

                    }
                    else
                    {
                        var icon = System.Drawing.Icon.ExtractAssociatedIcon(value);
                        BitmapSource X1 = Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle, System.Windows.Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        ImgS = X1;
                    }

                    OnPropertyChanged("Chfile");
                }//1
            }

        }
    }
    public partial class MainWindow : Window
    {
        private object imageControl;
        bool flag = false;
        
        public MainWindow()
        {
            IMG obj = new IMG();
            InitializeComponent();
            this.DataContext = obj;          
        }

        string DIRECTORY;
        string NAME;
        //Vector offset; 
        System.Windows.Point Pos;
        System.Windows.Point CurrPos;
        //List<string> FILEStoPROP = new List<string>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            //ofd.ShowDialog();   
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileName != "")
                {
                    (DataContext as IMG).Scale = 1;
                    NAME = ofd.FileName;
                    DIRECTORY = System.IO.Path.GetDirectoryName(NAME);
                    (DataContext as IMG).Files = Directory.GetFiles(DIRECTORY).ToList();
                    (DataContext as IMG).Chfile = NAME;

                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

            if (Directory.Exists(Properties.Settings.Default.FilePath))
            {
                DIRECTORY = Properties.Settings.Default.FilePath;
                (DataContext as IMG).Files = (Directory.GetFiles(Properties.Settings.Default.FilePath)).ToList();
                (DataContext as IMG).Chfile = Properties.Settings.Default.FileName;

            }
              

            
            (DataContext as IMG).Scale = Properties.Settings.Default.default_scaleX;


            scroll.ScrollToHorizontalOffset(Properties.Settings.Default.OffsetX);
            scroll.ScrollToVerticalOffset(Properties.Settings.Default.OffsetY);
            //(DataContext as IMG).MposX = Properties.Settings.Default.MousePos.X;
            //(DataContext as IMG).MposY = Properties.Settings.Default.MousePos.Y;           
            this.UpdateLayout();





        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Properties.Settings.Default.OffsetX = scroll.HorizontalOffset;
            Properties.Settings.Default.OffsetY = scroll.VerticalOffset;
            Properties.Settings.Default.FilePath = DIRECTORY;
            Properties.Settings.Default.FileName = (DataContext as IMG).Chfile;
            Properties.Settings.Default.default_scaleX = (DataContext as IMG).Scale;
            Properties.Settings.Default.Save();

        }     
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Pos = e.GetPosition(this);
        }
        
        private void image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CurrPos = e.GetPosition(this);
                double Xmove = this.scroll.HorizontalOffset;
                this.scroll.ScrollToHorizontalOffset(Xmove - (CurrPos.X - Pos.X));
                double Ymove = this.scroll.VerticalOffset;
                this.scroll.ScrollToVerticalOffset(Ymove - (CurrPos.Y - Pos.Y));
                (DataContext as IMG).MposX = CurrPos.X;
                (DataContext as IMG).MposY = CurrPos.Y;
                Pos = CurrPos;

            }
        }
        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (canVas.IsMouseOver)
            {
                System.Windows.Point mouseAtImage = e.GetPosition(canVas); 
                System.Windows.Point mouseAtScrollViewer = e.GetPosition(scroll);
                if (e.Delta > 0)
                {
                    (DataContext as IMG).Scale *= 1.25;
                    //st.ScaleX = st.ScaleY  =  st.ScaleX * 1.25;
                    if ((DataContext as IMG).Scale > 64) (DataContext as IMG).Scale = 64;
                }
                else
                {
                    (DataContext as IMG).Scale /= 1.25;
                    //st.ScaleX = st.ScaleY  = st.ScaleX / 1.25;
                    if ((DataContext as IMG).Scale < 0.1) (DataContext as IMG).Scale = 0.1;
                }
                              
                scroll.ScrollToHorizontalOffset(0);
                scroll.ScrollToVerticalOffset(0);
                this.UpdateLayout();               
                Vector offset = canVas.TranslatePoint(mouseAtImage, scroll) - mouseAtScrollViewer; 
                scroll.ScrollToHorizontalOffset(offset.X);
                scroll.ScrollToVerticalOffset(offset.Y);
                this.UpdateLayout();
                e.Handled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (DataContext as IMG).Scale = 1;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
           
            grid.Background = System.Windows.Media.Brushes.Gray;

            SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(213, 215, 212));          
            listBox.Background = brush;
            White_theme_button.Background = brush;
            Standart_theme_button.Background = brush;
            Dark_theme_button.Background = brush;
            Select_Button.Background = brush;
            Remove_Scale_Button.Background = brush;
            Rectangle_List_of_file.Fill = brush;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            grid.Background = System.Windows.Media.Brushes.White;
            SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(213, 240, 232));
            listBox.Background = brush;
            White_theme_button.Background = brush;
            Standart_theme_button.Background = brush;
            Dark_theme_button.Background = brush;
            Select_Button.Background = brush;
            Remove_Scale_Button.Background = brush;
            Rectangle_List_of_file.Fill = brush;

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            grid.Background = System.Windows.Media.Brushes.Black;
            SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(221, 145, 255));
            listBox.Background = brush;
            White_theme_button.Background = brush;
            Standart_theme_button.Background = brush;
            Dark_theme_button.Background = brush;
            Select_Button.Background = brush;
            Remove_Scale_Button.Background = brush;
            Rectangle_List_of_file.Fill = brush;

        }

        private void scroll_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void scroll_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
    }
}
