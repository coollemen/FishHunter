using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
namespace UGame
{
      public class WindowContext : ContextBase
      {
      }
      public partial class WindowView : UIView
      {
            public override void Start()
            {
                  //框架自动创建的代码，用于获取UI组件，请不要删除！
                  GetUIComponents();
                  DataBindings();
                  //添加你自己的代码
            }
            public override void DataBindings()
            {
            }
      }
}
