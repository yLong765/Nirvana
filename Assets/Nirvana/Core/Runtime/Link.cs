using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nirvana
{
    public class Link
    {
        private Node _sourceNode;
        private Node _targetNode;
        private string _sourcePortID;
        private string _targetPortID;
        private Port _sourcePort;
        private Port _targetPort;

        /// <summary>
        /// 源头Port
        /// </summary>
        [JsonIgnore]
        public Port sourcePort
        {
            get => _sourcePort ??= _sourceNode.GetOutPort(sourcePortId);
            set => _sourcePort = value;
        }

        /// <summary>
        /// 目标Port
        /// </summary>
        [JsonIgnore]
        public Port targetPort
        {
            get => _targetPort ??= _targetNode.GetInPort(_targetPortID);
            set => _targetPort = value;
        }

        /// <summary>
        /// 源头Node
        /// </summary>
        public Node sourceNode
        {
            get => _sourceNode;
            set => _sourceNode = value;
        }

        /// <summary>
        /// 目标Node
        /// </summary>
        public Node targetNode
        {
            get => _targetNode;
            set => _targetNode = value;
        }

        /// <summary>
        /// 源头Port的Id
        /// </summary>
        public string sourcePortId
        {
            get => _sourcePort != null ? _sourcePort.ID : _sourcePortID;
            set => _sourcePortID = value;
        }

        /// <summary>
        /// 目标Port的Id
        /// </summary>
        public string targetPortId
        {
            get => _targetPort != null ? _targetPort.ID : _targetPortID;
            set => _targetPortID = value;
        }

        /// <summary>
        /// 设置源头Port数据
        /// </summary>
        public void SetSourcePort(Port port)
        {
            sourcePort = port;
            sourceNode = port.node;
            sourcePortId = port.ID;
            sourcePort.node.outLinks.Add(this);
            sourcePort.linkCount++;
        }

        /// <summary>
        /// 设置目标Port数据
        /// </summary>
        public void SetTargetPort(Port port)
        {
            targetPort = port;
            targetNode = port.node;
            targetPortId = port.ID;
            targetPort.node.inLinks.Add(this);
            targetPort.linkCount++;
        }

        /// <summary>
        /// 刷新源头Port数据（加载后数据丢失，还原数据）
        /// </summary>
        public void RefreshSourcePort()
        {
            _sourcePort = null;
            if (sourcePort != null)
            {
                sourcePort.linkCount++;
            }
        }

        /// <summary>
        /// 刷新目标Port数据（加载后数据丢失，还原数据）
        /// </summary>
        public void RefreshTargetPort()
        {
            _targetPort = null;
            if (targetPort != null)
            {
                targetPort.linkCount++;
            }
        }

        /// <summary>
        /// 运行时初始化绑定所有的Port
        /// </summary>
        public void BindPorts()
        {
            switch (sourcePort)
            {
                case FlowOutPort foutPort when targetPort is FlowInPort finPort:
                    foutPort.BindTo(finPort);
                    break;
                case OutPort outPort when targetPort is InPort inPort:
                    inPort.BindTo(outPort);
                    break;
            }
        }

        /// <summary>
        /// 创建Link
        /// </summary>
        public static Link Create(Port sourcePort, Port targetPort)
        {
            var newLink = new Link();
            newLink.SetSourcePort(sourcePort);
            newLink.SetTargetPort(targetPort);
            if (sourcePort is InPort inPort && targetPort is OutPort outPort)
            {
                inPort.BindTo(outPort);
            }
            return newLink;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 绘制Link的InspectorGUI
        /// </summary>
        public void DrawInspectorGUI()
        {
            GUILayout.Label("Link Node: " + sourcePort.node.title + " link to " + sourcePort.node.title);
        }
#endif
    }
}