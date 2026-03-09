using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class GameObjectPool
    {
        readonly Stack<GameObject> _pool = new Stack<GameObject>();
        readonly Transform _container = new GameObject("Pool Container").transform;

        public GameObjectPool()
        {
            _container.gameObject.SetActive(false);
        }

        public GameObject Borrow()
        {
            if (_pool.Count == 0)
                return null;

            GameObject go = _pool.Pop();
            go.SetActive(true);
            return go;
        }

        public void Return(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(_container, false);
            _pool.Push(go);
        }
    }
}
