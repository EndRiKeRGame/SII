using System;
using System.Collections.Generic;
using Common.Enums;
using UnityEngine;

namespace Common.Configs
{
    [CreateAssetMenu(fileName = "TreeConfig", menuName = "Game/Tree Config")]
    public class MatrixTreeConfig : ScriptableObject
    {
        public ItemTag RootTag;
        public List<NodeConnection> Connections = new List<NodeConnection>();
    }

    [Serializable]
    public class NodeConnection
    {
        public ItemTag ParentTag;
        public ItemTag ChildTag;
        public int Weight = 1;
    }
}
