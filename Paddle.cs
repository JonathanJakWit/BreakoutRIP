using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutRIP
{
    public class Paddle
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle CollisionRectangle;
        public Color Color;

        private int speed;
        private Vector2 startPosition;
        private Keys moveLeftKey;
        private Keys moveRightKey;

        public Paddle(Texture2D texture, Vector2 position, Keys moveLKey, Keys moveRKey, Color color)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = color;

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
            CollisionRectangle = new Rectangle((int)newPos.X, (int)newPos.Y, Texture.Width, Texture.Height);
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
