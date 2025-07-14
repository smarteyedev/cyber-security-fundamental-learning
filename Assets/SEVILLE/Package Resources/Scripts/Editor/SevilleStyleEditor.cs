using UnityEngine;

namespace Seville
{
    public static class SevilleStyleEditor
    {
        private static GUIStyle _greenButtonStyle = null;
        private static GUIStyle _blueButtonStyle = null;

        public static GUIStyle GreenButton
        {
            get
            {
                if (_greenButtonStyle == null)
                {
                    _greenButtonStyle = CreateButtonStyle(Color.green, Color.white, Color.black, Color.green * 0.6f); // Stronger contrast for hover
                }
                return _greenButtonStyle;
            }
        }

        public static GUIStyle BlueButton
        {
            get
            {
                if (_blueButtonStyle == null)
                {
                    _blueButtonStyle = CreateButtonStyle(Color.blue, Color.white, Color.white, Color.blue * 0.6f); // Stronger contrast for hover
                }
                return _blueButtonStyle;
            }
        }

        private static GUIStyle CreateButtonStyle(Color normalBgColor, Color normalTextColor, Color hoverTextColor, Color hoverBgColor)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);

            // Default (normal) state
            style.normal.textColor = normalTextColor;
            style.normal.background = MakeTex(600, 1, normalBgColor);

            // Hover state with faster responsiveness
            style.hover.textColor = hoverTextColor;
            style.hover.background = MakeTex(600, 1, hoverBgColor);

            // No transition or Lerp, just immediate hover effect
            style.active.textColor = hoverTextColor;
            style.active.background = MakeTex(600, 1, hoverBgColor);

            return style;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
