using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FloatingParticles {
    public class Button {

        public Rectangle rectangle; //rectangle of the button
        public Color color; //color of the button
        private Color defaultColor; //default color of the button
        private Color hoverColor; //color of the button when hovered
        private Color clickColor; //color of the button when clicked
        public string text { get; set; } //the text of the button
        public bool isClicked = false; //flag for button click

        //constructor
        public Button(Rectangle rectangle, Color color) {

            this.rectangle = rectangle;
            this.color = color;
            this.defaultColor = color;
            this.hoverColor = new Color(color.R + 40, color.G + 40, color.B + 40);
            this.clickColor = new Color(hoverColor.R + 80, hoverColor.G + 80, hoverColor.B + 80);
            this.text = "";
        }

        //update button
        public void Update() {

            //get mouse rectangle
            Rectangle mouse = new Rectangle((int)Util.getMousePosition().X, (int)Util.getMousePosition().Y, 1, 1);

            //check if mouse is inside button rectangle
            if (mouse.Intersects(this.rectangle)) {

                //set color to hover color
                this.color = this.hoverColor;

                //check if mouse is clicked
                if (Util.IsLeftClickDown()) {

                    //set clicked flag to true
                    this.isClicked = true;

                    //set color to click color
                    this.color = this.clickColor;
                }

            } else {

                this.color = this.defaultColor;
            }
        }

        //draw button
        public void Draw(SpriteBatch b) {

            //draw button rectangle
            b.Draw(Main.pixel, this.rectangle, this.color);

            //draw text
            b.DrawString(Main.font, this.text, new Vector2(this.rectangle.X + this.rectangle.Width / 4, this.rectangle.Y + this.rectangle.Height / 32), Color.Black);
        }
    }
}