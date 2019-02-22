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
    public enum PowerUpType
    {
        powerup_c= 0,
        powerup_b,
        powerup_p
    }

    class PowerUp : GameObject
    {
        public float speed = 350;
        private bool isMarkedForRemoval;
        PowerUpType puType;

        public PowerUp(PowerUpType powerupType , Game myGame) : base(myGame)
        {
            puType = powerupType;


        }

        public override void Update(float deltaTime)
        {
            if( position.Y < 768)
            {
                position.Y += speed * deltaTime;

                base.Update(deltaTime);
            }
            else
            {
                MarkForRemoval(true);
            }
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
