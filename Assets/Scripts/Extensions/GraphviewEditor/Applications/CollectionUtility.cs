using System.Collections.Generic;

namespace DATN2.GraphviewEditor.Applications
{
    public static class CollectionUtility//bổ sung method vào method có sẵn
    {
        public static void AddItem<T, K>(this SerializableDictionary<T, List<K>> serializableDictionary, T key, K value)
        {
            if (serializableDictionary.ContainsKey(key))
            {
                serializableDictionary[key].Add(value);

                return;
            }

            serializableDictionary.Add(key, new List<K>() { value });
        }
    }
}
