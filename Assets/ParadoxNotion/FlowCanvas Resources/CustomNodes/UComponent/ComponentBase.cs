﻿using System;
using ParadoxNotion.Design;
using UnityEngine;
using Object = UnityEngine.Object;


namespace FlowCanvas.Nodes
{


    #region [ComponentBase]

    [Name("get_enable")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("返回该组件是否处于激活状态 ---return this component's enable state.")]
    public class Get_Enable : PureFunctionNode<bool, Behaviour>
    {
        public override bool Invoke(Behaviour behavior)
        {
            return behavior.enabled;
        }
    }

    [Name("get_type")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("返回该组件的类型 ---return this component's type.")]
    public class Get_Type : PureFunctionNode<Type, Behaviour>
    {
        public override Type Invoke(Behaviour behavior)
        {
            return behavior.GetType();
        }
    }

    [Name("set_enable")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("设置该组件的激活状态 ---set this component's enable state.")]
    public class Set_Enable : CallableActionNode<Behaviour, bool>
    {
        public override void Invoke(Behaviour behavior, bool enable)
        {
            behavior.enabled = enable;
        }
    }

    [Name("get_gameObject")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("返回该组件所在的游戏物体 ---return this component's attached gameObject.")]
    public class Get_GO : PureFunctionNode<GameObject, Component>
    {
        public override GameObject Invoke(Component behavior)
        {
            return behavior.gameObject;
        }
    }

    [Name("get_transform")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("返回该组件所在的游戏物体的Transform组件 ---return this component's attached Transform component.")]

    public class G_Transform : PureFunctionNode<Transform, Component>
    {
        public override Transform Invoke(Component behavior)
        {
            return behavior.transform;
        }
    }

    [Name("get_tag")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("返回该组件所在的游戏物体的Tag ---return this gameObject's tag.")]
    public class G_Tag : PureFunctionNode<string, Component>
    {
        public override string Invoke(Component behavior)
        {
            return behavior.tag;
        }
    }

    [Name("get_name")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("返回该组件所在的游戏物体的Name ---set this gameObject's name.")]
    public class G_Name : PureFunctionNode<string, Component>
    {
        public override string Invoke(Component behavior)
        {
            return behavior.name;
        }
    }

    [Name("set_tag")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("设置该组件所在的游戏物体的Tag ---set this gameObject's tag.")]
    public class S_Tag : CallableActionNode<Component, string>
    {
        public override void Invoke(Component behavior, string value)
        {
            behavior.tag = value;
        }
    }

    [Name("compareTag")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("比较该游戏物体的Tag,一致则返回true ---compare gameObject's tag.")]
    public class Compare_Tag : PureFunctionNode<bool, Component, string>
    {
        public override bool Invoke(Component behavior, string value)
        {
            return behavior.CompareTag(value);
        }
    }

    [Name("destroy")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("摧毁目标,可以设置延时时间 ---destroy this target in delay time")]
    public class Destroy : CallableActionNode<Object, float>
    {
        public override void Invoke(Object obj, float delay = 0)
        {
            Object.Destroy(obj, delay);
        }
    }

    [Name("dontDestroyOnLoad")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("切换场景时保留此物体 ---dont Destroy this target when a new scene is loaded.")]
    public class DontDestroyOnLoad : CallableActionNode<Component>
    {
        public override void Invoke(Component target)
        {
            Object.DontDestroyOnLoad(target);
        }
    }

    [Name("instantiate")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description(
         "生成一个新物体,可传入生成物的位置,角度和父物体 ---Instantiate object,alternative pass position eulerAngle,and parent transform.")]
    public class Instantiate : PureFunctionNode<GameObject,GameObject,Vector3,Vector3,Transform>
    {
        public override GameObject Invoke(GameObject original,Vector3 position, Vector3 eulerAngle, Transform parent)
        {
            if (original == null)
                return null;

            if (parent == null)
            {
                return Object.Instantiate(original, position, Quaternion.Euler(eulerAngle));
            }else{
                return Object.Instantiate(original, position, Quaternion.Euler(eulerAngle),parent);
            }
        }
    }

    [Name("instantiate（callable）")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description(
     "生成一个新物体,可传入生成物的位置,角度和父物体 ---Instantiate object,alternative pass position eulerAngle,and parent transform.")]
    public class InstantiateCall : FlowNode
    {
        public bool callable = true;

        ValueInput<GameObject> original;
        ValueInput<Vector3> pos;
        ValueInput<Vector3> eulerAngles;
        ValueInput<Transform> parent;

        GameObject result;

        FlowInput Input;
        FlowOutput Output;
        protected override void RegisterPorts()
        {
            original = AddValueInput<GameObject>("original");
            pos = AddValueInput<Vector3>("position");
            eulerAngles = AddValueInput<Vector3>("eulerAngles");
            parent = AddValueInput<Transform>("parent");
            AddValueOutput<GameObject>("original", () => result);
            if (callable)
            {
                Output = AddFlowOutput("Out");
                AddFlowInput("In", (f) => Output.Call(f));

                if (parent == null)
                {
                    result = Object.Instantiate(original.value, pos.value, Quaternion.Euler(eulerAngles.value.x, eulerAngles.value.y, eulerAngles.value.z));
                }
                if (parent != null)
                {
                    result = Object.Instantiate(original.value, pos.value, Quaternion.Euler(eulerAngles.value.x, eulerAngles.value.y, eulerAngles.value.z),
                        parent.value);
                }

            }
            else
            {
                if (parent == null)
                {
                    result = Object.Instantiate(original.value, pos.value, Quaternion.Euler(eulerAngles.value.x, eulerAngles.value.y, eulerAngles.value.z));
                }
                if (parent != null)
                {
                    result = Object.Instantiate(original.value, pos.value, Quaternion.Euler(eulerAngles.value.x, eulerAngles.value.y, eulerAngles.value.z),
                        parent.value);
                }
            }
        }
    }

    [Name("isExist")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("如果值不为空,返回true,否则是false ---if exist return true,else false.")]
    public class IsNull : PureFunctionNode<bool, Object>
    {
        public override bool Invoke(Object target)
        {
            return target != null ? true : false;
        }
    }

    [Name("!=")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("如果比较结果不同,返回true,否则是false ---if different then return true,else false.")]
    public class NotEqual : PureFunctionNode<bool, Object, Object>
    {
        public override bool Invoke(Object original, Object target)
        {
            return original != target ? true : false;
        }
    }

    [Name("==")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("如果比较结果相同,返回true,否则是false ---if compare result is same then return true,else false.")]
    public class Equal : PureFunctionNode<bool, Object, Object>
    {
        public override bool Invoke(Object original, Object target)
        {
            return original == target ? true : false;
        }
    }

    [Name("findObjectOfType")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("寻找目标类型的物体,返回最先找到符合条件的")]
    public class FindObjectOfType<T> : PureFunctionNode<T>where T:Component
    {
        public override string name
        {
            get
            {
                return string.Format("{0}<{1}>", base.name, typeof(T).Name);
            }
        }
        public override T Invoke()
        {
            return Object.FindObjectOfType<T>();
        }
    }

    [Name("findObjectsOfType")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("寻找目标类型的物体,返回所有到符合条件的Object组成的数组")]
    public class FindObjectsOfType<T> : PureFunctionNode<T[]> where T : Component
    {
        public override string name
        {
            get
            {
                return string.Format("{0}<{1}>", base.name,typeof(T).Name);
            }
        }
        public override T[] Invoke()
        {
            return Object.FindObjectsOfType<T>();
        }
    }

    [Name("getComponent")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("寻找组件.传入组件类型或实例参考对象")]
    public class G_Component<T> : PureFunctionNode<T, Component>where T:Component
    {
        public override string name
        {
            get
            {
                return string.Format("{0}<{1}>", base.name, typeof(T).Name);
            }
        }
        private T _component;

        public override T Invoke(Component behavior)
        {
            if (behavior == null) return null;
            if (_component == null || _component.gameObject != behavior)
            {
                _component = behavior.GetComponent<T>();
            }
            return _component;
        }
    }


    [Name("getComponentByTypeName")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("寻找组件.传入组件类型字符")]
    public class G_ComponentByTypeName : PureFunctionNode<Component, Component, string>
    {
        public override Component Invoke(Component behavior, string typeString)
        {
            return behavior.GetComponent(typeString);
        }
    }

    [Name("getComponents")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("寻找所有目标组件组成的数组.")]
    public class G_Components<T> : PureFunctionNode<T[], Component> where T : Component
    {
        public override string name
        {
            get
            {
                return string.Format("{0}<{1}>", base.name, typeof(T).Name);
            }
        }
        private T[] _components;

        public override T[] Invoke(Component behavior)
        {
            if (behavior == null) return null;
            if (_components == null || _components[0].gameObject != behavior)
            {
                _components = behavior.GetComponents<T>();
            }
            return _components;
        }
    }

    [Name("getComponentInChildren")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("在该组件下的子物体中寻找组件.传入组件类型或实例参考对象")]
    public class G_ComponentInChildren<T> : PureFunctionNode<T, Component> where T : Component
    {
        public override string name
        {
            get
            {
                return string.Format("{0}<{1}>", base.name, typeof(T).Name);
            }
        }
        private T _component;

        public override T Invoke(Component behavior)
        {
            if (behavior == null) return null;
            if (_component == null || _component.gameObject != behavior)
            {
                _component = behavior.GetComponentInChildren<T>();
            }
            return _component;
        }
    }

    [Name("getComponentsInChildren")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("在该组件下的子物体中寻找组件")]
    public class G_ComponentsInChildren<T> : PureFunctionNode<T[], Component> where T : Component
    {
        public override string name
        {
            get
            {
                return string.Format("{0}<{1}>", base.name, typeof(T).Name);
            }
        }
        private T[] _component;

        public override T[] Invoke(Component behavior)
        {
            if (behavior == null) return null;
            if (_component == null || _component[0].gameObject != behavior)
            {
                _component = behavior.GetComponentsInChildren<T>();
            }
            return _component;
        }
    }

    [Name("getComponentInParent")]
    [Category("UnityEngine/[ComponentBase]")]
    [Description("在该组件下的子物体中寻找组件数组集合")]
    public class G_ComponentInParent<T> : PureFunctionNode<T, Component> where T : Component
    {
        public override string name
        {
            get
            {
                return string.Format("{0}<{1}>", base.name, typeof(T).Name);
            }
        }
        private T _component;

        public override T Invoke(Component behavior)
        {
            if (behavior == null) return null;
            if (_component == null || _component.gameObject != behavior)
            {
                _component = behavior.GetComponentInParent<T>();

            }
            return _component;
        }
    }

    #endregion
}
