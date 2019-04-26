using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace UGame
{
    /// <summary>
    /// 对象池物体配置信息，用于PoolMananger自动管理
    /// </summary>
    public class AutoPoolItem : MonoBehaviour
    {
        /// <summary>
        /// 对象池地址
        /// </summary>
        public string url;

        /// <summary>
        /// 引用计数
        /// </summary>
        public string refCount;
        /// <summary>
        /// 最大缓存数
        /// </summary>
        public int maxCount=1;
        /// <summary>
        /// 装载Prefab的模式
        /// </summary>
        [EnumToggleButtons]
        public PrefabLoadMode loadMode;

        /// <summary>
        /// 销毁Prefab的模式
        /// </summary>
        [EnumToggleButtons]
        public PrefabDestroyMode destroyMode;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}