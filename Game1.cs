using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Policy;
using System.Transactions;

namespace BreakoutRIP
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Declarations
        private Random _random;
        
        // GameStates & Screen-Logic - BEGIN //
        public enum GameStates
        {
            Start,
            Playing,
            GameOver
        }
        private GameStates _gameState;
        private bool firstGameOverUpdate;

        public enum Levels
        {
            Level1,
            Level2
        }
        private Levels _currentLevel;
        // GameStates & Screen-Logic - END //

        // All Animation Declaration - BEGIN //
        private Coin _coin; // UPDATE TO NEW ANIMATION LATER
        // All Animation Declaration - END //

        // Mouse and Keyboard States - BEGIN //
        private MouseState mouseState;
        private MouseState oldMouseState;
        private KeyboardState keyboardState;
        private KeyboardState oldkeyboardState;
        // Mouse and Keyboard States - END //

        // Background Textures & Fonts - BEGIN //
        private SpriteFont gameInfoFont;
        private Rectangle backgroundBounds;
        private Texture2D startScreenBG;
        private Texture2D playingScreenBG;
        private Texture2D gameOverScreenBG;
        private Vector2 pointTextPosition;
        private Vector2 timerTextPosition;
        // Background Textures & Fonts - END //

        // Block Texture and Array - BEGIN //
        private Texture2D blockTexture;
        private int blockRowCount;
        private int blockColumnCount;
        private Block[,] filledBlock2DArray;
        private Block[,] block2DArray;
        private List<Block> closeBlockList;
        private List<Block> randomBlockList;
        private bool isCloseTime;
        private Dictionary<int, Color> blockColorOrder;
        // Block Texture and Array - END //

        // Ball, Paddle, PowerUp & PogSprite - BEGIN //
        private Texture2D ballTexture;
        private Ball ball1;
        private List<Ball> ballList;
        private int ballNr;

        private Texture2D paddleTexture;
        private Paddle paddle1;
        private Paddle paddle2;

        private Texture2D powerUpTexture;
        private PowerUp powerUp1;

        private Texture2D pogSpriteTexture;
        private List<PogSprite> pogSpriteList;
        // Ball, Paddle, & PogSprite - END //

        // FloatingPoints - BEGIN //
        private Texture2D floatingPointTexture;
        private List<FloatingPoints> floatingPointsList;
        // FloatingPoints - END //


        // Buttons - BEGIN //
        private Texture2D buttonTexture;
        private Button playButton;
        private Button playAgainButton;
        private Button exitButton;
        // Buttons - END //

        // Counter & UI-Logic - BEGIN //
        private int levelTimeCounter;
        private int levelTimeLimit;
        private float levelCountDuration;
        private float levelCurrentTime;

        private int spawnRandomBlockTimeCounter;
        private int spawnRandomBlockTimeLimit;
        private float spawnRandomBlockCountDuration;
        private float spawnRandomBlockCurrentTime;

        private int playerPoints;
        private int playerLives;
        // Counter & UI-Logic - END //


        // Game Window Properties - BEGIN //
        public int GameWindowWidthPixels;
        public int GameWindowHeightPixels;
        public int ExtraWidthPixels;
        // Game Window Properties - END //

        // Extras - BEGIN //
        private int brokenBlocksSincePaddleHit;
        private bool isPaused;
        private bool playerWon;
        // Extras - END //
        #endregion Declarations

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private bool IsHovering(MouseState mouseState, Rectangle bounds)
        {
            bool isHovering = false;

            //if (mouseState.Position.X > bounds.X && mouseState.Position.X < bounds.)
            if (bounds.Contains(mouseState.Position))
            {
                isHovering = true;
            }

            return isHovering;
        }

        protected override void Initialize()
        {
            _random = new Random();

            // GameStates & Screen-Logic - BEGIN //
            GameWindowWidthPixels = 1280;
            GameWindowHeightPixels = 720;
            ExtraWidthPixels = 384;
            _gameState = GameStates.Start;
            firstGameOverUpdate = true;
            _currentLevel = Levels.Level1;
            // GameStates & Screen-Logic - END //

            // Mouse & Keyboard States - BEGIN //
            mouseState = new MouseState();
            oldMouseState = mouseState;
            keyboardState = new KeyboardState();
            oldkeyboardState = keyboardState;
            // Mouse & Keyboard States - END //

            // Block-Logic - BEGIN //
            blockColorOrder = new Dictionary<int, Color>
            {
                { 1, Color.Red },
                { 2, Color.Orange },
                { 3, Color.Yellow },
                { 4, Color.Green },
                { 5, Color.Turquoise },
                { 6, Color.Blue },
                { 7, Color.DarkBlue },
                { 8, Color.DarkViolet },

                { 9, Color.PaleVioletRed },
                { 10, Color.Aqua },
                { 11, Color.LightGreen },
                { 12, Color.Lime },
                { 13, Color.PaleVioletRed },
                { 14, Color.Aqua },
                { 15, Color.LightGreen },
                { 16, Color.Lime },
            };

            blockRowCount = 8;
            blockColumnCount = 14;
            // Block-Logic - END //

            // Counter & UI Logic - BEGIN //
            levelTimeCounter = 1;
            levelTimeLimit = 20;
            levelCountDuration = 1f;
            levelCurrentTime = 0f;

            spawnRandomBlockTimeCounter = 1;
            spawnRandomBlockTimeLimit = 4;
            spawnRandomBlockCountDuration = 1f;
            spawnRandomBlockCurrentTime = 0f;

            playerPoints = 0;
            playerLives = 3;
            ballNr = 1;
            // Counter & UI Logic - END //

            // Animations - BEGIN //
            _coin = new Coin(new(300, 300), Content); // UPDATE TO NEW ANIMATION LATER
            // Animations - END //

            // Extras - BEGIN //
            brokenBlocksSincePaddleHit = 0;
            isPaused = false;
            playerWon = true;
            isCloseTime = false;
            // Extras - END //

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            gameInfoFont = Content.Load<SpriteFont>("Fonts/gameInfoFont");
            startScreenBG = Content.Load<Texture2D>("Backgrounds/start_screen");
            playingScreenBG = Content.Load<Texture2D>("Backgrounds/playing_screen");
            gameOverScreenBG = Content.Load<Texture2D>("Backgrounds/gameover_screen");

            blockTexture = Content.Load<Texture2D>("Images/block_white");
            ballTexture = Content.Load<Texture2D>("Images/ball_white");
            paddleTexture = Content.Load<Texture2D>("Images/paddle_white");
            buttonTexture = Content.Load<Texture2D>("Images/paddle_white"); // Placeholder tills fixat actual button textur
            powerUpTexture = Content.Load<Texture2D>("Images/ball_white"); // Placeholder tills fixat actual powerpellet textur

            pogSpriteTexture = Content.Load<Texture2D>("Images/ball_white"); // Till random sprites i GameOver-Screen
            pogSpriteList = new List<PogSprite>();

            int newWindowWidth = blockColumnCount * blockTexture.Width + ExtraWidthPixels;
            GameWindowWidthPixels = newWindowWidth;

            _graphics.PreferredBackBufferWidth = GameWindowWidthPixels;
            _graphics.PreferredBackBufferHeight = GameWindowHeightPixels;
            _graphics.ApplyChanges();

            backgroundBounds = new Rectangle(0, 0, GameWindowWidthPixels, GameWindowHeightPixels);
            pointTextPosition = new Vector2(GameWindowWidthPixels - 150, GameWindowHeightPixels / 2 - 100);
            timerTextPosition = new Vector2(GameWindowWidthPixels - 150, GameWindowHeightPixels / 2 + 100);

            // StartScreen - BEGIN //
            Vector2 playButtonPos = new Vector2(GameWindowWidthPixels / 2 - buttonTexture.Width / 2, GameWindowHeightPixels / 2);
            playButton = new Button(buttonTexture, playButtonPos, Color.Lime);
            // StartScreen - END //

            // GameOverScreen - BEGIN //
            Vector2 playAgainButtonPos = new Vector2(GameWindowWidthPixels / 2 - buttonTexture.Width / 2, GameWindowHeightPixels / 2);
            playAgainButton = new Button(buttonTexture, playAgainButtonPos, Color.Lime);
            Vector2 exitButtonPos = new Vector2(GameWindowWidthPixels / 2 - buttonTexture.Width / 2, GameWindowHeightPixels / 2 + buttonTexture.Height + 10);
            exitButton = new Button(buttonTexture, exitButtonPos, Color.Red);
            // GameOverScreen - END //

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

            // Fill the actual block-array
            for (int rowIndex = 0;rowIndex < blockRowCount;rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                {
                    Vector2 curBlockPos = new Vector2(columnIndex * blockTexture.Width + 10, rowIndex * blockTexture.Height + 10);
                    Color color = blockColorOrder[rowIndex+1];

                    Block block = new Block(blockTexture, curBlockPos, color);
                    block2DArray[rowIndex, columnIndex] = block;
                }
            }

            closeBlockList = new List<Block>();
            for (int i = 0; i < 8; i++)
            {
                Vector2 curCloseBlockPosition = new Vector2(i * blockTexture.Width + 10 + 4 * blockTexture.Width, blockTexture.Height * 10 + 10);
                Block curCloseBlock = new Block(blockTexture, curCloseBlockPosition, Color.Gold, 5);
                closeBlockList.Add(curCloseBlock);
            }

            randomBlockList = new List<Block>();

            // FloatingPoints - BEGIN //
            //floatingPointTexture = Content.Load<Texture2D>("Images/floating_point");
            floatingPointTexture = Content.Load<Texture2D>("Images/ball_white");
            floatingPointsList = new List<FloatingPoints>();
            // FloatingPoints - END //

            Vector2 paddle1Pos = new Vector2(GameWindowWidthPixels / 2 - paddleTexture.Width / 2 - paddleTexture.Width - ExtraWidthPixels, GameWindowHeightPixels - paddleTexture.Height / 2 - 5);
            Vector2 paddle2Pos = new Vector2(GameWindowWidthPixels / 2 - paddleTexture.Width / 2 + paddleTexture.Width - ExtraWidthPixels, GameWindowHeightPixels - paddleTexture.Height / 2 - 5);
            paddle1 = new Paddle(paddleTexture, paddle1Pos, Keys.Left, Keys.Right, Color.White);
            paddle2 = new Paddle(paddleTexture, paddle2Pos, Keys.S, Keys.D, Color.DarkBlue);
            
            //Vector2 ballPos = new Vector2(GameWindowWidthPixels / 2 - ballTexture.Width / 2 - ExtraWidthPixels, GameWindowHeightPixels / 2 + 80);
            Vector2 ballPos = new Vector2(paddle1.Position.X, GameWindowHeightPixels / 2 + 80);
            ball1 = new Ball(ballTexture, ballPos, GameWindowWidthPixels - ExtraWidthPixels);
            ballList = new List<Ball>();
            ballList.Add(ball1);

            Vector2 powerUpPos = new Vector2((GameWindowWidthPixels - ExtraWidthPixels) / 2, GameWindowHeightPixels / 2 + 50);
            powerUp1 = new PowerUp(powerUpTexture, powerUpPos, Color.Orange);
        }

        private void ResetLevel()
        {
            // Resets all values back to their original value depending on the current level
            switch (_currentLevel)
            {
                case Levels.Level1:
                    foreach (Ball ball in ballList)
                    {
                        ball.ResetBall();
                    }
                    //ball1.ResetBall();
                    paddle1.ResetPaddle();
                    paddle2.ResetPaddle();

                    for (int rowIndex = 0; rowIndex < blockRowCount; rowIndex++)
                    {
                        for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                        {
                            Vector2 curBlockPos = new Vector2(columnIndex * blockTexture.Width + 10, rowIndex * blockTexture.Height + 10);
                            Color color = blockColorOrder[rowIndex + 1];

                            Block block = new Block(blockTexture, curBlockPos, color);
                            block2DArray[rowIndex, columnIndex] = block;
                        }
                    }

                    playerPoints = 0;
                    playerLives = 3;
                    playerWon = true;
                    levelCurrentTime = 0f;
                    levelTimeCounter = 1;
                    firstGameOverUpdate = true;
                    pogSpriteList.Clear();
                    floatingPointsList.Clear();
                    randomBlockList.Clear();
                    isCloseTime = false;
                    powerUp1.ResetPowerUp();
                    DeactivatePower(powerUp1.PowerUpIndex);
                    break;

                case Levels.Level2:
                    foreach (Ball ball in ballList)
                    {
                        ball.ResetBall();
                    }
                    //ball1.ResetBall();
                    paddle1.ResetPaddle();
                    paddle2.ResetPaddle();

                    for (int rowIndex = 0; rowIndex < blockRowCount; rowIndex++)
                    {
                        for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                        {
                            Vector2 curBlockPos = new Vector2(columnIndex * blockTexture.Width + 10, rowIndex * blockTexture.Height + 10);
                            Color color = blockColorOrder[rowIndex + 1 + 8];

                            Block block = new Block(blockTexture, curBlockPos, color);
                            block2DArray[rowIndex, columnIndex] = block;
                        }
                    }

                    //playerPoints = 0;
                    playerLives = 3;
                    playerWon = true;
                    levelCurrentTime = 0f;
                    levelTimeCounter = 1;
                    firstGameOverUpdate = true;
                    pogSpriteList.Clear();
                    floatingPointsList.Clear();
                    randomBlockList.Clear();
                    isCloseTime = false;
                    powerUp1.ResetPowerUp();
                    DeactivatePower(powerUp1.PowerUpIndex);
                    break;

                default:
                    break;
            }
        }
        private void SpawnNewBall()
        {
            Ball newBall = new Ball(ball1.Texture, new(ball1.Position.X + ball1.Texture.Width, ball1.Position.Y), GameWindowWidthPixels - ExtraWidthPixels);
            ballList.Add(newBall);
            ballNr++;
        }

        private void SpawnRandomBlock()
        {
            Color blockColor = Color.DarkGoldenrod;
            int blockHealth = 1;
            int blockFallSpeed = 3;
            bool willFall = true;
            int randomWillFallNr = _random.Next(4);
            if (randomWillFallNr >= 2)
            {
                willFall = true;
            }
            else if (randomWillFallNr < 2)
            {
                blockHealth = 3;
                blockFallSpeed = 1;
                blockColor = Color.Magenta;
            }
            int xPosition = _random.Next(GameWindowWidthPixels - ExtraWidthPixels - blockTexture.Width);
            int yPosition = blockRowCount*blockTexture.Height + _random.Next(2*blockTexture.Height);
            Vector2 newBlockPosition = new Vector2(xPosition, yPosition);
            Block newBlock = new Block(blockTexture, newBlockPosition, blockColor, blockHealth, 3, willFall, blockFallSpeed);
            randomBlockList.Add(newBlock);
        }

        private void ActivatePower(int powerUpIndex)
        {
            // PowerUp1 - 2x PaddleWidth
            paddle1.CollisionRectangle = new Rectangle((int)paddle1.Position.X, (int)paddle1.Position.Y, paddleTexture.Width * 2, paddleTexture.Height);
        }
        private void DeactivatePower(int powerUpIndex)
        {
            // PowerUp1 - 2x PaddleWidth
            paddle1.CollisionRectangle = new Rectangle((int)paddle1.Position.X, (int)paddle1.Position.Y, paddleTexture.Width, paddleTexture.Height);
        }

        protected void UpdateStartScreen(GameTime gameTime)
        {
            // Update start menu
            _coin.Update(gameTime); // UPDATE TO NEW ANIMATION LATER

            // Check if the player is pressing the start-game button
            mouseState = Mouse.GetState();
            if (IsHovering(mouseState, playButton.Bounds))
            {
                playButton.Color = Color.DarkGreen;
            }
            else
            {
                playButton.Color = Color.Lime;
            }
            if (IsHovering(mouseState, playButton.Bounds) && mouseState.LeftButton == ButtonState.Pressed)
            {
                // Start Playing
                _gameState = GameStates.Playing;
            }
        }

        protected void UpdateGameOverScreen(GameTime gameTime)
        {
            // Update game over screen
            if (firstGameOverUpdate)
            {
                // Spawns 5 PogSprites on random locations
                for (int i = 0; i < 5; i++)
                {
                    int randomX = _random.Next(GameWindowWidthPixels - pogSpriteTexture.Width);
                    int randomY = _random.Next(GameWindowHeightPixels - pogSpriteTexture.Height);
                    Vector2 randomSpritePos = new Vector2(randomX, randomY);
                    PogSprite newPogSprite = new PogSprite(pogSpriteTexture, randomSpritePos, Color.White);
                    pogSpriteList.Add(newPogSprite);
                }
                _currentLevel = Levels.Level1;
                firstGameOverUpdate=false;
            }

            // Update the game title
            if (playerWon)
            {
                Window.Title = "Breakout RIP - " + playerPoints + " YOU WIN! - Lives " + playerLives;
            }
            else
            {
                Window.Title = "Breakout RIP - " + playerPoints + " YOU LOSE!";
            }

            // Check if the player is pressing the replay and or the exit game buttons
            mouseState = Mouse.GetState();
            if (IsHovering(mouseState, playAgainButton.Bounds) && mouseState.LeftButton == ButtonState.Pressed)
            {
                // Start Playing Again
                ResetLevel();
                _gameState = GameStates.Playing;
            }
            else if (IsHovering(mouseState, exitButton.Bounds) && mouseState.LeftButton == ButtonState.Pressed)
            {
                // Exit Game
                Exit();
            }

        }

        protected void UpdatePlayingScreen(GameTime gameTime)
        {
            // Update Playing-Screen

            keyboardState = Keyboard.GetState();
            if (oldkeyboardState.IsKeyDown(Keys.Space) && keyboardState.IsKeyUp(Keys.Space))
            {
                isPaused = true;
            }
            oldkeyboardState = keyboardState;

            // Counter - BEGIN //
            levelCurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (levelCurrentTime >= levelCountDuration)
            {
                levelTimeCounter++;
                levelCurrentTime -= levelCountDuration;
            }
            if (levelTimeCounter >= levelTimeLimit)
            {
                levelTimeCounter = 0;
                if (_currentLevel == Levels.Level1)
                {
                    _currentLevel = Levels.Level2;
                    ResetLevel();
                }
                else if (_currentLevel == Levels.Level2)
                {
                    _gameState = GameStates.GameOver;
                }
            }
            else if (levelTimeCounter >= levelTimeLimit - 3) // When the game is about to end
            {
                // Spawn closeTime blocks
                isCloseTime = true;
            }
            if (levelTimeCounter >= levelTimeLimit - 12)
            {
                // Spawns powerup after 8 seconds
                powerUp1.IsActive = true;
            }

            spawnRandomBlockCurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawnRandomBlockCurrentTime >= spawnRandomBlockCountDuration)
            {
                spawnRandomBlockTimeCounter++;
                spawnRandomBlockCurrentTime -= spawnRandomBlockCountDuration;
            }
            if (spawnRandomBlockTimeCounter >= spawnRandomBlockTimeLimit)
            {
                spawnRandomBlockTimeCounter = 0;
                SpawnRandomBlock();
            }
            // Counter - END //

            //Window.Title = "Breakout RIP - " + PlayerPoints + " Points - Tid - " + levelTimeCounter + " Seconds";
            Window.Title = "Breakout RIP - Level - " + _currentLevel.ToString() + " : Lives - " + playerLives;

            // Check if ball is below screen [RemoveLife / GameOver]
            if (ball1.Position.Y > GameWindowHeightPixels)
            {
                playerLives--;
                if (playerLives < 1)
                {
                    playerWon = false;
                    _gameState = GameStates.GameOver;
                }
                else
                {
                    ball1.ResetBall();
                }
            }

            // Add ball for every 10 points
            if (playerPoints >= 10 && ballNr <= playerPoints/10)
            {
                SpawnNewBall();
            }

            // Check if player1 is moving paddle using the Mouse
            mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseState.X > oldMouseState.X)
                {
                    paddle1.MoveRight();
                    paddle2.MoveRight();
                }
                else if (mouseState.X < oldMouseState.X)
                {
                    paddle1.MoveLeft();
                    paddle2.MoveLeft();
                }
            }
            oldMouseState = mouseState;

            // Check Collisions - BEGIN //
            foreach (Ball ball in ballList)
            {
                // Check if the ball is colliding with any paddle
                if (ball.CollisionRectangle.Intersects(paddle1.CollisionRectangle))
                {
                    // Ger spelaren 2x poäng när bollen tar mer än ett block i rad utan att träffa paddeln
                    if (brokenBlocksSincePaddleHit > 1)
                    {
                        playerPoints += brokenBlocksSincePaddleHit * 2;
                    }
                    ball.TestChangeDirection(paddle1, true);
                }
                else if (ball.CollisionRectangle.Intersects(paddle2.CollisionRectangle))
                {
                    // Ger spelaren 2x poäng när bollen tar mer än ett block i rad utan att träffa paddeln
                    if (brokenBlocksSincePaddleHit > 1)
                    {
                        playerPoints += brokenBlocksSincePaddleHit * 2;
                    }
                    ball.TestChangeDirection(paddle2, true);
                }
            }

            if (powerUp1.IsActive)
            {
                powerUp1.Update(gameTime);
                if (powerUp1.CollisionRectangle.Intersects(paddle1.CollisionRectangle))
                {
                    ActivatePower(powerUp1.PowerUpIndex);
                    powerUp1.IsHit = true;
                }
                else if (powerUp1.CollisionRectangle.Intersects(paddle2.CollisionRectangle))
                {
                    ActivatePower(powerUp1.PowerUpIndex);
                    powerUp1.IsHit = true;
                }

                if (powerUp1.IsActive)
                {
                    if (powerUp1.IsTimerUp(gameTime))
                    {
                        DeactivatePower(powerUp1.PowerUpIndex);
                    }
                }
            }

            // Checks all blocks that aren't broken to see if they have collided with the ball
            for (int rowIndex = 0; rowIndex < blockRowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < blockColumnCount; columnIndex++)
                {
                    if (!block2DArray[rowIndex, columnIndex].IsBroken)
                    {
                        foreach (Ball ball in ballList)
                        {
                            if (ball.CollisionRectangle.Intersects(block2DArray[rowIndex, columnIndex].CollisionRectangle))
                            {
                                ball.TestChangeDirection(paddle1);
                                playerPoints += block2DArray[rowIndex, columnIndex].PointAmount;

                                block2DArray[rowIndex, columnIndex].Health--;
                                if (block2DArray[rowIndex, columnIndex].Health <= 0)
                                {
                                    block2DArray[rowIndex, columnIndex].IsBroken = true;
                                    Vector2 newFloatingPointPosition = new Vector2(block2DArray[rowIndex, columnIndex].Position.X + block2DArray[rowIndex, columnIndex].Texture.Width / 2, block2DArray[rowIndex, columnIndex].Position.Y);
                                    FloatingPoints newFloatingPoint = new FloatingPoints(floatingPointTexture, newFloatingPointPosition,
                                        Color.White, block2DArray[rowIndex, columnIndex].PointAmount);
                                    floatingPointsList.Add(newFloatingPoint);
                                }

                                rowIndex = blockRowCount + 1;
                                columnIndex = blockColumnCount + 1;
                                break;
                            }
                        }
                    }
                }
            }

            if (isCloseTime)
            {
                foreach (Block curCloseBlock in closeBlockList)
                {
                    if (!curCloseBlock.IsBroken)
                    {
                        foreach (Ball ball in ballList)
                        {
                            if (ball.CollisionRectangle.Intersects(curCloseBlock.CollisionRectangle))
                            {
                                curCloseBlock.Health--;
                                if (curCloseBlock.Health <= 0)
                                {
                                    curCloseBlock.IsBroken = true;
                                    playerPoints += curCloseBlock.PointAmount;
                                    ball.TestChangeDirection(paddle1);
                                }
                            }
                        }
                    }
                }
            }

            if (randomBlockList.Count > 0)
            {
                foreach (Block randBlock in randomBlockList)
                {
                    if (!randBlock.IsBroken)
                    {
                        foreach (Ball ball in ballList)
                        {
                            if (ball.CollisionRectangle.Intersects(randBlock.CollisionRectangle))
                            {
                                randBlock.Health--;
                                if (randBlock.Health <= 0)
                                {
                                    randBlock.IsBroken = true;
                                    playerPoints += randBlock.PointAmount;
                                    ball.TestChangeDirection(paddle1);
                                }
                                else
                                {
                                    randBlock.Position.Y = randBlock.Position.Y - 10;
                                    randBlock.UpdateRectangle(new(randBlock.Position.X, randBlock.Position.Y));
                                    playerPoints += randBlock.PointAmount;

                                    ball.TestChangeDirection(paddle1);
                                }
                            }
                        }
                    }
                }
            }
            // Check Collisions - END //

            // Sort through and update the block-array to filter out broken blocks
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

            // Sort through and update the closeblocklist
            if (isCloseTime)
            {
                List<Block> newCloseBlockList = new List<Block>();
                foreach (Block curCloseBlock in closeBlockList)
                {
                    if (!curCloseBlock.IsBroken)
                    {
                        newCloseBlockList.Add(curCloseBlock);
                    }
                }
                closeBlockList = newCloseBlockList;
            }

            // Sort through and update the randomblocklist
            if (randomBlockList.Count > 0)
            {
                List<Block> newRandomBlockList = new List<Block>();
                foreach(Block curRandomBlock in randomBlockList)
                {
                    curRandomBlock.Update();
                    if (!curRandomBlock.IsBroken)
                    {
                        newRandomBlockList.Add(curRandomBlock);
                    }
                }
                randomBlockList = newRandomBlockList;
            }

            // Update all FloatingPoints if there are any
            if (floatingPointsList.Count > 0)
            {
                foreach (FloatingPoints floatingPoint in floatingPointsList)
                {
                    floatingPoint.Update();
                    if (!floatingPoint.IsBroken)
                    {
                        if (floatingPoint.CollisionRectangle.Intersects(paddle1.CollisionRectangle) || floatingPoint.CollisionRectangle.Intersects(paddle2.CollisionRectangle))
                        {
                            playerPoints += floatingPoint.PointAmount;
                            floatingPoint.IsBroken = true;
                        }
                    }
                }
            }

            // Update ball and paddle - BEGIN //
            keyboardState = Keyboard.GetState();
            paddle1.Update(keyboardState, oldkeyboardState);
            paddle2.Update(keyboardState, oldkeyboardState);
            foreach (Ball ball in ballList)
            {
                ball.Update();
            }
            //ball1.Update();
            // Update ball and paddle - END //
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            

            switch (_gameState)
            {
                case GameStates.Start:
                    UpdateStartScreen(gameTime);
                    break;
                case GameStates.Playing:
                    if (!isPaused)
                    {
                        UpdatePlayingScreen(gameTime);
                    }
                    else
                    {
                        keyboardState = Keyboard.GetState();
                        if (oldkeyboardState.IsKeyDown(Keys.Space) && keyboardState.IsKeyUp(Keys.Space))
                        {
                            isPaused = false;
                        }
                        oldkeyboardState = keyboardState;
                    }
                    break;
                case GameStates.GameOver:
                    UpdateGameOverScreen(gameTime);
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            switch (_gameState)
            {
                case GameStates.Start:
                    _spriteBatch.Draw(startScreenBG, backgroundBounds, Color.White);
                    _spriteBatch.Draw(playButton.Texture, playButton.Bounds, playButton.Color);

                    _coin.Draw(_spriteBatch); // UPDATE TO NEW ANIMATION LATER
                    break;

                case GameStates.Playing:
                    _spriteBatch.Draw(playingScreenBG, backgroundBounds, Color.White);
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

                    if (powerUp1.IsActive)
                    {
                        if (!powerUp1.IsBroken)
                        {
                            _spriteBatch.Draw(powerUp1.Texture, powerUp1.CollisionRectangle, powerUp1.Color);
                        }
                    }

                    if (isCloseTime)
                    {
                        foreach (Block curCBlock in closeBlockList)
                        {
                            if (!curCBlock.IsBroken)
                            {
                                _spriteBatch.Draw(curCBlock.Texture, curCBlock.CollisionRectangle, curCBlock.Color);
                            }
                        }
                    }

                    if (randomBlockList.Count > 0)
                    {
                        foreach (Block curRandomBlock in randomBlockList)
                        {
                            if (!curRandomBlock.IsBroken)
                            {
                                _spriteBatch.Draw(curRandomBlock.Texture, curRandomBlock.CollisionRectangle, curRandomBlock.Color);
                            }
                        }
                    }

                    foreach (FloatingPoints floatingPoint in floatingPointsList)
                    {
                        if (!floatingPoint.IsBroken)
                        {
                            _spriteBatch.Draw(floatingPoint.Texture, floatingPoint.CollisionRectangle, floatingPoint.Color);
                        }
                    }

                    foreach (Ball ball in ballList)
                    {
                        _spriteBatch.Draw(ball.Texture, ball.CollisionRectangle, ball.Color);
                    }
                    //_spriteBatch.Draw(ball1.Texture, ball1.CollisionRectangle, ball1.Color);
                    _spriteBatch.Draw(paddle1.Texture, paddle1.CollisionRectangle, paddle1.Color);
                    _spriteBatch.Draw(paddle2.Texture, paddle2.CollisionRectangle, paddle2.Color);

                    // Draw Points and Time - BEGIN //
                    Vector2 textMidPoint = gameInfoFont.MeasureString(playerPoints.ToString()) / 2;
                    _spriteBatch.DrawString(gameInfoFont, playerPoints.ToString(), pointTextPosition, Color.White, 0, textMidPoint, 10.0f, SpriteEffects.None, 0.5f);

                    textMidPoint = gameInfoFont.MeasureString(levelTimeCounter.ToString()) / 2;
                    _spriteBatch.DrawString(gameInfoFont, levelTimeCounter.ToString(), timerTextPosition, Color.White, 0, textMidPoint, 10.0f, SpriteEffects.None, 0.5f);
                    // Draw Points and Time - END //
                    break;

                case GameStates.GameOver:
                    _spriteBatch.Draw(gameOverScreenBG, backgroundBounds, Color.White);
                    _spriteBatch.Draw(playAgainButton.Texture, playAgainButton.Bounds, playAgainButton.Color);
                    _spriteBatch.Draw(exitButton.Texture, exitButton.Bounds, exitButton.Color);

                    foreach (PogSprite currentPogSprite in pogSpriteList)
                    {
                        _spriteBatch.Draw(currentPogSprite.Texture, currentPogSprite.Bounds, currentPogSprite.Color);
                    }
                    break;

                default:
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}