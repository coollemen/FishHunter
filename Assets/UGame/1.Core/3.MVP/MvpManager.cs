using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    public class MvpManager:Singleton<MvpManager>,IViewManager
    {
        protected MvpManager()
        {

        }
        public void RegisterView(ViewBase view)
        {
            throw new System.NotImplementedException();
        }

        public void UnregisterView(ViewBase view)
        {
            throw new System.NotImplementedException();
        }

        public bool HasView(ViewBase view)
        {
            throw new System.NotImplementedException();
        }

        public T LoadView<T>(string path, ContextBase context) where T : ViewBase
        {
            GameObject go = (GameObject) Resources.Load(path);
            var view = go.GetComponent<T>();
            if (view == null)
            {
                Debug.Log($"Load view from prefab at {path} is null!");
                return null;
            }
            else
            {
                //设置数据
                view.Context = context;
                return view;
            }
        }

        public ViewBase[] LoadVIews(string[] paths, ContextBase[] contexts)
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