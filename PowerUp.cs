using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutRIP
{
    public class PowerUp
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle CollisionRectangle;
        public Color Color;
        public bool IsActive;
        public bool IsBroken;
        public bool IsHit;
        public int PowerUpIndex;

        private Vector2 startPosition;
        private Rectangle startRectangle;
        private int fallSpeed;
        private int windowMaxY;

        private bool isFirstTimeHit = true;
        private int powerUpTimeCounter = 1;
        private int powerUpTimeLimit = 6;
        private float powerUpCountDuration = 1f;
        private float powerUpCurrentTime = 0f;
        private float timeWhenPowerUpHit;

        public PowerUp(Texture2D texture, Vector2 position, Color color, int powerUpIndex = 1, int fallDownSpeed = 3, int gameWindowMaxY = 720)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = color;
            IsBroken = false;
            PowerUpIndex = powerUpIndex;
            fallSpeed = fallDownSpeed;
            windowMaxY = gameWindowMaxY;
            IsActive = false;
            IsHit = false;

            startPosition = Position;
            startRectangle = CollisionRectangle;
        }

        public bool IsTimerUp(GameTime gameTime)
        {
            bool timesUp = false;

            powerUpCurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (powerUpCurrentTime >= powerUpCountDuration)
            {
                powerUpTimeCounter++;
                powerUpCurrentTime -= powerUpCountDuration;
            }
            if (powerUpTimeCounter >= powerUpTimeLimit)
            {
                timesUp = true;
            }

                return timesUp;
        }

        public void ResetPowerUp()
        {
            IsActive = false;
            IsBroken = false;

            Position = startPosition;
            CollisionRectangle = startRectangle;
            CollisionRectangle = new Rectangle(((int)Position.X), ((int)Position.Y), Texture.Width, Texture.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (IsHit)
            {
                if (isFirstTimeHit)
                {
                    timeWhenPowerUpHit = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    isFirstTimeHit = false;
                }
                //else
                //{
                //    if (IsTimerUp(gameTime))
                //    {

                //    }
                //}
            }

            if (!IsBroken && IsActive)
            {
                Position = new Vector2(Position.X, Position.Y + fallSpeed);
                CollisionRectangle = new Rectangle(((int)Position.X), ((int)Position.Y), Texture.Width, Texture.Height);
                if (Position.Y > windowMaxY)
                {
                    IsBroken = true;
                    IsActive = false;
                }
            }
            else
            {
                Position = new Vector2(0, 0);
                CollisionRectangle = new Rectangle(0, 0, 1, 1);
                IsActive = false;
            }
        }
    }
}
