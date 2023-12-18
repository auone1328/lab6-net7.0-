using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq.Expressions;
using System.Xml;

namespace lab6
{
    internal class Program
    {
        static string ends = "!?.";
        static string puncMarks = ",;:!?.";  

        static void ColorMistakeMessage(string message) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void ColorAnswerMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static int CheckInput(string message, string mistakeMessage, int flagNegative) //checking whether input is right
        {
            ///for integers 
            string buf;
            bool isConvert = false;
            int input = 0; 

            do
            {
                Console.WriteLine(message);
                buf = Console.ReadLine();
                isConvert = int.TryParse(buf, out input);
                if (isConvert && input < 0 && flagNegative == 1) //checking whether value is negative(if needed)
                {
                    isConvert = false;
                    ColorMistakeMessage("Данное значение не может быть отрицательным");
                }
                else
                {
                    if (!isConvert)
                    {
                        ColorMistakeMessage(mistakeMessage);
                    }
                }

            } while (!isConvert);

            return input;
        }


        static void DeleteMarks(ref string sentence) //function to delete the ,;:.!? having only words to count keywords
        {
            int markIndex;

            foreach (char s in puncMarks)
            {
                if (sentence.Contains(s))
                {
                    foreach (char elem in sentence)
                    {
                        if (elem == s)
                        {
                            sentence = sentence.Replace(s, ' ');
                        }
                    }
                }

            }
        }

        static bool IsKeyWord(string item)
        {
            string word;
            bool isKeyWord = false;

            DeleteMarks(ref item);

            try
            {
                StreamReader sr = new StreamReader("keywords.txt"); //open file with keywords
                word = sr.ReadLine();

                while (word != null)
                {
                    if (item.ToLower().Trim() == word)
                    {
                        isKeyWord = true;
                        break;
                    }
                    else
                    {
                        word = sr.ReadLine();
                    }
                }

                sr.Close();
            }
            catch (Exception e)
            {
                ColorMistakeMessage($"Возникло исключение: {e}");
            }

            return isKeyWord;
        }

        static bool TryStrCorrect(string input) 
        {
            bool TryFoundDoubledMarks(string input)
            {
                bool isDoubled = false; //for ordinary marks
                for (int i = 1; i < input.Length; i++)
                {
                    if (puncMarks.Contains(input[i]) && puncMarks.Contains(input[i - 1]))
                    {
                        isDoubled = true;
                        break;
                    }
                }

                //for space
                for (int i = 1; i < input.Length; i++)
                {
                    if (input[i] == ' ' && input[i - 1] == ' ')
                    { 
                        isDoubled = true;
                        break;
                    }
                }

                return isDoubled;
            }

            
            bool isCorrect = true;
            string[] words = input.Split(' ');


            if (input.Length == 0)
            {
                ColorMistakeMessage("Невозможно обработать строку: данная строка пуста, в ней невозможно найти нужный элемент.");
                isCorrect = false;
            }
            else
            {
                if (TryFoundDoubledMarks(input))
                {
                    ColorMistakeMessage("Невозможно обработать строку: строки, в которых содержаться повторения знаков препинания, как ',,', ';;', множеcтвенные пробелы и так далее, недопустимы.");
                    isCorrect = false;
                }
                else
                {
                    if (!ends.Contains(input[^1]))
                    {
                        ColorMistakeMessage("Невозможно обработать предложение: строка должна заканчиваться символами '.', '!' или '?'.");
                        isCorrect = false;
                    }
                    else 
                    {
                        if (!IsKeyWord(words[0]) && words[0][0] != words[0].ToUpper()[0])
                        {
                            ColorMistakeMessage("Если первое слово в вашем предложении не является ключевым словом, то оно должно начинаться с большой буквы.");
                            isCorrect = false;
                        }
                    }
                }
            }

            return isCorrect;
        }

        static string CheckInput(string message) 
        {
            string input;
            bool isCorrect;

            do 
            {
                isCorrect = true;
                Console.WriteLine(message);
                input = Console.ReadLine();
                isCorrect = TryStrCorrect(input);
            } while (!isCorrect);

            return input;
        }

        static int ChooseStandardSentences(string[] sentences) 
        {
            int count = 0;
            int sentenceChoice;

            Console.WriteLine("Выберите для обработки строку из приведённых: ");
            foreach (string s in sentences)
            {
                count++;
                Console.WriteLine(count.ToString() + ") " + s);
            }
            Console.WriteLine("0) Вернуться обратно.");

            do
            {
                sentenceChoice = CheckInput("Введите номер подходящего варианта: ", "Неправильный ввод: номер варианта может быть только целым числом, меньшим, чем количество номеров и большим, чем 0.", 0);
                if (sentenceChoice > sentences.Length || sentenceChoice < 0)
                {       
                    ColorMistakeMessage("Такого номера нет в числе стандартных вариантов! Попробуйте ещё раз.");
                }
            } while (sentenceChoice > sentences.Length || sentenceChoice < 0);

            return sentenceChoice;
        }

        static void CountKeyWords(string sentence, int actionType) //action type is variable, which can get values (like 1, 2), to find out, which number of menu was chosen, so we won't use TryStrCorrect twice without need
        {
            int CountUnique(string[] wordsArray, string substr) //count keywords in unique way
            {
                int countUniqueElem = 0;

                int index = Array.IndexOf(wordsArray, substr);
                while (index >= 0)
                {
                    index = Array.IndexOf(wordsArray, substr, index + 1);
                    countUniqueElem++;
                }

                return countUniqueElem;
            }

            void ConvertStrArrayToLower(ref string[] arr)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = arr[i].ToLower();
                }
            }

            bool isCorrect = true;
            if (actionType == 1) 
            {
                isCorrect = TryStrCorrect(sentence);
            }

            if (isCorrect) 
            {
                DeleteMarks(ref sentence);
                string[] words = sentence.Split(' '); //deleting marks and getting an array of words in string
                ConvertStrArrayToLower(ref words);

                string word;
                bool isUsed; //checks used words to avoid counting twice
                bool isFoundKeyWords = false; //checks whether if keywords are at least contained in string
                string[] used = new string[0];

                Console.WriteLine("Результат подсчёта:");
                foreach (string item in words)
                {
                    isUsed = false;
                    foreach (string s in used)
                    {
                        if (item == s)
                        {
                            isUsed = true;
                        }
                    }
                    if (IsKeyWord(item) && !used.Contains(item)) 
                    {
                        ColorAnswerMessage($"{item} - {CountUnique(words, item)}");
                        string[] newUsed = new string[used.Length + 1];
                        for (int i = 0; i < used.Length; i++) //update  an used array
                        {
                            newUsed[i] = used[i];
                        }
                        newUsed[^1] = item;
                        used = newUsed;
                        isFoundKeyWords = true;
                    }
                }
                if (!isFoundKeyWords) //if we have no keywords in string, we printing message about it
                {
                    ColorMistakeMessage("К сожалению, ключевые слова не найдены в данной строке.");
                }
            }          
        }

        static void Main(string[] args)
        {
            string[] standardSentences = new string[] {"Привет, user, static void поздороваться if привет, здравствуй else до свидания!",
                "Приветствую, пользователь, добро пожаловать в мою программу. Вы хотите найти квадратный корень из 2?",
                "if;; else, default   case:,,,;;case else.",
                "namespace.",
                ";;,.",
                ""
            };


            Console.WriteLine("Добро пожаловать в мастер поиска ключевых слов в строке.");
            int choice = -1;
            while (choice != 0) 
            {
                Console.WriteLine("Какую строку вы бы хотел обработать:\n" +
                "1 - Строку из стандартного набора.\n" +
                "2 - Вашу строку (строка вводится с клавиатуры).\n" +
                "0 - Выход");
                choice = CheckInput("Введите номер действия: ", "Неправильный ввод: номер действия может быть только целым числом!", 0);

                switch (choice) 
                {
                    case 0:
                        Console.WriteLine("До свидания!");
                        break;
                    case 1:
                        int sentenceChoice = -1;
                        while (sentenceChoice != 0)
                        {
                            sentenceChoice = ChooseStandardSentences(standardSentences);
                            if (sentenceChoice != 0) 
                            {
                                CountKeyWords(standardSentences[sentenceChoice - 1], 1);
                            }
                        }                   
                        break;
                    case 2:                       
                        CountKeyWords(CheckInput("Введите вашу строку (строка должна оканчиваться знаком препинания из списка '.!?', а также не должна иметь двойных знаков препинания, как ';;' и множественных пробелов):"), 2);
                        break;
                    default:
                        ColorMistakeMessage("Такого номера действия нет в меню! Выберите номер из приведённых.");
                        break;
                }
            }
            
        }
    }
}
