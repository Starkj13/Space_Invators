﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
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
        Texture2D P1Pic, BulletPic, EnemyPic, HouseMainPic, HouseDamagePic, EnemyBulletPic, Background, ButtonPic;
        Song Music;
        SoundEffect Shoot;
        SoundEffect EnemyKilled;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height + 150;

        // Ints
        int MovmentSpeed = 8;
        int xChange = 80;
        double yChange = Width / 10;
        int EnemyFire;
        int HouseHit = 3;
        int EnemyBulletTimer = 300;
        int Score;
        int HighScore = 0;

        // Bools 
        bool EnemyBulletVisible = false;
        bool BulletVisible = false;
        bool EnemyBulletSpawn = true;
        bool Hit = false;
        bool LastScen = false;
        bool GameOverScene = false;
        bool WinningScene = false;
        bool EnemyPoint = true;
        bool Housepoint = true;

        // Lists
        List<int> HouseHealth = new List<int>();
        List<Rectangle> EnemyRectList = new List<Rectangle>();
        List<Rectangle> HouseChangeRectList = new List<Rectangle>();
        List<Texture2D> HouseChangePicList = new List<Texture2D>();

        //Rectangle
        Rectangle RectP1 = new Rectangle(Width / 2, Height / 2 + 500, 120, 120);
        Rectangle BackgroundRect = new Rectangle(0,0, Width, Height);
        Rectangle ButtonRect = new Rectangle(Width / 2 - 100, Height / 2, 200, 100);
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
        Vector2 SceneText = new Vector2(Width / 2 - 50, Height / 2 - 600);

        enum GameState
        {
            MainMenu,
            GamePlay,
            LastScene,
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
            _state = GameState.MainMenu;

            // Load Sound 
            Music = Content.Load<Song>("spaceinvadersmusic");
            EnemyKilled = Content.Load<SoundEffect>("Ememykilled");
            Shoot = Content.Load<SoundEffect>("shoot");
            // Play Music
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Music);

            P1Pic = Content.Load<Texture2D>("SpaceShip");
            BulletPic = Content.Load<Texture2D>("P1Bullet");
            EnemyPic = Content.Load<Texture2D>("LargeAlien");
            EnemyBulletPic = Content.Load<Texture2D>("EnemyBullet");
            HouseMainPic = Content.Load<Texture2D>("SpaceInvaders_House");
            HouseDamagePic = Content.Load<Texture2D>("SpaceInvaders_House_Damage");
            arialFont = Content.Load<SpriteFont>("arial");
            Background = Content.Load<Texture2D>("SpaceInvaders_Background");
            ButtonPic = Content.Load<Texture2D>("Start_Button");

            // Add Image to House List
            for (int i = 0; i < 3; i++)
            {
                HouseChangePicList.Add(Content.Load<Texture2D>("SpaceInvaders_House"));
            }

            // Add House Rects to list
            // House 1
            HouseChangeRectList.Add(new Rectangle(Width / 4, Height / 2 + 200, 200, 200));
           
            // House 2
            HouseChangeRectList.Add(new Rectangle(Width / 3 + 180, Height / 2 + 200, 200, 200));
            
            // House 3
            HouseChangeRectList.Add(new Rectangle(Width / 2 + 220, Height / 2 + 200, 200 ,200));
            

            // Set position of enenmy with start
            for (int i = 0; i < 11; i++)
            {
                EnemyRectList.Add(new Rectangle(xChange, (int)yChange, 80, 80));
                xChange += 170;
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

            switch (_state)
            {
                case GameState.MainMenu:
                    MainMenu(gameTime);
                    break;
                case GameState.GamePlay:
                    GamePlayScene(gameTime);
                    break;
                case GameState.LastScene:
                    LastScene();
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }

        void MainMenu(GameTime gameTime)
        {
            mouse = Mouse.GetState();
            IsMouseVisible = true;
            
            if (ButtonRect.Contains(mouse.Position) == true && mouse.LeftButton == ButtonState.Pressed)
            {  
                _state = GameState.GamePlay;        
            }

            _spriteBatch.Begin();
            // Button Draw
            _spriteBatch.Draw(Background, BackgroundRect, Color.White);
            _spriteBatch.DrawString(arialFont, $"Main Menu", ScoreTextPosition, Color.White);
            _spriteBatch.Draw(ButtonPic, ButtonRect, Color.White);
            _spriteBatch.End();


            base.Draw(gameTime);
        }

        void GamePlayScene(GameTime gameTime)
        {
            KeyboardState KeyboardState = Keyboard.GetState();
            mouse = Mouse.GetState();
            IsMouseVisible = false;
            _spriteBatch.Begin();

            // Player Draw
            _spriteBatch.Draw(Background, BackgroundRect, Color.White);
            _spriteBatch.Draw(P1Pic, RectP1, Color.White);
            _spriteBatch.Draw(HouseChangePicList[0], HouseChangeRectList[0], Color.White);
            _spriteBatch.Draw(HouseChangePicList[1], HouseChangeRectList[1], Color.White);
            _spriteBatch.Draw(HouseChangePicList[2], HouseChangeRectList[2], Color.White);

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
    

            if (LastScen == true)
            {
                _state = GameState.LastScene;
                LastScene();
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

        void LastScene()
        {
            GraphicsDevice.Clear(Color.Red);
            if (GameOverScene)
            {
                _spriteBatch.Begin();

                _spriteBatch.Draw(Background, BackgroundRect, Color.White);
                _spriteBatch.DrawString(arialFont, $"Game Over", SceneText, Color.White);
                _spriteBatch.DrawString(arialFont, $"Score", ScoreTextPosition, Color.White);
                _spriteBatch.DrawString(arialFont, $"{Score}", ScorePosition, Color.White);
                _spriteBatch.DrawString(arialFont, $"Highscore", HighScoreTextPosition, Color.White);
                _spriteBatch.DrawString(arialFont, $"{HighScore}", HighScorePosition, Color.White);

                _spriteBatch.End();
            }
            if (WinningScene)
            {
                _spriteBatch.Begin();

                _spriteBatch.Draw(Background, BackgroundRect, Color.White);
                _spriteBatch.DrawString(arialFont, $"Winner", SceneText, Color.White);
                _spriteBatch.DrawString(arialFont, $"Score", ScoreTextPosition, Color.White);
                _spriteBatch.DrawString(arialFont, $"{Score}", ScorePosition, Color.White);
                _spriteBatch.DrawString(arialFont, $"Highscore", HighScoreTextPosition, Color.White);
                _spriteBatch.DrawString(arialFont, $"{HighScore}", HighScorePosition, Color.White);

                _spriteBatch.End();
            }

            HousePoint();
            FileManager();
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
                BulletPosition.X = RectP1.X + 50;
                BulletPosition.Y = RectP1.Y;
                Shoot.Play();
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
                    EnemyKilled.Play();
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

                if (EnemyRectList.Count == 0)
                {
                    LastScen = true;
                    WinningScene = true;
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
                LastScen = true;
                GameOverScene = true;
                EnemyBulletVisible = false;
            }

            for (int i = 0; i < 3; i++)
            {
                // EnemyBullet hit Houses check
                if (HouseChangeRectList[i].Intersects(EnemyBulletRect) == true && HouseHealth[i] != 1)
                {    
                    HouseChangePicList[i] = HouseDamagePic;
                    HouseHealth[i]++;
                }
            }   
        }

        void HousePoint()
        {

            // Check Game Over 
            if ((HouseHealth[0] + HouseHealth[1] + HouseHealth[2]) == 3)
            {
                LastScen = true;
                GameOverScene = true;
            }
           
            // Check House life and give point
            if (HouseHealth[0] + HouseHealth[1] + HouseHealth[2] == 0 && LastScen == true && Housepoint == true)
            { 
                Score += 100;
                Housepoint = false;
            }

            // Point Set for every house survived
            for (int i = 0; HouseHit != 0 && HouseHit < 0; i++)
            {
                if (HouseHealth[i] == 0 && LastScen == true)
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
                    if (EnemyRectList[i].Intersects(RectP1) == true || temp.Y > Height / 2 + 200)
                    {
                        LastScen = true;
                        GameOverScene = true;
                    }
                }
            }
        }

        void FileManager()
        {

            // Read text file 
            string HighScoreContent = File.ReadAllText("HighScore.txt");
            HighScore = int.Parse(HighScoreContent);

            // Sets a new Highscore if Score is higher
            if (Score > HighScore)
            {
                HighScore = Score;

                StreamWriter writer = new StreamWriter(@"C:../../../HighScore.txt");
                writer.WriteLine(HighScore);
                writer.Dispose();

            }

        }
    }
}