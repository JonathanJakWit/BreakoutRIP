using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakoutRIP
{
    public class PowerUp : MovableSprite
    {
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
            : base(texture, position, color, new(0, fallDownSpeed))
        {
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
