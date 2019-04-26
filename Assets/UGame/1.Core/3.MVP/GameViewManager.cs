using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    public class GameViewManager : Singleton<GameViewManager>
    {
        public Dictionary<string, GameView> views = new Dictionary<string, GameView>();
        public void RegisterView(ViewBase view)
        {
            if (views.ContainsKey(view.TypeID))
            {
                Debug.LogError($"重复注册**{view.TypeID}**视图");
            }
            else
            {
                views.Add(view.TypeID, view as GameView);
            }
        }

        public void UnregisterView(ViewBase view)
        {
            if (views.ContainsKey(view.TypeID))
            {
                views.Remove(view.TypeID);
            }
            else
            {
                Debug.LogError($"删除不存在的**{view.TypeID}**视图");

            }
        }

        public bool HasView(ViewBase view)
        {
            return views.ContainsKey(view.TypeID);
        }

        public ViewBase LoadView(string path)
        {
            throw new System.NotImplementedException();
        }

        public ViewBase[] LoadVIews(string[] paths)
        {
            throw new System.NotImplementedException();
        }

        public void UnloadView(string viewID)
        {
            throw new System.NotImplementedException();
        }

        public void UnloadAllViews()
        {
            throw new System.NotImplementedException();
        }
    }
}