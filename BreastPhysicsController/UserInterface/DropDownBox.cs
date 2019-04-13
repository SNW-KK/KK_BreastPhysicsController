using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BreastPhysicsController;

namespace BreastPhysicsController.UserInterface
{
    public class DropDownBox
    {
        float _width;
        float _height;

        public bool _show;
        public bool changed;

        Vector2 _scroll;
        string[] _list;
        public int[] _id;
        string _emptyString;
        int _selectedIndex;


        public DropDownBox(string[] list,int[] id,string emptyString, float width, float height)
        {
            _show = false;
            _list = list;
            _id = id;
            _emptyString = emptyString;
            _scroll = new Vector2(0, 0);
            _selectedIndex = 0;
            _width = width;
            _height = height;
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
                    if(selected>-1)
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

    /*
    public class DropDownBox
    {
        private Vector2 scrollViewVector = Vector2.zero;
        public Rect dropDownRect = new Rect(125, 50, 125, 300);
        //public static string[] list = { "Drop_Down_Menu" };
        public bool loaded = false;
        public List<string> list = new List<string>();
        public List<int> ids = new List<int>();

        public int indexNumber;
        bool show = false;

        public DropDownBox(Rect rect, List<BreastDynamicBoneController> controllers)
        {
            dropDownRect = rect;
            SetList(controllers);
        }

        public void SetList(List<BreastDynamicBoneController> controllers)
        {
            list.Clear();
            ids.Clear();
            foreach(BreastDynamicBoneController controller in controllers)
            {
                list.Add(controller.ChaControl.name);
                ids.Add(controller.controllerID);
            }
            if (list.Count > 0) loaded = true;
        }

        public int GetSelectedID()
        {
            return ids[indexNumber];
        }

        public void OnGUI()
        {
            if(list==null || list.Count==0)
            {
                Draw(new List<string> { "No character1","No character2" });
            }
            else
            {
                Draw(list);
            }
        }

        private void Draw(List<string> displayList)
        {
            if (GUI.Button(new Rect((dropDownRect.x - 100), dropDownRect.y, dropDownRect.width, 25), ""))
            {
                if (!show)
                {
                    show = true;
                }
                else
                {
                    show = false;
                }
            }

            if (show)
            {

                scrollViewVector = GUI.BeginScrollView(new Rect((dropDownRect.x - 100), (dropDownRect.y + 25), dropDownRect.width, dropDownRect.height),
                                    scrollViewVector, new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (displayList.Count * 25))));
                GUI.Box(new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (displayList.Count * 25))), "");


                for (int index = 0; index < displayList.Count; index++)
                {

                    if (GUI.Button(new Rect(0, (index * 25), dropDownRect.height, 25), ""))
                    {
                        show = false;
                        indexNumber = index;
                    }

                    GUI.Label(new Rect(5, (index * 25), dropDownRect.height, 25), displayList[index]);

                }

                GUI.EndScrollView();
            }
            else
            {
                GUI.Label(new Rect((dropDownRect.x - 95), dropDownRect.y, 300, 25), displayList[indexNumber]);
            }

        }
    }*/
}
