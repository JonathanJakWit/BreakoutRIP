using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakoutRIP
{
    public class Block : MovableSprite
    {
        public enum BlockTypes
        {
            Normal,
            Super,
            Falling,
            CloseTime
        }
        public BlockTypes BlockType;
        public bool IsBroken;
        public bool IsFalling;
        public int PointAmount;
        public int Health;
        public bool IsSuperBlock;

        private int fallDownSpeed;

        public Block(Texture2D texture, Vector2 position, Color color, BlockTypes blockType, int health = 1, int pointAmount = 1, bool isFalling = false, int fallSpeed = 3)
            : base(texture, position, color, new(0, fallSpeed))
        {
            BlockType = blockType;
            IsBroken = false;
            IsFalling = isFalling;
            PointAmount = pointAmount;
            Health = health;
            fallDownSpeed = fallSpeed;

            if (health > 1)
            {
                IsSuperBlock = true;
            }
            else
            {
                IsSuperBlock = false;
            }
        }

        public void Update()
        {
            if (!IsBroken)
            {
                if (IsFalling)
                {
                    Position = new Vector2(Position.X, Position.Y + fallDownSpeed);
                    UpdateRectangle(Position);
                }
            }
        }

        public void UpdateRectangle(Vector2 newPos)
        {
            CollisionRectangle = new Rectangle((int)newPos.X, (int)newPos.Y, Texture.Width, Texture.Height);
        }
    }
}
