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
            _mog = BackgroundSubtractorKNN.Create();
            //_CamModule = new WebCamModule();
            //Exposure = 100;
            //Focus = 100;

            //_CamModule.SetImageDeviceParam(Exposure, Focus);
            //_CamModule.ImageSendAction += SendImageAction;

            //_CamModule.StartGrabContinuous();
            InitRelayCommand();
        }

        #region Command
        public RelayCommand<object> MenuClickCommand { get; private set; }
        private void InitRelayCommand()
        {
            MenuClickCommand = new RelayCommand<object>(OnMenuClick);
        }

        private void OnMenuClick(object obj)
        {
            switch (obj.ToString())
            {
                case "Load":
                    LoadVideoFile();
                    break;
            }
        }
        #endregion
    }
}