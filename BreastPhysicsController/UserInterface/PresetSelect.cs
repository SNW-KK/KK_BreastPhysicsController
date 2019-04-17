using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BreastPhysicsController
{
    public class PresetSelect : SelectList
    {
        string _presetDir;
        public PresetSelect(string presetDir, string emptyString, float buttonWidth, float buttonHeight, float listWidth, float listHeight)
            : base(null, emptyString, true, buttonWidth, buttonHeight, listWidth, listHeight)
        {
            _presetDir = presetDir;
            ReloadPresetDir();
        }

        //public void SetList(string[] list)
        //{
        //    _list = list;
        //    changed = false;
        //    _selectedIndex = 0;
        //}

        private void ReloadPresetDir()
        {
            IEnumerable<string> xmls = System.IO.Directory.GetFiles(_presetDir, "*.xml").ToList();
            xmls = xmls.Select(x => System.IO.Path.GetFileNameWithoutExtension(x));
            List<string> newList = new List<string>();
            newList.Add("Cancel load preset.");
            newList.AddRange(xmls);
            _list = newList.ToArray();
        }

        public bool Draw()
        {
            //changed = false;

            //GUILayout.BeginVertical();

            if (_list == null || _list.Length == 0)
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
                            ReloadPresetDir();
                            _show = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Height(_height1)))
                        {
                            ReloadPresetDir();
                            _show = true;
                        }
                    }
                }
                else
                {
                    if (_width2 > 0) _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Width(_width2 + 15), GUILayout.Height(_height2));
                    else _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(_list.Count() * _height1));

                    int selected = -1;
                    selected = GUILayout.SelectionGrid(selected, _list, 1, GUILayout.Width(_width2), GUILayout.Height(_list.Count() * _height1));

                    if(selected==0)
                    {
                        changed = false;
                        _selectedIndex = selected;
                        _show = false;
                    }
                    else if (selected > 0)
                    {
                        changed = true;
                        _selectedIndex = selected;
                        _show = false;
                    }
                    GUILayout.Space(_list.Count() * 1);
                    GUILayout.EndScrollView();
                }

            }

            //GUILayout.EndVertical();

            return changed;
        }

        public string GetSelectedFilePath()
        {
            if(_selectedIndex>0)
            {
                return System.IO.Path.Combine(_presetDir, _list[_selectedIndex]) + ".xml";
            }
            return null;
        }
    }
}
