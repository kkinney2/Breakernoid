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
    public class Paddle : GameObject
    {
        public float speed;

        public Paddle(Game myGame) : base(myGame)
        {
            textureName = "paddle";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Left))
            {
                position.X -= speed * deltaTime;
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                position.X += speed * deltaTime;
            }
        }
    }
}
