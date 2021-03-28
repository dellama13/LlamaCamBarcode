﻿using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;
using System.Linq;

namespace LlamaCamBarcode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        VideoCaptureDevice LlamaCam;
        public FilterInfoCollection LlamaCamCollection;
        ZXing.Presentation.BarcodeReader LlamaBarcodeReader = new ZXing.Presentation.BarcodeReader();

        public MainWindow()
        {
            InitializeComponent();
            
            Loaded += MainWindow_Loaded;
            LlamaBarcodeReader.ResultFound += LlamaBarcodeReader_ResultFound;

        }

        private void LlamaBarcodeReader_ResultFound(Result obj)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                
                TextResults.Text = obj.Text;
            }));

        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (Bitmap)eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                    
                
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
               
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    LlamaFrame.Source = bi;
                    if(LlamaFrame.Source != null)
                    {
                        Result barcodeResult = LlamaBarcodeReader.Decode((BitmapSource)LlamaFrame.Source);
                        TextResults.Text = barcodeResult?.Text;

                    }
                    
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

        }

        private void TryDecode()
        {
            



        }



        private void StartCam(object sender, RoutedEventArgs e)
        {

            LlamaCam.Start();

        }

        private void StopCam(object sender, RoutedEventArgs e)
        {


            LlamaCam.Stop();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LlamaCamCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            LlamaCam = new VideoCaptureDevice(LlamaCamCollection[0].MonikerString);
            LlamaCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

            
        }
    }
}