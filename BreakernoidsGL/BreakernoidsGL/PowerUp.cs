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

        public PowerUp(PowerUpType powerupType , Game myGame) : base(myGame)
        {
            
        }

        public override void Update(float deltaTime)
        {
            position.Y +=  speed * deltaTime;

            base.Update(deltaTime);
        }
    }


}
