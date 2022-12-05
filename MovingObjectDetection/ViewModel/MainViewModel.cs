using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAPICodePack.Dialogs;
using MovingObjectDetection.Utils;
using OpenCvSharp;
using OpenCvSharp.Blob;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Task = System.Threading.Tasks.Task;

namespace MovingObjectDetection.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private BackgroundSubtractorKNN _mog;
        Mat _remove = new Mat();
        Mat mask;
        Mat background;

        private List<int>[,] backgroundArray = new List<int>[480,640];
        #endregion

        #region UI Variable
        private BitmapImage srcImage;
        public BitmapImage SrcImage
        {
            get { return srcImage; }
            set { Set(ref srcImage, value); }
        }

        private BitmapImage dstImage;
        public BitmapImage DstImage
        {
            get { return dstImage; }
            set { Set(ref dstImage, value); }
        }
        #endregion

        public MainViewModel()
        {
            mask = Cv2.ImRead(@"D:\mask.png", ImreadModes.Grayscale);
            _mog = BackgroundSubtractorKNN.Create();
            InitRelayCommand();
        }

        private void Run(string filePath)
        {
            VideoCapture video = new VideoCapture(filePath);
            Mat frame = new Mat();

            Task.Run(() =>
            {
                while (video.PosFrames != video.FrameCount)
                {
                    video.Read(frame);
                    CreateBackground(frame);
                }
            });
        }

        private void CreateBackground(Mat img)
        {

        }

        private void TestImage()
        {
            Mat src = Cv2.ImRead(@"C:\Users\dljdg\Documents\ss\frame148.png", ImreadModes.Unchanged);
            Cv2.CvtColor(src, src, ColorConversionCodes.BGR2GRAY);
            Cv2.BitwiseAnd(src, mask, src);
            Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(0, 0), 1.0);
            Cv2.AdaptiveThreshold(src, src, 150, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 25, 5);

            Cv2.Absdiff(src, background, src);
            // Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(0, 0), 1.0);
            SrcImage = MatToBitmapImage(src);
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

        private void LoadVideoFile()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.IsFolderPicker = false;
                dialog.Filters.Add(new CommonFileDialogFilter($"AVI", ".AVI"));

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ReadVideo(dialog.FileName);
                }
            }
        }

        private void ReadVideo(string filePath)
        {
            VideoCapture video = new VideoCapture(filePath);
            Mat frame = new Mat();

            Task.Run(() =>
            {
                while (video.PosFrames != video.FrameCount)
                {
                    video.Read(frame);
                    SendImageAction(frame);
                    Thread.Sleep(100);
                }
            });
        }

        private void SendImageAction(Mat obj)
        {
            _mog.Apply(obj, _remove);

            // Mat result = new Mat(obj.Size(), MatType.CV_8UC3);
            CvBlobs blobs = new CvBlobs();
            blobs.Label(_remove);
            foreach (var pair in blobs)
            {
                CvBlob blob = pair.Value;
                if (blob.Area < 100)
                    continue;
                Cv2.Rectangle(obj, new OpenCvSharp.Point(blob.MinX, blob.MinY), new OpenCvSharp.Point(blob.MaxX, blob.MaxY), Scalar.Red, 1, LineTypes.AntiAlias);
            }

            //CvBlob maxBlob = blobs.GreaterBlob();

            //if (maxBlob != null)
            //{
            //    if (maxBlob.Area > 500)
            //        Cv2.Rectangle(obj, new OpenCvSharp.Point(maxBlob.MinX, maxBlob.MinY), new OpenCvSharp.Point(maxBlob.MaxX, maxBlob.MaxY), Scalar.Red, 1, LineTypes.AntiAlias);
            //}

            SrcImage = MatToBitmapImage(obj);
        }

        private BitmapImage MatToBitmapImage(Mat obj)
        {
            BitmapImage resultBitmapImage;
            Bitmap tempBitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(obj);

            if (tempBitmap == null)
                return null;

            using (MemoryStream memory = new MemoryStream())
            {
                tempBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                resultBitmapImage = new BitmapImage();
                resultBitmapImage.BeginInit();
                resultBitmapImage.StreamSource = memory;
                resultBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                resultBitmapImage.EndInit();
                resultBitmapImage.Freeze();
            }
            return resultBitmapImage;
        }
    }
}