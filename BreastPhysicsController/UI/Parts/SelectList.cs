using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController;

namespace BreastPhysicsController.UI.Parts
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

        public SelectList(string[] list,string emptyString,bool useEmptyStringAlways, float buttonWidth=0, float buttonHeight=0, float listWidth = 0,float listHeight = 0)
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


                    if (_width1 <= 0 && _height1 <= 0)
                    {
                        if (GUILayout.Button(buttonLabel))
                        {
                            _show = true;
                        }
                    }
                    else if (_width1 <= 0)
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Height(_height1)))
                        {
                            _show = true;
                        }
                    }
                    else if (_height1 <= 0)
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Width(_width1)))
                        {
                            _show = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Width(_width1), GUILayout.Height(_height1)))
                        {
                            _show = true;
                        }
                    }
                }
                else
                {

                    if (_width2<=0 && _height2 <= 0)
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true);
                    }
                    else if(_width2<=0)
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true,GUILayout.Height(_height2));
                    }
                    else if(_height2<=0)
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Height(_width2));
                    }
                    else
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Width(_width2), GUILayout.Height(_height2));
                    }

                    int selected = -1;
                    selected = GUILayout.SelectionGrid(selected, _list, 1);

                    if (selected>-1)
                    {
                        changed = true;
                        _selectedIndex = selected;
                        _show = false;
                    }
                    GUILayout.Space(20);
                    GUILayout.EndScrollView();
                }
                
            }

            return changed;
        }
    }


}
