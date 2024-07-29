using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu]
    public class MeshFontTable : ScriptableObject
    {
        [Serializable]
        public class MeshFont
        {
            public uint unicode;
            public Sprite sprite;
        }

        public MeshFont[] meshFonts;


        private Dictionary<uint, Sprite> _lookUpTable;

        public Sprite this[uint unicode]
        {
            get => _lookUpTable[unicode];
            set => _lookUpTable[unicode] = value;
        }


        public void InitLookupTable()
        {
            _lookUpTable = new Dictionary<uint, Sprite>();

            foreach (var meshFont in meshFonts)
            {
                _lookUpTable.Add(meshFont.unicode, meshFont.sprite);
            }
        }
    }
}