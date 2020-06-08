using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController;

namespace BreastPhysicsController.UI.Parts
{
    public class CharaSelect
    {
        protected float _width1;
        protected float _height1;
        protected float _width2;
        protected float _height2;

        public bool _show;
        public bool changed;

        protected Vector2 _scroll;
        protected Dictionary<int, ParamCharaController> _controllers;
        protected int _selectedId;
        protected string _emptyString;

        public CharaSelect(Dictionary<int, ParamCharaController> controllers, string emptyString, float buttonWidth=0, float buttonHeight=0, float listWidth=0, float listHeight=0)
        {
            _emptyString = emptyString;
            _show = false;
            _controllers = controllers;
            _scroll = new Vector2(0, 0);
            _selectedId = 0;
            _width1 = buttonWidth;
            _width2 = listWidth;
            _height1 = buttonHeight;
            _height2 = listHeight;
        }

        public virtual bool Draw()
        {

            if (_controllers == null || _controllers.Count() == 0)
            {

                GUILayout.Button(_emptyString);

            }
            else
            {
                if (!_show)
                {
                    if (!_controllers.ContainsKey(_selectedId))
                    {
                        _selectedId = _controllers.Values.ElementAt(0).controllerID;
                    }
                    string buttonLabel = _controllers[_selectedId].ChaFileControl.parameter.fullname;

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
                        if (GUILayout.Button(buttonLabel, GUILayout.Height(_width1)))
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
                    if (_width2 <= 0 && _height2 <= 0)
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true);
                    }
                    else if (_width2 <= 0)
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Height(_height2));
                    }
                    else if (_height2 <= 0)
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Height(_width2));
                    }
                    else
                    {
                        _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Width(_width2), GUILayout.Height(_height2));
                    }

                    int selected = -1;
                    ParamCharaController[] buffControllers = _controllers.Values.ToArray();
                    string[] names = buffControllers.Select(x => x.ChaFileControl.parameter.fullname).ToArray();
                    selected = GUILayout.SelectionGrid(selected, names, 1);

                    if (selected > -1)
                    {
                        changed = true;
                        _selectedId = buffControllers[selected].controllerID;
                        _show = false;
                    }
                    GUILayout.Space(20);
                    GUILayout.EndScrollView();
                }

            }

            return changed;
        }

        public int GetSelectedId()
        {
            return _selectedId;
        }

        public ParamCharaController GetSelectedController()
        {
            if(_controllers.ContainsKey(_selectedId)) return _controllers[_selectedId];
            return null;
        }
    }


}
