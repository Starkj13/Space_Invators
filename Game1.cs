using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Space_Invators
{
    public class Game1 : Game
    {
        Random Random = new Random();

        GameState _state;
        MouseState mouse;
        SpriteFont arialFont;
        Texture2D P1Pic, BulletPic, EnemyPic, HouseMainPic, HouseDamagePic, EnemyBulletPic, Background;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height + 150;

        // Ints
        int MovmentSpeed = 8;
        int xChange = 110;
        double yChange = Width / 10;
        int Health = 3;
        int EnemyFire;
        int HouseHit = 3;
        int EnemyBulletTimer = 300;
        int Score;
        int HighScore;

        // Bools 
        bool EnemyBulletVisible = false;
        bool BulletVisible = false;
        bool EnemyBulletSpawn = true;
        bool Hit = false;
        bool GameOver = false;
        bool EnemyPoint = true;
        bool Housepoint = true;

        // Lists
        List<int> HouseHealth = new List<int>();
        List<int> ScoreList = new List<int>();
        List<Rectangle> EnemyRectList = new List<Rectangle>();
        List<Rectangle> HouseChangeRectList = new List<Rectangle>();
        List<Texture2D> HouseChangePicList = new List<Texture2D>();

        //Rectangle
        Rectangle RectP1 = new Rectangle(Width / 2, Height / 2 + 300, 80, 80);
        Rectangle BackgroundRect = new Rectangle(0,0, Width, Height);
        Rectangle EnemyBulletRect;
        Rectangle BulletRect;

        // Vector
        Vector2 BulletSpeed;
        Vector2 BulletPosition;
        Vector2 EnemyBulletSpeed;
        Vector2 EnemyBulletPosition;

        // Scorebord
        Vector2 ScorePosition = new Vector2(Width / 2, Height / 2 - 80);
        Vector2 ScoreTextPosition = new Vector2(Width / 2 -25, Height / 2 - 120);
        Vector2 HighScorePosition = new Vector2(Width / 2, Height / 2 +120);
        Vector2 HighScoreTextPosition = new Vector2(Width / 2 -50, Height / 2 +80);

        enum GameState
        {
            MainMenu,
            GamePlay,
            GameOver,
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;
            _graphics.ApplyChanges();
           
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            P1Pic = Content.Load<Texture2D>("parrot");
            BulletPic = Content.Load<Texture2D>("ball");
            EnemyPic = Content.Load<Texture2D>("LargeAlien");
            EnemyBulletPic = Content.Load<Texture2D>("SpaceInvaders_EnemyBullet");
            HouseMainPic = Content.Load<Texture2D>("SpaceInvaders_House");
            HouseDamagePic = Content.Load<Texture2D>("SpaceInvaders_House_Damage");
            arialFont = Content.Load<SpriteFont>("arial");
            Background = Content.Load<Texture2D>("SpaceInvaders_Background");

            // Add Image to House List
            for (int i = 0; i < 3; i++)
            {
                HouseChangePicList.Add(Content.Load<Texture2D>("SpaceInvaders_House"));
            }

            // Add House Rects to list
            // House 1
            HouseChangeRectList.Add(new Rectangle(Width / 4, Height / 2 + 400, 500, 500));
            HouseChangeRectList.Add(new Rectangle(Width / 4, Height / 2 + 300, 500, 500));

            // House 2
            HouseChangeRectList.Add(new Rectangle(Width / 2 + 100, Height / 2 + 400, 500, 500));
            HouseChangeRectList.Add(new Rectangle(Width / 2 + 100, Height / 2 + 300, 500, 500));

            // House 3
            HouseChangeRectList.Add(new Rectangle(Width / 2, Height / 2 + 400, 500, 500));
            HouseChangeRectList.Add(new Rectangle(Width / 2, Height / 2 + 300, 500, 500));

            // Set position of enenmy with start
            for (int i = 0; i < 10; i++)
            {
                EnemyRectList.Add(new Rectangle(xChange, (int)yChange, 80, 80));
                xChange += 180;
            }

            for (int i = 0; i < 3; i++)
            {
                HouseHealth.Add(0);
            }
            FileManager();

        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            _state = GameState.GamePlay;

            switch (_state)
            {
                case GameState.MainMenu:
                    break;
                case GameState.GamePlay:
                    GamePlayScene(gameTime);
                    break;
                case GameState.GameOver:
                    GameOverScene();
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }

        void GamePlayScene(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            KeyboardState KeyboardState = Keyboard.GetState();
            mouse = Mouse.GetState();

            _spriteBatch.Begin();

            // Player Draw
            _spriteBatch.Draw(Background, BackgroundRect, Color.White);
            _spriteBatch.Draw(P1Pic, RectP1, Color.White);
            _spriteBatch.Draw(HouseChangePicList[0], HouseChangeRectList[0], Color.White);
            _spriteBatch.Draw(HouseChangePicList[1], HouseChangeRectList[2], Color.White);
            _spriteBatch.Draw(HouseChangePicList[2], HouseChangeRectList[4], Color.White);

            // Draw Bullet
            if (BulletVisible == true)
            {
                _spriteBatch.Draw(BulletPic, BulletPosition, Color.White);
            }

            // Draw Enemy Bullet
            if (EnemyBulletVisible == true)
            {
                _spriteBatch.Draw(EnemyBulletPic, EnemyBulletPosition, Color.White);
            }

            // Draw Enemy
            foreach (Rectangle rect in EnemyRectList)
            {
                _spriteBatch.Draw(EnemyPic, rect, Color.White);
            }
            _spriteBatch.End();
    

            if (GameOver == true)
            {
                GameOverScene();
            }

            BulletPosition += BulletSpeed;
            EnemyBulletPosition += EnemyBulletSpeed;
            EnemyBulletTimer--;

            MovementPlayer();
            EnemyMovment();
            EnemyBulletCollision();
            BulletCollision();
            HousePoint();

            base.Draw(gameTime);
        }

        void GameOverScene()
        {
            GraphicsDevice.Clear(Color.Red);
            _spriteBatch.Begin();

            _spriteBatch.Draw(Background, BackgroundRect, Color.White);
            _spriteBatch.DrawString(arialFont, $"Score", ScoreTextPosition, Color.White);
            _spriteBatch.DrawString(arialFont, $"{Score}", ScorePosition, Color.White);
            _spriteBatch.DrawString(arialFont, $"Highscore", HighScoreTextPosition, Color.White);
            _spriteBatch.DrawString(arialFont, $"{HighScore}", HighScorePosition, Color.White);

            _spriteBatch.End();

            HousePoint();
        }

        void MovementPlayer()
        {
            KeyboardState KeyboardState = Keyboard.GetState();

            // Move Player with Keyboard
            if (KeyboardState.IsKeyDown(Keys.A) && RectP1.X !> 0)
            {
                RectP1.X -= MovmentSpeed;
            }
            if (KeyboardState.IsKeyDown(Keys.D) && RectP1.X !< Width -85)
            {
                RectP1.X += MovmentSpeed;
            }

            // Bullet Fire
            if (KeyboardState.IsKeyDown((Keys.Space)) && Hit == false)
            {
                BulletVisible = true;
                BulletPosition.X = RectP1.X + 25;
                BulletPosition.Y = RectP1.Y;

                BulletSpeed.Y = Random.Next(-10, -5);
                Hit = true;
            }

            // Set bullet hit to false if it out of screen
            if (BulletPosition.Y < -Height / 2)
            {
                Hit = false;
            }

        }

        void BulletCollision()
        {
            BulletRect.Y = (int)BulletPosition.Y;
            BulletRect.X = (int)BulletPosition.X;

            // Enemy hit by bullet
            for (int i = 0; i < EnemyRectList.Count; i++)
            {
                if (EnemyRectList[i].Intersects(BulletRect) == true)
                {
                    EnemyRectList.RemoveAt(i);
                    Hit = false;
                    Score += 50;
                    BulletVisible = false;
                }
            }

            // If bullet pass enemy line make invisibal
            for (int i = 0; i < EnemyRectList.Count; i++)
            {
                Rectangle temp = new Rectangle();
                temp = EnemyRectList[i];

                if (BulletRect.Y < temp.Y)
                {
                    BulletVisible = false;
                    Hit = false;
                }
            }

        }

        void EnemyBulletCollision()
        {
            EnemyBulletRect.Y = (int)EnemyBulletPosition.Y;
            EnemyBulletRect.X = (int)EnemyBulletPosition.X;

            // EnemyBullet Spawner
            if (EnemyBulletTimer == 0 && EnemyBulletSpawn == true)
            {
                EnemyBulletVisible = true;
                
                EnemyFire = Random.Next(0, EnemyRectList.Count);
                EnemyBulletSpeed.Y = Random.Next(5, 10);

                EnemyBulletTimer = 300;

                // Only one time bonus 
                if (EnemyRectList.Count == 0 && EnemyPoint == true)
                {
                    EnemyBulletSpawn = false;
                    EnemyPoint = false;
                    Score += 100;
                }

                if (EnemyBulletSpawn == true)
                {
                    EnemyBulletPosition.X = EnemyRectList[EnemyFire].X;
                    EnemyBulletPosition.Y = EnemyRectList[EnemyFire].Y;
                }

            }

            // EnemyBullet hit player check
            if (EnemyBulletRect.Intersects(RectP1) == true)
            {
                Health--;
                EnemyBulletVisible = false;
            }

            // EnemyBullet hit Houses check
            if (HouseChangeRectList[1].Intersects(EnemyBulletRect) == true && HouseHealth[0] != 1)
            {
                HouseChangePicList[0] = HouseDamagePic;
                HouseChangeRectList[0] = HouseChangeRectList[1];
                HouseHealth[0]++;
            }
            if (HouseChangeRectList[2].Intersects(EnemyBulletRect) == true && HouseHealth[1] != 1)
            {
                HouseChangePicList[1] = HouseDamagePic;
                HouseChangeRectList[2] = HouseChangeRectList[3];
                HouseHealth[1]++;
            }
            if (HouseChangeRectList[3].Intersects(EnemyBulletRect) == true && HouseHealth[2] != 1)
            {
                HouseChangePicList[2] = HouseDamagePic;
                HouseChangeRectList[4] = HouseChangeRectList[5];
                HouseHealth[2]++;
            }

            // Change EnemyBullet Visible
            if (EnemyBulletPosition.Y < 0)
            {
                EnemyBulletVisible = false;
            }
        }

        void HousePoint()
        {

            // Check Game Over 
            if ((HouseHealth[0] + HouseHealth[1] + HouseHealth[2]) == 3)
            {
                GameOver = true;     
            }
           
            // Check House life and give point
            if (HouseHealth[0] + HouseHealth[1] + HouseHealth[2] == 0 && GameOver == true && Housepoint == true)
            { 
                Score += 100;
                Housepoint = false;
            }

            // Point Set for every house survived
            for (int i = 0; HouseHit != 0 && HouseHit < 0; i++)
            {
                if (HouseHealth[i] == 0 && GameOver == true)
                {
                    Score += 100;
                    HouseHealth[i] = 0;
                    --HouseHit;
                }
            }
        }

        void EnemyMovment()
        {

            // Loop for moving the enemys
            for (int i = 0; i < 1; i++)
            {
                yChange += 0.2;
                for (int j = 0; j < EnemyRectList.Count; j++)
                {
                    Rectangle temp = new Rectangle();
                    temp = EnemyRectList[j];
                    temp.Y = (int)yChange;
                    EnemyRectList[j] = temp;

                    // Check if Enemy has past finish line
                    if (EnemyRectList[i].Intersects(RectP1) == true || temp.Y < -100)
                    {
                        GameOver = true;
                    }
                }
            }
        }

        void FileManager()
        {
               
            if (File.Exists("HighScore.txt")==false)
            {
                File.WriteAllText("HighScore.txt", $"{HighScore}");
                Debug.WriteLine("test");
            }
            string HighScoreContent = File.ReadAllText("HighScore.txt");
            HighScore = int.Parse(HighScoreContent);

        }
    }
}