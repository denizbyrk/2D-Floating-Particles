using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FloatingParticles {
    public class Particle {

        public int size;
        public float velocityX;
        public float velocityY;
        public Vector2 position;
        public bool collided;
        public bool destroy;
        public float alpha = 1f;

        public Color color { get; set; }

        public Particle(float velocityX, float velocityY, float x, float y, int size, Color color) {

            this.size = size;
            this.velocityX = velocityX;
            this.velocityY = velocityY;
            this.position = new Vector2(x, y);
            this.color = color;
            this.collided = false;
            this.destroy = false;
        }

        private void checkBounds() {

            if (this.position.X > Main.getScreenWidth() || this.position.X < 0 ||
                this.position.Y > Main.getScreenHeigth() || this.position.Y < 0) {

                this.destroy = true;
            }
        }

        private void checkCollision() {

            if (this.position.X > Main.getScreenWidth() || this.position.X < 0) {

                this.velocityX *= -1;
                this.collided = true;
            }

            if (this.position.Y > Main.getScreenHeigth() || this.position.Y < 0) {

                this.velocityY *= -1;
                this.collided = true;
            }
        }

        private void manageAlpha() {

            if (this.alpha <= 0) {

                this.destroy = true;
            }

            if (Main.enableAlpha == true) {

                this.alpha -= Main.decaySpeed;
            }
        }

        public void Update(GameTime dt) {

            this.position.X += this.velocityX;
            this.position.Y += this.velocityY;
            
            if (Main.enableCollision == true) {

                this.checkCollision();
            } else {

                this.checkBounds();
            }

            if (this.collided == true) {

                this.manageAlpha();
            }
        }

        public void Draw(SpriteBatch b) {

            b.Draw(Main.pixel, new Rectangle((int)this.position.X, (int)this.position.Y, this.size, this.size), this.color * this.alpha);
        }       
    }
}