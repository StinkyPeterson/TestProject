using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
    class Program
    {
        public static string ThreadText = "";
        public static System.Diagnostics.Stopwatch myStopWatch = new System.Diagnostics.Stopwatch();
        static object locker = new object();
        static Mutex mutexObj = new Mutex();
        static void Main(string[] args)
        {
            myStopWatch.Start();
            string path = @"Введите свой путь";
            MakeTriplet(path);
        }

        public static void StreamText(string text)
        { 
            ThreadText = text;
        }//получение доступа к переменной ThreadText

        public static void ReadText(object path)
        {
            using (StreamReader sr = new StreamReader((string)path, System.Text.Encoding.Default))
                StreamText(sr.ReadToEnd());
        }//чтение текста из файла

        public static void ReplaceString(object c)
        {
            StreamText(ThreadText.Replace((string)c, ""));
        }//удаление символов

        public static void CleanText()
        {
            string[] array = { ",", ".", "!", "?", "—", "_", "[", "]", "(", ")", ":", "\"", "'"};
            for (int i = 0; i < array.Length; i++)
                new Thread(new ParameterizedThreadStart(ReplaceString)).Start(array[i]); //создание потоков для очистки текста от лишних символов
            StreamText(ThreadText.Replace("-", " "));
            StreamText(Regex.Replace(ThreadText, @"\s{2,}", " ").Trim()); //метод, оставляет между словами 1 пробел
        }//очистка переменной от ненужных символов
        public static void MakeTriplet(string path)
        {
            Dictionary<string, int> triplets = new Dictionary<string, int>(); //создание словаря, в которые будет помещен триплет. Key - триплет, value - кол-во этого триплета в тексте
            ReadText(path);
            CleanText();
            string[] words = ThreadText.Split(" "); //разделение текста на слова
            for (int i = 0; i < words.Length; i++)
            {
                char[] chars = words[i].ToCharArray(); //разделение слов на символы
                for (int j = 0; j < chars.Length - 2; j++)
                {
                    string trip = chars[j].ToString() + chars[j + 1].ToString() + chars[j + 2].ToString(); //составление триплета
                    if (trip.Length > 2)
                    {
                        if (!triplets.ContainsKey(trip))
                            triplets.Add(trip, 1);
                        else
                            triplets[trip] += 1;
                    }//проверка, что триплет больше двух символов

                }//перебор слова на триплеты

            }//перебор слов
            Console.WriteLine("10 самых частовстречающихся триплетов: ");
            triplets = triplets.OrderByDescending(pair => pair.Value).Take(10).ToDictionary(pair => pair.Key, pair => pair.Value); //сортировка словаря в порядке возрастания значений, и выборка 10 триплетов словаря
            int k = 0;
            foreach (var triplet in triplets)
            {
                k++;
                if (k < triplets.Count)
                    Console.Write($"{triplet.Key}, ");
                else
                    Console.WriteLine($"{triplet.Key}. ");
            }
            myStopWatch.Stop();
            Console.WriteLine("");
            Console.WriteLine($"Время работы программы в милисекундах: {myStopWatch.ElapsedMilliseconds} м/c");
        }//создание и вывод триплетов
    }
}
