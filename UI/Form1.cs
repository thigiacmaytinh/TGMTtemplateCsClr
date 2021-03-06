﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGMTbridge;
using System.Threading;
using TGMTcs;
using System.IO;

namespace UI
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image file|*.jpg;*.png;*.bmp";
            ofd.ShowDialog();


            string filePath = ofd.FileName;
            pictureBox1.ImageLocation = filePath;

            pictureBox2.Image = CbridgeSample.LoadImage(filePath);

        }
    }
}
