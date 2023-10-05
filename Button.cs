﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutRIP
{
    public class Button
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle Bounds;
        public Color Color;

        public Button(Texture2D texture, Vector2 position, Color color)
        {
            Texture = texture;
            Position = position;
            Color = color;
            Bounds = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
        }
    }
}
