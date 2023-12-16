using Bogus;
using System;
using System.Text;

namespace IISLogLoader.Test.Common
{
    public class StringRandomizer
    {
        private static Random _random = new Random();

        private const string AlphaCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string Letters() 
        {
            return this.Letters(0, Int32.MaxValue);
        }

        public string Letters(int maxLength)
        {
            return this.Letters(0, maxLength);
        }

        public string Letters(int minLength, int maxLength)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetRandomLetters(minLength, 100));
            if (sb.Length > maxLength)
            {
                sb.Remove(maxLength, sb.Length - maxLength);
            }
            return sb.ToString();
        }

        public string Paragraph()
        {
            return Faker.Lorem.Paragraph();
        }

        public string Paragraph(int minSentenceCount)
        {
            return Faker.Lorem.Paragraph(minSentenceCount);
        }

        public string Word()
        {
            int i = Faker.RandomNumber.Next(0, 999);
            string[] words = Faker.Lorem.Words(1000).ToArray();
            return words[i];
        }


        private string GetRandomLetters(int min, int max)
        {
            int length = _random.Next(min, max);
            return new string(Enumerable.Repeat(AlphaCharacters, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
