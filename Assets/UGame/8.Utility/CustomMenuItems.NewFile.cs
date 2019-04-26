using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace UGame
{
   public partial class CustomMenuItems
   {
       [MenuItem("Assets/Create/uGame/MD(.md) File")]
       /// <summary>
       /// 创建新的MD文件
       /// </summary>
       public static void CreateNewMdFile()
       {
           CustomFileUtility.CreateFile("md");
       }

       [MenuItem("Assets/Create/uGame/JSON(.json) File")]
       /// <summary>
       /// 创建新的Json文件
       /// </summary>
       public static void CreateNewJsonFile()
       {
           CustomFileUtility.CreateFile("json");
       }

       [MenuItem("Assets/Create/uGame/XML(.xml) File")]
       /// <summary>
       /// 创建新的Xml文件
       /// </summary>
       public static void CreateNewXmlFile()
       {
           CustomFileUtility.CreateFile("xml");
       }
    }
}
