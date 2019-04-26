using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    /// <summary>
    /// 视图基类
    /// </summary>
    public abstract class ViewBase:MonoBehaviour
    {
        /// <summary>
        /// 视图类型名称
        /// </summary>
        public virtual string TypeID { get; private set; }
        /// <summary>
        /// 视图所在层
        /// </summary>
        public virtual ViewLayer Layer { get;private set; }
        /// <summary>
        /// 排序索引
        /// </summary>
        public virtual int OrderIndex { get;  set; }
        public ContextBase Context { get; set; }
        /// <summary>
        /// 子视图路径列表
        /// </summary>
        public List<string> subviewPaths = new List<string>();
        /// <summary>
        /// 子视图列表
        /// </summary>
        public List<ViewBase> subviews = new List<ViewBase>();
        //自定义函数
        public virtual void LoadViews()
        {

        }

        public virtual void UnloadViews()
        {

        }
        //默认函数
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnGUI() { }
        public virtual void OnDisable() { }
        public virtual void OnEnable() { }
        public virtual void OnDestroy() { }
    }
}