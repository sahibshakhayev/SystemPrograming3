namespace HW3
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cts;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.Title = "Select a file to encrypt";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            textBox1.Text = openFileDialog1.FileName;
        }


        
        
        
        
        
        
        
        
        
        
        
        
        private void EncryptFile(string fileName, int password, CancellationToken token)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                long totalBytesRead = 0;
                long fileSize = stream.Length;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    for (int i = 0; i < bytesRead; i++)
                    {
                        buffer[i] = (byte)(buffer[i] ^ password);
                    }

                    stream.Position -= bytesRead;
                    stream.Write(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    int percentComplete = (int)(totalBytesRead * 100 / fileSize);
                    progressBar1.Value = percentComplete;
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            
           
                if (cts != null)
                {
                    cts.Cancel();
                    cts = null;
                    return;
                }



            string fileName = textBox1.Text;
            if (fileName == "")
            {
                return;
            }

                int password;
                if (!int.TryParse(textBox2.Text, out password))
                {
                    MessageBox.Show("Please enter a valid password.");
                    return;
                }

            button2.Enabled = false;
            button3.Enabled = true;
            progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Value = 0;

                cts = new CancellationTokenSource();
            try
            {
                await Task.Run(() => EncryptFile(fileName, password, cts.Token), cts.Token);
                MessageBox.Show("Operation complete.");
            }
            catch (OperationCanceledException)
            {
                DecryptFile(fileName);
                MessageBox.Show("Operation canceled.");
                progressBar1.Value = 0;
            }
            finally
            {
                button2.Enabled = true;
                button3.Enabled = false;
                cts = null;
            }

            cts = null;
            }

        private void DecryptFile(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                long totalBytesRead = 0;
                long fileSize = stream.Length;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        buffer[i] = (byte)(buffer[i] ^ 0xff);
                    }

                    stream.Position -= bytesRead;
                    stream.Write(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    int percentComplete = (int)(totalBytesRead * 100 / fileSize);
                    progressBar1.Value = percentComplete;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }
    }
    }
