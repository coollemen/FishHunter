using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGame
{
    public class Router : Singleton<Router>
    {
        protected Router()
        {
            
        }
        /// <summary>
        /// 将路径根据‘/’划分
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] ParsePath(string path)
        {
            return path.Split('/');
        }
    }
}