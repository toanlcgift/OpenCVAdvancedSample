
using OpenCvSharp;
using SamplesCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LiveStreamServer.Samples
{
    public class FaceDetection : ISample
    {
        public void Run()
        {
            CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
            // Opens MP4 file (ffmpeg is probably needed)
            var capture = new VideoCapture(0);

            int sleepTime = (int)Math.Round(1000 / ((capture.Fps == 0) ? 60 : capture.Fps));
            var hog = new HOGDescriptor();
            hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
            var recognizer = OpenCvSharp.Face.EigenFaceRecognizer.Create(2);
            var img1 = Cv2.ImRead(SamplesCore.FilePath.Image.Girl, ImreadModes.Grayscale);

            var img2 = Cv2.ImRead(SamplesCore.FilePath.Image.Lenna, ImreadModes.Grayscale);
            System.Console.WriteLine("img1 width: " + img1.Size().Width + " height:" + img1.Size().Height);
            System.Console.WriteLine("img2 width: " + img2.Size().Width + " height:" + img2.Size().Height);
            recognizer.Train(new List<Mat>() { img1, img2 }, new List<int>() { 1, 2 });
            recognizer.Write("trainfile1.dat");
            recognizer.Read("trainfile1.dat");
            System.Console.WriteLine("before predict");
            System.Console.WriteLine("img1 width: " + img1.Size().Width + " height:" + img1.Size().Height);
            System.Console.WriteLine("img2 width: " + img2.Size().Width + " height:" + img2.Size().Height);

            recognizer.Predict(InputArray.Create(img1), out int label, out double confidence);
            System.Console.WriteLine("label: " + label + " confidence: " + confidence);

            using (var window = new Window("capture"))
            {
                // Frame image buffer
                Mat image = new Mat();
                // When the movie playback reaches end, Mat.data becomes NULL.
                while (true)
                {
                    capture.Read(image); // same as cvQueryFrame
                    if (image.Empty())
                        break;
                    var rect = cascadeClassifier.DetectMultiScale(image, 1.3, 5, HaarDetectionType.FindBiggestObject);
                    if (rect.Count() > 0)
                    {
                        for (int i = 0; i < rect.Count(); i++)
                        {
                            Cv2.Rectangle(image, rect[i], Scalar.Red, 2);
                        }
                    }

                    var found = hog.DetectMultiScale(image, 0, new Size(8, 8), new Size(24, 16), 1.05, 2);
                    if (found.Count() > 0)
                    {
                        for (int i = 0; i < found.Count(); i++)
                        {
                            Cv2.Rectangle(image, found[i], Scalar.Green, 2);
                        }
                    }

                    window.ShowImage(image);
                    Cv2.WaitKey(sleepTime);

                }
            }


            //mat.SaveImage("ahihi.png");
        }
    }
}
