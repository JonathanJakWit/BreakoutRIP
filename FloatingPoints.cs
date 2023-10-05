using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakoutRIP
{
    public class FloatingPoints : MovableSprite
    {
        public bool IsBroken;
        public int PointAmount;

        private int fallSpeed;
        private int windowMaxY;

        public FloatingPoints(Texture2D texture, Vector2 position, Color color, int pointAmount = 1, int fallDownSpeed = 3, int gameWindowMaxY = 720)
            : base( texture, position, color, new(fallDownSpeed, 0))
        {
            IsBroken = false;
            PointAmount = pointAmount;
            fallSpeed = fallDownSpeed;
            windowMaxY = gameWindowMaxY;
        }

        public void Update()
        {
            if (!IsBroken)
            {
                Position = new Vector2(Position.X, Position.Y + fallSpeed);
                CollisionRectangle = new Rectangle(((int)Position.X), ((int)Position.Y), Texture.Width, Texture.Height);
                if (Position.Y > windowMaxY)
                {
                    IsBroken = true;
                }
            }
            else
            {
                Position = new Vector2(0, 0);
                CollisionRectangle = new Rectangle(0, 0, 1, 1);
            }
        }
    }
}
