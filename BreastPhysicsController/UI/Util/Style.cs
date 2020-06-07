using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BreastPhysicsController.UI.Util
{
    public static class Style
    {
        public static int defaultSpace { get; private set; } = 5;

        public static GUIStyle LabelWindowTitle;
        public static GUIStyle LabelLarge;
        public static GUIStyle LabelLargeSubject;
        public static GUIStyle LabedMiddle;
        public static GUIStyle LabedMiddleSubject;
        public static GUIStyle LabelSmall;

        public static GUIStyle ToggleLarge;
        public static GUIStyle ToggleMiddle;

        public static GUIStyle Window;
        public static GUIStyle WindowInside;
        public static GUIStyle WindowContents;

        public static Texture2D texWindow;
        public static Texture2D texWindowInside;
        public static Texture2D texArea;

        static Style()
        {
            LabelWindowTitle = new GUIStyle(Skin.defaultSkin.label);
            LabelWindowTitle.fontSize = 14;
            LabelWindowTitle.margin = new RectOffset(0, 0, 2, 0);
            LabelWindowTitle.padding = new RectOffset(0, 0, 0, 0);
            LabelWindowTitle.alignment = TextAnchor.MiddleCenter;

            LabelLarge = new GUIStyle(Skin.defaultSkin.label);
            LabelLarge.fontSize = 16;
            LabelLarge.padding = new RectOffset(0, 0, 0, 0);

            LabelLargeSubject = new GUIStyle(Skin.defaultSkin.label);
            LabelLargeSubject.fontSize = 16;
            LabelLargeSubject.padding = new RectOffset(0, 0, 0, 0);
            LabelLargeSubject.alignment = TextAnchor.MiddleCenter;

            LabedMiddle = new GUIStyle(Skin.defaultSkin.label);
            LabedMiddle.fontSize = 14;
            LabedMiddle.padding = new RectOffset(0, 0, 0, 0);

            LabedMiddleSubject = new GUIStyle(Skin.defaultSkin.label);
            LabedMiddleSubject.fontSize = 14;
            LabedMiddleSubject.padding = new RectOffset(0, 0, 0, 0);
            LabedMiddleSubject.alignment = TextAnchor.MiddleCenter;

            LabelSmall = new GUIStyle(Skin.defaultSkin.label);
            LabelSmall.fontSize = 12;
            LabelSmall.padding = new RectOffset(0, 0, 0, 0);

            ToggleLarge = new GUIStyle(Skin.defaultSkin.toggle);
            ToggleLarge.fontSize = 16;

            ToggleMiddle = new GUIStyle(Skin.defaultSkin.toggle);
            ToggleMiddle.fontSize = 14;

            Window = new GUIStyle(Skin.defaultSkin.window);
            texWindow = ResourceLoader.GetTexture("BreastPhysicsController.UI.Resource.TexWindowBase.png", 5, 5);
            Window.onNormal.background = null;
            Window.normal.background = texWindow;

            WindowInside = new GUIStyle();
            texWindowInside = ResourceLoader.GetTexture("BreastPhysicsController.UI.Resource.TexWindowInside.png", 5, 5);
            WindowInside.onNormal.background = null;
            WindowInside.normal.background = texWindowInside;

            WindowContents = new GUIStyle();
            texArea = ResourceLoader.GetTexture("BreastPhysicsController.UI.Resource.TexWindowContents.png", 5, 5);
            WindowContents.onNormal.background = null;
            WindowContents.normal.background = texArea;

        }

    }
}
