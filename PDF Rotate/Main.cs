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
            FileInfo file = new FileInfo(files[0]);

            if (file.Extension == ".htm" || file.Extension == ".pdf")
            {
                Uri url = new Uri(file.FullName);
                webBrowser1.Url = url;
            }

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
                FileInfo file = new FileInfo(diag.FileName);

                if (file.Extension == ".htm" || file.Extension == ".pdf")
                {
                    Uri url = new Uri(file.FullName);
                    webBrowser1.Url = url; 
                }                
            }

        }
    }
}
