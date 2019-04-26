using System;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NodeCanvas.Framework
{
	public static class UConverter
	{
		/// <summary>
		/// Returns a function that converts a value from one type to another.
		/// </summary>
		public static Func<object, object> Get(Type fromType, Type toType)
		{
			// Normal assignment.
			if (toType.RTIsAssignableFrom(fromType))
				return (value) => value;

			// Convertible to convertible.
			if (typeof(IConvertible).RTIsAssignableFrom(toType) && typeof(IConvertible).RTIsAssignableFrom(fromType))
			{
				return (value) => { try { return Convert.ChangeType(value, toType); } catch { return !toType.RTIsAbstract() ? Activator.CreateInstance(toType) : null; } };
			}

			// Unity object to bool.
			if (typeof(UnityEngine.Object).RTIsAssignableFrom(fromType) && toType == typeof(bool))
				return (value) => value != null;

			// GameObject to component.
			if (fromType == typeof(GameObject) && typeof(Component).RTIsAssignableFrom(toType))
				return (value) => { var r =value as GameObject ; if(r!=null)return r.GetComponent(toType);else return null;};

			// Component to GameObject.
			if (typeof(Component).RTIsAssignableFrom(fromType) && toType == typeof(GameObject))
				return (value) =>  { var r =value as Component ; if(r!=null)return r.gameObject;else return null;};

			// Component to Component.
			if (typeof(Component).RTIsAssignableFrom(fromType) && typeof(Component).RTIsAssignableFrom(toType))
				return (value) => { var r =value as Component ; if(r!=null)return r.gameObject.GetComponent(toType);else return null;};
			// GameObject to Vector3 (position).
			if (fromType == typeof(GameObject) && toType == typeof(Vector3))
				return (value) => { return value != null ? (value as GameObject).transform.position : Vector3.zero; };

			// Component to Vector3 (position).
			if (typeof(Component).RTIsAssignableFrom(fromType) && toType == typeof(Vector3))
				return (value) => { return value != null ? (value as Component).transform.position : Vector3.zero; };

			// GameObject to Quaternion (rotation).
			if (fromType == typeof(GameObject) && toType == typeof(Quaternion))
				return (value) => { return value != null ? (value as GameObject).transform.rotation : Quaternion.identity; };

			// Component to Quaternion (rotation).
			if (typeof(Component).RTIsAssignableFrom(fromType) && toType == typeof(Quaternion))
				return (value) => { return value != null ? (value as Component).transform.rotation : Quaternion.identity; };

			// Quaternion to Vector3 (Euler angles).
			if (fromType == typeof(Quaternion) && toType == typeof(Vector3))
				return (value) => ((Quaternion)value).eulerAngles;

			// Vector3 (Euler angles) to Quaternion.
			if (fromType == typeof(Vector3) && toType == typeof(Quaternion))
				return (value) => Quaternion.Euler((Vector3)value);

			// Vector2 to Vector3.
			if (fromType == typeof(Vector2) && toType == typeof(Vector3))
				return (value) => (Vector3)(Vector2)value;

			// Vector3 to Vector2.
			if (fromType == typeof(Vector3) && toType == typeof(Vector2))
				return (value) => (Vector2)(Vector3)value;

			return null;
		}
	}
}

	namespace NodeCanvas.Tasks.Actions
	{
		[Name("Get Property of Variable (mp)", -1)]
		[Category("✫ Script Control/Multiplatform")]
		[Description("Get a property of a variable and save it to the blackboard")]
		public class GetPropertyOfVariable_Multiplatform : ActionTask
		{
			[SerializeField]
			[BlackboardOnly]
			protected BBObjectParameter variable;

			[SerializeField]
			protected SerializedMethodInfo method;

			[SerializeField]
			[BlackboardOnly]
			protected BBObjectParameter returnValue;

			protected MethodInfo targetMethod { 
				get{ 
					if(method!=null)
						return method.Get();
					else return null;
				}
			}


			protected override string info
			{
				get
				{
					if (method == null)
						return "No Property Selected";

					if (targetMethod == null)
						return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString());

					var mInfo = targetMethod.IsStatic ? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : variable.ToString();
					return string.Format("{0} = {1}.{2}",returnValue,mInfo,targetMethod.Name);
				}
			}

			public override void OnValidate(ITaskSystem ownerSystem)
			{
				if (method != null && method.HasChanged())
					SetMethod(method.Get());

				if (method != null && method.Get() == null)
					Error(string.Format("Missing Property '{0}'",method.GetMethodString()));
			}

			protected override string OnInit()
			{
				if (method == null)
					return "No Property selected";

				if (targetMethod == null)
					return string.Format("Missing Property '{0}'",method.GetMethodString());

				return null;
			}

			protected override void OnExecute()
			{
				object value = null;
				if (targetMethod.IsStatic)
					value = targetMethod.Invoke(null, null);
				else if (variable.value == null)
					Error(string.Format("Null variable value '{0}'",variable));
				else
				{
					// Convert the source value to the method's type first.
					var varValue = variable.value;
					var converter = UConverter.Get(varValue.GetType(), targetMethod.RTReflectedOrDeclaredType());
					if (converter == null)
						Error(string.Format("Cannot convert from '{0}' to '{1}'",varValue.GetType().FullName,targetMethod.RTReflectedOrDeclaredType().FullName));
					else
						value = targetMethod.Invoke(converter(variable.value), null);
				}

				if (value != null && returnValue != null)
				{
					var converter = UConverter.Get(value.GetType(), returnValue.varType);
					if (converter == null)
						Error(string.Format("Cannot convert from '{0}' to '{1}'",value.GetType().FullName,returnValue.varType.FullName));
					else
						returnValue.value = converter(value);
				}

				EndAction();
			}

			void SetMethod(MethodInfo method)
			{
				if (method == null)
					return;

				this.method = new SerializedMethodInfo(method);

				var returnType = method.ReturnType;
				//arrayToList = returnType.IsArray && returnValue.value != null && returnValue.value.GetType().Implements(ICollection<ArrayElementType>)
				returnValue.SetType(returnType);
			}

			///----------------------------------------------------------------------------------------------
			///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

			protected override void OnTaskInspectorGUI()
			{

				if (!Application.isPlaying && GUILayout.Button("Select Property"))
				{
					var menu = new UnityEditor.GenericMenu();
					if (variable.value != null)
					{
						menu = EditorUtils.GetInstanceMethodSelectionMenu(variable.value.GetType(), typeof(object), typeof(object), SetMethod, 0, true, true, menu);
						menu.AddSeparator("/");
					}
					foreach (var t in TypePrefs.GetPreferedTypesList(typeof(object)))
					{
						menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 0, true, true, menu);
						menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 0, true, true, menu);
					}
				
					menu.ShowAsBrowser("Select Property", GetType());

					Event.current.Use();
				}

				if (targetMethod != null)
				{
					GUILayout.BeginVertical("box");
					UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedOrDeclaredType().FriendlyName());
					UnityEditor.EditorGUILayout.LabelField("Property", targetMethod.Name);
					UnityEditor.EditorGUILayout.LabelField("Property Type", targetMethod.ReturnType.FriendlyName());
					GUILayout.EndVertical();

					if (!targetMethod.IsStatic)
						Editor.BBParameterEditor.ParameterField("Variable", variable, true);

					Editor.BBParameterEditor.ParameterField("Save As", returnValue, true);
				}
			}
		
#endif
		}

			[Name("Set Property of Variable (mp)", -1)]
			[Category("✫ Script Control/Multiplatform")]
			[Description("Set a property of a variable and save it to the blackboard")]
			public class SetPropertyOfVariable_Multiplatform : ActionTask
			{
				[SerializeField]
				[BlackboardOnly]
				protected BBObjectParameter variable;

				[SerializeField]
				protected SerializedMethodInfo method;

				[SerializeField]
				[BlackboardOnly]
				protected BBObjectParameter setValue;

				protected MethodInfo targetMethod { 
					get{ 
						if(method!=null)
							return method.Get();
						else return null;
					}
				}


				protected override string info
				{
					get
					{
						if (method == null)
							return "No Property Selected";

						if (targetMethod == null)
							return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString());

						var mInfo = targetMethod.IsStatic ? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : variable.ToString();
						return string.Format("{0}.{1}={2}",mInfo,targetMethod.Name,setValue);
					}
				}

				public override void OnValidate(ITaskSystem ownerSystem)
				{
					if (method != null && method.HasChanged())
						SetMethod(method.Get());

					if (method != null && method.Get() == null)
						Error(string.Format("Missing Property '{0}'",method.GetMethodString()));
				}

				protected override string OnInit()
				{
					if (method == null)
						return "No Property selected";

					if (targetMethod == null)
						return string.Format("Missing Property '{0}'",method.GetMethodString());

					return null;
				}

				protected override void OnExecute()
				{	
					targetMethod.Invoke(targetMethod.IsStatic? null : variable.value, new object[]{setValue.value});
					EndAction();
				}

				void SetMethod(MethodInfo method)
				{
					if (method == null)
						return;

					this.method = new SerializedMethodInfo(method);
					setValue.SetType(method.GetParameters()[0].ParameterType);
				}

				///----------------------------------------------------------------------------------------------
				///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

				protected override void OnTaskInspectorGUI()
				{

					if (!Application.isPlaying && GUILayout.Button("Select Property"))
					{
						var menu = new UnityEditor.GenericMenu();
						if (variable.value != null)
						{
							menu = EditorUtils.GetInstanceMethodSelectionMenu(variable.value.GetType(), typeof(void), typeof(object), SetMethod, 1, true, false, menu);
							menu.AddSeparator("/");
						}
						foreach (var t in TypePrefs.GetPreferedTypesList(typeof(object)))
						{
							menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(void), typeof(object), SetMethod, 1, true, false, menu);
							menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(void), typeof(object), SetMethod, 1, true, false, menu);
						}
										
						menu.ShowAsBrowser("Select Property", GetType());

						Event.current.Use();
					}
					
					
					if (targetMethod != null)
					{
						GUILayout.BeginVertical("box");
						UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedOrDeclaredType().FriendlyName());
						UnityEditor.EditorGUILayout.LabelField("Property", targetMethod.Name);
						UnityEditor.EditorGUILayout.LabelField("Set Type", setValue.varType.FriendlyName());
						GUILayout.EndVertical();

						if (!targetMethod.IsStatic)
							Editor.BBParameterEditor.ParameterField("Variable", variable, true);

						Editor.BBParameterEditor.ParameterField("Set From", setValue, true);
					}else
					{
						Editor.BBParameterEditor.ParameterField("Variable", variable, true);
					}
				}

#endif
			}
		
		[Name("Get Field of Variable", -1)]
		[Category("✫ Script Control/Common")]
		[Description("Get a Field of a variable and save it to the blackboard")]
		public class GetFieldOfVariable : ActionTask
		{	
			[SerializeField]
			protected System.Type targetType;
			[SerializeField]
			protected string fieldName;
			[SerializeField] [BlackboardOnly]
			protected BBObjectParameter saveAs;
			
			private FieldInfo field;
			
			[SerializeField]
			[BlackboardOnly]
			protected BBObjectParameter variable;

			protected override string info{
				get
				{
					if (string.IsNullOrEmpty(fieldName))
						return "No Field Selected";
					return string.Format("{0} = {1}.{2}", saveAs.ToString(), agentInfo, fieldName);
				}
			}

			protected override string OnInit(){
				field = variable.value.GetType().RTGetField(fieldName);
				if (field == null)
					return "Missing Field: " + fieldName;
				return null;
			}


			protected override void OnExecute(){
				saveAs.value = field.GetValue(variable.value);
				EndAction();
			}

			///----------------------------------------------------------------------------------------------
			///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

			protected override void OnTaskInspectorGUI()
			{

				if (!Application.isPlaying && GUILayout.Button("Select Field"))
				{	
					System.Action<FieldInfo> FieldSelected = (field)=> {
						targetType = field.DeclaringType;
						fieldName = field.Name;
						saveAs.SetType(field.FieldType);
					};
					
					var menu = new UnityEditor.GenericMenu();
					if (variable.value != null)
					{	
						menu = EditorUtils.GetInstanceFieldSelectionMenu(variable.value.GetType(), typeof(object), FieldSelected, menu);
						menu.AddSeparator("/");
					}
					foreach (var t in TypePrefs.GetPreferedTypesList(typeof(object))){
						menu = EditorUtils.GetInstanceFieldSelectionMenu(t, typeof(object), FieldSelected, menu);
					}
				
					menu.ShowAsBrowser("Select Field", GetType());

					Event.current.Use();
				}


				if (variable.value != null && !string.IsNullOrEmpty(fieldName)){
					GUILayout.BeginVertical("box");
					UnityEditor.EditorGUILayout.LabelField("Type", variable.varType.FriendlyName());
					UnityEditor.EditorGUILayout.LabelField("Field", fieldName);
					UnityEditor.EditorGUILayout.LabelField("Field Type", saveAs.varType.FriendlyName() );
					GUILayout.EndVertical();
					Editor.BBParameterEditor.ParameterField("Variable", variable, true);
					NodeCanvas.Editor.BBParameterEditor.ParameterField("Save As", saveAs, true);
				}else
				{
					Editor.BBParameterEditor.ParameterField("Variable", variable, true);
				}
			}
		
#endif
		}

		[Name("Set Field of Variable ", -1)]
		[Category("✫ Script Control/Common")]
		[Description("Set a Field of a variable and save it to the blackboard")]
		public class SetFieldOfVariable : ActionTask
		{	
			[SerializeField]
			[BlackboardOnly]
			protected BBObjectParameter variable;

			[SerializeField]
			protected BBObjectParameter setValue;
			[SerializeField]
			protected System.Type targetType;
			[SerializeField]
			protected string fieldName;

			private FieldInfo field;

			protected override string info{
				get
				{
					if (string.IsNullOrEmpty(fieldName))
						return "No Field Selected";
					return string.Format("{0}.{1} = {2}", agentInfo, fieldName, setValue);
				}
			}

			protected override string OnInit(){
				field = variable.value.GetType().RTGetField(fieldName);
				if (field == null)
					return "Missing Field: " + fieldName;
				return null;
			}

			protected override void OnExecute(){
				field.SetValue(variable.value, setValue.value);
				EndAction();
			}

			///----------------------------------------------------------------------------------------------
			///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

			protected override void OnTaskInspectorGUI()
			{

				if (!Application.isPlaying && GUILayout.Button("Select Property"))
				{	
					System.Action<FieldInfo> FieldSelected = (field)=>{
						targetType = field.DeclaringType;
						fieldName = field.Name;
						setValue.SetType(field.FieldType);
					};
						
					var menu = new UnityEditor.GenericMenu();
					if (variable.value != null)
					{
						menu = EditorUtils.GetInstanceFieldSelectionMenu(variable.varType.GetType(),typeof(object), FieldSelected, menu);
					}
					foreach (var t in TypePrefs.GetPreferedTypesList(typeof(object))){
							menu = EditorUtils.GetInstanceFieldSelectionMenu(t, typeof(object), FieldSelected, menu);
						}
										
					menu.ShowAsBrowser("Select Property", GetType());

					Event.current.Use();
				}
					
					
				if (variable.value != null && !string.IsNullOrEmpty(fieldName)){
					GUILayout.BeginVertical("box");
					UnityEditor.EditorGUILayout.LabelField("Type", agentType.Name);
					UnityEditor.EditorGUILayout.LabelField("Field", fieldName);
					UnityEditor.EditorGUILayout.LabelField("Field Type", setValue.varType.FriendlyName() );
					GUILayout.EndVertical();
					Editor.BBParameterEditor.ParameterField("Variable", variable, true);
					NodeCanvas.Editor.BBParameterEditor.ParameterField("Set Value", setValue);
				}else
				{
					Editor.BBParameterEditor.ParameterField("Variable", variable, true);
				}
			}

#endif
		}
		
		[Name("Execute Variable's Function (mp)")]
		[Category("✫ Script Control/Multiplatform")]
		[Description("Execute a function on a script and save the return if any. If function is an IEnumerator it will execute as a coroutine.")]
		public class ExecuteVariableFunction_Multiplatform : ActionTask {

			[SerializeField]
			[BlackboardOnly]
			protected BBObjectParameter variable;
			
			[SerializeField]
			protected SerializedMethodInfo method;
			[SerializeField]
			protected List<BBObjectParameter> parameters = new List<BBObjectParameter>();
			[SerializeField]
			protected List<bool> parameterIsByRef = new List<bool>();
			[SerializeField] [BlackboardOnly]
			protected BBObjectParameter returnValue;

			private object[] args;
			private bool routineRunning;

			private MethodInfo targetMethod{
				get {return method != null? method.Get() : null;}
			}

			protected override string info{
				get
				{
					if (method == null){
						return "No Method Selected";
					}
					if (targetMethod == null){
						return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString() );
					}

					var returnInfo = targetMethod.ReturnType == typeof(void) || targetMethod.ReturnType == typeof(IEnumerator)? "" : returnValue.ToString() + " = ";
					var paramInfo = "";
					for (var i = 0; i < parameters.Count; i++){
						paramInfo += (i != 0? ", " : "") + parameters[i].ToString();
					}
					var mInfo = targetMethod.IsStatic? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : agentInfo;
					return string.Format("{0}{1}.{2}({3})", returnInfo, mInfo, targetMethod.Name, paramInfo );
				}
			}

			public override void OnValidate(ITaskSystem ownerSystem){
				if (method != null && method.HasChanged()){	
					SetMethod(method.Get());
				}
				if (method != null && method.Get() == null){
					Error( string.Format("Missing Method '{0}'", method.GetMethodString()) );
				}
			}

			//store the method info on init
			protected override string OnInit(){
				if (method == null){
					return "No Method selected";
				}
				if (targetMethod == null){
					return string.Format("Missing Method '{0}'", method.GetMethodString());
				}
			
				if (args == null){
					args = new object[parameters.Count];
				}

				if (parameterIsByRef.Count != parameters.Count){
					parameterIsByRef = parameters.Select(p => false).ToList();
				}

				return null;
			}


			//do it by calling delegate or invoking method
			protected override void OnExecute(){

				for (var i = 0; i < parameters.Count; i++){
					args[i] = parameters[i].value;
				}			

				var instance = targetMethod.IsStatic? null : variable.value;
				if (targetMethod.ReturnType == typeof(IEnumerator)){
					StartCoroutine( InternalCoroutine( (IEnumerator)targetMethod.Invoke(instance, args) ));
					return;
				}

				returnValue.value = targetMethod.Invoke(instance, args);

				for (var i = 0; i < parameters.Count; i++){
					if (parameterIsByRef[i]){
						parameters[i].value = args[i];
					}
				}

				EndAction();
			}


			protected override void OnStop(){
				routineRunning = false;
			}


			IEnumerator InternalCoroutine(IEnumerator routine){
				routineRunning = true;
				while(routineRunning && routine.MoveNext()){
					if (routineRunning == false){
						yield break;
					}
					yield return routine.Current;
				}

				if (routineRunning){
					EndAction();
				}
			}


			void SetMethod(MethodInfo method){
				if (method == null){
					return;
				}
				this.method = new SerializedMethodInfo(method);
				this.parameters.Clear();
				var methodParameters = method.GetParameters();
				for (var i = 0; i < methodParameters.Length; i++){
					var p = methodParameters[i];
					var pType = p.ParameterType;
					var newParam = new BBObjectParameter( pType.IsByRef? pType.GetElementType() : pType ){bb = blackboard};
					if (p.IsOptional){
						newParam.value = p.DefaultValue;
					}
					parameters.Add(newParam);
					parameterIsByRef.Add(pType.IsByRef);
				}

				if (method.ReturnType != typeof(void) && targetMethod.ReturnType != typeof(IEnumerator)){
					this.returnValue = new BBObjectParameter(method.ReturnType){bb = blackboard};
				} else {
					this.returnValue = null;
				}
			}


			///----------------------------------------------------------------------------------------------
			///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR
		
			protected override void OnTaskInspectorGUI(){

				if (!Application.isPlaying && GUILayout.Button("Select Method")){

					var menu = new UnityEditor.GenericMenu();
					if (variable.value!= null){
						menu = EditorUtils.GetInstanceMethodSelectionMenu(variable.value.GetType(), typeof(object), typeof(object), SetMethod, 10, false, false, menu);
						
						menu.AddSeparator("/");
					}

					foreach (var t in TypePrefs.GetPreferedTypesList(typeof(object))){
						menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 10, false, false, menu);
						if (typeof(UnityEngine.Component).IsAssignableFrom(t)){
							menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 10, false, false, menu);
						}
					}
					menu.ShowAsBrowser("Select Method", this.GetType());
					Event.current.Use();
				}

				Editor.BBParameterEditor.ParameterField("Variable", variable, true);
				if (targetMethod != null){
					GUILayout.BeginVertical("box");
					UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedOrDeclaredType().FriendlyName());
					UnityEditor.EditorGUILayout.LabelField("Method", targetMethod.Name);
					UnityEditor.EditorGUILayout.LabelField("Returns", targetMethod.ReturnType.FriendlyName());

					if (targetMethod.ReturnType == typeof(IEnumerator)){
						GUILayout.Label("<b>This will execute as a Coroutine!</b>");
					}

					GUILayout.EndVertical();

					var paramNames = targetMethod.GetParameters().Select(p => p.Name.SplitCamelCase() ).ToArray();
					for (var i = 0; i < paramNames.Length; i++){
						NodeCanvas.Editor.BBParameterEditor.ParameterField(paramNames[i], parameters[i]);
					}

					if (targetMethod.ReturnType != typeof(void) && targetMethod.ReturnType != typeof(IEnumerator)){
						NodeCanvas.Editor.BBParameterEditor.ParameterField("Save Return Value", returnValue, true);
					}
				}
			}

		#endif
		}
		
	}