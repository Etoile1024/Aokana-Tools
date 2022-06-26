using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            //openFile.DefaultExt = "mp4";
            openFile.Filter = "Video file (*.mp4)|*.mp4|Data file (*.dat)|*.dat";
            openFile.ShowDialog();
            textBox1.Text = openFile.FileName;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            byte[] header = new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32 };
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("路徑唔可以冇野！", "錯誤：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("搵唔到個file wor!", "錯誤：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox1.Text.EndsWith(".mp4"))
            {
                if (!Directory.Exists("output"))
                {
                    Directory.CreateDirectory("output");
                }
                button2.Enabled = false;
                byte[] buffer = File.ReadAllBytes(textBox1.Text);
                BoyerMoore boyerMoore = new BoyerMoore(header);
                //boyerMoore.SetPattern(header);
                var result = boyerMoore.SearchAll(buffer);
                progressBar1.Value = 0;
                progressBar1.Maximum = result.Count;
                for (int i = 0; i < result.Count; i++)
                {
                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
                    {
                        int length = i + 1 != result.Count ? result[i + 1] - result[i] : buffer.Length - result[i];
                        binaryReader.BaseStream.Position = result[i];
                        using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream($"output\\output_{i}.mp4", FileMode.Create)))
                        {
                            byte[] partofbuffer = binaryReader.ReadBytes(length);
                            binaryWriter.Write(partofbuffer);
                        }
                    }
                    progressBar1.Value++;
                }
                MessageBox.Show("拆完了！", "訊息：", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button2.Enabled = true;
            }
            if (textBox1.Text.EndsWith(".dat"))
            {
                button2.Enabled = false;
                progressBar1.Maximum = 0;
                PRead pRead = new PRead(textBox1.Text);
                progressBar1.Maximum = pRead.ti.Count;
                foreach (string filename in pRead.ti.Keys)
                {
                    string directory = Path.GetDirectoryName($"output\\{filename}");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream($"output\\{filename}", FileMode.Create)))
                    {
                        byte[] buffer = pRead.Data(filename);
                        binaryWriter.Write(buffer);
                    }
                    progressBar1.Value++;
                }
                MessageBox.Show("Unpacked!", "Info:", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button2.Enabled = true;
            }
        }
    }
}
