using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BreakoutRIP
{
    public class Paddle : MovableSprite
    {
        private int speed;
        private Vector2 startPosition;
        private Keys moveLeftKey;
        private Keys moveRightKey;

        public Paddle(Texture2D texture, Vector2 position, Keys moveLKey, Keys moveRKey, Color color)
            : base(texture, position, color, new(0, 0))
        {
            speed = 15;
            startPosition = position;
            moveLeftKey = moveLKey;
            moveRightKey = moveRKey;
        }

        public void ResetPaddle()
        {
            Position = startPosition;
            UpdateRectangle(startPosition);
        }

        private void UpdateRectangle(Vector2 newPos)
        {
            CollisionRectangle = new Rectangle((int)newPos.X, (int)newPos.Y, CollisionRectangle.Width, CollisionRectangle.Height);
        }

        public void MoveLeft()
        {
            Position = new Vector2(Position.X - speed, Position.Y);
            UpdateRectangle(Position);
        }
        public void MoveRight()
        {
            Position = new Vector2(Position.X + speed, Position.Y);
            UpdateRectangle(Position);
        }

        public void Update(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            if (keyboardState.GetPressedKeys().Contains(moveLeftKey))
            {
                MoveLeft();
            }
            else if (keyboardState.GetPressedKeys().Contains(moveRightKey))
            {
                MoveRight();
            }
        }
    }
}
