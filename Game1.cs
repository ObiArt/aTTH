using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace aTTH
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D player_sprite;

        private List<Entity> Entities;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Entities = new List<Entity>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            player_sprite = Content.Load<Texture2D>("player");

            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(0, 0), new Vector2(25, 5)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(0, 100), new Vector2(25, 5)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(40, 110), new Vector2(5, 5)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(45, 90), new Vector2(5, 5)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(50, 85), new Vector2(5, 25)));
            Entities.Add(new Player(new Vector2(75, 50), player_sprite));
            Entities.Add(new Player(new Vector2(25, 50), player_sprite));
            Entities.Add(new Player(new Vector2(5, 50), player_sprite));
            Entities.Add(new Player(new Vector2(50, 50), player_sprite));
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            MouseState mouseState = Mouse.GetState(Window);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Update(deltaTime);
                if (Entities[i].controlable)
                {
                    Entities[i].Control(gamePadState, keyboardState, mouseState);
                }
                if (Entities[i].collide_important)
                {
                    Entities[i].CollissionCheck(Entities);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp);

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
