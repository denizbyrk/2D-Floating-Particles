using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FloatingParticles;

public class Main : Game {

    private ContentManager contentManager;
    private GraphicsDeviceManager graphics;
    private const int screenWidth = 1280;
    private const int screenHeight = 720;
    private const int menuHeight = 200;
    private SpriteBatch b;
    public static Texture2D pixel;
    private Random random = new Random();
    private Rectangle mouse;
    private float FPS = 60.0f;

    private List<Particle> particles;
    private bool mouseClicked = false;
    private int timer = 0;

    //settings
    private bool randomSpawn = false;
    public static bool enableCollision = true;
    public static bool enableAlpha = true;
    private int particleSpeed = 5;
    private int particleSize = 2;
    private int particleCount = 60;
    private int spawnFrequency = 0;
    private float radius = 0;
    public static float decaySpeed = 0.01f;
    private float hue = 0;
    private bool switchingColors = false;
    private bool randomColors = false;
    private Color color = Color.White;
    private String selectedColor = "White";

    //buttons
    private List<Button> buttons;
    private Dictionary<Button, Action> buttonActions;
    private Dictionary<Button, (Color color, string name)> colorButtons;
    private int buttonSize = 20;
    private int change = 1;
    private bool fastIncrement = false;
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

    private Button enableRandomSpawnButton;
    private Button enableEdgeCollisionButton;
    private Button enableDecayButton;

    private Button resetValuesButton;
    private Button clearParticlesButton;

    private Button redColorButton;
    private Button orangeColorButton;
    private Button yellowColorButton;
    private Button greenColorButton;
    private Button blueColorButton;
    private Button darkBlueColorButton;
    private Button purpleColorButton;
    private Button whiteColorButton;
    private Button switchingColorButton;
    private Button randomColorButton;

    //text font
    public static SpriteFont font;

    public Main() {

        this.graphics = new GraphicsDeviceManager(this);
        this.graphics.SynchronizeWithVerticalRetrace = false;

        this.Window.AllowAltF4 = true;
        this.Window.AllowUserResizing = false;

        this.Content.RootDirectory = "Content";
        this.IsFixedTimeStep = true;
        this.IsMouseVisible = true;

        this.TargetElapsedTime = TimeSpan.FromSeconds(1 / this.FPS);
    }

    protected override void Initialize() {

        this.graphics.PreferredBackBufferWidth = Main.screenWidth;
        this.graphics.PreferredBackBufferHeight = Main.screenHeight + Main.menuHeight;
        this.graphics.ApplyChanges();

        this.contentManager = new ContentManager(this.Content.ServiceProvider, "Content");

        this.particles = new List<Particle>();

        this.createButtons();

        base.Initialize();
    }

    protected override void LoadContent() {

        this.b = new SpriteBatch(this.GraphicsDevice);

        Main.pixel = new Texture2D(this.GraphicsDevice, 1, 1);
        Main.pixel.SetData(new Color[] { Color.White });

        Main.font = this.contentManager.Load<SpriteFont>("Font");
    }

    protected override void Update(GameTime dt) {

        Util.Update(dt);

        this.checkButtonClick();

        this.particleSpeed = MathHelper.Clamp(this.particleSpeed, 1, 20);
        this.particleSize = MathHelper.Clamp(this.particleSize, 1, 10);
        this.particleCount = MathHelper.Clamp(this.particleCount, 1, 500);
        this.spawnFrequency = MathHelper.Clamp(this.spawnFrequency, 0, 60);
        this.radius = MathHelper.Clamp(this.radius, 0, 100);

        this.manageParticles(dt);

        base.Update(dt);
    }

    protected override void Draw(GameTime dt) {

        this.GraphicsDevice.Clear(Color.Black);

        this.b.Begin();

        this.b.Draw(Main.pixel, mouse, Color.White);

        foreach (Particle p in this.particles) {

            p.Draw(this.b);
        }

        this.drawMenu(this.b);

        this.b.End();

        base.Draw(dt);
    }

    private void manageParticles(GameTime dt) {

        this.mouse = new Rectangle((int)Util.getMousePosition().X, (int)Util.getMousePosition().Y, 1, 1);

        if (Util.IsLeftClickHold() == true && Util.checkMouseCoordinates() == true) {

            this.mouseClicked = true;

            if (this.hue >= 359) {

                this.hue = 1;
            }

            this.hue += 1f;

            if (timer == 0) {

                this.spawnParticle(this.mouse.X, this.mouse.Y);
            }
        }

        if (this.mouseClicked == true) {

            timer++;
            if (timer > this.spawnFrequency) {
             
                timer = 0;
            }
            this.mouseClicked = false;
        }

        for (int i = 0; i < this.particles.Count; i++) {

            this.particles[i].Update(dt);

            if (particles[i].destroy == true) {

                this.particles.Remove(particles[i]);
            }
        }            
    }

    private void spawnParticle(int x, int y) {

        Color c = Color.Black;

        if (this.switchingColors == false && this.randomColors == false) {

            c = this.color;
        }

        for (int i = 0; i < this.particleCount; i++) {

            float angle;
            float spawnRadius;

            if (this.randomSpawn) {

                angle = MathHelper.TwoPi * (float)this.random.NextDouble();
                spawnRadius = this.radius * (float)Math.Sqrt(this.random.NextDouble());

            } else {

                angle = MathHelper.TwoPi * i / this.particleCount;
                spawnRadius = this.radius;
            }

            float velocityX = this.particleSpeed * (float)Math.Cos(angle);
            float velocityY = this.particleSpeed * (float)Math.Sin(angle);

            float spawnX = x + spawnRadius * (float)Math.Cos(angle);
            float spawnY = y + spawnRadius * (float)Math.Sin(angle);

            c = this.randomColors ? new Color(random.Next(255), random.Next(255), random.Next(255)) : this.switchingColors ? Util.ColorFromHSV(this.hue, 1f, 1f) : c;

            this.particles.Add(new Particle(velocityX, velocityY, spawnX, spawnY, this.particleSize, c));
        }
    }

    private void drawMenu(SpriteBatch b) {

        b.Draw(Main.pixel, new Rectangle(0, Main.screenHeight + 8, Main.screenWidth, Main.menuHeight), Color.Black);
        b.Draw(Main.pixel, new Rectangle(0, Main.screenHeight + 8, Main.screenWidth, 5), Color.White);

        b.DrawString(Main.font, "Particle Speed: " + this.particleSpeed, new Vector2(this.buttonSize * 3.5f, this.increaseSpeedButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Particle Size: " + this.particleSize, new Vector2(this.buttonSize * 3.5f, this.increaseSizeButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Spawned Particles: " + this.particleCount, new Vector2(this.buttonSize * 3.5f, this.increaseParticleCountButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Frequency (Frames): " + this.spawnFrequency, new Vector2(this.buttonSize * 3.5f, this.increaseSpawnFrequencyButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Radius: " + this.radius, new Vector2(this.buttonSize * 3.5f, this.increaseRadiusButton.rectangle.Y), Color.White);

        b.DrawString(Main.font, "Hold LShift to change values by 5", new Vector2(10, this.increaseSpeedButton.rectangle.Y - this.buttonSize - 4), Color.Red);

        b.DrawString(Main.font, "Random Spawn: " + this.randomSpawn, new Vector2(this.enableRandomSpawnButton.rectangle.X + 48, this.enableRandomSpawnButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Edge Collision: " + Main.enableCollision, new Vector2(this.enableEdgeCollisionButton.rectangle.X + 48, this.enableEdgeCollisionButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Decay: " + Main.enableAlpha, new Vector2(this.enableDecayButton.rectangle.X + 48, this.enableDecayButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Reset Values", new Vector2(this.resetValuesButton.rectangle.X + 48, this.resetValuesButton.rectangle.Y), Color.White);
        b.DrawString(Main.font, "Clear Particles", new Vector2(this.clearParticlesButton.rectangle.X + 48, this.clearParticlesButton.rectangle.Y), Color.White);    

        b.DrawString(Main.font, "Particle Color: ", new Vector2(this.redColorButton.rectangle.X, this.redColorButton.rectangle.Y - 48), Color.White);  
        b.DrawString(Main.font, "" + this.selectedColor, new Vector2(this.redColorButton.rectangle.X + 140, this.redColorButton.rectangle.Y - 48), this.color);

        foreach (Button btn in this.buttons) {

            btn.Draw(b);
        }
    }

    private void checkButtonClick() {

        foreach (Button b in this.buttons) {

            b.Update();
        }

        this.fastIncrement = Util.IsKeyHold(Microsoft.Xna.Framework.Input.Keys.LeftShift);

        this.change = this.fastIncrement ? 5 : 1;

        foreach (var a in this.buttonActions) {

            if (a.Key.isClicked) {

                a.Value.Invoke();
                a.Key.isClicked = false;
                break;
            }
        }

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
            this.switchingColors = false;
            this.randomColorButton.isClicked = false;

        } else if (this.switchingColorButton.isClicked) {

            this.selectedColor = "Switching";
            this.switchingColors = true;
            this.randomColors = false;
            this.switchingColorButton.isClicked = false;
        }

        if (!(this.selectedColor.Equals("Random") || this.selectedColor.Equals("Switching"))) {
            this.switchingColors = false;
            this.randomColors = false;
        }
    }

    private void createButtons() {

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

        this.enableRandomSpawnButton = new Button(new Rectangle(350, this.increaseSpeedButton.rectangle.Y, this.buttonSize * 2, this.buttonSize), Color.Gray);
        this.enableEdgeCollisionButton = new Button(new Rectangle(this.enableRandomSpawnButton.rectangle.X, this.enableRandomSpawnButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);
        this.enableDecayButton = new Button(new Rectangle(this.enableEdgeCollisionButton.rectangle.X, this.enableEdgeCollisionButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);

        //reset buttons
        this.resetValuesButton = new Button(new Rectangle(350, this.enableDecayButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);
        this.clearParticlesButton = new Button(new Rectangle(this.resetValuesButton.rectangle.X, this.resetValuesButton.rectangle.Y + 32, this.buttonSize * 2, this.buttonSize), Color.Gray);

        //color buttons
        this.redColorButton = new Button(new Rectangle(800, Main.screenHeight + 90, this.buttonSize * 2, this.buttonSize * 2), Color.Red);
        this.orangeColorButton = new Button(new Rectangle(this.redColorButton.rectangle.X + this.redColorButton.rectangle.Width, this.redColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Orange);
        this.yellowColorButton = new Button(new Rectangle(this.orangeColorButton.rectangle.X + this.orangeColorButton.rectangle.Width, this.orangeColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Yellow);
        this.greenColorButton = new Button(new Rectangle(this.yellowColorButton.rectangle.X + this.yellowColorButton.rectangle.Width, this.yellowColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Green);
        this.blueColorButton = new Button(new Rectangle(this.greenColorButton.rectangle.X + this.greenColorButton.rectangle.Width, this.greenColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.SkyBlue);
        this.darkBlueColorButton = new Button(new Rectangle(this.redColorButton.rectangle.X, this.redColorButton.rectangle.Y + this.redColorButton.rectangle.Height, this.buttonSize * 2, this.buttonSize * 2), Color.DarkBlue);
        this.purpleColorButton = new Button(new Rectangle(this.darkBlueColorButton.rectangle.X + this.darkBlueColorButton.rectangle.Width, this.darkBlueColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Purple);
        this.whiteColorButton = new Button(new Rectangle(this.purpleColorButton.rectangle.X + this.purpleColorButton.rectangle.Width, this.purpleColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.White);
        this.randomColorButton = new Button(new Rectangle(this.whiteColorButton.rectangle.X + this.whiteColorButton.rectangle.Width, this.whiteColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.Coral);
        this.switchingColorButton = new Button(new Rectangle(this.randomColorButton.rectangle.X + this.randomColorButton.rectangle.Width, this.randomColorButton.rectangle.Y, this.buttonSize * 2, this.buttonSize * 2), Color.SaddleBrown);

        this.randomColorButton.text = "?";

        buttons = new List<Button> {

            this.increaseSpeedButton,
            this.decreaseSpeedButton,
            this.increaseSizeButton,
            this.decreaseSizeButton,
            this.increaseParticleCountButton,
            this.decreaseParticleCountButton,
            this.increaseSpawnFrequencyButton,
            this.decreaseSpawnFrequencyButton,
            this.increaseRadiusButton,
            this.decreaseRadiusButton,
            this.enableRandomSpawnButton,
            this.enableEdgeCollisionButton,
            this.enableDecayButton,
            this.resetValuesButton,
            this.clearParticlesButton,
            this.redColorButton,
            this.orangeColorButton,
            this.yellowColorButton,
            this.greenColorButton,
            this.blueColorButton,
            this.darkBlueColorButton,
            this.purpleColorButton,
            this.whiteColorButton,
            this.switchingColorButton,
            this.randomColorButton
        };

        this.buttonActions = new Dictionary<Button, Action> {

            { this.increaseSpeedButton, () => this.particleSpeed += change },
            { this.decreaseSpeedButton, () => this.particleSpeed -= change },
            { this.increaseSizeButton, () => this.particleSize += change },
            { this.decreaseSizeButton, () => this.particleSize -= change },
            { this.increaseParticleCountButton, () => this.particleCount += change },
            { this.decreaseParticleCountButton, () => this.particleCount -= change },
            { this.increaseSpawnFrequencyButton, () => this.spawnFrequency += change },
            { this.decreaseSpawnFrequencyButton, () => this.spawnFrequency -= change },
            { this.increaseRadiusButton, () => this.radius += change },
            { this.decreaseRadiusButton, () => this.radius -= change },
            { this.enableRandomSpawnButton, () => this.randomSpawn = !this.randomSpawn },
            { this.enableEdgeCollisionButton, () => Main.enableCollision = !Main.enableCollision },
            { this.enableDecayButton, () => Main.enableAlpha = !Main.enableAlpha },
            { this.resetValuesButton, () =>
                {
                    this.particleSpeed = 5;
                    this.particleSize = 2;
                    this.particleCount = 60;
                    this.spawnFrequency = 0;
                    this.radius = 0;
                    this.randomSpawn = false;
                    Main.enableCollision = true;
                    Main.enableAlpha = true;
                }
            },
            { this.clearParticlesButton, () => this.particles.Clear() }
        };

        this.colorButtons = new Dictionary<Button, (Color color, string name)> {

            { this.redColorButton, (Color.Red, "Red") },
            { this.orangeColorButton, (Color.Orange, "Orange") },
            { this.yellowColorButton, (Color.Yellow, "Yellow") },
            { this.greenColorButton, (Color.Green, "Green") },
            { this.blueColorButton, (Color.SkyBlue, "Blue") },
            { this.darkBlueColorButton, (Color.DarkBlue, "Dark Blue") },
            { this.purpleColorButton, (Color.Purple, "Purple") },
            { this.whiteColorButton, (Color.White, "White") }
        };
    }

    public static int getScreenWidth() => Main.screenWidth;

    public static int getScreenHeigth() => Main.screenHeight;
}