using System;
using System.Collections.Generic;
using System.Text;

namespace GenericsTest
{
    /// <summary>
    /// Triggers an exception.
    /// </summary>
    /// <typeparam name="TKey">key</typeparam>
    /// <typeparam name="TValue">Value</typeparam>
    public class Table<TKey, TValue> //: Dictionary<TKey, TValue>
    {
        /// <summary>
        /// Apas the specified bepa.
        /// </summary>
        /// <param name="bepa">The bepa.</param>
        public void apa(out int bepa)
        {
            bepa = 7;
        }

        /// <summary>
        /// Tests the specified ref out.
        /// </summary>
        /// <param name="refOut">The ref out.</param>
        public void test(ref Class<int, int, int>.cEnum refOut)
        {
        }
    }

    /// <summary>
    /// publik statik klass SClass
    /// </summary>
    /// <typeparam name="X"></typeparam>
    /// <typeparam name="Y"></typeparam>
    /// <typeparam name="Z"></typeparam>
    public class Class<X, Y, Z> : IGen<X>
    {
        /// <summary>
        /// Torsk
        /// </summary>
        public enum cEnum
        {
            /// <summary>
            /// hoppsan
            /// </summary>
            hejsan = 8
        };

        /// <summary>
        /// Tests the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void Test(X type) { }

    }

    /// <summary>
    /// public interface IGen(T)
    /// </summary>
    /// <typeparam name="T">Typ kommentar.</typeparam>
    public interface IGen<T>
    {
        /// <summary>
        /// Tests the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        void Test(T type);
    }

    /// <summary>
    /// Hej hopp apa.
    /// </summary>
    /// <typeparam name="X">apa X</typeparam>
    /// <typeparam name="Y">apa Y</typeparam>
    /// <param name="o1">apa o1</param>
    /// <param name="o2">apa o2</param>
    public delegate void apa<X,Y>(X o1, Y o2);

    /// <summary>
    /// Sss
    /// </summary>
    /// <typeparam name="X">typeparam name="X"</typeparam>
    /// <typeparam name="Y"></typeparam>
    /// <typeparam name="Z"></typeparam>
    public struct SStruct<X, Y, Z>
    {
        /// <summary>
        /// haj
        /// </summary>
        public enum sEnum { 
            /// <summary>
            /// hopp
            /// </summary>
            hej = 7 };
        /// <summary>
        /// gurka
        /// </summary>
        public X apa;
        /// <summary>
        /// banan
        /// </summary>
        public Y bepa;
        /// <summary>
        /// citron
        /// </summary>
        public Z depa;
    }



    /// <summary>
    /// Ja ja class1
    /// </summary>
    /// <typeparam name="T">lass öäå</typeparam>
    public class Class1<T> where T : Comparer<T>
    {
        /// <summary>
        /// public class EventClass&lt;TT&gt; : EventArgs { }
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        public class EventClass<TT> : EventArgs { }

        /// <summary>
        /// Statisk konstruktor
        /// </summary>
        static Class1() { }

        /// <summary>
        /// EventHandler
        /// </summary>
        public event EventHandler<EventClass<T>> theEvent;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public apa<T, int> theProperty { get { return null; } }

        /// <summary>
        /// Gurkspad i långa banor.
        /// </summary>
        public apa<T,string> gurka;

        /// <summary>
        /// operator ==
        /// </summary>
        /// <param name="t"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static T operator ==(Class1<T> t, T y) { return default(T); }

        /// <summary>
        /// operator !=
        /// </summary>
        /// <param name="t"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static T operator !=(Class1<T> t, T y) { return default(T); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static implicit operator Class1<T>(T[] f)
        {
            return null;
        }


        /// <summary>
        /// public List 
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public List<TOutput> ConvertAll<TOutput>(
            Converter<T, TOutput> converter)
        {
            return null;
        }

        /// <summary>
        /// the <see cref="Int32"/> with the specified ERROR.
        /// </summary>
        /// <value></value>
        public int this[Converter<T, int> t]
        {
            get { return 0; }
        }

                /// <summary>
        /// Gets the T.
        /// </summary>
        /// <returns></returns>
        public Class1<T> GetT()
        {
            return null;
        }

        /// <summary>
        /// Test field
        /// </summary>
        public SStruct<T, T, T> test;

        /// <summary>
        /// Gets the A.
        /// </summary>
        /// <param name="Apa">The apa.</param>
        /// <returns></returns>
        public Action<TT> GetA<TT>(TT Apa)
        {
            return null;
        }
    }
}
