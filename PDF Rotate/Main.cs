using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace PDF_Rotate
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            webBrowser1.AllowWebBrowserDrop = false;
            
            
        }

        private void Main_Load(object sender, EventArgs e)
        {
            NavigateTo(@"html\welcome.htm");
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //do something with files: 
            PreviewFile(files[0]);          
        }

        private void NavigateTo(string path)
        {
            string rawUrl = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
            Uri url = new Uri(rawUrl);
            webBrowser1.Url = url;
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog diag = new OpenFileDialog();            
            var result = diag.ShowDialog(); 

            if (result == DialogResult.OK)
            {
                PreviewFile(diag.FileName);
            }

        }

        private void BtnRotateRight_Click(object sender, EventArgs e)
        {

        }

        private void BtnRotateLeft_Click(object sender, EventArgs e)
        {

        }


        private void PreviewFile(string path)
        {
            FileInfo file = new FileInfo(path);

            if (file.Extension == ".htm" || file.Extension == ".pdf")
            {
                string tempFile = Path.Combine(Path.GetTempPath(), file.Name);
                File.Copy(file.FullName, tempFile);
                Application.DoEvents();
                Uri url = new Uri(tempFile);
                webBrowser1.Url = url; 
            }
        }

        private void WebBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            
            //Can't do it this way because it would stop navigating as soon as PreviewFile is called. I think....

            //webBrowser1.Stop();
            //string url = webBrowser1.Url.ToString();
            //PreviewFile(url);

        }
    }
}
