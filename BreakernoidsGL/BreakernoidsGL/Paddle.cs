﻿using System;
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
    public class Paddle : GameObject
    {
        public float speed = 500;
        bool isPoweredUp;

        public Paddle(Game myGame) : base(myGame)
        {
            textureName = "paddle";
            isPoweredUp = false;
        }

        public override void Update(float deltaTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Left))
            {
                position.X -= speed * deltaTime;
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                position.X += speed * deltaTime;
            }

            position.X = MathHelper.Clamp
                (
                    position.X,
                    32 + texture.Width / 2,
                    992 - texture.Width / 2
                );

            if (isPoweredUp)
            {
                textureName = "paddle_long";
            }
            else
            {
                textureName = "paddle";
            }

            base.Update(deltaTime);
        }

        public void ResetPosition()
        {
            position = new Vector2(512, 740);
        }

        public void SetIsPoweredUp(bool newBool)
        {
            isPoweredUp = newBool;
        }
    }
}
