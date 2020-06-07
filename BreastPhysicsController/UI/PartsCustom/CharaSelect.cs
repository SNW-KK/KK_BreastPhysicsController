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

        public CharaSelect(Dictionary<int, ParamCharaController> controllers, string emptyString, float buttonWidth, float buttonHeight, float listWidth, float listHeight)
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
                    if (_width2 > 0) _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Width(_width2 + 15), GUILayout.Height(_height2));
                    else _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(_controllers.Count() * _height1));

                    int selected = -1;
                    ParamCharaController[] buffControllers = _controllers.Values.ToArray();
                    string[] names = buffControllers.Select(x => x.ChaFileControl.parameter.fullname).ToArray();
                    selected = GUILayout.SelectionGrid(selected, names, 1, GUILayout.Width(_width2), GUILayout.Height(names.Count() * (_height1)));

                    if (selected > -1)
                    {
                        changed = true;
                        _selectedId = buffControllers[selected].controllerID;
                        _show = false;
                    }
                    GUILayout.Space(_controllers.Count() * 1);
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
