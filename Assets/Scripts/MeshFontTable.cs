using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class MeshFontTable : ScriptableObject
{
    [Serializable]
    public class MeshFont
    {
        public uint unicode;
        public Sprite sprite;
    }

    public List<MeshFont> meshFonts;

    private Dictionary<uint, Sprite> _lookUpTable;

    public Sprite this[uint unicode]
    {
        get
        {
            if (_lookUpTable == null)
            {
                _lookUpTable = new Dictionary<uint, Sprite>();
                InitLookUpTable();
            }
            return _lookUpTable.GetValueOrDefault(unicode);
        }
        set
        {
            if (_lookUpTable == null)
            {
                _lookUpTable = new Dictionary<uint, Sprite>();
                InitLookUpTable();
            }

            if (!_lookUpTable.TryAdd(unicode, value))
            {
                if (_lookUpTable[unicode] == value) return;
                _lookUpTable[unicode] = value;
                foreach (var meshFont in meshFonts.Where(meshFont => meshFont.unicode == unicode))
                {
                    meshFont.sprite = value;
                }
            }
            else
            {
                meshFonts.Add(new MeshFont
                {
                    unicode = unicode,
                    sprite = value
                });
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }

    private void InitLookUpTable()
    {
        _lookUpTable.Clear();
        foreach (var meshFont in meshFonts)
        {
            _lookUpTable.Add(meshFont.unicode, meshFont.sprite);
        }
    }

    private void OnValidate()
    {
        _lookUpTable ??= new Dictionary<uint, Sprite>();
        InitLookUpTable();
    }
}