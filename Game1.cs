using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Policy;

namespace BreakoutRIP
{
    // 8 Rader block med olika färger / (ISH)
    // Anpassa spel så att poäng skrivs ut på skärmen
    // StartSkärm och GameOverSkärm som hanteras med 3 olika GameStates genom enum
    // Båda ska ha en bakgrundsbild
    // Startskärmen ska innehålla en animation genom spritesheet
    // Button som startar spelet på StartSkärmen
    // Skriv en timer och avsluta spelet efter X-antal sekunder
    // GameOverSkärmen ska ha minst 5 sprites på slumpmässiga positioner

    // Blocken ska va i en 2D Array istället för BlockList //

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
        //private List<Block> blockList = new List<Block>();
        private int blockRowCount = 8;
        private int blockColumnCount = 14;
        //private Block[,] block2DArray = new Block[8,14];
        private Block[,] filledBlock2DArray;
        private Block[,] block2DArray;

        private Dictionary<int, Color> blockColorOrder;

        private Texture2D ballTexture;
        private Ball ball1;

        private Texture2D paddleTexture;
        private Paddle paddle1;

        public enum GameStates
        {
            Start,
            Playing,
            GameOver
        }
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
            //_graphics.IsFullScreen = true;

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

            blockColorOrder = new Dictionary<int, Color>();
            blockColorOrder.Add(1, Color.Red);
            blockColorOrder.Add(2, Color.Orange);
            blockColorOrder.Add(3, Color.Yellow);
            blockColorOrder.Add(4, Color.Green);
            blockColorOrder.Add(5, Color.Turquoise);
            blockColorOrder.Add(6, Color.Blue);
            blockColorOrder.Add(7, Color.DarkBlue);
            blockColorOrder.Add(8, Color.DarkViolet);
            blockColorOrder.Add(9, Color.LightGray);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            blockTexture = Content.Load<Texture2D>("Images/block_red");
            ballTexture = Content.Load<Texture2D>("Images/ball_purple");
            paddleTexture = Content.Load<Texture2D>("Images/paddle");

            //int blockCount = 14;
            //for (int i = 0; i < blockCount; i++)
            //{
            //    Vector2 position = new Vector2(i * blockTexture.Width, 10);
            //    Block block = new Block(blockTexture, position);
            //    blockList.Add(block);
            //}


            //int blockColumnCount = 14;
            //int blockRowCount = 8;

            // Initialize block2darray
            Vector2 fBP = new Vector2(-100, -100);
            Block fB = new Block(blockTexture, fBP, Color.White);
            filledBlock2DArray = new Block[,]
            {
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
                { fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB, fB },
            };
            block2DArray = filledBlock2DArray;
            // End of Initialization of array

            for (int rowIndex = 0;rowIndex < blockRowCount;rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                {
                    Vector2 curBlockPos = new Vector2(columnIndex * blockTexture.Width, rowIndex * blockTexture.Height);
                    Color color = blockColorOrder[rowIndex+1];

                    Block block = new Block(blockTexture, curBlockPos, color);
                    block2DArray[rowIndex, columnIndex] = block;
                }
            }

            int newWindowWidth = blockColumnCount * blockTexture.Width;
            GameWindowWidthPixels = newWindowWidth;
            int newWindowHeight = (blockRowCount * blockTexture.Height) * 2 - 100;
            GameWindowHeightPixels = newWindowHeight;

            _graphics.PreferredBackBufferWidth = GameWindowWidthPixels;
            _graphics.PreferredBackBufferHeight = GameWindowHeightPixels;
            _graphics.ApplyChanges();

            Vector2 ballPos = new Vector2(GameWindowWidthPixels / 2 - ballTexture.Width / 2, GameWindowHeightPixels / 2 + 80);
            ball1 = new Ball(ballTexture, ballPos, GameWindowWidthPixels);

            Vector2 paddlePos = new Vector2(GameWindowWidthPixels / 2 - paddleTexture.Width / 2, GameWindowHeightPixels - paddleTexture.Height / 2 - 5);
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

            // Check Collisions Start //
            if (ball1.CollisionRectangle.Intersects(paddle1.CollisionRectangle))
            {
                //ball1.ChangeDirection(paddle1.DirectionString, true);
                ball1.TestChangeDirection(paddle1, true);
                //ball1.ChangeDirection();
            }

            for (int rowIndex = 0; rowIndex < blockRowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                {
                    if (!block2DArray[rowIndex, columnIndex].IsBroken)
                    {
                        if (ball1.CollisionRectangle.Intersects(block2DArray[rowIndex, columnIndex].CollisionRectangle))
                        {
                            block2DArray[rowIndex, columnIndex].IsBroken = true;
                            PlayerPoints++;
                            ball1.TestChangeDirection(paddle1);
                            //ball1.ChangeDirection();
                            rowIndex = blockRowCount + 1;
                            columnIndex = blockColumnCount + 1;
                        }
                    }
                }
            }
            // Check Collisions End //

            //foreach (Block block in blockList)
            //{
            //    if (ball1.CollisionRectangle.Intersects(block.CollisionRectangle))
            //    {
            //        block.IsBroken = true;
            //        PlayerPoints++;

            //        //ball1.ChangeDirection("BLOCK");
            //        ball1.ChangeDirection();
            //    }
            //}

            

            //List<Block> newblockList = new List<Block>();
            //foreach(Block block in blockList)
            //{
            //    if (!block.IsBroken)
            //    {
            //        newblockList.Add(block);
            //    }
            //}
            //blockList.Clear();
            //blockList.AddRange(newblockList);

            //Block[,] newBlock2DArray = new Block[blockRowCount,blockColumnCount];
            Block[,] newBlock2DArray = filledBlock2DArray;
            for (int rowIndex = 0; rowIndex < blockRowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                {
                    if (!block2DArray[rowIndex, columnIndex].IsBroken)
                    {
                        newBlock2DArray[rowIndex, columnIndex] = block2DArray[rowIndex, columnIndex];
                    }
                }
            }
            block2DArray = newBlock2DArray;

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
            //foreach (Block block in blockList)
            //{
            //    if (!block.IsBroken)
            //    {
            //        _spriteBatch.Draw(block.Texture, block.CollisionRectangle, block.Color);
            //    }
            //}

            for (int rowIndex = 0; rowIndex < blockRowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                {
                    if (!block2DArray[rowIndex, columnIndex].IsBroken)
                    {
                        _spriteBatch.Draw(block2DArray[rowIndex, columnIndex].Texture, block2DArray[rowIndex, columnIndex].CollisionRectangle, block2DArray[rowIndex, columnIndex].Color);
                    }
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

        public Block(Texture2D texture, Vector2 position, Color color)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = color;
            IsBroken = false;
        }

        public void Update()
        {
            if (IsBroken)
            {
                //Vector2 
            }
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
        //private int maxSpeed;
        private Vector2 direction;

        private int windowSizeX;

        public Ball(Texture2D texture, Vector2 position, int windowXSize)
        {
            Texture = texture;
            Position = position;
            CollisionRectangle = new Rectangle(((int)position.X), ((int)position.Y), texture.Width, texture.Height);
            Color = Color.White;
            DirectionString = "NONE";
            speed = 7;
            //maxSpeed = 10;
            direction = new Vector2(1.2f, speed);

            windowSizeX = windowXSize;
        }

        private void UpdateRectangle(Vector2 newPos)
        {
            CollisionRectangle = new Rectangle((int)newPos.X, (int)newPos.Y, Texture.Width, Texture.Height);
        }

        public void TestChangeDirection(Paddle paddle, bool isPaddle=false)
        {
            float changeXValue = 0f;

            if (isPaddle)
            {
                Vector2 paddleMidPoint = new Vector2(paddle.Position.X + paddle.Texture.Width / 2, paddle.Position.Y);
                Vector2 ballMidPoint = new Vector2(Position.X + Texture.Width / 2, Position.Y + Texture.Height / 2);

                if (ballMidPoint.X > paddleMidPoint.X)
                {
                    changeXValue = ballMidPoint.X - paddleMidPoint.X;
                    changeXValue = changeXValue / 10;
                }
                else if (ballMidPoint.X < paddleMidPoint.X)
                {
                    changeXValue = paddleMidPoint.X - ballMidPoint.X;
                    changeXValue = changeXValue / 10;
                    changeXValue = changeXValue * -1;
                }
                else
                {
                    changeXValue = ballMidPoint.X - paddleMidPoint.X + 0.1f;
                    changeXValue = changeXValue / 10;
                }
            }
            else
            {
                changeXValue = direction.X * -1 * 0.75f;
            }

            Vector2 newDir = new Vector2(direction.X + changeXValue, direction.Y*-1);
            //Vector2 newDir = new Vector2(direction.X, direction.Y*-1);
            direction = newDir;
            if (isPaddle)
            {
                Position = new Vector2(Position.X + direction.X, Position.Y + direction.Y);
                UpdateRectangle(Position);
            }
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
            //if (posToCheck.X >= 0 && posToCheck.X <= 1600 - Texture.Width && posToCheck.Y >= 0 && posToCheck.Y <= 900 - Texture.Height)
            //{
            //    inBounds = true;
            //}

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
                //ChangeDirection(true);
            }

            //Position = new Vector2(Position.X + direction.X, Position.Y + direction.Y);
            //CollisionRectangle = new Rectangle(((int)Position.X), ((int)Position.Y), Texture.Width, Texture.Height);
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
            speed = 30;
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