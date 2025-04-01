using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FloatingParticles {
    public class Util {

        private static MouseState prevState;
        private static MouseState currentState;

        public static bool IsLeftClickDown() => Util.prevState.LeftButton == ButtonState.Released && Util.currentState.LeftButton == ButtonState.Pressed;

        public static bool IsLeftClickHold() => Util.currentState.LeftButton == ButtonState.Pressed;

        public static Vector2 getMousePosition => currentState.Position.ToVector2();

        public static bool checkMouseCoordinates() {

            if (Util.getMousePosition.X < Main.getScreenWidth() &&
                Util.getMousePosition.X > 0 &&
                Util.getMousePosition.Y < Main.getScreenHeigth() &&
                Util.getMousePosition.Y > 0) {

                return true;
            }

            return false;
        }

        public static void Update() {

            prevState = currentState;
            currentState = Mouse.GetState();
        }

        public static Color ColorFromHSV(float hue, float saturation, float value) {

            int hi = (int)(hue / 60) % 6;
            float f = (hue / 60) - hi;
            float p = value * (1 - saturation);
            float q = value * (1 - f * saturation);
            float t = value * (1 - (1 - f) * saturation);

            float r = 0;
            float g = 0;
            float b = 0;

            switch (hi) {

                case 0:
                    r = value;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = value;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = value;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = value;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = value;
                    break;
                case 5:
                    r = value;
                    g = p;
                    b = q;
                    break;
            }

            return new Color(r, g, b);
        }
    }
}