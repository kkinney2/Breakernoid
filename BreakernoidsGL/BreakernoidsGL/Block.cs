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
    class Block : GameObject
    {
        private bool isMarkedForRemoval;

        public Block(Game myGame) : base(myGame)
        {
            textureName = "block_red";

            isMarkedForRemoval = false;
        }

        public bool IsMarkedForRemoval()
        {
            return isMarkedForRemoval;
        }

    }
    
}
