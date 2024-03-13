using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPathfinding
{
    public class BinaryHeap
    {
        // https://en.wikipedia.org/wiki/Binary_heap
        // This might be the wrong data structure for a priority queue, because I needed to add the dictionary
        // to avoid O(n) things.

        private Node[] _items = new Node[10000];
        private Dictionary<Vector3Int, int> _itemIdentifiersToIndex = new(10000);
        public int Count => _itemIdentifiersToIndex.Count;

        public bool Contains(Vector3Int coords)
        {
            return _itemIdentifiersToIndex.ContainsKey(coords);
        }

        public void Clear()
        {
            _itemIdentifiersToIndex.Clear();
        }

        public void Insert(Node node)
        {
            if (_items.Length == Count)
            {
                ExpandArray();
            }

            _items[Count] = node;
            _itemIdentifiersToIndex.Add(node.coords, Count);
            BubbleUp(Count - 1);
        }

        public Node ExtractNodeWithMinCost()
        {
            Node result = _items[0];
            _items[0] = _items[Count - 1];
            _itemIdentifiersToIndex.Remove(result.coords);
            SinkDown(0);
            return result;
        }


        public void ChangeCostIfLower(Vector3Int coords, Vector3Int cameFrom, float gForCameFrom)
        {
            int i = _itemIdentifiersToIndex[coords];
            if (gForCameFrom < _items[i].g)
            {
                // does this ever happen? might be a bug or just not testing with the right situation

                _items[i].g = gForCameFrom;
                _items[i].cameFrom = cameFrom;

                // The node's cost decreased, so move it towards the root.
                BubbleUp(i);
            }
        }

        private void BubbleUp(int i)
        {
            if (i == 0)
            {
                // Can't bubble up further.
                return;
            }

            // If the node has a lower cost than its parent, swap them. Repeat until the node stops moving up.
            int parentIndex = (i - 1) / 2;
            if (_items[i].F < _items[parentIndex].F)
            {
                Swap(parentIndex, i);
                BubbleUp(parentIndex);
            }
        }

        private void SinkDown(int i)
        {
            int firstChildIndex = 2 * i + 1;
            int secondChildIndex = 2 * i + 2;

            // Handle cases where child indexes are beyond count, so can't sink down further.
            if (firstChildIndex >= Count)
            {
                return;
            }
            else if (secondChildIndex >= Count)
            {
                if (_items[i].F > _items[firstChildIndex].F)
                {
                    Swap(firstChildIndex, i);
                }
                return;
            }

            // Normal cases. Swap with child nodes and repeat until the node stops moving down.
            float parentCost = _items[i].F;
            float firstChildCost = _items[firstChildIndex].F;
            float secondChildCost = _items[secondChildIndex].F;

            bool firstChildIsBetter = firstChildCost < secondChildCost;
            if (parentCost > firstChildCost && firstChildIsBetter)
            {
                Swap(firstChildIndex, i);
                SinkDown(firstChildIndex);
            }
            else if (parentCost > secondChildCost && !firstChildIsBetter)
            {
                Swap(secondChildIndex, i);
                SinkDown(secondChildIndex);
            }
        }

        private void ExpandArray()
        {
            Node[] old = _items;
            _items = new Node[old.Length + old.Length / 2 + 1];
            for (int i = 0; i < old.Length; i++)
            {
                _items[i] = old[i];
            }
        }

        private void Swap(int index1, int index2)
        {
            _itemIdentifiersToIndex[_items[index1].coords] = index2;
            _itemIdentifiersToIndex[_items[index2].coords] = index1;

            Node temp = _items[index1];
            _items[index1] = _items[index2];
            _items[index2] = temp;
        }
    }
}