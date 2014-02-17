using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InCrypt
{
    public partial class Form1 : Form
    {
        public AESKey m_CurrentKey;
        private string m_KeyFilePath;
        private string m_PassFilePath;
        private string[] m_PassFilePaths;
        private string m_Password;
        private string m_Association;

        public Form1()
        {
            InitializeComponent();
            m_CurrentKey = new AESKey();
            m_CurrentKey.key = null;
            m_CurrentKey.iv = null;
            this.Text = "InCrypt";
        }

        #region Password Tab

        private void CreatePasswordButton_Click(object sender, EventArgs e)
        {
            string password = "";

            if (textBox1.Text != "")
            {
                password = Scrambler.Scramble(textBox1.Text);
                PasswordBox.Text = password;
            }
        }

        #endregion

        #region Load/Create Key

        private void LoadKey_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                using (FileStream fs = File.Open(m_KeyFilePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    m_CurrentKey.key = (byte[])formatter.Deserialize(fs);
                    m_CurrentKey.iv = (byte[])formatter.Deserialize(fs);
                }

                MessageBox.Show("Your Key has been loaded!", "Success!", MessageBoxButtons.OK);
            }
        }

        private void CreateKey_Click(object sender, EventArgs e)
        {
            m_CurrentKey = AES.CreateKey();

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                using (FileStream fs = File.Create(m_KeyFilePath, 2048, FileOptions.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, m_CurrentKey.key);
                    formatter.Serialize(fs, m_CurrentKey.iv);
                }

                MessageBox.Show("Your Key has been created and saved!", "Success!", MessageBoxButtons.OK);
            }
        }

        #endregion

        #region File Dialogs

        // Load Key
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (openFileDialog1.FileName.EndsWith(".enckey"))
            {
                m_KeyFilePath = openFileDialog1.FileName;
            }
        }

        // Load File
        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            if (openFileDialog2.FileName.EndsWith(".enc"))
            {
                m_PassFilePath = openFileDialog2.FileName;
            }
        }

        // Save Key
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (saveFileDialog1.FileName.EndsWith(".enckey"))
            {
                m_KeyFilePath = saveFileDialog1.FileName;
            }
        }

        // Save File
        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            if (saveFileDialog2.FileName.EndsWith(".enc"))
            {
                m_PassFilePath = saveFileDialog2.FileName;
            }
        }

        #endregion

        #region Encrypt Tab

        // Encrypt file
        private void EncryptButton_Click(object sender, EventArgs e)
        {
            bool valid = true;
            bool valid2 = true;

            if (textBox2.Text != "")
                m_Password = textBox2.Text;
            else
                valid = false;

            if (textBox3.Text != "")
                m_Association = textBox3.Text;
            else
                valid = false;

            if (m_CurrentKey.key == null || m_CurrentKey.iv == null)
            {
                valid = false;
                valid2 = false;
            }

            if (valid)
            {
                if (saveFileDialog2.ShowDialog(this) == DialogResult.OK)
                {
                    using (FileStream fs = File.Create(m_PassFilePath, 2048, FileOptions.None))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fs, AES.EncryptStringToBytes(m_Association, m_CurrentKey.key, m_CurrentKey.iv));
                        formatter.Serialize(fs, AES.EncryptStringToBytes(m_Password, m_CurrentKey.key, m_CurrentKey.iv));
                    }

                    MessageBox.Show("Your password has been successfully encrypted and saved!", "Success!", MessageBoxButtons.OK);
                }
            }
            else if (!valid && valid2)
                MessageBox.Show("Error: You must enter a Password and an Association!", "Error!", MessageBoxButtons.OK);
            else
                MessageBox.Show("Error: You must Create or Load a Key to Encrypt a file!", "Error!", MessageBoxButtons.OK);
        }

        #endregion

        #region Decrypt Tab
        // Load Encrypted File
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            bool valid = true;

            if (m_CurrentKey.key == null || m_CurrentKey.iv == null)
                valid = false;

            if (valid)
            {
                if (openFileDialog2.ShowDialog(this) == DialogResult.OK)
                {
                    m_PassFilePaths = openFileDialog2.FileNames;

                    foreach (string filepath in m_PassFilePaths)
                    {
                        using (FileStream fs = File.Open(filepath, FileMode.Open))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            m_Association = AES.DecryptStringFromBytes((byte[])formatter.Deserialize(fs), m_CurrentKey.key, m_CurrentKey.iv);
                            m_Password = AES.DecryptStringFromBytes((byte[])formatter.Deserialize(fs), m_CurrentKey.key, m_CurrentKey.iv);
                        }

                        listBox1.Items.Add(m_Association);
                        listBox2.Items.Add(m_Password);
                    }
                }
            }
            else
                MessageBox.Show("Error: You must Create or Load a Key to Decrypt a file!", "Error!", MessageBoxButtons.OK);
        }

        // Remove Encrypted File
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;

            if (index >= 0)
            {
                listBox1.Items.RemoveAt(index);
                listBox2.Items.RemoveAt(index);
            }
        }

        // Clear All
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
        }

        // Listbox selection
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex;
        }

        // Listbox selection
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
        }

        #endregion
    }
}
