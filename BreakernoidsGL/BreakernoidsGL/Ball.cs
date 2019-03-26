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
    class Ball : GameObject
    {
        public float speed = 350;
        public Vector2 direction = new Vector2(0.707f, -0.707f);

        bool isBallCaught = false;
        bool isMarkedForRemoval = false;

        public Ball(Game myGame) : base(myGame)
        {
            textureName = "ball";
        }

        public override void Update(float deltaTime)
        {
            if (!isBallCaught)
            {
                position += direction * speed * deltaTime;
            }

            base.Update(deltaTime);
        }

        public void ResetDirection()
        {
            direction = new Vector2(0.707f, -0.707f);
        }

        public void ToggleBallCaught()
        {
            isBallCaught = !isBallCaught;
        }

        public bool IsBallCaught()
        {
            return isBallCaught;
        }

        public bool IsMarkedForRemoval()
        {
            return isMarkedForRemoval;
        }

        public void MarkForRemoval(bool newMark)
        {
            isMarkedForRemoval = newMark;
        }
    }
}
