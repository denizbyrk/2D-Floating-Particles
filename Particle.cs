using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FloatingParticles {
    public class Particle {

        public int size; //particle size
        public float velocityX; //particle velocity X
        public float velocityY; //particle velocity Y
        public Vector2 position; //particle position
        public bool collided; //flag for particle collision
        public bool destroy; //flag for particle destroying
        public float alpha = 1f; //particle alpha
        public Color color { get; set; } //particle color

        //constructor
        public Particle(float velocityX, float velocityY, float x, float y, int size, Color color) {

            this.size = size;
            this.velocityX = velocityX;
            this.velocityY = velocityY;
            this.position = new Vector2(x, y);
            this.color = color;
            this.collided = false;
            this.destroy = false;
        }

        //apply friction to the particle
        private void applyFriction() {

            //random friction value between min and max
            float min = 0.01f; //minimum friction
            float max = Main.friction + 0.02f; //maximum friction

            //random friction value
            float randomFriction = (float)(Main.random.NextDouble() * (max - min) + min);

            //apply friction to the particle velocity
            this.velocityX += this.velocityX > 0 ? -randomFriction : (this.velocityX < 0 ? randomFriction : 0);
            this.velocityY += this.velocityY > 0 ? -randomFriction : (this.velocityY < 0 ? randomFriction : 0);

            //apply friction to the particle velocity X
            if (this.velocityX < 0 && this.velocityX > -randomFriction) {

                this.velocityX = 0;
            }

            //apply friction to the particle velocity Y
            if (this.velocityY < 0 && this.velocityY > -randomFriction) {

                this.velocityY = 0;
            }  
        }

        //check if the particle is within the bounds
        private void checkBounds() {

            if (this.position.X > Main.getScreenWidth() || this.position.X < 0 ||
                this.position.Y > Main.getScreenHeigth() || this.position.Y < 0) {

                this.destroy = true;
            }
        }

        //check if the particle has collided with the screen edges
        private void checkCollision() {

            if (this.position.X + this.size > Main.getScreenWidth() || this.position.X < 0) {

                this.velocityX *= -1;
                this.collided = true;
            }

            if (this.position.Y > Main.getScreenHeigth() || this.position.Y < 0) {

                this.velocityY *= -1;
                this.collided = true;
            }
        }

        //manage the alpha of the particle
        private void manageAlpha() {

            if (this.alpha <= 0) {

                this.destroy = true;
            }

            if (Main.enableAlpha == true) {

                this.alpha -= Main.decaySpeed;
            }
        }

        //update the particle
        public void Update(GameTime dt) {

            //update position
            this.position.X += this.velocityX;
            this.position.Y += this.velocityY;
            
            if (Main.enableCollision == true) {

                this.checkCollision();

            } else {

                this.checkBounds();
            }

            if (Main.enableFriction == true) {

                this.applyFriction();
            }

            if (this.collided == true || (this.velocityX == 0 && this.velocityY == 0)) {

                this.manageAlpha();
            }
        }

        //draw the particle
        public void Draw(SpriteBatch b) {

            b.Draw(Main.pixel, new Rectangle((int)this.position.X, (int)this.position.Y, this.size, this.size), this.color * this.alpha);
        }       
    }
}