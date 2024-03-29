using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public static class GraphUtils
    {
        /// <summary>
        /// 是否需要SetDirty
        /// </summary>
        public static bool willSetDirty = false;
        /// <summary>
        /// 是否需要刷新
        /// </summary>
        public static bool willRepaint = false;
        /// <summary>
        /// 是否允许点击
        /// </summary>
        public static bool allowClick = true;

        public static Graph currentGraph { get; set; }

        //------Node相关------
        
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
                GUIUtility.keyboardControl = 0;
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
        
        //------链接相关------
        
        private static WeakReference<Link> _activeLink = new WeakReference<Link>(null);
        public static Link activeLink
        {
            get
            {
                if (_activeLink.TryGetTarget(out Link link))
                {
                    return link;
                }

                return null;
            }
            set
            {
                GUIUtility.keyboardControl = 0;
                _activeLink = new WeakReference<Link>(value);
            }
        }

        public static void Select<T>(T select)
        {
            if (select is Node node)
            {
                if (activeNodes.Count > 0)
                {
                    if (!activeNodes.Contains(node))
                    {
                        ClearSelect();
                        AddActiveNode(node);
                    }
                }
                else
                {
                    AddActiveNode(node);
                }
            }

            if (select is List<Node> nodes)
            {
                activeNodes = nodes;
            }
            
            if (select is Link link)
            {
                activeLink = link;
            }
        }
        
        public static void ClearSelect()
        {
            activeNodes = null;
            activeLink = null;
        }
    }
}