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
    private int particleSpeed = 10;
    private int particleSize = 5;
    private int particleCount = 60;
    private int spawnFrequency = 60;
    private float radius = 0;
    public static float decaySpeed = 0.01f;
    private float hue = 0;

    private int buttonSize = 30;
    private Button increaseSpeedButton;
    private Button decreaseSpeedButton;

    private SpriteFont font;

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
        this.spawnFrequency = (int)this.FPS / this.spawnFrequency;

        this.decreaseSpeedButton = new Button(new Rectangle(420, Main.screenHeight + this.buttonSize / 2, this.buttonSize, this.buttonSize), Color.Gray);
        this.increaseSpeedButton = new Button(new Rectangle(this.decreaseSpeedButton.rectangle.X + this.buttonSize + 12, Main.screenHeight + this.buttonSize / 2, this.buttonSize, this.buttonSize), Color.Gray);

        this.decreaseSpeedButton.text = "-";
        this.increaseSpeedButton.text = "+";

        base.Initialize();
    }

    protected override void LoadContent() {

        this.b = new SpriteBatch(this.GraphicsDevice);

        Main.pixel = new Texture2D(this.GraphicsDevice, 1, 1);
        Main.pixel.SetData(new Color[] { Color.White });

        this.font = this.contentManager.Load<SpriteFont>("Font");
    }

    protected override void Update(GameTime dt) {

        Util.Update();

        this.checkButtonClick();

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

    private void drawMenu(SpriteBatch b) {

        b.Draw(Main.pixel, new Rectangle(0, Main.screenHeight + 8, Main.screenWidth, Main.menuHeight), Color.Black);
        b.Draw(Main.pixel, new Rectangle(0, Main.screenHeight + 8, Main.screenWidth, 5), Color.White);

        b.DrawString(this.font, "Particle Speed: " + this.particleSpeed, new Vector2(10, this.increaseSpeedButton.rectangle.Y + 6), Color.White);

        this.increaseSpeedButton.Draw(b, this.font);
        this.decreaseSpeedButton.Draw(b, this.font);
    }

    private void checkButtonClick() {

        this.increaseSpeedButton.Update();
        this.decreaseSpeedButton.Update();

        if (this.increaseSpeedButton.isClicked == true) {

            this.particleSpeed++;
            this.increaseSpeedButton.isClicked = false;
        }

        else if (this.decreaseSpeedButton.isClicked == true) {

            this.particleSpeed--;
            this.decreaseSpeedButton.isClicked = false;
        }

        this.particleSpeed = MathHelper.Clamp(this.particleSpeed, 1, 20);
    }

    private void manageParticles(GameTime dt) {

        this.mouse = new Rectangle((int)Util.getMousePosition.X, (int)Util.getMousePosition.Y, 1, 1);

        if (Util.IsLeftClickHold() == true && Util.checkMouseCoordinates() == true) {

            this.mouseClicked = true;

            if (this.hue >= 360) {

                this.hue = 0;
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

            //this.particles[i].color = ColorFromHSV(globalHue, 1f, 1f);

            this.particles[i].Update(dt);

            if (particles[i].destroy == true) {

                this.particles.Remove(particles[i]);
            }
        }            
    }

    private void spawnParticle(int x, int y) {

        if (this.randomSpawn == false) {

            for (int i = 0; i < this.particleCount; i++) {

                float angle = MathHelper.TwoPi * i / particleCount;
                float velocityX = this.particleSpeed * (float)Math.Cos(angle);
                float velocityY = this.particleSpeed * (float)Math.Sin(angle);

                float spawnX = x + radius * (float)Math.Cos(angle);
                float spawnY = y + radius * (float)Math.Sin(angle);

                Particle p = new Particle(velocityX, velocityY, spawnX, spawnY, this.particleSize, Util.ColorFromHSV(this.hue, 1f, 1f));
                this.particles.Add(p);
            }
        } else {
            for (int i = 0; i < this.particleCount; i++) {

                float angle = MathHelper.TwoPi * (float)this.random.NextDouble();
                float radius = this.radius * (float)Math.Sqrt(this.random.NextDouble());

                float velocityX = this.particleSpeed * (float)Math.Cos(angle);
                float velocityY = this.particleSpeed * (float)Math.Sin(angle);

                float spawnX = x + this.radius * (float)Math.Cos(angle);
                float spawnY = y + this.radius * (float)Math.Sin(angle);

                Particle p = new Particle(velocityX, velocityY, spawnX, spawnY, this.particleSize, Util.ColorFromHSV(this.hue, 1f, 1f));
                this.particles.Add(p);
            }
        }
    }

    public static int getScreenWidth() => Main.screenWidth;

    public static int getScreenHeigth() => Main.screenHeight;
}
