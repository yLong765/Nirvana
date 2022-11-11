using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class GenericMenuItem
    {
        public string name;
        public Action onClick1;
        public Action<object> onClick2;
        public object data;

        public GenericMenuItem(string name, Action onClick1)
        {
            this.name = name;
            this.onClick1 = onClick1;
        }

        public GenericMenuItem(string name, Action<object> onClick2, object data)
        {
            this.name = name;
            this.onClick2 = onClick2;
            this.data = data;
        }
    }
    
    public class GenericMenuPopup : PopupWindowContent
    {
        private List<GenericMenuItem> _items = new List<GenericMenuItem>();
        private string _title;
        private Vector2 _scrollPos;

        public GenericMenuPopup(string title)
        {
            _title = title;
        }

        public void AddItem(string name, Action onClickCallback)
        {
            _items.Add(new GenericMenuItem(name, onClickCallback));
        }

        public void AddItem(string name, Action<object> onClickCallback, object data)
        {
            _items.Add(new GenericMenuItem(name, onClickCallback, data));
        }

        public void Show()
        {
            var e = Event.current;
            PopupWindow.Show(new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0), this);
        }

        private void OnClick(GenericMenuItem item)
        {
            if (item.data == null)
            {
                item.onClick1?.Invoke();
            }
            else
            {
                item.onClick2?.Invoke(item.data);
            }
            editorWindow.Close();
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label(_title, EditorStyles.boldLabel);
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (GUILayout.Button(item.name, EditorStyles.toolbarButton))
                {
                    OnClick(item);
                }
            }
            GUILayout.EndScrollView();
        }
    }
}