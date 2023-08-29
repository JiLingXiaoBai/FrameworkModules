using UnityEngine;

namespace JLXB.Framework.GameDataBase
{
    public class GameDataTable<T> : ScriptableObject where T : GameData
    {
        [SerializeField] public T[] table;

        public int Count => table.Length;
    }
}