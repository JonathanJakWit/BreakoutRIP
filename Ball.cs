using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakoutRIP
{
    public class Ball : MovableSprite
    {
        private Vector2 direction;
        private int speed;
        private Vector2 startPosition;
        private Vector2 startDirection;
        private int windowSizeX;

        public Ball(Texture2D texture, Vector2 position, Color color, int windowXSize)
            : base(texture, position, color, new(0, 0))
        {
            speed = 7;
            direction = new Vector2(0.1f, speed);

            startPosition = position;
            startDirection = direction;
            windowSizeX = windowXSize;
        }

        public void ResetBall()
        {
            Position = startPosition;
            direction = startDirection;
            UpdateRectangle(startPosition);
        }

        private void UpdateRectangle(Vector2 newPos)
        {
            CollisionRectangle = new Rectangle((int)newPos.X, (int)newPos.Y, Texture.Width, Texture.Height);
        }

        public void TestChangeDirection(Paddle paddle, bool isPaddle = false)
        {
            float changeXValue = 0f;
            int xValueDivider = 10;
            float blockSlowDownDirectionFactor = 0.6f;

            if (isPaddle)
            {
                Vector2 paddleMidPoint = new Vector2(paddle.Position.X + paddle.Texture.Width / 2, paddle.Position.Y);
                Vector2 ballMidPoint = new Vector2(Position.X + Texture.Width / 2, Position.Y + Texture.Height / 2);

                if (ballMidPoint.X > paddleMidPoint.X)
                {
                    changeXValue = ballMidPoint.X - paddleMidPoint.X;
                    changeXValue = changeXValue / xValueDivider;
                }
                else if (ballMidPoint.X < paddleMidPoint.X)
                {
                    changeXValue = paddleMidPoint.X - ballMidPoint.X;
                    changeXValue = changeXValue / xValueDivider;
                    changeXValue = changeXValue * -1;
                }
                else
                {
                    changeXValue = ballMidPoint.X - paddleMidPoint.X + 0.1f;
                    changeXValue = changeXValue / xValueDivider;
                }
            }
            else
            {
                changeXValue = direction.X * -1 * blockSlowDownDirectionFactor;
            }

            Vector2 newDir = new Vector2(direction.X + changeXValue, direction.Y * -1);
            direction = newDir;
            if (isPaddle)
            {
                Position = new Vector2(Position.X + direction.X, Position.Y + direction.Y);
                UpdateRectangle(Position);
            }
        }

        private bool CheckIfInBounds_X(Vector2 posToCheck)
        {
            bool inBoundsX = false;

            if (posToCheck.X >= 0 && posToCheck.X <= windowSizeX - Texture.Width)
            {
                inBoundsX = true;
            }

            return inBoundsX;
        }
        private bool CheckIfInBounds_Y(Vector2 posToCheck)
        {
            bool inBoundsY = false;

            //if (posToCheck.Y >= 0 && posToCheck.Y <= 900 - Texture.Height)
            if (posToCheck.Y >= 0)
            {
                inBoundsY = true;
            }

            return inBoundsY;
        }
        private bool CheckIfInBounds(Vector2 posToCheck)
        {
            bool inBounds = false;

            if (CheckIfInBounds_X(posToCheck) && CheckIfInBounds_Y(posToCheck))
            {
                inBounds = true;
            }

            return inBounds;
        }

        public void Update()
        {
            Position = new Vector2(Position.X + direction.X, Position.Y + direction.Y);
            if (CheckIfInBounds(Position))
            {
                CollisionRectangle = new Rectangle(((int)Position.X), ((int)Position.Y), Texture.Width, Texture.Height);
            }
            else
            {
                if (CheckIfInBounds_X(Position) == false)
                {
                    direction = new Vector2(direction.X * -1, direction.Y);
                }
                if (CheckIfInBounds_Y(Position) == false)
                {
                    direction = new Vector2(direction.X, direction.Y * -1);
                }
                Position = new Vector2(Position.X + direction.X, Position.Y + direction.Y);
                CollisionRectangle = new Rectangle(((int)Position.X), ((int)Position.Y), Texture.Width, Texture.Height);
            }
        }
    }
}
