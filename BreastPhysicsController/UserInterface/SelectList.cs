using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController;

namespace BreastPhysicsController
{
    public class SelectList
    {
        protected float _width1;
        protected float _height1;
        protected float _width2;
        protected float _height2;

        public bool _show;
        public bool changed;
        public bool _useEmptyStringAlways;

        protected Vector2 _scroll;
        protected string[] _list;
        protected int _selectedIndex;
        protected string _emptyString;

        public SelectList(string[] list,string emptyString,bool useEmptyStringAlways, float buttonWidth, float buttonHeight,float listWidth,float listHeight)
        {
            _emptyString = emptyString;
            _useEmptyStringAlways = useEmptyStringAlways;
            _show = false;
            _list = list;
            _scroll = new Vector2(0, 0);
            _selectedIndex = 0;
            _width1 = buttonWidth;
            _width2 = listWidth;
            _height1 = buttonHeight;
            _height2 = listHeight;
        }

        public virtual bool Draw()
        {

            if (_list==null || _list.Length==0)
            {
                
                GUILayout.Button(_emptyString);
                
            }
            else
            {
                if (!_show)
                {
                    string buttonLabel;
                    if (_useEmptyStringAlways) buttonLabel = _emptyString;
                    else buttonLabel = _list[_selectedIndex];

                    if (_width1 > 0)
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Width(_width1), GUILayout.Height(_height1)))
                        {
                            _show = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Height(_height1)))
                        {
                            _show = true;
                        }
                    }
                }
                else
                {
                    if(_width2>0) _scroll = GUILayout.BeginScrollView(_scroll, false,true,GUILayout.Width(_width2+15), GUILayout.Height(_height2));
                    else _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(_list.Count() * _height1));

                    int selected = -1;
                    selected = GUILayout.SelectionGrid(selected, _list, 1, GUILayout.Width(_width2),GUILayout.Height(_list.Count() * (_height1)));
                    
                    if (selected>-1)
                    {
                        changed = true;
                        _selectedIndex = selected;
                        _show = false;
                    }
                    GUILayout.Space(_list.Count()*1);
                    GUILayout.EndScrollView();
                }
                
            }

            return changed;
        }
    }


}
