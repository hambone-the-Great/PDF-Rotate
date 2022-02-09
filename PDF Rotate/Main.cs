﻿using System;
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
using SchuffSharp.Files;
using SchuffSharp.StringExtender;

namespace PDF_Rotate
{
    public partial class Main : Form
    {
        private static readonly string InstallDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"PDF_Rotate");
        private static readonly string TempDir = Path.Combine(AppDataDir, @"Temp");
        private static readonly string ResourceDir = Path.Combine(AppDataDir, @"Resources");
        private static readonly string HtmlDir = Path.Combine(ResourceDir, @"html"); 
        private string TempFile { get; set; }

        public Main(string filePath = null)
        {
            InitializeComponent();

            if (!Directory.Exists(AppDataDir)) Directory.CreateDirectory(AppDataDir);
            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);
            if (!Directory.Exists(ResourceDir)) Directory.CreateDirectory(ResourceDir);
            if (!Directory.Exists(HtmlDir)) Directory.CreateDirectory(HtmlDir);

            CopyResourcesToAppData(); 

            if (filePath != null)
            {

            }

        }

        private void CopyResourcesToAppData()
        {
            
            string HtmlInstallDir = Path.Combine(InstallDir, @"html");
            FileExt.CopyDirectory(HtmlInstallDir, HtmlDir, true);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            webBrowser1.BringToFront(); 
            NavigateToLocalResource(@"welcome.htm");
        }

        private void NavigateToLocalResource(string path)
        {
            string rawUrl = Path.Combine(HtmlDir, path);
            //Uri url = new Uri(rawUrl);
            //webBrowser1.Url = url;
            webBrowser1.Navigate(rawUrl);
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
                    TempFile = Path.Combine(TempDir.ToString(), RandomGear.GenerateRandomString(4) + droppedFile.Extension);
                    File.Copy(droppedFile.FullName, TempFile);
                    Application.DoEvents();                     
                }
            }
            
        }


        private void RotatePDF(int degrees)
        {
                      
            if (TempFile != string.Empty)
            {
                var info = new FileInfo(HttpUtility.UrlDecode(TempFile));

                if (info.Extension == ".pdf")
                {

                    var doc = PdfReader.Open(TempFile);

                    foreach (PdfPage page in doc.Pages)
                    {
                        page.Rotate = (page.Rotate + degrees) % 360;
                    }

                    var newName = Path.Combine(TempDir, "rotated_" + info.Name);

                    doc.Save(newName);

                    Application.DoEvents(); //give time to save. 

                    webBrowser1.Navigate(newName); 

                    //var url = new Uri(newName);

                    //webBrowser1.Url = url;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            //webBrowser1.ShowSaveAsDialog();



            MessageBox.Show("File Saved Successfully.");

            NavigateToLocalResource(@"Welcome.htm");

        }

        private void CleanTempDir()
        {
            var files = Directory.GetFiles(TempDir, "*.*", SearchOption.AllDirectories);

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

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanTempDir();
        }
    }
}
