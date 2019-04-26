using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    /// <summary>
    /// 游戏视图基类
    /// </summary>
    public class GameView:ViewBase
    {
        public override void Awake()
        {
            base.Awake();
            switch (Layer)
            {
                case ViewLayer.UI:
                    
                    break;
                case ViewLayer.Game:
                    break;
                default:
                    break;
            }
        }
        public override void LoadViews()
        {
            base.LoadViews();
            foreach (var p in subviewPaths)
            {
                var v = GameViewManager.Instance.LoadView(p);
                subviews.Add(v);
            }
        }
        public override void UnloadViews()
        {
            base.UnloadViews();
            foreach (var v in subviews)
            {
                GameViewManager.Instance.UnloadView(v.TypeID);
            }
        }
    }
}
