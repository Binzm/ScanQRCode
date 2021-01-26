using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using ZXing;

namespace ScanQRCode
{
    public partial class CameraQR : Form
    {
        public string resultStr = "";
        private AForge.Video.DirectShow.FilterInfoCollection _videoDevices;//摄像设备
        System.Timers.Timer timer;
        CameraHelper _cameraHelper = new CameraHelper();//摄像操作类
        public CameraQR()
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            InitializeComponent();
            LoadTitleCenterData();
            CheckForIllegalCrossThreadCalls = false;//非主线程可以操作关闭窗体操作
            AddTimer();
        }
        /// <summary>
        /// 定时识别图片二维码
        /// </summary>
        private void AddTimer()
        {
            timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 200;
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler(PicToQRCode);
        }

        /// <summary>
        /// 识别图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicToQRCode(object sender, ElapsedEventArgs e)
        {
            if (_cameraHelper.img == null)
                return;
            BinaryBitmap bitmap = null;
            try
            {
                MemoryStream ms = new MemoryStream();
                _cameraHelper.img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] bt = ms.GetBuffer();
                ms.Close();
                LuminanceSource source = new RGBLuminanceSource(bt, _cameraHelper.img.Width, _cameraHelper.img.Height);
                bitmap = new BinaryBitmap(new ZXing.Common.HybridBinarizer(source));
            }
            catch (Exception ex)
            {
                return;
            }

            Result result=null;
            try
            {
                //开始解码
                result = new MultiFormatReader().decode(bitmap);
            }
            catch (ReaderException ex)
            {
                resultStr = ex.ToString();
            }
            if (result != null)
            {
                resultStr = result.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void LoadTitleCenterData()
        {
            string titleMsg = "扫码页面";
            Graphics g = this.CreateGraphics();
            Double startingPoint = (this.Width / 2) - (g.MeasureString(titleMsg, this.Font).Width / 2);
            Double widthOfASpace = g.MeasureString(" ", this.Font).Width;
            String tmp = " ";
            Double tmpWidth = 0;

            while ((tmpWidth + widthOfASpace) < startingPoint)
            {
                tmp += " ";
                tmpWidth += widthOfASpace;
            }
            this.Text = tmp + titleMsg;
        }

        private void CameraQR_Load(object sender, EventArgs e)
        {
            // 获取视频输入设备
            _videoDevices = _cameraHelper.CreateFilterInfoCollection();//获取拍照设备列表
            if (_videoDevices.Count == 0)
            {
                MessageBox.Show("无设备");
                this.Dispose();
                this.Close();
                return;
            }
            resultStr = "";//二维码识别字符串清空
            _cameraHelper.ConnectDevice(videoSourcePlayer1);//连接打开设备
        }

        /// <summary>
        /// 关闭窗口 释放定制器 关闭摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraQR_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer != null)
            {
                timer.Dispose();
            }
            _cameraHelper.CloseDevice();
        }
    }
}
