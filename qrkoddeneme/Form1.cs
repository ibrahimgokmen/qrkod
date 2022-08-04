using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessagingToolkit.QRCode.Codec;
using ZXing;
using AForge.Video;
using AForge.Video.DirectShow;
using MessagingToolkit.QRCode.Codec.Data;


namespace qrkoddeneme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QRCodeEncoder encod = new QRCodeEncoder();  // qr kodu oluşturuyoruz.
            pictureBox1.Image = encod.Encode(richTextBox1.Text);
            if (pictureBox1.Image!=null)
            {
                button3.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {                 // bitmap'e çevirerek kodu çözdürüyoruz.
            QRCodeDecoder decod = new QRCodeDecoder();
            richTextBox2.Text = (decod.Decode(new QRCodeBitmapImage(pictureBox2.Image as Bitmap)));
        }

        FilterInfoCollection fico;   // bilgisayardaki kamerları combobox'a listeleyebilmek için.
        VideoCaptureDevice vcd;   // video yakalma aygıtı

        private void Form1_Load(object sender, EventArgs e)
        {
            fico = new FilterInfoCollection(FilterCategory.VideoInputDevice); // pc'ye bağlı kameraları getir.
            foreach (FilterInfo f in fico)  // fico dizisinden f adında değişken türettik.
            {
                comboBox1.Items.Add(f.Name);
                comboBox1.SelectedIndex = 0; // 0. index seçili gelsin. Zaten bir kamera oldupu için seçmekle uğraşmamk için.Harici kamera katarsan o başka
            }
        }

        private void btttnbaslat_Click(object sender, EventArgs e)
        {
            vcd = new VideoCaptureDevice(fico[comboBox1.SelectedIndex].MonikerString);  // seçilen kameraya vcd diye takma isim verdik
            vcd.NewFrame += Vcd_NewFrame;
            vcd.Start();
            timer1.Start();
            pictureBox3.Visible = true;

            if (pictureBox3 != null)
            {
                bttnkapat.Visible = true;
            }
        }

        private void Vcd_NewFrame(object sender, NewFrameEventArgs eventArgs)
        { // kameraya çerçeveyi tanıtcazve aktarcaz yukarda vcdnewframe ile tanıttığımız kod sayesinde
            pictureBox3.Image = (Bitmap)eventArgs.Frame.Clone();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                BarcodeReader brd = new BarcodeReader();
                Result sonuc = brd.Decode((Bitmap)pictureBox3.Image);
                if (sonuc != null)
                {
                    richTextBox3.Text = sonuc.ToString();
                    timer1.Stop();
                }
            }
        }

        private void bttnkapat_Click(object sender, EventArgs e)
        {
            if (vcd.IsRunning)
            {
                vcd.Stop();
                pictureBox3.Visible = false;
               
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "(*.jpg)|*.jpg";
            DialogResult dr = s.ShowDialog();
            if (dr == DialogResult.OK)
            {  // kaydettiği anda çekiyor
                pictureBox1.Image.Save(s.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            
            file.FilterIndex = 2;
            file.RestoreDirectory = true;
            file.CheckFileExists = false;
            file.Title = "qr Dosyasını Seçiniz..";
            file.Multiselect = true;
            // bu kod çoklu seçim yapabilmemizi sağlar.

            if (file.ShowDialog() == DialogResult.OK)
            {
                string DosyaYolu = file.FileName;
                string DosyaAdi = file.SafeFileName;
            }
            pictureBox2.ImageLocation = file.FileName;

            if (pictureBox2!=null)
            {
                button2.Visible = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {//Form kapatılırken kamera açıksa kapatıyoruz.
            if (pictureBox3.Image!=null)
            {
                if (vcd.IsRunning)
                {
                    vcd.Stop();
                    pictureBox3.Visible = false;
                }
            }

        }
    }
}


