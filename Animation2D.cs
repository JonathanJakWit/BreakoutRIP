using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutRIP
{
    public class Animation2D
    {
        private readonly Texture2D _texture;
        private readonly List<Rectangle> _sourceRectangles = new List<Rectangle>();
        private readonly int _frames;
        private int _frame;
        private readonly float _frameTime;
        private float _frameTimeLeft;
        private bool _active = true;

        public Animation2D(Texture2D texture, int framesX, float frameTime)
        {
            _texture = texture;
            _frameTime = frameTime;
            _frameTimeLeft = _frameTime;
            _frames = framesX;
            var frameWidth = _texture.Width / framesX;
            var frameHeight = _texture.Height;

            for (int i = 0; i < _frames; i++)
            {
                _sourceRectangles.Add(new(i * frameWidth, 0, frameWidth, frameHeight));
            }
        }

        public void Stop()
        {
            _active = false;
        }
        public void Start()
        {
            _active = true;
        }

        public void Reset()
        {
            _frame = 0;
            _frameTimeLeft = _frameTime;
        }

        public void Update(GameTime gameTime)
        {
            if (!_active)
            {
                return;
            }

            //_frameTimeLeft -= Globals.TotalSeconds;
            _frameTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_frameTimeLeft <= 0 )
            {
                _frameTimeLeft += _frameTime;
                _frame = (_frame + 1) % _frames;
            }
        }

        public void Draw(Vector2 position, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, position, _sourceRectangles[_frame], Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
        }
    }
}
