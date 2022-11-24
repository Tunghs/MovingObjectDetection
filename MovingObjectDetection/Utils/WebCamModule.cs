using System;
using System.Threading;
using System.Diagnostics;
using OpenCvSharp;
using System.Threading.Tasks;

namespace MovingObjectDetection.Utils
{
    public class WebCamModule : IDisposable
    {
        public Action<Mat> ImageSendAction;
        private VideoCapture _Camera = null;
        private Stopwatch stopWatch = new Stopwatch();
        private bool _IsAvailable = false;
        //private bool _IsContinuous = false;
        private CancellationTokenSource _CanceltokenSource = null;

        private int _Exposure;
        private int _Focus;

        const double RESIZE_RATIO = 0.2666666666666667;

        public WebCamModule()
        {
            _Camera = new VideoCapture();
            _Camera.Open(0);
            _Camera.FrameWidth = 1920;
            _Camera.FrameHeight = 1080;
        }
        ~WebCamModule()
        {
            DestroyCamera();
        }
        public bool GetIsAvaliable()
        {
            return _IsAvailable;
        }

        public void SetImageDeviceParam(int exposure, int focus)
        {
            _Exposure = exposure;
            _Focus = focus;
        }

        public Mat GetSingleImage()
        {
            if (_Camera == null)
                return null;
            Mat image = new Mat();
            try
            {
                _Camera.Exposure = _Exposure;
                _Camera.Focus = _Focus;
                //_Camera.Exposure = -10;
                //_Camera.Focus = 100;
                bool isSuccess = _Camera.Read(image);
                if (isSuccess == false)
                    return null;
                image = image.Resize(new OpenCvSharp.Size(image.Width * RESIZE_RATIO, image.Height * RESIZE_RATIO), 0, 0, InterpolationFlags.Cubic);
            }
            catch (Exception e)
            {
            }
            return image;
        }

        public void StartGrabContinuous()
        {
            if (_CanceltokenSource != null)
                _CanceltokenSource.Dispose();

            _CanceltokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = _CanceltokenSource.Token;

            //_IsContinuous = true;

            Task.Run(() =>
            {
                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                        cancelToken.ThrowIfCancellationRequested();

                    Mat image = GetSingleImage();
                    if (ImageSendAction != null)
                        ImageSendAction(image);
                }
            }, cancelToken);

        }

        public void StopGrabContinuous()
        {
            if (_CanceltokenSource != null)
                _CanceltokenSource.Cancel();
        }

        private void DestroyCamera()
        {
            if (_Camera.IsEnabledDispose == true)
                _Camera.Dispose();
        }

        public void Dispose()
        {
            StopGrabContinuous();
            DestroyCamera();
        }
    }
}
