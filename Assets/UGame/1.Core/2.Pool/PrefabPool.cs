using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace UGame
{
    public class PrefabPool : SerializedMonoBehaviour
    {
        /// <summary>
        /// Prefab路径
        /// </summary>
        public string path;

        /// <summary>
        /// Prefab对象
        /// </summary>
        public GameObject prefab;

        public GameObject gameObjCache;

        /// <summary>
        /// 激活状态的对象列表
        /// </summary>
        public List<GameObject> activeObjects;

        /// <summary>
        /// 隐藏状态的对象列表
        /// </summary>
        public List<GameObject> hideObjects;

        /// <summary>
        /// 最大存储上限
        /// </summary>
        public int maxCount = 1;

        /// <summary>
        /// 对象销毁模式
        /// </summary>
        [EnumToggleButtons] public PoolTrimMode trimMode = PoolTrimMode.Immediate;

        /// <summary>
        /// 延迟销毁时间间隔
        /// </summary>
        public float delayTimeSpan;

        /// <summary>
        /// 装载Prefab的模式
        /// </summary>
        [EnumToggleButtons] public PrefabLoadMode loadMode;

        /// <summary>
        /// 销毁Prefab的模式
        /// </summary>
        [EnumToggleButtons] public PrefabDestroyMode destroyMode;

        public void Awake()
        {
//            if (maxCount > 1)
//            {
            this.activeObjects = new List<GameObject>();
            this.hideObjects = new List<GameObject>();
//            }
        }

        public void Init()
        {
            if (loadMode == PrefabLoadMode.OnInit)
            {
                this.LoadPrefab();
            }
        }

        /// <summary>
        /// 生产新的对象
        /// </summary>
        /// <returns>对象</returns>
        public GameObject Spawn()
        {
            if (loadMode == PrefabLoadMode.OnUsed && prefab == null)
            {
                prefab= LoadPrefab();
            }

            if (maxCount == 1)
            {
                if (gameObjCache == null)
                {
                    gameObjCache = GameObject.Instantiate(prefab);
                }
                else
                {
                    gameObjCache.SetActive(true);
                }

                gameObjCache.transform.parent = this.transform;
                return gameObjCache;
            }
            else
            {
                GameObject activeObj;
                //如果在隐藏物体列表中有待用的物体，则取出（并从隐藏列表中移除）
                if (this.hideObjects.Count > 0)
                {
                    activeObj = this.hideObjects[0];
                    this.hideObjects.RemoveAt(0);
                }
                else
                {
                    //重新创建新的物体
                    activeObj = GameObject.Instantiate(prefab);
                }

                //加入激活列表
                activeObj.SetActive(true);
                activeObj.transform.parent = this.transform;
                this.activeObjects.Add(activeObj);
                return activeObj;
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="go">游戏对象</param>
        public void Despawn(GameObject go)
        {
            if (maxCount == 1)
            {
                if (go.name == this.gameObjCache.name)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                go.SetActive(false);
                this.activeObjects.Remove(go);
                if (this.hideObjects.Count >= maxCount)
                {
                    //如果对象池中存储的对象达到最大值
                    if (trimMode == PoolTrimMode.Immediate)
                    {
                        var destoryObj = this.hideObjects[0];
                        this.hideObjects.RemoveAt(0);
                        Destroy(destoryObj);
                        this.hideObjects.Add(go);
                    }
                    else
                    {
                        var destoryObj = this.hideObjects[0];
                        this.hideObjects.RemoveAt(0);
                        Destroy(destoryObj, delayTimeSpan);
                        this.hideObjects.Add(go);
                    }
                }
                else
                {
                    this.hideObjects.Add(go);
                }
            }
        }

        public virtual void OnDestroy()
        {
            //清空隐藏列表的物体
            for (int i = 0; i < hideObjects.Count; i++)
            {
                Destroy(hideObjects[i]);
            }

            //清空显示列表的物体
            for (int i = 0; i < activeObjects.Count; i++)
            {
                Destroy(activeObjects[i]);
            }

            //清空游戏物体单个缓存
            Destroy(gameObjCache);
            //删除Prefab
            if (destroyMode == PrefabDestroyMode.Immediate)
            {
                UnloadPrefab();
            }
            else
            {
                UnloadPrefabDelay();
            }
        }

        /// <summary>
        /// 装载Prefab
        /// </summary>
        /// <returns></returns>
        public GameObject LoadPrefab()
        {
            return Resources.Load(path) as GameObject;
        }

        public GameObject LoadPrefabPreload()
        {
            return Resources.Load(path) as GameObject;
        }

        /// <summary>
        /// 卸载Prefab
        /// </summary>
        private void UnloadPrefab()
        {
            Destroy(prefab);
        }

        private void UnloadPrefabDelay()
        {
            Destroy(prefab, delayTimeSpan);
        }
    }
}