using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutRIP
{
    public class Block
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle CollisionRectangle;
        public Color Color;
        public bool IsBroken;
        public bool IsFalling;
        public int PointAmount;
        public int Health;
        public bool IsSuperBlock;

        private int fallDownSpeed;

        public Block(Texture2D texture, Vector2 position, Color color, int health = 1, int pointAmount = 1, bool isFalling = false, int fallSpeed = 3)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = color;
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
