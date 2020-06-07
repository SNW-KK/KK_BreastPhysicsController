using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace BreastPhysicsController.UI.Util
{
    public static class ResourceLoader
    {
        public static byte[] GetByte(string resourceName)
        {
            Assembly assm = Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assm.GetManifestResourceStream(resourceName);
            byte[] buff = new byte[stream.Length];
            stream.Read(buff, 0, (int)stream.Length);

            return buff;
        }

        public static Texture2D GetTexture(string resourceName,int width,int height)
        {
            Texture2D tex = new Texture2D(width,height);
            tex.LoadImage(GetByte(resourceName));
            return tex;
        }
    }
}
