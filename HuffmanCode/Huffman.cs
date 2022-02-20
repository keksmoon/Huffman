using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuffmanCode {
    public static class HuffmanTree {
        private class TreeElem : IComparable<TreeElem> {
            #region Fields
            public TreeElem Left { get; private set; }
            public TreeElem Right { get; private set; }
            public int Weight { get; private set; }
            public char Symbol { get; private set; }
            #endregion

            #region Constructors
            public TreeElem(TreeElem left, TreeElem right) {
                Left = left;
                Right = right;
                Weight = Left.Weight + Right.Weight;
                Symbol = '\0';
            }

            public TreeElem(char symbol, int weight) {
                Symbol = symbol;
                Weight = weight;
                Left = Right = null;
            }
            #endregion

            #region Methods
            public int CompareTo(TreeElem other) {
                return Weight.CompareTo(other.Weight);
            }
            #endregion
        }

        private static TreeElem tree { get; set; }

        private static void Bypass(TreeElem Current, ref Dictionary<char, string> Codes, StringBuilder CurrentCode) {
            if (Current.Symbol != '\0') {
                Codes.Add(Current.Symbol, CurrentCode.ToString());
                return;
            }

            CurrentCode.Append('0');
            Bypass(Current.Left, ref Codes, CurrentCode);
            CurrentCode.Remove(CurrentCode.Length - 1, 1);

            CurrentCode.Append('1');
            Bypass(Current.Right, ref Codes, CurrentCode);
            CurrentCode.Remove(CurrentCode.Length - 1, 1);
        }

        public static Dictionary<char, string> GetCodesForEncoding(string input, ref Dictionary<char, int> Frequencies) {
            #region Объявление переменных
            // Frequencies     = new Dictionary<char, int>();
            var Tree = new List<TreeElem>();
            var Codes = new Dictionary<char, string>();
            #endregion

            #region Создание словаря с частотами символов
            foreach (var item in input)
                if (Frequencies.ContainsKey(item))
                    Frequencies[item]++;
                else
                    Frequencies.Add(item, 1);
            #endregion

            #region Перенос словаря в лист
            foreach (var item in Frequencies)
                Tree.Add(new TreeElem(item.Key, item.Value));
            #endregion

            #region Построение дерева
            Tree.Sort();


            if (Tree.Count == 1) {
                tree = Tree[0];
                Codes.Add(tree.Symbol, "1");
                return Codes;
            } else
            while (Tree.Count > 1) {
                var elem = new TreeElem(Tree[0], Tree[1]);
                Tree.RemoveRange(0, 2);
                var index = Tree.BinarySearch(elem);
                if (index < 0)
                    index = ~index;

                Tree.Insert(index, elem);
            }

            tree = Tree[0];
            #endregion

            #region Обход дерева
            Bypass(Tree[0], ref Codes, new StringBuilder(""));
            return Codes;
            #endregion
        }

        public static Dictionary<string, char> GetCodesForDecoding(Dictionary<char, string> Codes) {
            return Codes.ToDictionary(x => x.Value, x => x.Key);
        }

        public static string Code(Dictionary<char, string> Codes, string text) {
            GC.Collect();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
                sb.Append(Codes[text[i]]);

            return sb.ToString();
        }

        public static string Decode(Dictionary<string, char> Codes, string text) {
            GC.Collect();

            StringBuilder resultText = new StringBuilder();
            var temp = tree;

            for (int i = 0; i < text.Length; i++) {
                temp = text[i] == '0' ? temp.Left : temp.Right;
                if (temp.Symbol != '\0') {
                    resultText.Append(temp.Symbol);
                    temp = tree;
                }
            }

            return resultText.ToString();
        }
    }

        //static void Main(string[] args)
        //{
        //    string file = "anna-karenina.txt";
        //    var CodesForEncoding = Huffman.GetCodesForEncoding(File.ReadAllText(file, Encoding.GetEncoding(1251)));
        //    var CodesForDecoding = Huffman.GetCodesForDecoding(CodesForEncoding);

        //    //кодирование:   каждый символ заменить на соответствующий код
        //    //декодирование: при нахождении кода в словаре заменить его на символ
        //    Console.ReadKey();
        //}
}
