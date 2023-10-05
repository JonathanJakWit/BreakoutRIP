using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutRIP
{
    public class Coin
    {
        private static Texture2D _texture;
        private Vector2 _position;
        private readonly Animation2D _animation;

        public Coin(Vector2 position, ContentManager Content)
        {
            _texture ??= Content.Load<Texture2D>("Animations/coin_sprite_sheet");
            _animation = new Animation2D(_texture, 5, 0.1f);
            _position = position;
        }

        public void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animation.Draw(_position, spriteBatch);
        }
    }
}
