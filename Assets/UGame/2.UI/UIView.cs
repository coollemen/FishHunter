﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UGame
{
    /// <summary>
    /// UI视图
    /// </summary>
    public abstract class  UIView: ViewBase
    {
        public UIViewType type = UIViewType.Normal;
        public UIViewShowMode showMode = UIViewShowMode.Normal;
        public int orderID=-1;
        public Animator animater;
        public override string TypeID
        {
            get { return "UIView"; }
        }
        public UIContext Context { get; set; }
        public override void Awake()
        {
            //获取动画组件
            this.animater = GetComponent<Animator>();
            //初始化
            this.Init();
        }
        public override void Start()
        {
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
        }
        /// <summary>
        /// 显示状态
        /// </summary>
        public virtual void Show()
        {
            if (animater != null)
            {
                animater.SetBool("IsShow",true);
            }
        }
        /// <summary>
        /// 隐藏状态，窗口不显示时处于隐藏状态，而不是删除
        /// </summary>
        public virtual void Hide()
        {
            if (animater != null)
            {
                animater.SetBool("IsShow", false);
            }
        }
        /// <summary>
        /// 冻结状态，在模式窗口显示时，底下的窗口处于冻结状态
        /// </summary>
        public virtual void Freeze()
        {
            
        }
        /// <summary>
        /// 关闭视图，调用Destroy函数
        /// </summary>
        public virtual void Close()
        {
            if (animater != null)
            {
                animater.SetBool("IsShow", false);
            }
        }
        /// <summary>
        /// 将数据绑定到视图
        /// </summary>
        public virtual void DataBindings()
        {

        }


    }
}