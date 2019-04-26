using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
namespace UGame
{
//    public class AutoAddNamespace : UnityEditor.AssetModificationProcessor
//    {
//        private static void OnWillCreateAsset(string assetName)
//        {
//            assetName = assetName.Replace(".meta", "");
//            //判读是否是C#脚本文件
//            if (assetName.EndsWith(".cs"))
//            {
//                string text = "";
//                text += File.ReadAllText(assetName);
//                string className = GetClassName(text);
//                var script = GetNewDefaultScript(className);
//                File.WriteAllText(assetName, script);
//            }
//        }
//
//        private static string GetNewDefaultScript(string className)
//        {
//            ScriptBuilder builder = new ScriptBuilder();
//            builder.AddUsingLib("System.Collections");
//            builder.AddUsingLib("System.Collections.Generic");
//            builder.AddUsingLib("UnityEngine");
//            builder.BeginNamespace("UGame");
//
//            builder.BeginClass(className);
//            builder.WriteComments("初始函数");
//            builder.AddSimpleFunction("Start");
//            builder.AddSimpleFunction("Update");
//
//            builder.EndClass();
//
//            builder.EndNamespace();
//
//            return builder.ToString();
//        }
//
//        /// <summary>
//        /// 通过文本比对获取文本
//        /// </summary>
//        /// <param name="content"></param>
//        /// <returns></returns>
//        private static string GetClassName(string content)
//        {
//            string[] data = content.Split(' ');
//            int index = 0;
//            for (int i = 0; i < data.Length; i++)
//            {
//                if (data[i].Contains("class"))
//                {
//                    index = i + 1;
//                    break;
//                }
//            }
//
//            if (data[index].Contains(":"))
//            {
//                return data[index].Split(':')[0];
//            }
//            else
//            {
//                return data[index];
//            }
//
//        }
//
//        /// <summary>
//        /// 通过正则表达式获取类名
//        /// </summary>
//        /// <param name="content"></param>
//        /// <returns></returns>
//        private static string GetClassNameRegex(string content)
//        {
//            string patterm = "public class ([A-Za-z0-9_]+)\\s*:\\s*MonoBehaviour";
//            var match = Regex.Match(content, patterm);
//            if (match.Success)
//            {
//                return match.Groups[1].Value;
//            }
//
//            return "";
//        }
//    }
}
