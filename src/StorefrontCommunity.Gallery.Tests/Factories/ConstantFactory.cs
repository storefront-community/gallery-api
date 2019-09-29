using System;
using System.Collections.Generic;

namespace StorefrontCommunity.Gallery.Tests.Factories
{
    public sealed class ConstantFactory
    {
        private static int id = 0;

        public static int Id => ++id;

        public static string Text(int length = 20, int wordCount = 3)
        {
            var words = new List<string>();

            var spaces = wordCount - 1;
            var wordLength = length / wordCount - spaces;

            while (words.Count < wordCount)
            {
                var word = Guid.NewGuid().ToString("N");
                words.Add(word.Substring(0, wordLength));
            }

            return string.Join(" ", words);
        }
    }
}
