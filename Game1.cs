using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
//using SharpDX.Direct2D1;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace BreakoutRIP
{
    // Anpassa fönstret efter blocken (Idk how)
    
    // 14 block / (Finns men kan inte ses)

    // Muskontrol //
    // Namn och poäng i title //
    // Boll inte nedkanten //
    // Spelare får poäng för krossade block //

    // Maxspeed / Random Direction?

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private MouseState mouseState;
        private MouseState oldMouseState;
        private MouseState newMouseState;
        private KeyboardState keyboardState;
        private KeyboardState oldkeyboardState;

        private Texture2D blockTexture;
        private List<Block> blockList = new List<Block>();

        private Texture2D ballTexture;
        private Ball ball1;

        private Texture2D paddleTexture;
        private Paddle paddle1;

        public int PlayerPoints = 0;

        public int GameWindowWidthPixels = 1600;
        public int GameWindowHeightPixels = 900;

        public Random Rng = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = GameWindowWidthPixels;
            _graphics.PreferredBackBufferHeight = GameWindowHeightPixels;

            Window.Title = "Breakout RIP - " + PlayerPoints;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            mouseState = new MouseState();
            oldMouseState = mouseState;
            newMouseState = oldMouseState;
            keyboardState = new KeyboardState();
            oldkeyboardState = keyboardState;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            blockTexture = Content.Load<Texture2D>("Images/block_red");
            ballTexture = Content.Load<Texture2D>("Images/ball_purple");
            paddleTexture = Content.Load<Texture2D>("Images/paddle");

            int blockCount = 14;
            for (int i = 0; i < blockCount; i++)
            {
                Vector2 position = new Vector2(i * blockTexture.Width, 10);
                Block block = new Block(blockTexture, position);
                blockList.Add(block);
            }

            int newWindowWidth = blockCount * blockTexture.Width + 50;
            GameWindowWidthPixels = newWindowWidth;
            _graphics.PreferredBackBufferWidth = GameWindowWidthPixels;
            _graphics.PreferredBackBufferHeight = GameWindowHeightPixels;

            Vector2 ballPos = new Vector2(GameWindowWidthPixels / 2 - ballTexture.Width / 2, GameWindowHeightPixels / 2);
            ball1 = new Ball(ballTexture, ballPos);

            Vector2 paddlePos = new Vector2(GameWindowWidthPixels / 2 - paddleTexture.Width / 2, GameWindowHeightPixels - paddleTexture.Height - 5);
            paddle1 = new Paddle(paddleTexture, paddlePos);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            Window.Title = "Breakout RIP - " + PlayerPoints + " Points";

            mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseState.X > oldMouseState.X)
                {
                    paddle1.MoveRight();
                }
                else if (mouseState.X < oldMouseState.X)
                {
                    paddle1.MoveLeft();
                }
            }
            oldMouseState = mouseState;

            // Check Collisions
            if (ball1.CollisionRectangle.Intersects(paddle1.CollisionRectangle))
            {
                //ball1.ChangeDirection(paddle1.DirectionString, true);
                ball1.ChangeDirection();
            }

            foreach (Block block in blockList)
            {
                if (ball1.CollisionRectangle.Intersects(block.CollisionRectangle))
                {
                    block.IsBroken = true;
                    PlayerPoints++;

                    //ball1.ChangeDirection("BLOCK");
                    ball1.ChangeDirection();
                }
            }

            List<Block> newblockList = new List<Block>();
            foreach(Block block in blockList)
            {
                if (!block.IsBroken)
                {
                    newblockList.Add(block);
                }
            }
            blockList.Clear();
            blockList.AddRange(newblockList);

            keyboardState = Keyboard.GetState();

            paddle1.Update(keyboardState, oldkeyboardState);
            ball1.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // TODO: Add your drawing code here
            foreach (Block block in blockList)
            {
                if (!block.IsBroken)
                {
                    _spriteBatch.Draw(block.Texture, block.CollisionRectangle, block.Color);
                }
            }

            _spriteBatch.Draw(ball1.Texture, ball1.CollisionRectangle, ball1.Color);
            _spriteBatch.Draw(paddle1.Texture, paddle1.CollisionRectangle, paddle1.Color);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class Block
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle CollisionRectangle;
        public Color Color;
        public bool IsBroken;

        public Block(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = Color.White;
            IsBroken = false;
        }

        public void Update()
        {

        }
    }
    public class Ball
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle CollisionRectangle;
        public Color Color;
        public string DirectionString;
        private int speed;
        private int maxSpeed;
        private Vector2 direction;

        public Ball(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = Color.White;
            DirectionString = "NONE";
            speed = 5;
            maxSpeed = 8;
            direction = new Vector2(0.8f, speed);
        }

        public void ChangeDirection(bool outOfBounds = false)
        {
            Random random = new Random();
            float nextDirChangeIndex = (float)random.NextDouble();

                if (direction.X > 0f) // Moving RIGHT
                {
                    if (direction.Y > 0f) // Moving RIGHT & DOWN
                    {
                        Vector2 newDir = new Vector2(direction.X + nextDirChangeIndex, (direction.Y + nextDirChangeIndex) * -1);
                        direction = newDir;
                    }
                    else if (direction.Y < 0f) // Moving RIGHT & UP
                    {
                        Vector2 newDir = new Vector2(direction.X + nextDirChangeIndex, (direction.Y + nextDirChangeIndex) * -1);
                        direction = newDir;
                    }
                    else
                    {
                        Vector2 newDir = new Vector2(direction.X + nextDirChangeIndex, (direction.Y + nextDirChangeIndex) * -1);
                        direction = newDir;
                    }
                }
                else if (direction.X < 0f) // Moving LEFT
                {
                    if (direction.Y > 0f) // Moving LEFT & DOWN
                    {
                        Vector2 newDir = new Vector2(direction.X + nextDirChangeIndex, (direction.Y + nextDirChangeIndex) * -1);
                        direction = newDir;
                    }
                    else if (direction.Y < 0f) // LEFT RIGHT & UP
                    {
                        Vector2 newDir = new Vector2(direction.X + nextDirChangeIndex, (direction.Y + nextDirChangeIndex) * -1);
                        direction = newDir;
                    }
                    else
                    {
                        Vector2 newDir = new Vector2(direction.X + nextDirChangeIndex, (direction.Y + nextDirChangeIndex) * -1);
                        direction = newDir;
                    }

                }
                //if (direction.X > 0f) // Moving RIGHT
                //{
                //    if (direction.Y > 0f) // Moving RIGHT & DOWN
                //    {
                //        Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
                //        direction = newDir;
                //    }
                //    else if (direction.Y < 0f) // Moving RIGHT & UP
                //    {
                //        Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
                //        direction = newDir;
                //    }
                //    else
                //    {
                //        Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
                //        direction = newDir;
                //    }
                //}
                //else if (direction.X < 0f) // Moving LEFT
                //{
                //    if (direction.Y > 0f) // Moving LEFT & DOWN
                //    {
                //        Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
                //        direction = newDir;
                //    }
                //    else if (direction.Y < 0f) // LEFT RIGHT & UP
                //    {
                //        Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
                //        direction = newDir;
                //    }
                //    else
                //    {
                //        Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
                //        direction = newDir;
                //    }
                //}
            
            //if (outOfBounds && Position.X <= 0 && Position.X >= 1600 - Texture.Width)
            //{
            //    direction.X = -speed;
            //}
        }

        //public  void ChangeDirection(string newDirectionString, bool isPaddleCollition=false)
        //{
            //if (DirectionString == "NONE")
            //if (false)
            //{
            //    if (newDirectionString == "NONE")
            //    {
            //        if (isPaddleCollition)
            //        {
            //            Vector2 newDirection = new Vector2(direction.X, direction.Y * -1);
            //            direction = newDirection;
            //        }
            //    }
            //    else if (newDirectionString == "LEFT")
            //    {
            //        Vector2 newDirection = new Vector2(-speed, direction.Y * -1);
            //        direction = newDirection;
            //    }
            //    else if (newDirectionString == "RIGHT")
            //    {
            //        Vector2 newDirection = new Vector2(speed, direction.Y * -1);
            //        direction = newDirection;
            //    }
            //}
            //else
            //{
            //    if (direction.X > 0) // Moving RIGHT
            //    {
            //        if (direction.Y > 0) // Moving RIGHT & DOWN
            //        {
            //            Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
            //            direction = newDir;
            //        }
            //        else if (direction.Y < 0) // Moving RIGHT & UP
            //        {
            //            Vector2 newDir = new Vector2(direction.X, direction.Y * -1);
            //            direction = newDir; 
            //        }
            //    }
            //}

            //if (DirectionString == "NONE")
            //{
            //    if (newDirectionString == "NONE")
            //    {
            //        if (isPaddleCollition)
            //        {
            //            Vector2 newDirection = new Vector2(direction.X, direction.Y *-1);
            //            direction = newDirection;
            //        }
            //    }
            //    else if (newDirectionString == "LEFT")
            //    {
            //        Vector2 newDirection = new Vector2(-speed, direction.Y*-1);
            //        direction = newDirection;
            //    }
            //    else if (newDirectionString == "RIGHT")
            //    {
            //        Vector2 newDirection = new Vector2(speed, direction.Y*-1);
            //        direction = newDirection;
            //    }
            //}
            //else if (newDirectionString == "BLOCK")
            //{
            //    Vector2 newDirection = new Vector2(direction.X * -1, speed);
            //    direction = newDirection;
            //    if (DirectionString == "LEFT")
            //    {
            //        DirectionString = "RIGHT";
            //    }
            //    else if (DirectionString == "RIGHT")
            //    {
            //        DirectionString = "LEFT";
            //    }
            //}
            //else
            //{
            //    if (DirectionString == "UP")
            //    {
            //        Vector2 newDirection = new Vector2(direction.X, speed);
            //        direction = newDirection;
            //        DirectionString = "DOWN";
            //    }
            //    else if (DirectionString == "DOWN")
            //    {
            //        Vector2 newDirection = new Vector2(direction.X, -speed);
            //        direction = newDirection;
            //        DirectionString = "UP";
            //    }
            //    else if (DirectionString == "LEFT") // HERE
            //    {
            //        Vector2 newDirection = new Vector2(direction.X * -1, direction.Y *-1);
            //        direction = newDirection;
            //        DirectionString = "RIGHT";
            //    }
            //    else if (DirectionString == "RIGHT")
            //    {
            //        Vector2 newDirection = new Vector2(direction.X * -1, direction.Y *-1);
            //        direction = newDirection;
            //        DirectionString = "LEFT";
            //    }
            //}

        //}

        private bool CheckIfInBounds_X(Vector2 posToCheck)
        {
            bool inBoundsX = false;

            if (posToCheck.X >= 0 && posToCheck.X <= 1600 - Texture.Width)
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
            //if (posToCheck.X >= 0 && posToCheck.X <= 1600 - Texture.Width && posToCheck.Y >= 0 && posToCheck.Y <= 900 - Texture.Height)
            //{
            //    inBounds = true;
            //}

            return inBounds;
        }

        public void Update()
        {
            Position = new Vector2(Position.X + direction.X, Position.Y  + direction.Y);
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
                //ChangeDirection(true);
            }
        }
    }
    public class Paddle
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle CollisionRectangle;
        public Color Color;
        public string DirectionString;
        private int speed;

        public Paddle(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = Color.White;
            DirectionString = "NONE";
            speed = 10;
        }

        private void UpdateRectangle(Vector2 newPos)
        {
            CollisionRectangle = new Rectangle((int)newPos.X, (int)newPos.Y, Texture.Width, Texture.Height);
        }

        public void MoveLeft()
        {
            Position = new Vector2(Position.X - speed, Position.Y);
            UpdateRectangle(Position);
            DirectionString = "LEFT";
        }
        public void MoveRight()
        {
            Position = new Vector2(Position.X + speed, Position.Y);
            UpdateRectangle(Position);
            DirectionString = "RIGHT";
        }

        public void Update(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            if (keyboardState.GetPressedKeys().Contains(Keys.Left))
            {
                MoveLeft();
            }
            else if (keyboardState.GetPressedKeys().Contains(Keys.Right))
            {
                MoveRight();
            }
        }

        public void MovePaddle(Vector2 direction)
        {
            Position += direction;
        }
    }
}