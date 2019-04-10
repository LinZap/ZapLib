using System;

namespace ZapLib
{
    /// <summary>
    /// Expend SQL Script，將依據封裝的陣列把 SQL prepare stament 參數 @var 拆解成 @var0, @var1, @var2, ...
    /// </summary>
    public class ExpParam<T>: IExpParam
    {
        private object[] data;
        /// <summary>
        /// 建構子，只允許給予陣列
        /// </summary>   
        public ExpParam(object[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// 取得展開個別元素的資料型態
        /// </summary>
        /// <returns></returns>
        public Type GetEleType() => typeof(T);

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <returns></returns>
        public object[] GetData() => data;
    }
}
