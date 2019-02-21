using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        Paddle paddle;
        Ball ball;
        private int collisionCount = 0;
        List<Block> blocks = new List<Block>();
        int[,] blockLayout = new int[,]{
            {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
            {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
        };

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

            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(512, 740);

            ball = new Ball(this);
            ball.LoadContent();
            ball.position = new Vector2(512, 740);

            for (int i = 0; i < 15; i++)
            {
                Block tempBlock = new Block(BlockColor.Purple, this);
                tempBlock.LoadContent();
                tempBlock.position = new Vector2(64 + i * 64, 200);
                blocks.Add(tempBlock);
            }

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
            ball.Update(deltaTime);

            CheckCollisions();

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
            ball.Draw(spriteBatch);
            foreach (Block b in blocks)
            {
                b.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void CheckCollisions()
        {
            float radius = ball.Width / 2;

            // Paddle Collisions
            if (collisionCount == 0)
            {
                if ((ball.position.X > (paddle.position.X - radius - paddle.Width / 2)) &&      // Left Check

                (ball.position.X < (paddle.position.X + radius + paddle.Width / 2))     &&      // Right Check

                (ball.position.Y < paddle.position.Y + radius + paddle.Height / 2)      &&      // Bottom Check

                (ball.position.Y > (paddle.position.Y - radius - paddle.Height / 2))      )     // Top Check -- Pixel based game (0,0) is top left)
                {
                                                                                                                  // Paddle Bounds
                    // Right 1/3 of paddle
                    if (ball.position.X < (paddle.position.X + paddle.Width / 2) &&                               // Right paddle bound
                        ball.position.X > (paddle.position.X + paddle.Width / 2 - paddle.Width / 3))              // Right Inner Third
                    {
                        ball.direction = Vector2.Reflect(ball.direction, new Vector2(0.196f, -0.981f));
                    }

                    // Center 1/3 of paddle
                    else if (ball.position.X > (paddle.position.X - paddle.Width / 2 + paddle.Width / 3) &&        // Middle Left Bound
                             ball.position.X < (paddle.position.X + paddle.Width / 2 - paddle.Width / 3))          // Middle Right Bound
                    {
                        ball.direction = Vector2.Reflect(ball.direction, new Vector2(0, -1));
                    }

                    // Left 1/3 of paddle
                    else if (ball.position.X > paddle.position.X - paddle.Width / 2 &&                             // Left paddle bound
                             ball.position.X < (paddle.position.X - paddle.Width / 2 + paddle.Width / 3))          // Left Inner Third
                    {
                        ball.direction = Vector2.Reflect(ball.direction, new Vector2(-0.196f, -0.981f));
                    }
                    collisionCount = 20;
                }
            }

            if (collisionCount > 0)
            {
                collisionCount--;
            }

            // Boundary Collisions
            if (Math.Abs(ball.position.X - 32) < radius)
            {
                // Left wall collision
                ball.direction.X = -ball.direction.X;
            }
            if (Math.Abs(ball.position.X - 992) < radius)
            {
                // Right wall collision
                ball.direction.X = -ball.direction.X;
            }
            if( Math.Abs(ball.position.Y - 32) < radius)
            {
                // Ceiling collision
                ball.direction.Y = -ball.direction.Y;
            }
            if (ball.position.Y > 768 + radius)
            {
                // "Bottom Out"
                LoseLife();
            }


            // Block Collisions
            foreach (Block b in blocks)
            {
                if ((ball.position.X > (b.position.X - b.Width / 2 - radius)) &&
                    (ball.position.X < (b.position.X + b.Width / 2 + radius)) &&
                    (ball.position.Y > (b.position.Y - b.Height / 2 - radius)) &&
                    (ball.position.Y < (b.position.Y + b.Height / 2 + radius)))
                {
                    if ((b.position.X - b.Width / 2) > ball.position.X)
                    {
                        // Left block side collision
                        ball.direction.X = -ball.direction.X;
                        b.MarkForRemoval(true);
                        continue;
                    }


                    if ((b.position.X + b.Width / 2) < ball.position.X)
                    {
                        // Right block side collision
                        ball.direction.X = -ball.direction.X;
                        b.MarkForRemoval(true);
                        continue;
                    }

                    if ((b.position.Y + b.Height / 2) < ball.position.Y)
                    {
                        // Top block side collision
                        ball.direction.Y = -ball.direction.Y;
                        b.MarkForRemoval(true);
                        continue;
                    }

                    if ((b.position.Y - b.Height / 2) > ball.position.Y)
                    {
                        // Bottom block side collision
                        ball.direction.Y = -ball.direction.Y;
                        b.MarkForRemoval(true);
                        continue;
                    }
                }
            }
            RemoveBlocks();
        }

        void LoseLife()
        {
            paddle.ResetPosition();
            ball.position = new Vector2(paddle.position.X, paddle.position.Y - ball.Height - paddle.Height);
            ball.ResetDirection();
        }

        void RemoveBlocks()
        {
            for (int b = blocks.Count-1; b > 0; b--)
            {
                if(blocks[b].IsMarkedForRemoval() == true)
                {
                    blocks.Remove(blocks[b]);
                }
            }
        }
        
    }
}
