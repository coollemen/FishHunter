using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace UGame
{
    public class PoolManager : OdinMonoSingleton<PoolManager>
    {
        [OdinSerialize] private Dictionary<string, SpawnPool> spawnPools = new Dictionary<string, SpawnPool>();

        protected PoolManager()
        {
        }

        /// <summary>
        /// 创建游戏对象
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>游戏对象</returns>
        public GameObject Spawn(string url)
        {
            string[] paras = Router.Instance.ParsePath(url);
            if (paras.Length == 2)
            {
                //如果url格式正确，将包含SpawnPool的名称和PrefabPool的名称
                string spawnPoolName = paras[0];
                string prefabPoolName = paras[1];
                var go= spawnPools[spawnPoolName].GetPrefabPool(prefabPoolName).Spawn();
                var item= go.AddComponent<AutoPoolItem>();
                item.url = url;
                return go;
            }
            else
            {
                Debug.LogError($"Url格式错误，Spawn失败！***{url}***");
                return null;
            }
        }

        /// <summary>
        /// 回收游戏对象
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="go">游戏对象</param>
        public void Despawn(GameObject go)
        {
            var item = go.GetComponent<AutoPoolItem>();
            if (item == null)
            {
                Debug.LogError($"游戏物体未找到AutoPoolItem组件，无法执行Despawn操作！");
            }
            string[] paras = Router.Instance.ParsePath(item.url);
            if (paras.Length == 2)
            {
                //如果url格式正确，将包含SpawnPool的名称和PrefabPool的名称
                string spawnPoolName = paras[0];
                string prefabPoolName = paras[1];
                spawnPools[spawnPoolName].GetPrefabPool(prefabPoolName).Despawn(go);
            }
            else
            {
                Debug.LogError($"Url格式错误，Despawn失败！***{item.url}***");
            }
        }

        public void CreatePool(string url, string prefabPath = "",   int maxCount=1,
            PrefabLoadMode prefabLoadMode=PrefabLoadMode.OnUsed,
            PrefabDestroyMode prefabDestroyMode=PrefabDestroyMode.Immediate)
        {
            string[] paras = Router.Instance.ParsePath(url);
            if (paras.Length == 1)
            {
                string spawnPoolName = paras[0];
                //若果不存在SpawnPool，那么创建SpawnPool
                if (this.spawnPools.ContainsKey(spawnPoolName) == false)
                {
                    this.CreateSpawnPool(spawnPoolName);
                }
            }
            else if (paras.Length == 2)
            {
                //如果url格式正确，将包含SpawnPool的名称和PrefabPool的名称
                string spawnPoolName = paras[0];
                string prefabPoolName = paras[1];
                //若果不存在SpawnPool，那么创建SpawnPool
                SpawnPool sp;
                if (this.spawnPools.ContainsKey(spawnPoolName))
                {
                    sp = this.spawnPools[spawnPoolName];
                }
                else
                {
                    sp = this.CreateSpawnPool(spawnPoolName);
                }
                //如果SpawnPool不存在该PrefabPool，那么创建PrefabPool
                if (sp.ExistPrefabPool(prefabPoolName) == false)
                {
                    sp.CreatePrefabPool(prefabPoolName,prefabPath,prefabLoadMode,prefabDestroyMode,maxCount);
                }
            }
            else
            {
                //若果参数是其他情况，那么url的格式是错误的
                Debug.LogError($"Url格式错误，CreatePool失败！***{url}***");
            }
        }

        /// <summary>
        /// 移除对象池
        /// </summary>
        /// <param name="url">url</param>
        public void DestroyPool(string url)
        {
            string[] paras = Router.Instance.ParsePath(url);
            if (paras.Length == 1)
            {
                string spawnPoolName = paras[0];
                //若果不存在SpawnPool，那么创建SpawnPool
                if (this.spawnPools.ContainsKey(spawnPoolName))
                {
                    Destroy(this.spawnPools[spawnPoolName]);
                }
            }
            else if (paras.Length == 2)
            {
                //如果url格式正确，将包含SpawnPool的名称和PrefabPool的名称
                string spawnPoolName = paras[0];
                string prefabPoolName = paras[1];
                //若果不存在SpawnPool，那么创建SpawnPool
                SpawnPool sp = this.spawnPools[spawnPoolName];
                //如果SpawnPool不存在该PrefabPool，那么创建PrefabPool
                if (sp.ExistPrefabPool(prefabPoolName))
                {
                    sp.DestroyPrefabPool(prefabPoolName);
                }
            }
            else
            {
                Debug.LogError($"Url格式错误，CreatePool失败！***{url}***");
            }
        }
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void RemoveAll()
        {
            foreach (var keyValue in spawnPools)
            {
                Destroy(keyValue.Value);
            }
            this.spawnPools.Clear();
        }

//        private PrefabPool GetPrefabPool(string url)
//        {
//            string[] paras = Router.Instance.ParsePath(url);
//            string spawnPoolName = paras[0];
//            string prefabPoolName = paras[1];
//            return spawnPools[spawnPoolName].GetPrefabPool(prefabPoolName);
//        }
//
//        private PrefabPool AddPrefabPool(string url, string prefabPath)
//        {
//            string[] paras = Router.Instance.ParsePath(url);
//            string spawnPoolName = paras[0];
//            string prefabPoolName = paras[1];
//            //创建
//            SpawnPool sp;
//            if (spawnPools.ContainsKey(spawnPoolName) == false)
//            {
//                sp = CreateSpawnPool(spawnPoolName);
//            }
//            else
//            {
//                sp = spawnPools[spawnPoolName];
//            }
//
//            return sp.CreatePrefabPool(prefabPoolName, prefabPath);
//        }

        private void RemovePrefabPool(string path)
        {
            string[] paras = Router.Instance.ParsePath(path);
            string spawnPoolName = paras[0];
            string prefabPoolName = paras[1];
            if (spawnPools.ContainsKey(spawnPoolName))
            {
                spawnPools[spawnPoolName].DestroyPrefabPool(prefabPoolName);
            }
        }

        private SpawnPool CreateSpawnPool(string name)
        {
            var go = new GameObject(name);
            go.transform.parent = this.transform;
            var pool = go.AddComponent<SpawnPool>();
            spawnPools.Add(pool.name, pool);
            return pool;
        }

        private void RemoveSpawnPool(string name)
        {
            if (spawnPools.ContainsKey(name))
            {
                spawnPools.Remove(name);
            }
        }
    }
}