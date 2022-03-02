using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace Space_Invators
{
    public class Game1 : Game
    {
        Random Random = new Random();

        GameState _state; 
        MouseState mouse;
        Texture2D P1Pic, BulletPic, EnemyPic, HouseMainPic, HouseDamagePic, EnemyBulletPic;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height + 150;

        bool Hit = false;


        // ints
        int MovmentSpeed = 8;
        int xChange = 110;
        int yChange = Width / 10;
        int HouseHitPoint;
        int EnemyFire;

        List<Rectangle> rectList = new List<Rectangle>();

        //Rectangle
        Rectangle RectP1 = new Rectangle(Width/2, Height /2 + 300, 80, 80);
        Rectangle HouseRect = new Rectangle(Width / 4, Height/2 + 400, 500, 500);
        Rectangle EnemyBulletRect;
        Rectangle BulletRect;

        // Vector
        Vector2 BulletSpeed;
        Vector2 BulletPosition;
        Vector2 EnemyBulletSpeed;
        Vector2 EnemyBulletPosition;

        enum GameState
        {
            MainMenu,
            GamePlay,
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
            HouseMainPic = Content.Load<Texture2D>("SpaceInvaders_House");
            HouseDamagePic = Content.Load<Texture2D>("SpaceInvaders_House_Damage");
            EnemyBulletPic = Content.Load<Texture2D>("SpaceInvaders_EnemyBullet");


            // Set position of enenmy with start
            for (int i = 0; i < 10; i++)
            {
                rectList.Add(new Rectangle(xChange, yChange, 80, 80));
                xChange += 180;
            }

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
                    GamePlay(gameTime);
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }

        void GamePlay(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            KeyboardState KeyboardState = Keyboard.GetState();
            mouse = Mouse.GetState();

            _spriteBatch.Begin();

            // Player Draw
            _spriteBatch.Draw(P1Pic, RectP1, Color.White);
            _spriteBatch.Draw(BulletPic, BulletPosition, Color.White);
            _spriteBatch.Draw(HouseMainPic, HouseRect, Color.White);
            _spriteBatch.Draw(EnemyBulletPic, EnemyBulletPosition, Color.White);

            // Draw Enemy
            foreach (Rectangle rect in rectList)
            {
                _spriteBatch.Draw(EnemyPic, rect, Color.White);
            }

            _spriteBatch.End();

            BulletPosition += BulletSpeed;
            EnemyBulletPosition += EnemyBulletSpeed;

            MovementPlayer();
            EnemyBulletCollision();
            BulletCollision();

            base.Draw(gameTime);
        }

        void MovementPlayer()
        {
            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.A))
            {
                RectP1.X -= MovmentSpeed;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                RectP1.X += MovmentSpeed;
            }


            if (RectP1.X > Width - 85)
            {
                RectP1.X -= MovmentSpeed;
            }
            if (RectP1.X < 0)
            {
                RectP1.X += MovmentSpeed;
            }


            if (KeyboardState.IsKeyDown((Keys.Space)) && Hit == false)
            {
                BulletPosition.X = RectP1.X+25;
                BulletPosition.Y = RectP1.Y;

                BulletSpeed.Y = Random.Next(-10, -5);
                Hit = true;
            }
           
            if (BulletPosition.Y < -Height/2)
            { 
                Hit = false;
            }

        }

        void BulletCollision()
        {
            BulletRect.Y = (int)BulletPosition.Y;
            BulletRect.X = (int)BulletPosition.X;

            for (int i = 0; i < rectList.Count; i++)
            {
                if (rectList[i].Intersects(BulletRect) == true)
                {
                    rectList.RemoveAt(i);
                }
            }
            
            
            if (HouseRect.Intersects(EnemyBulletRect) == true)
            {
                HouseMainPic = HouseDamagePic;
            }
        }

        void EnemyBulletCollision()
        {

            EnemyBulletRect.Y = (int)EnemyBulletPosition.Y;
            EnemyBulletRect.X = (int)EnemyBulletPosition.X;

            EnemyFire = Random.Next(0, 10);

            EnemyBulletSpeed.Y = Random.Next(-10, -5);

            EnemyBulletPosition.X = rectList[EnemyFire].X;
            EnemyBulletPosition.Y = rectList[EnemyFire].Y;
            //make it reset
         
        }

        void EnemyMovment()
        {


        }
    }   
}
