﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SamplesCore
{
    /// <summary>
    /// https://docs.opencv.org/3.3.0/d5/de7/tutorial_dnn_googlenet.html
    /// </summary>
    class CaffeSample : ISample
    {
        public void Run()
        {
            const string protoTxt = @"Data/Text/bvlc_googlenet.prototxt";
            const string caffeModel = "bvlc_googlenet.caffemodel";
            const string synsetWords = @"Data/Text/synset_words.txt";
            var classNames = File.ReadAllLines(synsetWords)
                .Select(line => line.Split(' ').Last())
                .ToArray();
            var capture = new VideoCapture(0);
            Console.Write("Downloading Caffe Model...");
            //PrepareModel(caffeModel);
            Console.WriteLine("Done");
            using (var img = new Mat())
            using (var window = new Window("capture"))
            using (var net = CvDnn.ReadNetFromCaffe(protoTxt, caffeModel))
                while (true)
                {
                    capture.Read(img);
                    if (img.Empty())
                        break;
                    Console.WriteLine("Layer names: {0}", string.Join(", ", net.GetLayerNames()));
                    Console.WriteLine();

                    // Convert Mat to batch of images
                    using (var inputBlob = CvDnn.BlobFromImage(img, 1, new Size(224, 224), new Scalar(104, 117, 123)))
                    {
                        net.SetInput(inputBlob, "data");
                        using (var prob = net.Forward("prob"))
                        {
                            // find the best class
                            GetMaxClass(prob, out int classId, out double classProb);
                            Console.WriteLine("Best class: #{0} '{1}'", classId, classNames[classId]);
                            Console.WriteLine("Probability: {0:P2}", classProb);
                            img.PutText(classNames[classId] + String.Format(" Probability: {0:P2}", classProb),
                                new Point(0 + 10, img.Size().Height - 10), HersheyFonts.HersheyDuplex, 1, Scalar.Black);
                            window.ShowImage(img);
                            Cv2.WaitKey(1);
                        }
                    }
                }
        }

        private static byte[] DownloadBytes(string url)
        {
            var client = WebRequest.CreateHttp(url);
            using (var response = client.GetResponseAsync().GetAwaiter().GetResult())
            using (var responseStream = response.GetResponseStream())
            {
                using (var memory = new MemoryStream())
                {
                    responseStream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
        }

        private static void PrepareModel(string fileName)
        {
            if (!File.Exists(fileName))
            {
                var contents = DownloadBytes("http://dl.caffe.berkeleyvision.org/bvlc_googlenet.caffemodel");
                File.WriteAllBytes(fileName, contents);
            }
        }

        /// <summary>
        /// Find best class for the blob (i. e. class with maximal probability)
        /// </summary>
        /// <param name="probBlob"></param>
        /// <param name="classId"></param>
        /// <param name="classProb"></param>
        private static void GetMaxClass(Mat probBlob, out int classId, out double classProb)
        {
            // reshape the blob to 1x1000 matrix
            using (var probMat = probBlob.Reshape(1, 1))
            {
                Cv2.MinMaxLoc(probMat, out _, out classProb, out _, out var classNumber);
                classId = classNumber.X;
            }
        }
    }
}