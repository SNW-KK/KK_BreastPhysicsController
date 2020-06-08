using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BreastPhysicsController.UI.Parts
{
    public class PresetSelect : SelectList
    {
        string _presetDir;
        public PresetSelect(string presetDir, string emptyString, float buttonWidth=0, float buttonHeight=0, float listWidth=0, float listHeight=0)
            : base(null, emptyString, true, buttonWidth, buttonHeight, listWidth, listHeight)
        {
            _presetDir = presetDir;
            ReloadPresetDir();
        }

        private void ReloadPresetDir()
        {
            IEnumerable<string> xmls = System.IO.Directory.GetFiles(_presetDir, "*.xml").ToList();
            xmls = xmls.Select(x => System.IO.Path.GetFileNameWithoutExtension(x));
            List<string> newList = new List<string>();
            newList.Add("Cancel loading preset.");
            newList.AddRange(xmls);
            _list = newList.ToArray();
        }

        public override bool Draw()
        {
            changed = false;

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

                    if (_width1 <= 0 && _height1<=0)
                    {
                        if (GUILayout.Button(buttonLabel))
                        {
                            ReloadPresetDir();
                            _show = true;
                        }
                    }
                    else if (_width1 <= 0)
                    {
                        if (GUILayout.Button(buttonLabel,GUILayout.Height(_height1)))
                        {
                            ReloadPresetDir();
                            _show = true;
                        }
                    }
                    else if (_height1 <= 0)
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Height(_width1)))
                        {
                            ReloadPresetDir();
                            _show = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(buttonLabel, GUILayout.Height(_width1),GUILayout.Height(_height1)))
                        {
                            ReloadPresetDir();
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
                    selected = GUILayout.SelectionGrid(selected, _list, 1);

                    if (selected==0)
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
                    GUILayout.Space(20);
                    GUILayout.EndScrollView();
                }

            }

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
