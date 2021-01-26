using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScanQRCode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.MinimizeBox = false; 
            this.MaximizeBox = false; 
            InitializeComponent();
            LoadTitleCenterData();
        }

        private void LoadTitleCenterData()
        {
            string titleMsg ="二维码识别主界面";
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

        private void button1_Click(object sender, EventArgs e)
        {
            CameraQR camera = new CameraQR();
            var result = camera.ShowDialog();
            if (result == DialogResult.OK)
            {
                label2.Text = camera.resultStr;
            }
            
        }
    }
}
