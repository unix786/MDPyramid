using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDPyramid
{
    /// <summary>
    /// An attempt to speed up <see cref="PrimitiveMethod"/> using threads and cached results.
    /// </summary>
    internal class Method2 : PrimitiveMethod
    {
        private class Node2 : Node
        {
            public Node2(int value, Node2 bottomNode, Node2 bottomRight)
                : base(value, bottomNode, bottomRight)
            {
            }

            private async Task<int> EvaluatePart(Node bottomNode)
            {
                // "You should walk over the numbers as evens and odds subsequently."
                if (IsEven(this) == IsEven(bottomNode))
                {
                    return 0;
                }
                else
                {
                    var sum = ((Node2)bottomNode).GetMaxSum();
                    return await sum;
                }
            }

            private async Task<int> GetMaxSumInternal()
            {
                if (BottomNode == null) return Value;

                // Additional threads may be created for the child nodes.
                var sumDown = EvaluatePart(BottomNode);
                var sumRight = EvaluatePart(BottomRight);
                return Value + Math.Max(await sumDown, await sumRight);
            }

            private volatile Task<int> maxSumTask; // field might be modified by multiple threads that are executing at the same time.
            public async Task<int> GetMaxSum()
            {
                if (maxSumTask == null) maxSumTask = GetMaxSumInternal();
                return await maxSumTask;
            }
        }

        protected override Node OnCreateNode(int value, Node bottomNode, Node bottomRight)
        {
            return new Node2(value, (Node2)bottomNode, (Node2)bottomRight);
        }

        protected override void OnGetMaxPath(Node rootNode, out int maxSum, out List<int> maxSumPath)
        {
            maxSum = ((Node2)rootNode).GetMaxSum().Result; // Should run this synchronously.
            maxSumPath = new List<int>(1) { maxSum };
        }
    }
}
