using System.Diagnostics;
using System.Linq;

namespace MDPyramid
{
    internal static class Test<T> where T : IMethod, new()
    {
        private static readonly T instance = new T();

        public static void Parser()
        {
            instance.TestParser();
        }

        internal const string QuestionStr = "215\n192 124\n117 269 442\n218 836 347 235\n320 805 522 417 345\n229 601 728 835 133 124\n248 202 277 433 207 263 257\n359 464 504 528 516 716 871 182\n461 441 426 656 863 560 380 171 923\n381 348 573 533 448 632 387 176 975 449\n223 711 445 645 245 543 931 532 937 541 444\n330 131 333 928 376 733 017 778 839 168 197 197\n131 171 522 137 217 224 291 413 528 520 227 229 928\n223 626 034 683 839 052 627 310 713 999 629 817 410 121\n924 622 911 233 325 139 721 218 253 223 107 233 230 124 233";

        public static void Run(bool checkSequence = true)
        {
            int[] res;
            // Sample:
            res = instance.Run("1\n8 9\n1 5 9\n4 5 2 3");
            Debug.Assert(res.Sum() == 16);
            if (checkSequence) Debug.Assert(res.SequenceEqual(new[] { 1, 8, 5, 2 }));

            // Question:
            res = instance.Run(QuestionStr);
            //Debug.Print($"{ res.Sum() }: { res.ToStringAggregate() }.");
            // Tests based on PrimitiveMethod:
            Debug.Assert(res.Sum() == 8186);
            if (checkSequence) Debug.Assert(res.SequenceEqual(new[] { 215, 192, 269, 836, 805, 728, 433, 528, 863, 632, 931, 778, 413, 310, 253 }));
        }

        public static void All(bool checkSequence = true)
        {
            Parser();
            Run(checkSequence);
        }
    }
}
