using OpenCvSharp;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace UIExtractTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        MainViewModel viewmodel;
        public MainWindow()
        {
            InitializeComponent();
            viewmodel = new MainViewModel();
            viewmodel.Init();
            viewmodel.PropertyChanged += Viewmodel_PropertyChanged;
            this.DataContext = viewmodel;
        }

        private void Viewmodel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("FramePos".Equals(e.PropertyName))
            {
                Mat frame = new Mat();
                viewmodel.cap.PosFrames = viewmodel.FramePos;
                viewmodel.cap.Read(frame);
                viewmodel.FrameImage = frame.ToMemoryStream();
                frame.Release();
            }
            if ("FrameToPos".Equals(e.PropertyName))
            {
                Mat frame = new Mat();
                viewmodel.cap.PosFrames = viewmodel.FrameToPos;
                viewmodel.cap.Read(frame);
                viewmodel.FrameToImage = frame.ToMemoryStream();
                frame.Release();
            }
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Stream FrameImage { get; set; }
        public int FramePos { get; set; }
        public Stream FrameToImage { get; set; }
        public int FrameToPos { get; set; }
        public int FrameCount { get; set; }

        public VideoCapture cap;
        public void Init()
        {
            //https://drive.google.com/file/d/1rc13wZ9zC03ObG5zB3uccUtsg_rsI8hC/view
            cap = VideoCapture.FromFile("Megaman.mp4");
            var time = (double)cap.FrameCount / cap.Fps;
            FrameCount = cap.FrameCount;

        }

    }
}
