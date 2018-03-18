using System;
using System.Collections.Generic;

namespace MDPyramid
{
    /// <summary>
    /// An attempt to speed up <see cref="PrimitiveMethod"/> using cached results.
    /// </summary>
    internal class Method3 : PrimitiveMethod
    {
        private class Node2 : Node
        {
            public Node2(int value, Node2 bottomNode, Node2 bottomRight)
                : base(value, bottomNode, bottomRight)
            {
            }

            private int EvaluatePart(Node bottomNode)
            {
                // "You should walk over the numbers as evens and odds subsequently."
                if (IsEven(this) == IsEven(bottomNode))
                {
                    return 0;
                }
                else
                {
                    return ((Node2)bottomNode).GetMaxSum();
                }
            }

            private int GetMaxSumInternal()
            {
                if (BottomNode == null) return Value;

                // Additional threads may be created for the child nodes.
                var sumDown = EvaluatePart(BottomNode);
                var sumRight = EvaluatePart(BottomRight);
                return Value + Math.Max(sumDown, sumRight);
            }

            private int? maxSum;
            public int GetMaxSum()
            {
                if (!maxSum.HasValue) maxSum = GetMaxSumInternal();
                return maxSum.Value;
            }
        }

        protected override Node OnCreateNode(int value, Node bottomNode, Node bottomRight)
        {
            return new Node2(value, (Node2)bottomNode, (Node2)bottomRight);
        }

        protected override void OnGetMaxPath(Node rootNode, out int maxSum, out List<int> maxSumPath)
        {
            maxSum = ((Node2)rootNode).GetMaxSum();
            maxSumPath = new List<int>(1) { maxSum };
        }
    }
}
