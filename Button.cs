using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FloatingParticles {
    public class Button {

        public Rectangle rectangle;
        public Color color;
        public string text { get; set; }
        public bool isClicked = false;

        public Button(Rectangle rectangle, Color color) {

            this.rectangle = rectangle;
            this.color = color;
        }

        public void Update() {

            Rectangle mouse = new Rectangle((int)Util.getMousePosition.X, (int)Util.getMousePosition.Y, 1, 1);

            if (mouse.Intersects(this.rectangle) && Util.IsLeftClickDown()) {

                this.isClicked = true;
            }
        }

        public void Draw(SpriteBatch b, SpriteFont font) {

            b.Draw(Main.pixel, this.rectangle, this.color);
            b.DrawString(font, this.text, new Vector2(this.rectangle.X + this.rectangle.Width / 4, this.rectangle.Y + this.rectangle.Height / 4), Color.Black);
        }
    }
}