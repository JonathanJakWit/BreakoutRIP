using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakoutRIP
{
    public class MovableSprite
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public Rectangle CollisionRectangle;

        public MovableSprite(Texture2D texture, Vector2 position, Color color, Vector2 velocity)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Color = color;
            CollisionRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CollisionRectangle, Color);
        }
    }
}
