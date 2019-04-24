using OpenCvSharp;
using SamplesCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCV.Samples
{
    public class InVisible : ISample
    {
        public void Run()
        {
            //https://drive.google.com/file/d/1rc13wZ9zC03ObG5zB3uccUtsg_rsI8hC/view
            VideoCapture cap = VideoCapture.FromFile("Input.mp4");

            Mat background = new Mat();
            for (int i = 0; i < 60; i++)
            {
                cap.Read(background);
            }
            Cv2.ImShow("background", background);
            //flip(background,background,1);

            while (true)
            {

                Mat frame = new Mat();
                // Capture frame-by-frame
                cap.Read(frame);

                // If the frame is empty, break immediately
                if (frame.Empty())
                    break;

                Mat hsv = new Mat();
                //flip(frame,frame,1);

                Cv2.CvtColor(frame, hsv, ColorConversionCodes.BGR2HSV);

                Mat mask1 = new Mat(), mask2 = new Mat();
                Cv2.InRange(hsv, new Scalar(0, 120, 70), new Scalar(10, 255, 255), mask1);
                Cv2.InRange(hsv, new Scalar(170, 120, 70), new Scalar(180, 255, 255), mask2);

                mask1 = mask1 + mask2;

                Mat kernel = Mat.Ones(rows: 3, cols: 3, type: MatType.CV_32F);
                Cv2.MorphologyEx(mask1, mask1, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(mask1, mask1, MorphTypes.Dilate, kernel);

                Cv2.BitwiseNot(mask1, mask2);

                Mat res1 = new Mat(), res2 = new Mat(), final_output = new Mat();
                Cv2.BitwiseAnd(frame, frame, res1, mask2);
                Cv2.BitwiseAnd(background, background, res2, mask1);
                Cv2.AddWeighted(res1, 1, res2, 1, 0, final_output);



                Cv2.ImShow("Magic !!!", final_output);
                // Display the resulting frame
                //imshow( "Frame", frame );

                // Press  ESC on keyboard to exit
                char c = (char)Cv2.WaitKey(25);
                if (c == 27)
                    break;
                // Also relese all the mat created in the code to avoid memory leakage.
                frame.Release();
                hsv.Release();
                mask1.Release();
                mask2.Release();
                res1.Release();
                res2.Release();
                final_output.Release();

            }

            // When everything done, release the video capture object
            cap.Release();

            // Closes all the frames
            Cv2.DestroyAllWindows();
        }
    }
}
