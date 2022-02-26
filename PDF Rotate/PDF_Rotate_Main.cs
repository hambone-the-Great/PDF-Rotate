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
using SchuffSharp.Files;
using SchuffSharp.StringExtender;

namespace PDF_Rotate
{
    public partial class PDF_Rotate_Main : Form
    {
        private static readonly string InstallDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"PDF_Rotate");
        private static readonly string TempDir = Path.Combine(AppDataDir, @"Temp");
        private static readonly string ResourceDir = Path.Combine(AppDataDir, @"Resources");
        private static readonly string HtmlDir = Path.Combine(ResourceDir, @"html");
        private string TempFile { get; set; }
        private string BrowserFile { get; set; }

        private FileInfo OG_File_Info { get; set; }

        public PDF_Rotate_Main(string filePath = null)
        {
            InitializeComponent();

            if (!Directory.Exists(AppDataDir)) Directory.CreateDirectory(AppDataDir);
            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);
            if (!Directory.Exists(ResourceDir)) Directory.CreateDirectory(ResourceDir);
            if (!Directory.Exists(HtmlDir)) Directory.CreateDirectory(HtmlDir);

            CopyResourcesToAppData();
          
            if (filePath != null)
            {
                if (!File.Exists(filePath)) return;
                BrowserFile = filePath;
                OG_File_Info = new FileInfo(filePath); 
            }

        }


        private async Task InitializeAsync()
        {
            await webview.EnsureCoreWebView2Async(null);            
        }

        private async void Main_Load(object sender, EventArgs e)
        {

            await InitializeAsync();

            if (BrowserFile != null)
            {                
                if (BrowserFile != string.Empty) webview.Navigate(BrowserFile);
            }
            else
            {
                NavigateToLocalResource(@"welcome.htm");
            }
        }


        private void CopyResourcesToAppData()
        {
            string HtmlInstallDir = Path.Combine(InstallDir, @"html");
            FileExt.CopyDirectory(HtmlInstallDir, HtmlDir, true);
        }

        private void NavigateToLocalResource(string path)
        {
            string rawUrl = Path.Combine(HtmlDir, path);
            webview.Navigate(rawUrl);
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog diag = new OpenFileDialog();            
            var result = diag.ShowDialog(); 

            if (result == DialogResult.OK)
            {
                OG_File_Info = new FileInfo(diag.FileName);
                webview.Navigate(diag.FileName);
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


        private void WebBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {

            BrowserFile = HttpUtility.UrlDecode(e.Url.AbsolutePath);

            if (!BrowserFile.Contains("rotate"))
            {
                if (OG_File_Info == null) OG_File_Info = new FileInfo(BrowserFile);
            }

            var droppedFile = new FileInfo(BrowserFile);

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

            if (TempFile == null) return;
            if (TempFile == string.Empty) return; 
                
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

                Application.DoEvents();

                webview.Navigate(newName); 

            }
            
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            
            
            string filePath = HttpUtility.UrlDecode(webview.Source.AbsolutePath);

            string newPath = Path.Combine(OG_File_Info.DirectoryName, "Rotated_" + OG_File_Info.Name);

            File.Copy(filePath, newPath, true); 
            
            Application.DoEvents();             
            
            this.DialogResult = DialogResult.OK;

            this.Close();

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

        private void webview_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            
        }

        private void webview_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            BrowserFile = HttpUtility.UrlDecode(e.Uri.ToString());

            if (!BrowserFile.Contains("rotate"))
            {
                if (OG_File_Info == null) OG_File_Info = new FileInfo(BrowserFile);
            }

            BrowserFile = BrowserFile.Replace(@"file:///", @"");

            FileInfo droppedFile = new FileInfo(BrowserFile);

            if (File.Exists(droppedFile.FullName))
            {
                if (droppedFile.Extension == ".pdf")
                {
                    TempFile = Path.Combine(TempDir.ToString(), RandomGear.GenerateRandomString(4) + droppedFile.Extension);
                    File.Copy(droppedFile.FullName, TempFile);                    
                }
            }
        }
    }

    static class WebViewHelper
    {

        public static void Navigate(this Microsoft.Web.WebView2.WinForms.WebView2 webview, string path)
        {
            if (webview != null && webview.CoreWebView2 != null)
            {
                webview.CoreWebView2.Navigate(path);
            }
        }

    }

}
