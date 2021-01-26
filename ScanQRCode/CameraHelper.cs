using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ScanQRCode
{
    public class CameraHelper
    {
        public FilterInfoCollection _videoDevices;//本机摄像硬件设备列表
        public VideoSourcePlayer _videoSourcePlayer;//视频画布
        public Bitmap img = null;//全局变量，保存每一次捕获的图像
        public System.Drawing.Image CaptureImage(VideoSourcePlayer sourcePlayer = null)
        {

            if (sourcePlayer == null || sourcePlayer.VideoSource == null)
            {
                if (_videoSourcePlayer == null)
                    return null;
                else
                {
                    sourcePlayer = _videoSourcePlayer;
                }
            }

            try
            {
                if (sourcePlayer.IsRunning)
                {
                    System.Drawing.Image bitmap = sourcePlayer.GetCurrentVideoFrame();
                    return bitmap;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// 截取一帧图像并保存
        /// </summary>
        /// <param name="filePath">图像保存路径</param>
        /// <param name="fileName">保存的图像文件名</param>
        /// <returns>如果保存成功，则返回完整路径，否则为 null</returns>
        public string CaptureImage(string filePath, string fileName = null, VideoSourcePlayer sourcePlayer = null)
        {
            if (sourcePlayer == null || sourcePlayer.VideoSource == null)
            {
                if (_videoSourcePlayer == null)
                    return null;
                else
                {
                    sourcePlayer = _videoSourcePlayer;
                }
            }

            ;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            try
            {
                if (sourcePlayer.IsRunning)
                {
                    System.Drawing.Image bitmap = sourcePlayer.GetCurrentVideoFrame();
                    if (fileName == null) fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                    string fullPath = Path.Combine(filePath, fileName + ".jpg");
                    bitmap.Save(fullPath, ImageFormat.Jpeg);
                    bitmap.Dispose();
                    return fullPath;
                }
                return null;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public FilterInfoCollection CreateFilterInfoCollection()
        {
            if (_videoDevices != null)
                return _videoDevices;
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return _videoDevices;
        }

        public VideoCaptureDevice ConnectDevice(VideoSourcePlayer videoSourcePlayer, FilterInfo filterInfo = null)
        {
            VideoCaptureDevice videoSource = new VideoCaptureDevice();
            if (filterInfo == null)
            {
                videoSource = new VideoCaptureDevice(_videoDevices[_videoDevices.Count - 1].MonikerString);
            }
            else
            {
                videoSource = new VideoCaptureDevice(filterInfo.MonikerString);
            }

            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSourcePlayer.VideoSource = videoSource;
            videoSourcePlayer.Start();
            _videoSourcePlayer = videoSourcePlayer;
            return videoSource;
        }


        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            img = (Bitmap)eventArgs.Frame.Clone();
        }

        public void CloseDevice(VideoSourcePlayer videoSourcePlayer = null)
        {
            if (videoSourcePlayer == null)
            {
                if (_videoSourcePlayer == null)
                    return;
                _videoSourcePlayer.SignalToStop();
                //_videoSourcePlayer.WaitForStop();
            }
            else
            {
                videoSourcePlayer.SignalToStop();
                //videoSourcePlayer.WaitForStop();
            }
        }
    }
}
