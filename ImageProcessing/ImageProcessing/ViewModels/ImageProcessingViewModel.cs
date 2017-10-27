using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ImageProcessing.Helpers;
using ImageProcessing.Models;
using Microsoft.Win32;

namespace ImageProcessing.ViewModels
{
    public class ImageProcessingViewModel : ObservableObject
    {
        private bool isNegative;
        private bool isGray;

        private ImageProcessingModel imageModel;

        private Timer updateConstrastTimer = new Timer();
        private Timer updateBrightnessTimer = new Timer();

        public bool IsNegative
        {
            get { return isNegative; }
            set
            {
                isNegative = value;
                imageModel.SetNegative(isNegative);
                RedrawImage();
            }
        }

        public bool IsGray
        {
            get { return isGray; }
            set
            {
                isGray = value;
                imageModel.SetGray(isGray);
                RedrawImage();
            }
        }

        public int Contrast { get; set; } = 0;
        public int Brightness { get; set; } = 0;
        public BitmapImage ImageSource { get; set; }
        public BitmapImage Histogram { get; set; }
        public BitmapImage VerticalProjection { get; set; }
        public BitmapImage HorizontalProjection { get; set; }

        public ImageProcessingViewModel()
        {
            string path = @"E:\Downloads\eye.jpg";
            if(File.Exists(path))
            {
                imageModel = new ImageProcessingModel(path);
                RedrawImage();
            }

            updateBrightnessTimer = new Timer()
            {
                AutoReset = false,
                Interval = 500,
            };
            updateBrightnessTimer.Elapsed += delegate(object o, ElapsedEventArgs e)
            {
                imageModel.SetBrightness(Brightness);
                RedrawImage();
            };

            updateConstrastTimer = new Timer()
            {
                AutoReset = false,
                Interval = 500,
            };
            updateConstrastTimer.Elapsed += delegate (object o, ElapsedEventArgs e)
            {
                imageModel.SetConstrast(Contrast);
                RedrawImage();
            };
        }

        private void RedrawImage()
        {
            ImageSource = imageModel.ImageSource.ToBitmapImage();
            Histogram = imageModel.Histogram.ToBitmapImage();
            VerticalProjection = imageModel.VerticalProjection.ToBitmapImage();
            HorizontalProjection = imageModel.HorizontalProjection.ToBitmapImage();
            RaisePropertyChanged("ImageSource");
            RaisePropertyChanged("Histogram");
            RaisePropertyChanged("VerticalProjection");
            RaisePropertyChanged("HorizontalProjection");
        }

        public ICommand ImageSelect => new RelayCommand(() =>
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            if (dialog.FileName != null && File.Exists(dialog.FileName))
            {
                try
                {
                    imageModel = new ImageProcessingModel(dialog.FileName);
                    Contrast = 0;
                    Brightness = 1;
                    isGray = false;
                    isNegative = false;
                    RaisePropertyChanged("Contrast");
                    RaisePropertyChanged("Brightness");
                    RaisePropertyChanged("IsGray");
                    RaisePropertyChanged("IsNegative");
                    RedrawImage();

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        });

        public ICommand AdjustContrast => new RelayCommand(() =>
        {
            updateConstrastTimer.Stop();
            updateConstrastTimer.Start();
        });

        public ICommand AdjustBrightness => new RelayCommand(() =>
        {
            updateBrightnessTimer.Stop();
            updateBrightnessTimer.Start();
        });
    }
}
