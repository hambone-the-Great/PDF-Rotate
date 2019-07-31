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
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Web;
using SchuffSharp;

namespace PDF_Rotate
{
    public partial class Main : Form
    {

        string TempDir = string.Empty;
        string TempFile = string.Empty;

        public Main()
        {
            InitializeComponent();                                   
        }

        private void Main_Load(object sender, EventArgs e)
        {
            webBrowser1.BringToFront(); 
            NavigateToLocalResource(@"html\welcome.htm");
            TempDir = Path.Combine(Path.GetTempPath(), "PDF-Rotate");
            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);
            CleanTempDir();
        }

        private void NavigateToLocalResource(string path)
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
                Uri url = new Uri(diag.FileName);
                webBrowser1.Url = url;
            }

        }

        private void BtnRotateRight_Click(object sender, EventArgs e)
        {

            RotatePDF(90);

        }

        private void BtnRotateLeft_Click(object sender, EventArgs e)
        {

            RotatePDF(-90);

        }


        //private void PreviewFile(string path)
        //{
        //    FileInfo file = new FileInfo(path);

        //    if (file.Extension == ".htm" || file.Extension == ".pdf")
        //    {
        //        string tempFile = Path.Combine(Path.GetTempPath(), file.Name);
        //        File.Copy(file.FullName, tempFile);
        //        Application.DoEvents();
        //        Uri url = new Uri(tempFile);
        //        webBrowser1.Url = url; 
        //    }
        //}

        private void WebBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {

            string path = HttpUtility.UrlDecode(e.Url.AbsolutePath);
            var droppedFile = new FileInfo(path);

            if (File.Exists(droppedFile.FullName))
            {
                if (droppedFile.Extension == ".pdf")
                {                    
                    TempFile = Path.Combine(TempDir.ToString(), RandomGear.GenerateRandomString(8) + droppedFile.Extension);
                    File.Copy(droppedFile.FullName, TempFile);
                    Application.DoEvents();                     
                }
            }
            
        }


        private void RotatePDF(int degrees)
        {
            var info = new FileInfo(TempFile);

            if (TempFile != string.Empty)
            {
                var doc = PdfReader.Open(TempFile);

                foreach (PdfPage page in doc.Pages)
                {
                    page.Rotate = (page.Rotate + degrees) % 360;
                }

                var newName = Path.Combine(TempDir, "rotated_" + info.Name);

                doc.Save(newName);

                var url = new Uri(newName);

                webBrowser1.Url = url;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowSaveAsDialog();

        }

        private void CleanTempDir()
        {
            var files = Directory.GetFiles(TempDir);

            try
            {
                foreach (string file in files)
                {
                    if (File.Exists(file)) File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
