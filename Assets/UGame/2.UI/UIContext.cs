using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGameDesigner;
namespace UGame
{
    /// <summary>
    /// UI视图模型
    /// </summary>
    public abstract class UIContext: ContextBase
    {
        public override string ID => "UIContext";


        public virtual void GetData()
        {
            
        }

        public virtual void SetData()
        {
            
        }

        public virtual void Init()
        {
            //从模型获取数据
            this.GetData();
        }

    }
}
