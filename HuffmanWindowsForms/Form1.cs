using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HuffmanCode;

namespace HuffmanWindowsForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            richTextBox1.Clear();
            richTextBox2.Clear();

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                string path = openFileDialog.FileName;
                var Frequencies = new Dictionary<char, int>();

                var textString = string.Empty;
                var CodesForEncoding = HuffmanTree.GetCodesForEncoding(textString = File.ReadAllText(path, Encoding.GetEncoding("utf-8")), ref Frequencies);
                var CodesForDecoding = HuffmanTree.GetCodesForDecoding(CodesForEncoding);

                Frequencies = Frequencies.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

                foreach (var CharCode in Frequencies) {
                    string visualChar = (CharCode.Key == '\n') ? "\\n" :
                                        (CharCode.Key == '\r') ? "\\r" :
                                        (CharCode.Key == '\t') ? "\\t" :
                                        (CharCode.Key == ' ') ? "Space" :
                                        (CharCode.Key.ToString());
                    int freqChar = CharCode.Value;
                    string codeChar = CodesForEncoding[CharCode.Key];

                    listBox1.Items.Add($"{visualChar}\t{freqChar}\t{codeChar}");
                }

                richTextBox1.Text = textString;

                string result = HuffmanTree.Code(CodesForEncoding, textString);
                richTextBox2.Text = result;
            }
            else {
                //textBox1.IsReadOnly = false;
                //textBox2.IsReadOnly = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            richTextBox1.Text = HuffmanTree.Decode(new Dictionary<string, char>(), richTextBox2.Text);
        }
    }
}
