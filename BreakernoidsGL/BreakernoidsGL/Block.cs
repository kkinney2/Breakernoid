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
    public enum BlockColor
    {
            Red = 0,
            Yellow,
            Blue,
            Green,
            Purple,
            GreyHi,
            Grey
    }

    class Block : GameObject
    {
        private bool isMarkedForRemoval;

        public Block(BlockColor bColor, Game myGame) : base(myGame)
        {
            switch (bColor)
            {
                case BlockColor.Red:
                    textureName = "block_red";
                    break;
                case BlockColor.Yellow:
                    textureName = "block_yellow";
                    break;
                case BlockColor.Blue:
                    textureName = "block_blue";
                    break;
                case BlockColor.Green:
                    textureName = "block_green";
                    break;
                case BlockColor.Purple:
                    textureName = "block_purple";
                    break;
                case BlockColor.GreyHi:
                    textureName = "block_grey_hi";
                    break;
                case BlockColor.Grey:
                    textureName = "block_grey";
                    break;
                default:
                    textureName = "block_red";
                    break;
            }

            isMarkedForRemoval = false;
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
