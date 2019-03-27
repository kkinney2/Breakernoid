using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Xml.Serialization;

namespace BreakernoidsGL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D bgTexture;
        Texture2D puBTexture, puCTexture, puPTexture;

        Paddle paddle;
        Level level;
        private int collisionCount = 0;
        List<Ball> balls = new List<Ball>();
        List<Block> blocks = new List<Block>();
        List<PowerUp> powerUps = new List<PowerUp>();

        SoundEffect ballBounceSFX;
        SoundEffect ballHitSFX;
        SoundEffect deathSFX;
        SoundEffect powerupSFX;

        Random random = new Random();
        double probPowerUp = 0.2; // Which means that when you destroy a block, a power-up will spawn 20% of the time
        float speedMult = 0;
        int levelNum = 1;
        bool isPuBActive = false;
        bool isPuCActive = false;
        bool isPuPActive = false;
        Vector2 ballPosDisplaceTemp;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; // Where all content is found

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            bgTexture = Content.Load<Texture2D>("bg");
            puBTexture = Content.Load<Texture2D>("powerup_b");
            puCTexture = Content.Load<Texture2D>("powerup_c");
            puPTexture = Content.Load<Texture2D>("powerup_p");

            LoadLevel("Level5.xml");

            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(512, 740);

            SpawnBall();


            /*for (int row = 0; row < blockLayout.GetLength(0); row++)
            {
                for (int col = 0; col < blockLayout.GetLength(1) ; col++)
                {
                    Block tempBlock = new Block((BlockColor)blockLayout[row, col], this);
                    tempBlock.LoadContent();
                    tempBlock.position = new Vector2(64 + col * 64, 100 + row * 32);
                    blocks.Add(tempBlock);
                }
            }*/

            ballBounceSFX = Content.Load<SoundEffect>("ball_bounce");
            ballHitSFX = Content.Load<SoundEffect>("ball_hit");
            deathSFX = Content.Load<SoundEffect>("death");
            powerupSFX = Content.Load<SoundEffect>("powerup");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            paddle.Update(deltaTime);

            foreach (Ball b in balls)
            {
                b.Update(deltaTime);
                CheckCollisions(b);
            }

            foreach (PowerUp pu in powerUps)
            {
                pu.Update(deltaTime);
            }

            CheckForPowerups();

            PowerUpBehaviors();

            RemoveBalls();
            RemoveBlocks();
            RemovePowerUps();

            if (balls.Count == 0)
            {
                LoseLife();
            }

            if (blocks.Count == 0)
            {
                NextLevel();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);


            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // Draw all sprites 
            spriteBatch.Draw(bgTexture, new Vector2(0, 0), Color.White);
            paddle.Draw(spriteBatch);

            foreach (Ball b in balls)
            {
                b.Draw(spriteBatch);
            }

            foreach (Block bl in blocks)
            {
                bl.Draw(spriteBatch);
            }

            foreach (PowerUp pu in powerUps)
            {
                pu.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        void CheckCollisions(Ball ball)
        {
            KeyboardState keyState = Keyboard.GetState();

            float radius = ball.Width / 2;

            // Paddle Collisions
            if (collisionCount == 0)
            {
                if ((ball.position.X > (paddle.position.X - radius - paddle.Width / 2)) &&      // Left Check

                (ball.position.X < (paddle.position.X + radius + paddle.Width / 2)) &&      // Right Check

                (ball.position.Y < paddle.position.Y + radius + paddle.Height / 2) &&      // Bottom Check

                (ball.position.Y > (paddle.position.Y - radius - paddle.Height / 2)))     // Top Check -- Pixel based game (0,0) is top left)
                {
                    if (isPuCActive && ball.IsBallCaught() == false)
                    {
                        ball.ToggleBallCaught(); // Toggles to say ball is "caught" and to stop movement
                        ballPosDisplaceTemp = ball.position - paddle.position;
                    }
                    // Paddle Bounds
                    // Right 1/3 of paddle
                    if (ball.position.X < (paddle.position.X + paddle.Width / 2) &&                               // Right paddle bound
                        ball.position.X > (paddle.position.X + paddle.Width / 2 - paddle.Width / 3))              // Right Inner Third
                    {
                        ball.direction = Vector2.Reflect(ball.direction, new Vector2(0.196f, -0.981f));
                    }

                    // Center 1/3 of paddle
                    if (ball.position.X > (paddle.position.X - paddle.Width / 2 + paddle.Width / 3) &&        // Middle Left Bound
                             ball.position.X < (paddle.position.X + paddle.Width / 2 - paddle.Width / 3))          // Middle Right Bound
                    {
                        ball.direction = Vector2.Reflect(ball.direction, new Vector2(0, -1));
                    }

                    // Left 1/3 of paddle
                    if (ball.position.X > paddle.position.X - paddle.Width / 2 &&                             // Left paddle bound
                             ball.position.X < (paddle.position.X - paddle.Width / 2 + paddle.Width / 3))          // Left Inner Third
                    {
                        ball.direction = Vector2.Reflect(ball.direction, new Vector2(-0.196f, -0.981f));
                    }

                    ballBounceSFX.Play();
                    collisionCount = 20;
                }
            }

            if (collisionCount > 0)
            {
                collisionCount--;
            }

            // Boundary Collisions
            if (Math.Abs(ball.position.X - 32) < radius || Math.Abs(ball.position.X - 992) < radius) // Left wall || Right Wall
            {
                ballBounceSFX.Play();
                ball.direction.X = -ball.direction.X;
            }
            if (Math.Abs(ball.position.Y - 32) < radius)
            {
                // Ceiling collision
                ball.direction.Y = -ball.direction.Y;
                ballBounceSFX.Play();
            }

            if (ball.position.Y > 768 + radius)
            {
                ball.MarkForRemoval(true);
            }


            // Block Collisions
            foreach (Block b in blocks)
            {
                if ((ball.position.X > (b.position.X - b.Width / 2 - radius)) &&
                    (ball.position.X < (b.position.X + b.Width / 2 + radius)) &&
                    (ball.position.Y > (b.position.Y - b.Height / 2 - radius)) &&
                    (ball.position.Y < (b.position.Y + b.Height / 2 + radius)))
                {
                    ballBounceSFX.Play();
                    ballHitSFX.Play();

                    if ((b.position.X - b.Width / 2) > ball.position.X || (b.position.X + b.Width / 2) < ball.position.X) // Left || Right
                    {
                        ball.direction.X = -ball.direction.X;
                    }

                    if ((b.position.Y + b.Height / 2) < ball.position.Y || (b.position.Y - b.Height / 2) > ball.position.Y) // Top || Bottom
                    {
                        ball.direction.Y = -ball.direction.Y;
                    }

                    if (b.OnHit() == false) // Case False: Means that block does not get destroyed(Color is GreyHi), change color to Grey
                    {
                        b.ChangeColor(BlockColor.Grey);
                        b.LoadContent();
                    }
                    else
                    {
                        b.MarkForRemoval(true);
                    }
                    continue;
                }
            }
        }

        void LoseLife()
        {
            deathSFX.Play();
            paddle.ResetPosition();
            paddle.SetIsPoweredUp(false);
            paddle.LoadContent();
            SpawnBall();

            isPuBActive = false;
            isPuCActive = false;
            isPuPActive = false;
        }

        void RemoveBlocks()
        {
            for (int b = blocks.Count - 1; b > 0; b--)
            {
                if (blocks[b].IsMarkedForRemoval() == true)
                {
                    // Determines whether to spawn a powerup
                    if (random.NextDouble() < probPowerUp)
                    {
                        // Spawns powerup
                        SpawnPowerUp(blocks[b].position);
                    }

                    blocks.Remove(blocks[b]);
                }
            }
        }

        void RemoveBalls()
        {
            for (int b = balls.Count - 1; b >= 0; b--)
            {
                if (balls[b].IsMarkedForRemoval() == true)
                {
                    balls.Remove(balls[b]);
                }
            }
        }

        void CheckForPowerups()
        {
            float xVal = paddle.position.X - (paddle.Width / 2);  // the x value of the top-left corner
            float yVal = paddle.position.Y - (paddle.Height / 2); // the y value of the top-left corner
            Rectangle paddleBR = paddle.BoundingRect(xVal, yVal, paddle.Width, paddle.Height);

            foreach (PowerUp pu in powerUps)
            {
                xVal = pu.position.X - (pu.Width / 2);  // the x value of the top-left corner
                yVal = pu.position.Y - (pu.Height / 2); // the y value of the top-left corner
                Rectangle puBR = pu.BoundingRect(xVal, yVal, paddle.Width, paddle.Height);

                if (puBR.Intersects(paddleBR))
                {
                    ActivatePowerUp(pu);
                    pu.MarkForRemoval(true);
                }
            }
        }

        void RemovePowerUps()
        {
            for (int pu = powerUps.Count - 1; pu >= 0; pu--)
            {
                if (powerUps[pu].IsMarkedForRemoval() == true)
                {
                    powerUps.Remove(powerUps[pu]);
                }
            }
        }

        void SpawnBall()
        {
            Ball ball = new Ball(this);
            ball.LoadContent();
            ball.position = new Vector2(512, 740);
            ball.SetBallSpeed(level.ballSpeed * 100f * speedMult);
            balls.Add(ball);
        }

        void SpawnPowerUp(Vector2 position)
        {
            PowerUp tempPowerUp = new PowerUp((PowerUpType)random.Next(3), this);
            tempPowerUp.LoadContent();
            tempPowerUp.position = position;
            powerUps.Add(tempPowerUp);
        }

        void ActivatePowerUp(PowerUp pu)
        {
            powerupSFX.Play();

            switch (pu.GetPUType())
            {
                case "powerup_b":
                    isPuBActive = true;
                    break;
                case "powerup_c":
                    isPuCActive = true;
                    break;
                case "powerup_p":
                    isPuPActive = true;
                    break;
                default:
                    break;
            }
        }

        void PowerUpBehaviors()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (isPuCActive) // Line ~207 for ball catch check
            {
                foreach (Ball ball in balls)
                {
                    if (ball.IsBallCaught() == true)
                    {
                        ball.position = new Vector2(paddle.position.X + ballPosDisplaceTemp.X, ball.position.Y);

                        if (keyState.IsKeyDown(Keys.Space))
                        {
                            ball.ToggleBallCaught();
                            isPuCActive = false;
                        }
                    }
                }
            }

            if (isPuBActive)
            {
                SpawnBall();
                isPuBActive = false;
            }

            if (isPuPActive)
            {
                paddle.SetIsPoweredUp(true);
                paddle.LoadContent();
            }
        }

        protected void LoadLevel(string levelName)
        {
            using (FileStream fs = File.OpenRead("Levels/" + levelName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Level));
                level = (Level)serializer.Deserialize(fs);
            }

            // TODO: Generate blocks based on level.layout array
            for (int row = 0; row < level.layout.Length; row++)
            {
                for (int col = 0; col < level.layout[row].Length; col++)
                {
                    BlockColor newColor = (BlockColor)level.layout[row][col];
                    if (level.layout[row][col] == 9)
                    {
                        continue;
                    }
                    Block tempBlock = new Block(newColor, this);
                    tempBlock.LoadContent();
                    tempBlock.position = new Vector2(64 + col * 64, 100 + row * 32);
                    blocks.Add(tempBlock);
                }
            }
            speedMult = levelNum--; // Ball speed multiplier
            level.nextLevel = "Level" + (levelNum > 5 ? levelNum%5 : levelNum) + ".xml"; // if levelNum > 5 then return levelNum mod 5 else return levelnum
        }

        void NextLevel()
        {
            foreach(Ball b in balls)
            {
                b.MarkForRemoval(true);
            }

            paddle.ResetPosition();
            paddle.SetIsPoweredUp(false);
            paddle.LoadContent();
            SpawnBall();

            isPuBActive = false;
            isPuCActive = false;
            isPuPActive = false;

            levelNum++;
            LoadLevel(level.nextLevel);
        }
    }
}