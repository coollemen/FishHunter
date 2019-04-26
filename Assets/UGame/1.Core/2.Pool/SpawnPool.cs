using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace UGame
{
    public class SpawnPool : SerializedMonoBehaviour
    {
        /// <summary>
        /// Prefab对象池字段
        /// </summary>
        [OdinSerialize]
        private Dictionary<string, PrefabPool> prefabPools;

        public void Awake()
        {
            prefabPools = new Dictionary<string, PrefabPool>();
        }

        public PrefabPool GetPrefabPool(string name)
        {
            return this.prefabPools[name];
        }

        /// <summary>
        /// 是否存在Prefab对象池
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>是否存在</returns>
        public bool ExistPrefabPool(string name)
        {
            return prefabPools.ContainsKey(name);
        }

        /// <summary>
        /// 添加Prefab对象池
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        public PrefabPool CreatePrefabPool(string name,string path="",  
            PrefabLoadMode prefabLoadMode=PrefabLoadMode.OnUsed,
            PrefabDestroyMode prefabDestroyMode=PrefabDestroyMode.Immediate,int maxCount=1)
        {
            if (prefabPools.ContainsKey(name) == false)
            {
                var go = new GameObject(name);
                go.transform.parent = this.transform;
                var p = go.AddComponent<PrefabPool>();
                //初始化prefabPool参数
                p.path = path;
                p.loadMode = prefabLoadMode;
                p.destroyMode = prefabDestroyMode;
                p.maxCount = maxCount;
                //添加入pools
                prefabPools.Add(p.name, p);
                return p;
            }
            else
            {
                return prefabPools[name];
            }
        }
        /// <summary>
        /// 移除Prefab对象池
        /// </summary>
        /// <param name="name">名称</param>
        public void DestroyPrefabPool(string name)
        {
            if (prefabPools.ContainsKey(name) == true)
            {
                var p = this.prefabPools[name];
                this.prefabPools.Remove(name);
                Destroy(p);
            }
            else
            {
                Debug.LogError($"移除已经不存在的PrefabPool***{name}***");
            }
        }

        public void OnDestroy()
        {
            foreach (var keyValue in prefabPools)
            {
                Destroy(keyValue.Value);
            }
            prefabPools.Clear();
        }
    }
}