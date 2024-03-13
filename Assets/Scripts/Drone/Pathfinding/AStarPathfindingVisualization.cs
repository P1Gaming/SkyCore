using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPathfinding
{
    public class AStarPathfindingVisualization
    {
        private GameObjectPool _pathPool;
        private GameObjectPool _closedPool;
        private GameObjectPool _obstructedPool;

        public AStarPathfindingVisualization(GameObject pathNodeVisual, Transform pathNodeVisualsParent
            , GameObject closedNodeVisual, Transform closedNodeVisualsParent
            , GameObject obstructedNodeVisual, Transform obstructedNodeVisualsParent
            )
        {
            _pathPool = new GameObjectPool(pathNodeVisual, pathNodeVisualsParent);
            _closedPool = new GameObjectPool(closedNodeVisual, closedNodeVisualsParent);
            _obstructedPool = new GameObjectPool(obstructedNodeVisual, obstructedNodeVisualsParent);
        }

        public void Reset()
        {
            _pathPool.Reset();
            _closedPool.Reset();
            _obstructedPool.Reset();
        }

        public void ShowPathNode(Vector3 position)
        {
            Show(_pathPool, position);
        }

        public void ShowClosed(Vector3 position)
        {
            Show(_closedPool, position);
        }

        public void ShowObstructed(Vector3 position)
        {
            Show(_obstructedPool, position);
        }

        private void Show(GameObjectPool pool, Vector3 position)
        {
            GameObject gameObject = pool.GetOne();
            gameObject.transform.position = position;
        }

        private class GameObjectPool
        {
            private Transform _parent;
            private GameObject _prefab;
            private List<GameObject> _pool = new();
            private int _countActive;

            public GameObjectPool(GameObject prefab, Transform parent)
            {
                _prefab = prefab;
                _parent = parent;
            }

            public void Reset()
            {
                for (int i = 0; i < _countActive; i++)
                {
                    _pool[i].SetActive(false);
                }
                _countActive = 0;
            }

            public GameObject GetOne()
            {
                if (_countActive == _pool.Count)
                {
                    _pool.Add(Object.Instantiate(_prefab, _parent));
                    _countActive++;
                    return _pool[^1];
                }
                GameObject result = _pool[_countActive];
                result.SetActive(true);
                _countActive++;
                return result;
            }
        }
    }
}
