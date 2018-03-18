using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MDPyramid
{
    /// <summary>
    /// This class should be the most reliable, since it is written in the most straightforward manner.
    /// </summary>
    /// <remarks>
    /// <see cref="Run(string)"/>
    /// </remarks>
    internal class PrimitiveMethod : IMethod
    {
        protected class Node
        {
            public int Value { get; private set; }
            public Node BottomNode { get; private set; }
            public Node BottomRight { get; private set; }

            public Node(int value, Node bottomNode, Node bottomRight)
            {
                Value = value;
                BottomNode = bottomNode;
                BottomRight = bottomRight;
            }
        }

        protected static bool IsEven(Node node)
        {
            return node.Value % 2 == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="bottomNode"><see cref="Node.BottomNode"/> or <see cref="Node.BottomRight"/> of <paramref name="rootNode"/>.</param>
        private static void EvaluatePart(Node rootNode, Node bottomNode, out int maxSum, out List<int> maxSumPath)
        {
            // "You should walk over the numbers as evens and odds subsequently."
            if (IsEven(rootNode) == IsEven(bottomNode))
            {
                maxSum = 0;
                maxSumPath = new List<int>(0);
            }
            else
            {
                Evaluate(bottomNode, out maxSum, out maxSumPath);
            }
        }

        private static void Evaluate(Node rootNode, out int maxSum, out List<int> maxSumPath)
        {
            if (rootNode.BottomNode == null)
            {
                maxSum = rootNode.Value;
                maxSumPath = new List<int>(1) { maxSum };
            }
            else
            {
                EvaluatePart(rootNode, rootNode.BottomNode, out int downwardsSum, out List<int> downwardsPath);
                EvaluatePart(rootNode, rootNode.BottomRight, out int diagSum, out List<int> diagPath);
                if (diagSum < downwardsSum)
                {
                    maxSum = downwardsSum;
                    maxSumPath = downwardsPath;
                }
                else
                {
                    maxSum = diagSum;
                    maxSumPath = diagPath;
                }
                maxSum += rootNode.Value;
                maxSumPath.Insert(0, rootNode.Value);
            }
        }

        protected virtual void OnGetMaxPath(Node rootNode, out int maxSum, out List<int> maxSumPath)
        {
            Evaluate(rootNode, out maxSum, out maxSumPath);
        }

        private static char[] whitespace = new[] { ' ', '\t' };
        private static char[] newline = new[] { '\n', '\r' };

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Method does not use <see cref="Char.IsWhiteSpace"/> for clarity.
        /// </remarks>
        private static int[] ParseLastRow(string input, ref int? lastIndex)
        {
            Queue<int> numbers = new Queue<int>();
            bool rowEntered = false;
            bool readDigits = true;
            int digits = 0;
            int i = -1;
            Action tryEnqueueDigits = () =>
            {
                if (digits == 0) return;
                numbers.Enqueue(Int32.Parse(input.Substring(i + 1, digits)));
                digits = 0;
            };
            for (i = lastIndex ?? input.Length - 1; i >= 0; i--)
            {
                char c = input[i];
                if (readDigits && Char.IsNumber(c))
                {
                    rowEntered = true;
                    digits++;
                }
                else if (whitespace.Contains(c))
                {
                    tryEnqueueDigits();
                }
                else if (newline.Contains(c))
                {
                    if (rowEntered) break;
                }
                else
                {
                    // ignore whole row
                    rowEntered = true;
                    readDigits = false;
                    numbers.Clear();
                    digits = 0;
                }
            }
            tryEnqueueDigits();
            lastIndex = i;
            int[] res = new int[numbers.Count];
            for (i = 0; i < res.Length; i++) res[i] = numbers.Dequeue();
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Method does not use <see cref="Char.IsWhiteSpace"/> for clarity.
        /// </remarks>
        private Node Parse(string input)
        {
            Node[] lastRow = null;
            int? lastIndex = null;
            while (!lastIndex.HasValue || lastIndex.Value > 0)
            {
                var numbers = new Stack<int>();
                bool rowEntered = false;
                bool readDigits = true;
                int digits = 0;
                int i = -1;
                Action tryEnqueueDigits = () =>
                {
                    if (digits == 0) return;
                    numbers.Push(Int32.Parse(input.Substring(i + 1, digits)));
                    digits = 0;
                };
                for (i = lastIndex ?? input.Length - 1; i >= 0; i--)
                {
                    char c = input[i];
                    if (readDigits && Char.IsNumber(c))
                    {
                        rowEntered = true;
                        digits++;
                    }
                    else if (whitespace.Contains(c))
                    {
                        tryEnqueueDigits();
                    }
                    else if (newline.Contains(c))
                    {
                        if (rowEntered) break;
                    }
                    else
                    {
                        // ignore whole row
                        rowEntered = true;
                        readDigits = false;
                        numbers.Clear();
                        digits = 0;
                    }
                }
                tryEnqueueDigits();
                lastIndex = i;
                if (numbers.Count > 0)
                {
                    var row = new Node[numbers.Count];
                    for (i = 0; i < row.Length; i++) row[i] = OnCreateNode(numbers.Pop(), lastRow?[i], lastRow?[i + 1]);
                    lastRow = row;
                }
                else
                {
                    break;
                }
            }
            if (lastRow.Length > 1) throw new Exception("Missing common root (" + lastRow.Select((x) => x.Value).ToStringAggregate() + ").");
            return lastRow.FirstOrDefault();
        }

        protected virtual Node OnCreateNode(int value, Node bottomNode, Node bottomRight)
        {
            return new Node(value, bottomNode, bottomRight);
        }

        void IMethod.TestParser()
        {
            const string sampleInput = "1\n8 9\n1 5 9\n4 5 2 3";
            Node root = Parse(sampleInput);
            Debug.Assert(root.Value == 1);
            Debug.Assert(root.BottomNode.Value == 8);
            Debug.Assert(root.BottomNode.BottomNode.Value == 1);
            Debug.Assert(root.BottomNode.BottomNode.BottomNode.Value == 4);
            Debug.Assert(root.BottomNode.BottomNode.BottomNode.BottomNode == null);
            Debug.Assert(root.BottomNode.BottomNode.BottomNode.BottomRight == null);
            Debug.Assert(root.BottomRight.Value == 9);
            Debug.Assert(root.BottomRight.BottomNode.Value == 5);
            Debug.Assert(root.BottomRight.BottomNode.BottomNode.Value == 5);
            Debug.Assert(root.BottomRight.BottomRight.Value == 9);
            Debug.Assert(root.BottomRight.BottomRight.BottomNode.Value == 2);
            Debug.Assert(root.BottomRight.BottomRight.BottomRight.Value == 3);
            // Extra tests; not mandatory.
            Debug.Assert(root.BottomNode.BottomRight == root.BottomRight.BottomNode);
            Debug.Assert(root.BottomNode.BottomNode.BottomRight == root.BottomNode.BottomRight.BottomNode);
            Debug.Assert(root.BottomRight.BottomNode.BottomRight == root.BottomRight.BottomRight.BottomNode);
            Debug.Assert(root.BottomRight.BottomNode != root.BottomRight.BottomNode.BottomNode);
            Debug.Assert(root.BottomRight != root.BottomRight.BottomRight);
            Debug.Assert(root != root.BottomNode.BottomNode);
        }

        public int[] Run(string input)
        {
            OnGetMaxPath(Parse(input), out int maxSum, out List<int> maxSumPath);
            return maxSumPath.ToArray();
        }
    }
}
