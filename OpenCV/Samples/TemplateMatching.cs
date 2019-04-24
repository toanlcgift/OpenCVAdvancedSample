using System;
using OpenCvSharp;

namespace SamplesCore
{
    /// <summary>
    /// sample of photo module methods
    /// </summary>
    class TemplateMatching : ISample
    {
        bool use_mask;
        Mat img; Mat templ; Mat mask; Mat result = new Mat();
        const string image_window = "Source Image";
        const string result_window = "Result window";
        int match_method;
        int max_Trackbar = 5;
        public void Run()
        {
            Mat src = new Mat();
            img = Cv2.ImRead(FilePath.Image.Src, ImreadModes.Color);
            templ = Cv2.ImRead(FilePath.Image.Test, ImreadModes.Color);
            if (img.Empty() || templ.Empty())
            {

                throw new System.Exception("Can't read one of the images");
            }
            Cv2.NamedWindow(image_window, WindowMode.AutoSize);
            Cv2.NamedWindow(result_window, WindowMode.AutoSize);

            const string trackbar_label = "Method: \n 0: SQDIFF \n 1: SQDIFF NORMED \n 2: TM CCORR \n 3: TM CCORR NORMED \n 4: TM COEFF \n 5: TM COEFF NORMED";
            //Cv2.CreateTrackbar(trackbar_label, image_window, ref match_method, max_Trackbar, MatchingMethod);
            MatchingMethod(0, 0);

            //! [wait_key]
            Cv2.WaitKey(0);
        }

        private void MatchingMethod(int pos, object userdata)
        {
            Mat img_display=new Mat();
            img.CopyTo(img_display);
            //! [copy_source]

            //! [create_result_matrix]
            /// Create the result matrix
            int result_cols = img.Cols - templ.Cols + 1;
            int result_rows = img.Rows - templ.Rows + 1;

            result.Create(result_rows, result_cols, MatType.CV_32FC1);
            //! [create_result_matrix]

            //! [match_template]
            /// Do the Matching and Normalize
            bool method_accepts_mask = ((int)TemplateMatchModes.SqDiff == match_method || match_method == (int)TemplateMatchModes.CCoeffNormed);
            if (use_mask && method_accepts_mask)
            {
                Cv2.MatchTemplate(img, templ, result, (TemplateMatchModes)match_method, mask);
            }
            else
            {
                Cv2.MatchTemplate(img, templ, result, (TemplateMatchModes)match_method);
            }
            //! [match_template]

            //! [normalize]
            Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax, -1, new Mat());
            //! [normalize]

            //! [best_match]
            /// Localizing the best match with minMaxLoc
            double minVal; double maxVal; Point minLoc; Point maxLoc;
            Point matchLoc;

            Cv2.MinMaxLoc(result,out minVal,out maxVal, out minLoc,out maxLoc, new Mat());
            //! [best_match]

            //! [match_loc]
            /// For SQDIFF and SQDIFF_NORMED, the best matches are lower values. For all the other methods, the higher the better
            if (match_method == (int)TemplateMatchModes.SqDiff || match_method == (int)TemplateMatchModes.SqDiffNormed)
            {
                matchLoc = minLoc;
            }
            else
            {
                matchLoc = maxLoc;
            }
            //! [match_loc]

            //! [imshow]
            /// Show me what you got
            Cv2.Rectangle(img, matchLoc, new Point(matchLoc.X + templ.Cols, matchLoc.Y + templ.Rows), Scalar.White, 2, LineTypes.Link8, 0);
            //rectangle(result, matchLoc, Point(matchLoc.x + templ.cols, matchLoc.y + templ.rows), Scalar::all(0), 2, 8, 0);

            Cv2.ImShow(image_window, img);
        }
    }
}