﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

//Block the words array until it finishes reading not for every use

namespace tareas4.Ejercicio2
{
    class Program
    {
        private static readonly object _lockWords = new object();
        static void Main()
        {
            //If the execution is synchronous use this.
            Task<string[]> task = CreateWordArray(@"http://www.gutenberg.org/cache/epub/2000/pg2000.txt");
            string[] words = task.Result;
            
            //If it is asynchronous, just use await.
            //string[] words =  await CreateWordArray();

            #region ParallelTasks
            // Perform three tasks in parallel on the source array
            Parallel.Invoke(() =>
                             {
                                 Console.WriteLine("Begin first task...");
                                 GetLongestWord(words);
                             },  // close first Action

                             () =>
                             {
                                 Console.WriteLine("Begin second task...");
                                 GetMostCommonWords(words);
                             }, //close second Action

                             () =>
                             {
                                 Console.WriteLine("Begin third task...");
                                 GetCountForWord(words, "Quijote");
                             } //close third Action
                         ); //close parallel.invoke

            Console.WriteLine("Returned from Parallel.Invoke");
            #endregion

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        #region HelperMethods
        private static void GetCountForWord(string[] words, string term)
        {
            lock (_lockWords){
                var findWord = from word in words
                           where word.ToUpper().Contains(term.ToUpper())
                           select word;

                Console.WriteLine($@"Task 3 -- The word ""{term}"" occurs {findWord.Count()} times.");
            }
            
        }

        private static void GetMostCommonWords(string[] words)
        {
            lock(_lockWords){
                var frequencyOrder = from word in words
                                 where word.Length > 6
                                 group word by word into g
                                 orderby g.Count() descending
                                 select g.Key;

                var commonWords = frequencyOrder.Take(10);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Task 2 -- The most common words are:");
                foreach (var v in commonWords)
                {
                sb.AppendLine("  " + v);
                }
                Console.WriteLine(sb.ToString());
            }
            
        }

        private static string GetLongestWord(string[] words)
        {
            lock (_lockWords){
                var longestWord = (from w in words
                               orderby w.Length descending
                               select w).First();

                Console.WriteLine($"Task 1 -- The longest word is {longestWord}.");
                return longestWord;
            }
            
        }

        // An http request performed synchronously for simplicity.
        public static async Task<string[]> CreateWordArray(string uri)
        {
            Console.WriteLine($"Retrieving from {uri}");

            // Download the web page content asynchronously.
            using (HttpClient client = new HttpClient())
            {
                string content = await client.GetStringAsync(uri);

                // Separate string into an array of words, removing common punctuation.
                string[] words = content.Split(
                    new char[] { ' ', '\u000A', '\r', ',', '.', ';', ':', '-', '_', '/', '!', '?', '(', ')', '[', ']', '{', '}', '\"', '\'' },
                    StringSplitOptions.RemoveEmptyEntries);

                return words;
            }
        }
        #endregion
    }
}