using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InCrypt
{
    public static class Scrambler
    {
        private static Random m_Rand = new Random();
        private static char[] m_ValidChars = { 'A', 'a', 'E', 'e', 'I', 'i', 'L', 'l', 'O', 'o', 'S', 's', 'T', 't' };

        public static string Scramble(string text)
        {
            string value = "";

            value = ConvertTextToLeet(text);
            value = AddRandomSymbols(value);

            return value;
        }

        private static string ConvertTextToLeet(string text)
        {
            string leet = "";

            foreach (char c in text)
            {
                if (IsConvertible(c))
                    leet += ConvertCharToLeet(c);
                else
                {
                    if (c != ' ')
                        leet += c;
                }
            }

            return leet;
        }

        private static bool IsConvertible(char letter)
        {
            foreach (char c in m_ValidChars)
            {
                if (c == letter)
                    return true;
            }

            return false;
        }

        private static string ConvertCharToLeet(char letter)
        {
            string value = "";

            if (letter == 'A')
                value = "4";
            else if (letter == 'a')
                value = "@";
            else if (letter == 'E' || letter == 'e')
                value = "3";
            else if (letter == 'I' || letter == 'i')
                value = "1";
            else if (letter == 'L' || letter == 'l')
                value = "1";
            else if (letter == 'O' || letter == 'o')
                value = "0";
            else if (letter == 'S')
                value = "$";
            else if (letter == 's')
                value = "5";
            else if (letter == 'T' || letter == 't')
                value = "7";

            return value;
        }

        private static string AddRandomSymbols(string text)
        {
            string value = text;

            int numberOfSymbols = text.Length / 5;
            List<int> alreadyDone = new List<int>();
            int rand = 0;

            while (numberOfSymbols > 0)
            {
                rand = m_Rand.Next(0, text.Length);

                bool exists = false;

                for (int i = 0; i < alreadyDone.Count; i++)
                {
                    if (rand == alreadyDone[i])
                        exists = true;
                }

                if (exists)
                    continue;

                value = AddRandomSymbol(value, rand, CreateRandomSymbol());
                alreadyDone.Add(rand);
                numberOfSymbols--;
            }

            return value;
        }

        private static string CreateRandomSymbol()
        {
            int rand = m_Rand.Next(0, 8);
            string value = "";

            switch (rand)
            {
                case 0:
                    value = "!";
                    break;
                case 1:
                    value = "@";
                    break;
                case 2:
                    value = "#";
                    break;
                case 3:
                    value = "$";
                    break;
                case 4:
                    value = "%";
                    break;
                case 5:
                    value = "^";
                    break;
                case 6:
                    value = "&";
                    break;
                case 7:
                    value = "*";
                    break;
            }

            return value;
        }

        private static string AddRandomSymbol(string text, int index, string symbol)
        {
            string value = "";

            for (int i = 0; i < text.Length; i++)
            {
                if (i == index)
                    value += symbol;
                else
                    value += text[i];
            }

            return value;
        }
    }
}
