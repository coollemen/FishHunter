using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    /// <summary>
    /// 模型基类
    /// </summary>
    public abstract  class ContextBase
    {
        public virtual string ID { get; private set; }

        protected ContextBase()
        {
            
        }
    }
}