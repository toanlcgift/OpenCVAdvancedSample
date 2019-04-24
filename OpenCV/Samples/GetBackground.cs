using OpenCvSharp;
using SamplesCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCV.Samples
{
    public class GetBackground : ISample
    {
        public void Run()
        {
            //https://drive.google.com/file/d/1rc13wZ9zC03ObG5zB3uccUtsg_rsI8hC/view
            VideoCapture cap = VideoCapture.FromFile("Input.mp4");
            
            Mat avg = new Mat();
            Mat output = new Mat();

            while (true)
            {

                Mat frame = new Mat();
                // Capture frame-by-frame
                cap.Read(frame);

                // If the frame is empty, break immediately
                if (frame.Empty())
                    break;

                if (cap.Get(CaptureProperty.PosFrames) == 1)
                    frame.ConvertTo(avg, MatType.CV_32F);

                Cv2.AccumulateWeighted(frame, avg, 0.0005, null);
                Cv2.ConvertScaleAbs(avg, output);
                
                Cv2.ImShow("output", output);
                // Press  ESC on keyboard to exit
                char c = (char)Cv2.WaitKey(25);
                if (c == 27)
                    break;
                frame.Release();
            }

            // When everything done, release the video capture object
            cap.Release();
            avg.Release();
            output.Release();
            // Closes all the frames
            Cv2.DestroyAllWindows();
        }
    }
}
