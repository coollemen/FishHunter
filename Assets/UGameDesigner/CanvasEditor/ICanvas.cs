using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UGameDesigner
{
    /// <summary>
    /// 画布接口
    /// </summary>
    interface ICanvas
    {
        void Awake();
        void OnFocus();
        void OnLostFocus();
        void OnGUI();
        void OnDestroy();
        void Update();
        void ProcessEvent();
    }
}
