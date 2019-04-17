using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BreastPhysicsController
{
    public class CharaSelect : SelectList
    {
        public int[] _id;

        public CharaSelect(string[] list, int[] id, string emptyString, float buttonWidth, float buttonHeight,float listWidth,float listHeight)
            : base(list,emptyString,false, buttonWidth, buttonHeight, listWidth, listHeight)
        {
            _id = id;
        }

        public void SetList(string[] list, int[] id)
        {
            _list = list.Reverse().ToArray();
            _id = id.Reverse().ToArray();
            changed = true;
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
    }
}
