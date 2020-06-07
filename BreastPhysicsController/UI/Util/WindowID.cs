using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BreastPhysicsController.UI.Util
{
    public static class WindowID
    {
        static int _nextId = 10000;

        public static int GetNewID()
        {
            return _nextId++;
        }
    }
}
