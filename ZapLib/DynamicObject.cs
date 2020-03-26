using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib
{
    /// <summary>
    /// 允許在執行期間才新增成員的實體物件，等價實體的 Class，而非 dynamic。因此可適應全部 reflaction 功能
    /// </summary>
    public class DynamicObject
    {
        private Dictionary<string, object> PropMap;
        private string TypeName;
        private TypeBuilder TypeBuilder;

        /// <summary>
        /// 實體物件
        /// </summary>
        public object Core { get; private set; }
        /// <summary>
        /// 實體物件的 Type 物件
        /// </summary>
        public Type CoreType { get; private set; }

        /// <summary>
        /// 建構子，定義全新物件類別
        /// </summary>
        /// <param name="TypeName">類別名稱</param>
        public DynamicObject(string TypeName)
        {
            PropMap = new Dictionary<string, object>();
            this.TypeName = TypeName;
            CreateTypeBuilder();
        }

        /// <summary>
        /// 將類別建立實例 (new 出來)
        /// </summary>
        /// <returns>建立的實體物件</returns>
        public object CreateObject()
        {
            CoreType = TypeBuilder.CreateType();
            Core = Activator.CreateInstance(CoreType);
            foreach (KeyValuePair<string, object> kv in PropMap) SetProperty(kv.Key, kv.Value);
            return Core;
        }


        private void CreateTypeBuilder()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
            TypeBuilder = moduleBuilder.DefineType(TypeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, null);
        }

        /// <summary>
        /// 設定實體物件的成員數值
        /// </summary>
        /// <param name="PropertyName">成員名稱</param>
        /// <param name="Value">成員數值</param>
        public void SetProperty(string PropertyName, object Value)
        {
            var prop = CoreType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (prop != null) prop.SetValue(Core, Value, null);
        }

        /// <summary>
        /// 定義並新增實體類別的成員
        /// </summary>
        /// <param name="PropertyName">成員名稱</param>
        /// <param name="PropertyType">成員類型，例如: typeof(string)</param>
        /// <param name="DefaultValue">成員的預設數值</param>
        public void CreateProperty(string PropertyName, Type PropertyType, object DefaultValue = null)
        {
            PropMap.Add(PropertyName, DefaultValue);

            var backingFieldBuilder = TypeBuilder.DefineField("_" + PropertyName, PropertyType, FieldAttributes.Private);
            var propertyBuilder = TypeBuilder.DefineProperty(PropertyName, PropertyAttributes.HasDefault, PropertyType, null);
            // Build getter
            var getterMethodBuilder = TypeBuilder.DefineMethod("get_" + PropertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, PropertyType, Type.EmptyTypes);
            var getterIl = getterMethodBuilder.GetILGenerator();
            getterIl.Emit(OpCodes.Ldarg_0);
            getterIl.Emit(OpCodes.Ldfld, backingFieldBuilder);
            getterIl.Emit(OpCodes.Ret);

            // Build setter
            var setterMethodBuilder = TypeBuilder.DefineMethod("set_" + PropertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { PropertyType });
            var setterIl = setterMethodBuilder.GetILGenerator();
            setterIl.Emit(OpCodes.Ldarg_0);
            setterIl.Emit(OpCodes.Ldarg_1);
            setterIl.Emit(OpCodes.Stfld, backingFieldBuilder);
            setterIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethodBuilder);
            propertyBuilder.SetSetMethod(setterMethodBuilder);
        }

    }
}
