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
using System.Resources;
using System.Runtime.InteropServices;
using Ionic.Zip;
using System.Net;
using System.Threading;

namespace Encryption_algo
{
    public partial class Form1 : Form
    {
        #region Random functions
        // Generates a random integer for password
        private int RandomInt(int lenght)
        {

            int tmp = 0;
            Random R = new Random();
            tmp = R.Next(0, lenght);
            return tmp;

        }
        // Never used. Use it in other projects if you want too. It generates a ranodom string
        private string RandomString(int length)
        {
            string pool = "abcdefghijklmnopqrstuvwxyz!#¤%&/()=?}][{€$£@";
            pool += pool.ToUpper();
            string tmp = "";
            Random R = new Random();
            for (int x = 0; x < length; x++)
            {
                tmp += pool[R.Next(0, pool.Length)].ToString();
            }
            return tmp;

        }

        #endregion

        #region Form1 Load
        private void Form1_Load(object sender, EventArgs e)
        {
            // This allows drag and drop
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            /*WebClient client = new WebClient();
            client.DownloadFile("http://download1642.mediafire.com/98p5j3neg8ag/266m5mmg8tmmv9q/DotNetZip.dll", Environment.SpecialFolder.LocalApplicationData + @"\CobraEncryptor" + @"\DotNetZip.dll");
            */
        }

        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region Drag and drop
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            //Adds the dropped file into the list
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (!listBox1.Items.Contains(file))
                {
                    listBox1.Items.Add(file);
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        #endregion

        #region Checks delete keypress
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //If something is selected in the listbox and you press delete it will delete that string
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(listBox1);
                selectedItems = listBox1.SelectedItems;
                e.Handled = true;
                if (listBox1.SelectedIndex != -1)
                {
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                    {
                        listBox1.Items.Remove(selectedItems[i]);
                    }
                    e.Handled = true;
                }
                else if (listBox1.SelectedIndex != 0)
                {
                }

                e.Handled = true;
            }
        }
        #endregion

        #region All Buttons
        private void button3_Click(object sender, EventArgs e)
        {
            bool isfileordir = false;
            foreach (var item in listBox1.Items)
            {
                FileAttributes fileatt = File.GetAttributes(item.ToString());

                if (fileatt.HasFlag(FileAttributes.Directory))
                {
                    isfileordir = true;
                }
                else if (fileatt.HasFlag(FileAttributes.Normal))
                {
                    isfileordir = false;
                }
            }

            //Makes string of the textbox1 password
            string key = textBox1.Text;

            if (Properties.Settings.Default.Key != null)
            {
                DialogResult dresult = MessageBox.Show("Do you really want to overwrite your old decryption key?", "Overwrite?", MessageBoxButtons.YesNoCancel);

                if (dresult == DialogResult.Yes)
                {
                    Properties.Settings.Default.Key = textBox1.Text;
                    Properties.Settings.Default.Salt = RandomString(100);
                    Properties.Settings.Default.Save();

                    string text = "";
                    foreach (var item in listBox1.Items)
                    {
                        text += item.ToString() + Environment.NewLine; // /n to print each item on new line or you omit /n to print text on same line
                    }

                    Properties.Settings.Default.Files = text;
                    Properties.Settings.Default.Save();

                    goto lol;
                }
                else if (dresult == DialogResult.No)
                {
                    Properties.Settings.Default.Salt = RandomString(100);
                    Properties.Settings.Default.Save();
                    goto lmao;
                }
                else if (dresult == DialogResult.Cancel)
                {
                    return;
                }
            }
            

            DialogResult result = MessageBox.Show("Do you want to save this key for decryption?", "Save?", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                
                if (textBox1.Text.Length == 100)
                {
                    Properties.Settings.Default.Key = textBox1.Text;
                    Properties.Settings.Default.Salt = RandomString(100);
                    Properties.Settings.Default.Save();

                    FolderBrowserDialog savee = new FolderBrowserDialog();
                    if (savee.ShowDialog() == DialogResult.OK)
                    {

                        string text = "";
                        foreach (var item in listBox1.Items)
                        {
                            text += item.ToString() + Environment.NewLine; // /n to print each item on new line or you omit /n to print text on same line
                        }

                        Properties.Settings.Default.Files = text;
                        Properties.Settings.Default.Save();
                        int i = 0;

                        if (File.Exists(savee.SelectedPath.ToString() + @"\Encrypted Files" + @"\Encrypted File 1.ENCRYPTED"))
                        {
                            i = 1;
                        }


                        // Creates the folder named "Encrypted Files"
                        Directory.CreateDirectory(savee.SelectedPath.ToString() + @"\Encrypted Files");
                        foreach (var fileName in listBox1.Items)
                        {
                            string item = fileName.ToString();

                            i++;
                            // Encrypts every file in the listbox
                            Encrypt.EncryptFile(item, savee.SelectedPath.ToString() + @"\Encrypted Files" + @"\Encrypted File " + i.ToString() + ".ENCRYPTED", key, Properties.Settings.Default.Salt);


                        }
                    }
                }

                return;
            }
            else if (result == DialogResult.No)
            {
                Properties.Settings.Default.Salt = RandomString(100);
                Properties.Settings.Default.Save();
                goto lmao;
            }
            else if (result == DialogResult.Cancel)
            {
                return;
            }
            lmao:
            // New folderbrowser so we know where the user wants to save the folder with files
            FolderBrowserDialog save = new FolderBrowserDialog();
            if (textBox1.Text.Length == 100)
            {
                if (save.ShowDialog() == DialogResult.OK)
                {
                    int i = 0;

                    if (File.Exists(save.SelectedPath.ToString() + @"\Encrypted Files" + @"\Encrypted File 1.ENCRYPTED"))
                    {
                        i = 1;
                    }


                    // Creates the folder named "Encrypted Files"
                    Directory.CreateDirectory(save.SelectedPath.ToString() + @"\Encrypted Files");
                    foreach (var fileName in listBox1.Items)
                    {
                        string item = fileName.ToString();

                        i++;
                        // Encrypts every file in the listbox
                        Encrypt.EncryptFile(item, save.SelectedPath.ToString() + @"\Encrypted Files" + @"\Encrypted File " + i.ToString() + ".ENCRYPTED", key, Properties.Settings.Default.Salt);


                    }
                }
            }
            else if (textBox1.Text.Length != 100)
            {
                MessageBox.Show("Please press the Generate Password button again.", "Error", MessageBoxButtons.OK);
            }

            lol:
            FolderBrowserDialog saveee = new FolderBrowserDialog();
            if (textBox1.Text.Length == 100)
            {
                if (saveee.ShowDialog() == DialogResult.OK)
                {
                    int i = 0;

                    Directory.CreateDirectory(saveee.SelectedPath.ToString() + @"\Encrypted Files");

                    if (File.Exists(saveee.SelectedPath.ToString() + @"\Encrypted Files" + @"\Encrypted File 1.ENCRYPTED"))
                    {
                        i = 1;
                    }

                    if (isfileordir == true)
                    {
                        foreach (var filename in listBox1.Items)
                        {
                            string item = filename.ToString();

                            string[] fileEntries = Directory.GetFiles(item);

                            foreach (string fileName in fileEntries)
                            {
                                string file = fileName.ToString();

                                i++;

                                Encrypt.EncryptFile(file, saveee.SelectedPath.ToString() + @"\Encrypted Files" + @"\Encrypted File " + i.ToString() + ".ENCRYPTED", key, Properties.Settings.Default.Salt);
                            }
                        }
                        return;
                    }

                    

                    // Creates the folder named "Encrypted Files"
                    Directory.CreateDirectory(saveee.SelectedPath.ToString() + @"\Encrypted Files");
                    foreach (var fileName in listBox1.Items)
                    {
                        string item = fileName.ToString();

                        i++;
                        // Encrypts every file in the listbox
                        Encrypt.EncryptFile(item, saveee.SelectedPath.ToString() + @"\Encrypted Files" + @"\Encrypted File " + i.ToString() + ".ENCRYPTED", key, Properties.Settings.Default.Salt);
                    }
                }
            }
            else if (textBox1.Text.Length != 100)
            {
                MessageBox.Show("Please press the Generate Password button again.", "Error", MessageBoxButtons.OK);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog save = new FolderBrowserDialog()
            {

            };
            try
            {

                bool isfileordir = false;
                foreach (var item in listBox1.Items)
                {
                    FileAttributes fileatt = File.GetAttributes(item.ToString());

                    if (fileatt.HasFlag(FileAttributes.Directory))
                    {
                        isfileordir = true;
                    }
                    else if (fileatt.HasFlag(FileAttributes.Normal))
                    {
                        isfileordir = false;
                    }
                }

                //Same as button3 except the encrypt.decryptfile
                string key = textBox1.Text;

                DialogResult result = MessageBox.Show("Do you want to use the decryption key that was saved? It was used to encrypt: " + Properties.Settings.Default.Files, "Use Decryption Key?", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    FolderBrowserDialog savee = new FolderBrowserDialog();

                    if (savee.ShowDialog() == DialogResult.OK)
                    {

                        int x = 0;

                    Directory.CreateDirectory(savee.SelectedPath.ToString() + @"\Decrypted Files");

                    if (File.Exists(savee.SelectedPath.ToString() + @"\Decrypted Files" + @"\Decrypted File 1.changethis"))
                    {
                        x = 1;
                    }

                    if (isfileordir == true)
                    {
                        foreach (var filename in listBox1.Items)
                        {
                            string item = filename.ToString();

                            string[] fileEntries = Directory.GetFiles(item);

                            foreach (string fileName in fileEntries)
                            {
                                string file = fileName.ToString();

                                x++;

                                Encrypt.DecryptFile(file, savee.SelectedPath.ToString() + @"\Decrypted Files" + @"\Decrypted File " + x.ToString() + ".changethis", Properties.Settings.Default.Key, Properties.Settings.Default.Salt);
                            }
                        }
                        return;
                    }


                    
                        int i = 0;

                        if (File.Exists(savee.SelectedPath.ToString() + @"\Decrypted Files" + @"\Decrypted File 1.changethis"))
                        {
                            i = 1;
                        }

                        Directory.CreateDirectory(savee.SelectedPath.ToString() + @"\Decrypted Files");
                        foreach (var fileName in listBox1.Items)
                        {
                            string item = fileName.ToString();

                            i++;
                            
                            Encrypt.DecryptFile(item, savee.SelectedPath.ToString() + @"\Decrypted Files" + @"\Decrypted File " + i.ToString() + ".changethis", Properties.Settings.Default.Key, Properties.Settings.Default.Salt);
                        }
                    }
                    return;
                }
                else if (result == DialogResult.No)
                {
                    goto lmao;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                lmao:

                /*FolderBrowserDialog save = new FolderBrowserDialog()
                {

                };*/
                if (save.ShowDialog() == DialogResult.OK)
                {
                    int i = 0;

                    if (File.Exists(save.SelectedPath.ToString() + @"\Decrypted Files" + @"\Decrypted File 1.changethis"))
                    {
                        i = 1;
                    }

                    Directory.CreateDirectory(save.SelectedPath.ToString() + @"\Decrypted Files");
                    foreach (var fileName in listBox1.Items)
                    {
                        string item = fileName.ToString();

                        i++;

                        if (Properties.Settings.Default.Salt != null)
                        {
                            Encrypt.DecryptFile(item, save.SelectedPath.ToString() + @"\Decrypted Files" + @"\Decrypted File " + i.ToString() + ".changethis", key, Properties.Settings.Default.Salt);
                        }
                        else if (Properties.Settings.Default.Salt == null)
                        {
                            MessageBox.Show("Something went wrong when decrypting. Salt does not exist. Please try again later", "Error", MessageBoxButtons.OK);                           
                        }

                    }
                }
            }
            catch
            {
                MessageBox.Show("Error: Key is not the right one.", "Error", MessageBoxButtons.OK);
                Directory.Delete(save.SelectedPath.ToString() + @"\Decrypted Files");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Sets random string everytime we press this button
            textBox1.Text = RandomString(100);
            //Properties.Settings.Default.Save();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Calculates the MIME from a string selected from the listbox
            

                if (listBox1.SelectedItems.Count == 0)
                {
                    label1.Text = "MIME: Error. No file selected.";
                    label1.Visible = true;                   
                }
                else if (listBox1.SelectedItems.Count > 0)
                {
                    ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(listBox1);
                    selectedItems = listBox1.SelectedItems;
                    string selecteditem = listBox1.SelectedItem.ToString();

                if (selecteditem.EndsWith(".ENCRYPTED"))
                    {
                        MessageBox.Show("This will not work with encrypted files because of the algorithm used.", "Error", MessageBoxButtons.OK);                        
                    }
                    else
                    foreach (var filename in listBox1.SelectedItems)
                    {
                        label1.Text = "MIME: " + GetMime.getMimeFromFile(filename.ToString());
                        label1.Visible = true;
                    }
                }
        }
        #endregion
    }
}
