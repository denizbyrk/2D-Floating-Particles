using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FloatingParticles {
    public class Main : Game {

        //program variables
        private ContentManager contentManager; //content manager for loading assets (only font in this project)
        private GraphicsDeviceManager graphics; //graphics manager
        private const int screenWidth = 1280; //screen width
        private const int screenHeight = 720; //screen height
        private const int menuHeight = 200; //bottom menu height
        private SpriteBatch b; //sprite batch for drawing textures
        public static Texture2D pixel; //pixel texture for drawing rectangles
        public static Random random = new Random(); //random number generator
        private Rectangle mouse; //mouse rectangle for checking mouse position and clicks
        private float FPS = 60.0f; //frames per second for the game loop

        //particles
        private List<Particle> particles; //list of particles to be drawn and updated
        private bool mouseClicked = false; //flag for checking if the mouse is clicked
        private int timer = 0; //timer for spawning particles at a certain frequency

        //settings
        private bool pause = false; //flag for pausing simulation
        private bool randomSpawn = false; //flag for random spawn
        public static bool enableCollision = true; //flag for enabling edge collision
        public static bool enableAlpha = true; //flag for enabling decay
        public static bool enableFriction = false; //flag for enabling friction
        private int particleSpeed = 5; //particle speed
        private int particleSize = 2; //particle size
        private int particleCount = 60; //number of particles to spawn
        private int spawnFrequency = 0; //frequency of spawning particles (in frames)
        public static float friction = 0.005f; //friction value for particles
        private float radius = 0; //radius for spawning particles
        public static float decaySpeed = 0.01f; //decay speed for particles
        private float hue = 0; //hue value for color cycling
        private bool cyclingColors = false; //flag for cycling colors
        private bool randomColors = false; //flag for random colors
        private Color color = Color.White; //default color for particles
        private String selectedColor = "White"; //selected color for particles

        //buttons
        private List<Button> buttons; //list of buttons for the menu
        private Dictionary<Button, Action> buttonActions; //dictionary for button actions
        private Dictionary<Button, (Color color, string name)> colorButtons; //dictionary for color buttons
        private int buttonSize = 20; //button size
        private int change = 1; //change value for increasing/decreasing particle properties

        //button for increasing/decreasing properties
        private Button increaseSpeedButton;
        private Button decreaseSpeedButton;
        private Button increaseSizeButton;
        private Button decreaseSizeButton;
        private Button increaseParticleCountButton;
        private Button decreaseParticleCountButton;
        private Button increaseSpawnFrequencyButton;
        private Button decreaseSpawnFrequencyButton;
        private Button increaseRadiusButton;
        private Button decreaseRadiusButton;
        private Button increaseDecaySpeedButton;
        private Button decreaseDecaySpeedButton;
        private Button increaseFrictionButton;
        private Button decreaseFrictionButton;

        //button for enabling/disabling features
        private Button enableRandomSpawnButton;
        private Button enableEdgeCollisionButton;
        private Button enableDecayButton;
        private Button enableFrictionButton;
        private Button pauseButton;
        private Button resetValuesButton;
        private Button clearParticlesButton;

        //buttons for colors
        private Button redColorButton;
        private Button orangeColorButton;
        private Button yellowColorButton;
        private Button lightGreenColorButton;
        private Button greenColorButton;
        private Button blueColorButton;
        private Button darkBlueColorButton;
        private Button purpleColorButton;
        private Button pinkColorButton;
        private Button whiteColorButton;
        private Button cyclingColorButton;
        private Button randomColorButton;

        //text font
        public static SpriteFont font;

        //constructor
        public Main() {

            this.graphics = new GraphicsDeviceManager(this); //initialize graphics device manager
            this.graphics.SynchronizeWithVerticalRetrace = false;

            this.Window.AllowAltF4 = true; //allow alt+f4 to close the window
            this.Window.AllowUserResizing = false; //don't allow user to resize the window

            this.Content.RootDirectory = "Content"; //set content root directory
            this.IsFixedTimeStep = true; //set fixed time step to true
            this.IsMouseVisible = true; //make mouse visible

            this.TargetElapsedTime = TimeSpan.FromSeconds(1 / this.FPS); //set target elapsed time to 60 FPS
        }

        //initialize the program
        protected override void Initialize() {

            //set window dimensions
            this.graphics.PreferredBackBufferWidth = Main.screenWidth;
            this.graphics.PreferredBackBufferHeight = Main.screenHeight + Main.menuHeight;
            this.graphics.ApplyChanges();

            this.contentManager = new ContentManager(this.Content.ServiceProvider, "Content");

            this.particles = new List<Particle>();

            //create buttons
            this.createButtons();

            base.Initialize();
        }

        //load content
        protected override void LoadContent() {

            //initialize sprite batch
            this.b = new SpriteBatch(this.GraphicsDevice);

            //set pixel texture to white
            Main.pixel = new Texture2D(this.GraphicsDevice, 1, 1);
            Main.pixel.SetData(new Color[] { Color.White });

            //load font
            Main.font = this.contentManager.Load<SpriteFont>("Font");
        }

        //update
        protected override void Update(GameTime dt) {

            //update mouse and keyboard states
            Util.Update(dt);

            //check button clicks
            this.checkButtonClick();

            //clamp values
            this.clampValues();

            //check if the program is paused
            if (this.pause == false) {

                //manage particles
                this.manageParticles(dt);
            }

            base.Update(dt);
        }

        //draw
        protected override void Draw(GameTime dt) {

            //set background to black
            this.GraphicsDevice.Clear(Color.Black);

            this.b.Begin(); //begin sprite batch

            //draw particles
            foreach (Particle p in this.particles) {

                p.Draw(this.b);
            }

            //draw menu
            this.drawMenu(this.b);

            this.b.End(); //end sprite batch

            base.Draw(dt);
        }

        //manage particles
        private void manageParticles(GameTime dt) {

            //get mouse rectangle
            this.mouse = new Rectangle((int)Util.getMousePosition().X, (int)Util.getMousePosition().Y, 1, 1);

            //check if mouse is clicked and is within screen bounds
            if (Util.IsLeftClickHold() == true && Util.checkMouseCoordinates() == true) {

                this.mouseClicked = true;

                //increase hue
                if (this.cyclingColors == true) {

                    if (this.hue >= 359) this.hue = 1;
                    this.hue += 1f;
                }

                if (timer == 0) {

                    //spawn particles
                    this.spawnParticle(this.mouse.X, this.mouse.Y);
                }
            }

            //timer for spawning particles
            if (this.mouseClicked == true) {

                timer++;
                if (timer > this.spawnFrequency) {

                    timer = 0;
                }
                this.mouseClicked = false;
            }

            //update particles
            for (int i = 0; i < this.particles.Count; i++) {

                this.particles[i].Update(dt);

                if (particles[i].destroy == true) {

                    this.particles.Remove(particles[i]);
                }
            }
        }

        //spawn particles
        private void spawnParticle(int x, int y) {

            Color c = Color.Black;

            //set color
            if (this.cyclingColors == false && this.randomColors == false) {

                c = this.color;
            }

            //spawn particles
            for (int i = 0; i < this.particleCount; i++) {

                float angle;
                float spawnRadius;

                //random spawn
                if (this.randomSpawn) {

                    angle = MathHelper.TwoPi * (float)Main.random.NextDouble();
                    spawnRadius = this.radius * (float)Math.Sqrt(Main.random.NextDouble());

                } else {

                    angle = MathHelper.TwoPi * i / this.particleCount;
                    spawnRadius = this.radius;
                }

                //calculate velocity
                float velocityX = this.particleSpeed * (float)Math.Cos(angle);
                float velocityY = this.particleSpeed * (float)Math.Sin(angle);

                //calculate spawn position
                float spawnX = x + spawnRadius * (float)Math.Cos(angle);
                float spawnY = y + spawnRadius * (float)Math.Sin(angle);

                //set color
                c = this.randomColors ? new Color(random.Next(255), random.Next(255), random.Next(255)) : this.cyclingColors ? Util.ColorFromHSV(this.hue, 1f, 1f) : c;

                //create particle
                this.particles.Add(new Particle(velocityX, velocityY, spawnX, spawnY, this.particleSize, c));
            }
        }

        //clamp values
        private void clampValues() {

            this.particleSpeed = MathHelper.Clamp(this.particleSpeed, 1, 20);
            this.particleSize = MathHelper.Clamp(this.particleSize, 1, 10);
            this.particleCount = MathHelper.Clamp(this.particleCount, 1, 500);
            this.spawnFrequency = MathHelper.Clamp(this.spawnFrequency, 0, 60);
            this.radius = MathHelper.Clamp(this.radius, 0, 100);
            Main.decaySpeed = (float)Math.Round(MathHelper.Clamp(Main.decaySpeed, 0.005f, 1f), 3);
            Main.friction = (float)Math.Round(MathHelper.Clamp(Main.friction, 0.005f, 1f), 3);
        }

        //draw menu
        private void drawMenu(SpriteBatch b) {

            b.Draw(Main.pixel, new Rectangle(0, Main.screenHeight + 8, Main.screenWidth, Main.menuHeight), Color.Black);
            b.Draw(Main.pixel, new Rectangle(0, Main.screenHeight + 8, Main.screenWidth, 5), Color.White);

            b.DrawString(Main.font, "Particle Speed: " + this.particleSpeed, new Vector2(this.buttonSize * 3.5f, this.increaseSpeedButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Particle Size: " + this.particleSize, new Vector2(this.buttonSize * 3.5f, this.increaseSizeButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Spawned Particles: " + this.particleCount, new Vector2(this.buttonSize * 3.5f, this.increaseParticleCountButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Frequency (Frames): " + this.spawnFrequency, new Vector2(this.buttonSize * 3.5f, this.increaseSpawnFrequencyButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Radius: " + this.radius, new Vector2(this.buttonSize * 3.5f, this.increaseRadiusButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Decay Speed: " + Main.decaySpeed, new Vector2(this.increaseDecaySpeedButton.rectangle.X + this.buttonSize * 1.5f, this.increaseDecaySpeedButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Friction: " + Main.friction, new Vector2(this.increaseFrictionButton.rectangle.X + this.buttonSize * 1.5f, this.increaseFrictionButton.rectangle.Y), Color.White);

            b.DrawString(Main.font, "Hold LShift to change values by 5", new Vector2(10, this.increaseSpeedButton.rectangle.Y - this.buttonSize - 4), Color.Red);

            b.DrawString(Main.font, "Random Spawn: " + this.randomSpawn, new Vector2(this.enableRandomSpawnButton.rectangle.X + 48, this.enableRandomSpawnButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Edge Collision: " + Main.enableCollision, new Vector2(this.enableEdgeCollisionButton.rectangle.X + 48, this.enableEdgeCollisionButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Decay: " + Main.enableAlpha, new Vector2(this.enableDecayButton.rectangle.X + 48, this.enableDecayButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Friction: " + Main.enableFriction, new Vector2(this.enableFrictionButton.rectangle.X + 48, this.enableFrictionButton.rectangle.Y), Color.White);

            b.DrawString(Main.font, "Pause: " + this.pause, new Vector2(this.pauseButton.rectangle.X + 48, this.pauseButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Reset Values", new Vector2(this.resetValuesButton.rectangle.X + 48, this.resetValuesButton.rectangle.Y), Color.White);
            b.DrawString(Main.font, "Clear Particles", new Vector2(this.clearParticlesButton.rectangle.X + 48, this.clearParticlesButton.rectangle.Y), Color.White);

            b.DrawString(Main.font, "Particle Color: ", new Vector2(this.redColorButton.rectangle.X, this.redColorButton.rectangle.Y - 48), Color.White);
            b.DrawString(Main.font, "" + this.selectedColor, new Vector2(this.redColorButton.rectangle.X + 140, this.redColorButton.rectangle.Y - 48), this.color);

            foreach (Button btn in this.buttons) {

                btn.Draw(b);
            }
        }

        //check button clicks
        private void checkButtonClick() {

            //update buttons
            foreach (Button b in this.buttons) {

                b.Update();
            }

            //check if left shift is held down and set change value accordingly
            this.change = Util.IsKeyHold(Microsoft.Xna.Framework.Input.Keys.LeftShift) ? 5 : 1;

            //update button actions
            foreach (var a in this.buttonActions) {

                if (a.Key.isClicked) {

                    a.Value.Invoke();
                    a.Key.isClicked = false;
                    break;
                }
            }

            //update color button actions
            foreach (var c in this.colorButtons) {

                if (c.Key.isClicked) {

                    this.color = c.Value.color;
                    this.selectedColor = c.Value.name;
                    c.Key.isClicked = false;
                    break;
                }
            }

            if (this.randomColorButton.isClicked) {

                this.selectedColor = "Random";
                this.randomColors = true;
                this.cyclingColors = false;
                this.randomColorButton.isClicked = false;

            } else if (this.cyclingColorButton.isClicked) {

                this.selectedColor = "Cycling";
                this.cyclingColors = true;
                this.randomColors = false;
                this.cyclingColorButton.isClicked = false;
            }

            if (!(this.selectedColor.Equals("Random") || this.selectedColor.Equals("Cycling"))) {

                this.cyclingColors = false;
                this.randomColors = false;
            }
        }

        //create buttons
        private void createButtons() {

            //increase/decrease buttons
            this.decreaseSpeedButton = new Button(new Rectangle(10, Main.screenHeight + this.buttonSize * 2, this.buttonSize, this.buttonSize), Color.Gray);
            this.increaseSpeedButton = new Button(new Rectangle(this.decreaseSpeedButton.rectangle.X + this.buttonSize + 12, this.decreaseSpeedButton.rectangle.Y, this.buttonSize, this.buttonSize), Color.Gray);
            this.decreaseSizeButton = new Button(new Rectangle(10, decreaseSpeedButton.rectangle.Y + 32, this.buttonSize, this.buttonSize), Color.Gray);
            this.increaseSizeButton = new Button(new Rectangle(this.decreaseSizeButton.rectangle.X + this.buttonSize + 12, this.decreaseSizeButton.rectangle.Y, this.buttonSize, this.buttonSize), Color.Gray);
            this.decreaseParticleCountButton = new Button(new Rectangle(10, this.increaseSizeButton.rectangle.Y + 32, this.buttonSize, this.buttonSize), Color.Gray);
            this.increaseParticleCountButton = new Button(new Rectangle(this.decreaseParticleCountButton.rectangle.X + this.buttonSize + 12, this.decreaseParticleCountButton.rectangle.Y, this.buttonSize, this.buttonSize), Color.Gray);
            this.decreaseSpawnFrequencyButton = new Button(new Rectangle(10, increaseParticleCountButton.rectangle.Y + 32, this.buttonSize, this.buttonSize), Color.Gray);
            this.increaseSpawnFrequencyButton = new Button(new Rectangle(this.decreaseSpawnFrequencyButton.rectangle.X + this.buttonSize + 12, this.decreaseSpawnFrequencyButton.rectangle.Y, this.buttonSize, this.buttonSize), Color.Gray);
            this.decreaseRadiusButton = new Button(new Rectangle(10, increaseSpawnFrequencyButton.rectangle.Y + 32, this.buttonSize, this.buttonSize), Color.Gray);
            this.increaseRadiusButton = new Button(new Rectangle(this.decreaseRadiusButton.rectangle.X + this.buttonSize + 12, this.decreaseRadiusButton.rectangle.Y, this.buttonSize, this.buttonSize), Color.Gray);
            this.decreaseDecaySpeedButton = new Button(new Rectangle(330, this.increaseParticleCountButton.rectangle.Y + 4, this.buttonSize, this.buttonSize), Color.Gray);
            this.increaseDecaySpeedButton = new Button(new Rectangle(this.decreaseDecaySpeedButton.rectangle.X + this.buttonSize + 12, this.decreaseDecaySpeedButton.rectangle.Y, this.buttonSize, this.buttonSize), Color.Gray);
            this.decreaseFrictionButton = new Button(new Rectangle(330, increaseSpawnFrequencyButton.rectangle.Y + 32, this.buttonSize, this.buttonSize), Color.Gray);
            this.increaseFrictionButton = new Button(new Rectangle(this.decreaseFrictionButton.rectangle.X + this.buttonSize + 12, this.decreaseFrictionButton.rectangle.Y, this.buttonSize, this.buttonSize), Color.Gray);

            //increase/decrease button texts
            this.decreaseSpeedButton.text = "-";
            this.increaseSpeedButton.text = "+";
            this.decreaseSizeButton.text = "-";
            this.increaseSizeButton.text = "+";
            this.increaseParticleCountButton.text = "+";
            this.decreaseParticleCountButton.text = "-";
            this.increaseSpawnFrequencyButton.text = "+";
            this.decreaseSpawnFrequencyButton.text = "-";
            this.increaseRadiusButton.text = "+";
            this.decreaseRadiusButton.text = "-";
            this.decreaseDecaySpeedButton.text = "-";
            this.increaseDecaySpeedButton.text = "+";
            this.decreaseFrictionButton.text = "-";
            this.increaseFrictionButton.text = "+";

            //enable/disable buttons
            this.enableRandomSpawnButton = new Button(new Rectangle(330, this.increaseSpeedButton.rectangle.Y - 24, this.buttonSize * 2, this.buttonSize), Color.Gray);
            this.enableEdgeCollisionButton = new Button(new Rectangle(this.enableRandomSpawnButton.rectangle.X, this.enableRandomSpawnButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);
            this.enableDecayButton = new Button(new Rectangle(this.enableEdgeCollisionButton.rectangle.X, this.enableEdgeCollisionButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);
            this.enableFrictionButton = new Button(new Rectangle(this.enableDecayButton.rectangle.X, this.increaseDecaySpeedButton.rectangle.Y + 30, this.buttonSize * 2, this.buttonSize), Color.Gray);
            this.pauseButton = new Button(new Rectangle(630, this.enableRandomSpawnButton.rectangle.Y, this.buttonSize * 2, this.buttonSize), Color.Gray);
            this.resetValuesButton = new Button(new Rectangle(630, this.pauseButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);
            this.clearParticlesButton = new Button(new Rectangle(this.resetValuesButton.rectangle.X, this.resetValuesButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);

            //color buttons
            this.redColorButton = new Button(new Rectangle(950, Main.screenHeight + 90, this.buttonSize * 2, this.buttonSize * 2), Color.Red);
            this.orangeColorButton = new Button(new Rectangle(this.redColorButton.rectangle.X + this.redColorButton.rectangle.Width, this.redColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Orange);
            this.yellowColorButton = new Button(new Rectangle(this.orangeColorButton.rectangle.X + this.orangeColorButton.rectangle.Width, this.orangeColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Yellow);
            this.lightGreenColorButton = new Button(new Rectangle(this.yellowColorButton.rectangle.X + this.yellowColorButton.rectangle.Width, this.yellowColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.LimeGreen);
            this.greenColorButton = new Button(new Rectangle(this.lightGreenColorButton.rectangle.X + this.lightGreenColorButton.rectangle.Width, this.lightGreenColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Green);
            this.blueColorButton = new Button(new Rectangle(this.greenColorButton.rectangle.X + this.greenColorButton.rectangle.Width, this.greenColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.SkyBlue);
            this.darkBlueColorButton = new Button(new Rectangle(this.redColorButton.rectangle.X, this.redColorButton.rectangle.Y + this.redColorButton.rectangle.Height, this.buttonSize * 2, this.buttonSize * 2), Color.DarkBlue);
            this.pinkColorButton = new Button(new Rectangle(this.darkBlueColorButton.rectangle.X + this.darkBlueColorButton.rectangle.Width, this.darkBlueColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.HotPink);
            this.purpleColorButton = new Button(new Rectangle(this.pinkColorButton.rectangle.X + this.pinkColorButton.rectangle.Width, this.pinkColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Purple);
            this.whiteColorButton = new Button(new Rectangle(this.purpleColorButton.rectangle.X + this.purpleColorButton.rectangle.Width, this.purpleColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.White);
            this.randomColorButton = new Button(new Rectangle(this.whiteColorButton.rectangle.X + this.whiteColorButton.rectangle.Width, this.whiteColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Gray);
            this.cyclingColorButton = new Button(new Rectangle(this.randomColorButton.rectangle.X + this.randomColorButton.rectangle.Width, this.randomColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.OrangeRed);

            this.randomColorButton.text = "?";

            //buttons list
            buttons = new List<Button> {

                this.increaseSpeedButton, this.decreaseSpeedButton,
                this.increaseSizeButton, this.decreaseSizeButton,
                this.increaseParticleCountButton, this.decreaseParticleCountButton,
                this.increaseSpawnFrequencyButton, this.decreaseSpawnFrequencyButton,
                this.increaseRadiusButton, this.decreaseRadiusButton,
                this.increaseDecaySpeedButton, this.decreaseDecaySpeedButton,
                this.increaseFrictionButton, this.decreaseFrictionButton,
                this.enableRandomSpawnButton,
                this.enableEdgeCollisionButton,
                this.enableDecayButton,
                this.enableFrictionButton,
                this.pauseButton,
                this.resetValuesButton,
                this.clearParticlesButton,
                this.redColorButton, this.orangeColorButton,
                this.yellowColorButton, this.lightGreenColorButton,
                this.greenColorButton, this.blueColorButton,
                this.darkBlueColorButton, this.pinkColorButton,
                this.purpleColorButton, this.whiteColorButton,
                this.cyclingColorButton, this.randomColorButton
            };

            //button actions
            this.buttonActions = new Dictionary<Button, Action> {

                { this.increaseSpeedButton, () => this.particleSpeed += this.change },
                { this.decreaseSpeedButton, () => this.particleSpeed -= this.change },
                { this.increaseSizeButton, () => this.particleSize += this.change },
                { this.decreaseSizeButton, () => this.particleSize -= this.change },
                { this.increaseParticleCountButton, () => this.particleCount += this.change },
                { this.decreaseParticleCountButton, () => this.particleCount -= this.change },
                { this.increaseSpawnFrequencyButton, () => this.spawnFrequency += this.change },
                { this.decreaseSpawnFrequencyButton, () => this.spawnFrequency -= this.change },
                { this.increaseRadiusButton, () => this.radius += this.change },
                { this.decreaseRadiusButton, () => this.radius -= this.change },
                { this.increaseDecaySpeedButton, () => Main.decaySpeed += this.change / 200f },
                { this.decreaseDecaySpeedButton, () => Main.decaySpeed -= this.change / 200f },
                { this.decreaseFrictionButton, () => Main.friction -= this.change / 200f },
                { this.increaseFrictionButton, () => Main.friction += this.change / 200f },
                { this.enableRandomSpawnButton, () => this.randomSpawn = !this.randomSpawn },
                { this.enableEdgeCollisionButton, () => Main.enableCollision = !Main.enableCollision },
                { this.enableDecayButton, () => Main.enableAlpha = !Main.enableAlpha },
                { this.enableFrictionButton, () => Main.enableFriction = !Main.enableFriction },
                { this.pauseButton, () => this.pause = !this.pause },
                { this.resetValuesButton, () =>
                    {
                        this.particleSpeed = 5;
                        this.particleSize = 2;
                        this.particleCount = 60;
                        this.spawnFrequency = 0;
                        this.radius = 0;
                        Main.decaySpeed = 0.01f;
                        Main.friction = 0.005f;
                        this.randomSpawn = false;
                        Main.enableCollision = true;
                        Main.enableAlpha = true;
                        Main.enableFriction = false;
                        this.pause = false;
                    }
                },
                { this.clearParticlesButton, () => this.particles.Clear() }
            };

            //color button actions
            this.colorButtons = new Dictionary<Button, (Color color, string name)> {

                { this.redColorButton, (Color.Red, "Red") },
                { this.orangeColorButton, (Color.Orange, "Orange") },
                { this.yellowColorButton, (Color.Yellow, "Yellow") },
                { this.lightGreenColorButton, (Color.LimeGreen, "Light Green") },
                { this.greenColorButton, (Color.Green, "Green") },
                { this.blueColorButton, (Color.SkyBlue, "Blue") },
                { this.darkBlueColorButton, (Color.DarkBlue, "Dark Blue") },
                { this.pinkColorButton, (Color.HotPink, "Pink") },
                { this.purpleColorButton, (Color.Purple, "Purple") },
                { this.whiteColorButton, (Color.White, "White") }
            };
        }

        //getters for screen width and height
        public static int getScreenWidth() => Main.screenWidth;

        public static int getScreenHeigth() => Main.screenHeight;
    }
}