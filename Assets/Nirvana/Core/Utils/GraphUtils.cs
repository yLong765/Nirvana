using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public static class GraphUtils
    {
        public static bool willSetDirty = false;
        public static bool allowClick = true;
        public static bool isInspectorPanel = false;
        public static bool showBlackboardPanel = true;

        private static List<WeakReference<Node>> _activeNodes = new List<WeakReference<Node>>();

        public static List<Node> activeNodes
        {
            get
            {
                var result = new List<Node>();
                foreach (var weakNode in _activeNodes)
                {
                    if (weakNode.TryGetTarget(out Node target))
                    {
                        result.Add(target);
                    }
                }

                return result;
            }
            set
            {
                if (value != null)
                {
                    _activeNodes = new List<WeakReference<Node>>(value.Count);
                    foreach (var node in value)
                    {
                        _activeNodes.Add(new WeakReference<Node>(node));
                    }
                }
                else
                {
                    _activeNodes = new List<WeakReference<Node>>();
                }
            }
        }

        public static void AddActiveNode(Node node)
        {
            _activeNodes.Add(new WeakReference<Node>(node));
        }

        public static void RemoveActiveNode(Node node)
        {
            foreach (var weakNode in _activeNodes)
            {
                if (weakNode.TryGetTarget(out Node target) && ReferenceEquals(target, node))
                {
                    _activeNodes.Remove(weakNode);
                    break;
                }
            }
        }
    }
}