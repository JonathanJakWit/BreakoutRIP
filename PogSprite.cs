using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutRIP
{
    public class PogSprite
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle Bounds;
        public Color Color;

        public PogSprite(Texture2D texture, Vector2 position, Color color)
        {
            Texture = texture;
            Position = position;
            Bounds = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = color;
        }
    }
}
