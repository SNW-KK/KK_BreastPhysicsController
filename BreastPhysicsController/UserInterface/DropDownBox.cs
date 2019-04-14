using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController;

namespace BreastPhysicsController
{
    public class DropDownBox
    {
        float _width;
        float _height;
        float _maxHeight;

        public bool _show;
        public bool changed;

        Vector2 _scroll;
        string[] _list;
        public int[] _id;
        string _emptyString;
        int _selectedIndex;


        public DropDownBox(string[] list,int[] id,string emptyString, float width, float height,float maxHeight)
        {
            _show = false;
            _list = list;
            _id = id;
            _emptyString = emptyString;
            _scroll = new Vector2(0, 0);
            _selectedIndex = 0;
            _width = width;
            _height = height;
            _maxHeight = maxHeight;
        }

        public void SetList(string[] list,int[] id)
        {
            _list = list;
            _id = id;
            changed = false;
            _selectedIndex = 0;
        }

        public int GetSelectedId()
        {
            if (_id != null && _selectedIndex < _id.Length)
            {
                return _id[_selectedIndex];
            }
            return 0;
        }

        public bool Draw()
        {
            changed = false;

            GUILayout.BeginVertical();

            if (_list==null || _list.Length==0)
            {
                
                GUILayout.Button(_emptyString);
                
            }
            else
            {
                if (!_show)
                {
                    if(_width>0)
                    {
                        if (GUILayout.Button(_list[_selectedIndex], GUILayout.Width(_width), GUILayout.Height(_height)))
                        {
                            _show = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(_list[_selectedIndex], GUILayout.Height(_height)))
                        {
                            _show = true;
                        }
                    }
                }
                else
                {
                    if(_width>0) _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Width(_width + 20), GUILayout.Height(_list.Count() * _height + 20));
                    else _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(_list.Count() * _height + 20));

                    int selected = -1;
                    selected = GUILayout.SelectionGrid(selected, _list, 1, GUILayout.Height(_list.Count() * _height));
                    
                    if (selected>-1)
                    {
                        changed = true;
                        _selectedIndex = selected;
                        _show = false;
                    }
                    GUILayout.EndScrollView();
                }
                
            }

            GUILayout.EndVertical();

            return changed;
        }
    }


}
