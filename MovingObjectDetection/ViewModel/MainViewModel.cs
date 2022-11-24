using GalaSoft.MvvmLight;
using MovingObjectDetection.Utils;
using OpenCvSharp;
using System.Windows.Media.Imaging;

namespace MovingObjectDetection.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BitmapImage _Image;
        private BackgroundSubtractorKNN _mog;
        Mat _remove = new Mat();

        public BitmapImage Image
        {
            get { return _Image; }
            set { Set(ref _Image, value); }
        }

        private int _Exposure;
        public int Exposure
        {
            get { return _Exposure; }
            set
            {
                _Exposure = value;
                _CamModule?.SetImageDeviceParam(Exposure, Focus);
                RaisePropertyChanged("Exposure");
            }
        }

        private int _Focus;
        public int Focus
        {
            get { return _Focus; }
            set
            {
                _Focus = value;
                _CamModule?.SetImageDeviceParam(Exposure, Focus);
                RaisePropertyChanged("Focus");
            }
        }

        private WebCamModule _CamModule;

        public MainViewModel()
        {

        }
    }
}