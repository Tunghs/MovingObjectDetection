using GalaSoft.MvvmLight;
using System.Windows.Media.Imaging;

namespace MovingObjectDetection.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BitmapImage _Image;
        public BitmapImage Image
        {
            get { return _Image; }
            set { Set(ref _Image, value); }
        }

        public MainViewModel()
        {

        }
    }
}