using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

using HuffmanCode;
using System;
using System.Threading;
using System.Diagnostics;

namespace Huffman {
    public class DictionaryByValueComparer : IComparer<KeyValuePair<char, int>> {
        public int Compare(KeyValuePair<char, int> x, KeyValuePair<char, int> y) {
            return x.Value.CompareTo(y.Value);
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private Dictionary<string, char> CodesForDecoding;

        /// <summary>
        /// Чтение текста из файла. Его анализ. 
        /// </summary>
        private void OpenText_Button(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            textToAnalyze.Clear();
            textInCoding.Clear();
            frequencyDictionaryListBox.Items.Clear();

            if (openFileDialog.ShowDialog() == true) {
                textToAnalyze.IsReadOnly = true;
                textInCoding.IsReadOnly = true;
                string path = openFileDialog.FileName;
                var Frequencies = new Dictionary<char, int>();

                var textString = string.Empty;
                var CodesForEncoding = HuffmanTree.GetCodesForEncoding(textString = File.ReadAllText(path, Encoding.GetEncoding("utf-8")), ref Frequencies);
                CodesForDecoding = HuffmanTree.GetCodesForDecoding(CodesForEncoding);

                Frequencies = Frequencies.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

                foreach (var CharCode in Frequencies) {
                    string visualChar = (CharCode.Key == '\n') ? "\\n" :
                                        (CharCode.Key == '\r') ? "\\r" :
                                        (CharCode.Key == '\t') ? "\\t" :
                                        (CharCode.Key == ' ') ? "Space" :
                                        (CharCode.Key.ToString());
                    int freqChar = CharCode.Value;
                    string codeChar = CodesForEncoding[CharCode.Key];

                    frequencyDictionaryListBox.Items.Add($"{visualChar}\t{freqChar}\t{codeChar}");
                }

                textToAnalyze.Text = textString;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                string result = HuffmanTree.Code(CodesForEncoding, textString);

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                MessageBox.Show($"Text has been encoded! \r\n{ts.Milliseconds}ms");
                textInCoding.Text = result;
            }
            else {
                textToAnalyze.IsReadOnly = false;
                textInCoding.IsReadOnly = false;
            }
        }

        /// <summary>
        /// Кодирование текста
        /// </summary>
        private void CodeText_Button(object sender, RoutedEventArgs e) {
            if (textToAnalyze.IsReadOnly == true) {
                // Следовательно текст уже закодирован при помощи файла
                MessageBox.Show("The text is already encoded! Use the cleaning method.");
                return;
            }

            //Алгоритм Кодирования Текста
            textToAnalyze.IsReadOnly = true;
            textInCoding.IsReadOnly = true;

            var Frequencies = new Dictionary<char, int>();

            var textString = textToAnalyze.Text;

            if (string.IsNullOrEmpty(textString)) {
                // Текст для кодирования отсутствует
                MessageBox.Show("Please enter the text to encode.");
                textToAnalyze.IsReadOnly = false;
                textInCoding.IsReadOnly = false;
                return;
            }

            var CodesForEncoding = HuffmanTree.GetCodesForEncoding(textString, ref Frequencies);
            CodesForDecoding = HuffmanTree.GetCodesForDecoding(CodesForEncoding);

            foreach (var CharCode in Frequencies) {
                string visualChar = (CharCode.Key == '\n') ? "\\n" :
                                    (CharCode.Key == '\r') ? "\\r" :
                                    (CharCode.Key == '\t') ? "\\t" :
                                    (CharCode.Key == ' ') ? "Space" :
                                    (CharCode.Key.ToString());
                int freqChar = CharCode.Value;
                string codeChar = CodesForEncoding[CharCode.Key];

                frequencyDictionaryListBox.Items.Add($"{visualChar}\t{freqChar}\t{codeChar}");
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = HuffmanTree.Code(CodesForEncoding, textString);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            MessageBox.Show($"Text has been encoded! \r\n{ts.Milliseconds}ms");
            GC.Collect();
            textInCoding.Clear();
            textInCoding.Text = result;
        }

        /// <summary>
        /// Декодирование текста
        /// </summary>
        private void DeCodeText_Button(object sender, RoutedEventArgs e) {
            //Алгоритм ДеКодирования Текста
            if (string.IsNullOrEmpty(textInCoding.Text)) {
                // Текст для декодирования отсутствует
                MessageBox.Show("The text hasn't been encoded yet!");
                textToAnalyze.IsReadOnly = false;
                textInCoding.IsReadOnly = false;
                return;
            }

            string textToDecode = textInCoding.Text;
            textToAnalyze.Clear();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = HuffmanTree.Decode(CodesForDecoding, textToDecode);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            MessageBox.Show($"Text has been decoded! \r\n{ts.Milliseconds}ms");
            GC.Collect();
            textToAnalyze.Text = result;
        }

        /// <summary>
        /// Метод, позволяющий перемещать окно программы при нажатии ЛКМ на панель меню.
        /// </summary>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) 
                DragMove();
        }

        private void ClearAll_Button(object sender, RoutedEventArgs e) {
            GC.Collect();

            textInCoding.Clear();
            textToAnalyze.Clear();
            frequencyDictionaryListBox.Items.Clear();

            textToAnalyze.IsReadOnly = false;
            textInCoding.IsReadOnly = false;
        }

        #region Кнопки управления окном программы
        // Закрытия программы (красная кнопка)
        private void CloseAppClick(object sender, RoutedEventArgs e) => Close();

        // Открытие программы во весь экран (зеленая кнопка)
        private void MaximizeFrame(object sender, RoutedEventArgs e) {
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized;
        }

        // Возвращение программы в нормальное положение (желтая кнопка)
        private void MinimizeFrame(object sender, RoutedEventArgs e) {
            this.ResizeMode = ResizeMode.CanResizeWithGrip;
            this.WindowState = WindowState.Normal;
        }
        #endregion
    }
}
