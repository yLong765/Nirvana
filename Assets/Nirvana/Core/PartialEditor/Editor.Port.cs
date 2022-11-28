using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public partial class Port
    {
        private int _order;
        private Rect _rect;
        private int _linkCount;
        private int _maxLinkCount;
        private bool _canDragLink;
        private PortType _portType;

        public int order
        {
            get => _order;
            set => _order = value;
        }
        
        public Rect rect
        {
            get => _rect;
            set => _rect = value;
        }

        public bool isLink => linkCount > 0;

        public int linkCount
        {
            get => _linkCount;
            set => _linkCount = value;
        }
        
        public int maxLinkCount
        {
            get => _maxLinkCount;
            set => _maxLinkCount = value;
        }
        
        public bool canDragLink
        {
            get => _canDragLink;
            set => _canDragLink = value;
        }
        
        public PortType portType
        {
            get => _portType;
            set => _portType = value;
        }
    }
}