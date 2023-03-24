//How to play: Left and right key to move around. Press enter to restart.
//
//Problems:
//  Some refactoring is needed to not have a bunch of "global" variables
//  If the framerate is low collisiondetection can potentially fail at high movement speeds, like in stages 1 and 2.
//Aside from that the game itself is actually enjoyable.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperSquare
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D circle;
        private Player player;
        private Vector2 center;
        private Color gameColor;
        private float centerSquareHalfWidth;
        private List<Wall> walls;
        private readonly Color[] GAME_COLORS = { new Color(150, 10, 0) , new Color(10, 10, 180), new Color(0,150, 15), new Color(150, 150, 15), new Color(0, 150, 150), new Color(120, 0,210) };
        private Random random = new Random();
        private const float wallSpawnDistance = 400;
        private float score;
        private float highScore;
        private SpriteFont font;
        private int stage;
        private float gameRotation;
        private float gameRPM;
        private float wallSpeed;
        private float wallsPerSecond;
        private float wallSpawnTime;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            center = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight/2);
            centerSquareHalfWidth = 25;
            player = new Player(5,new Vector2(1,0), centerSquareHalfWidth +20, 100);
            highScore = 0;

            InitGame();
        }

        private void InitGame()
        {
            player.RPM = 100;
            gameColor = GAME_COLORS[random.Next(GAME_COLORS.Length)];
            walls = new List<Wall> { };
            wallSpeed = 200f;
            player.IsDead = false;
            score = 0;
            stage = 1;
            gameRotation = 0;
            gameRPM = 0;
            wallsPerSecond = 4;

        }
        

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            circle = Content.Load<Texture2D>("circle");
            font = Content.Load<SpriteFont>("Score");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            const int stage1Time = 10;
            const int stage2Time = 20; //Time at which stage changes
            if (stage == 1 && score >= stage1Time) { gameRPM = 15; stage = 2; player.RPM *= 1.25f; wallSpeed *= 1.25f; wallsPerSecond = 6; walls.Clear(); gameColor = Color.Lerp(gameColor, Color.White, 0.2f); }
            if (stage == 2 && score >= stage2Time) { gameRPM = -30;  stage = 3; player.RPM *= 1.25f; wallSpeed *= 1.25f;wallsPerSecond = 8; walls.Clear(); gameColor = Color.Lerp(gameColor, Color.White, 0.2f); }
            if (player.IsDead)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter)) { InitGame(); }
                return;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) { player.Rotate(deltaTime *  MathHelper.Tau * player.RPM / 60); }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) { player.Rotate(-deltaTime *  MathHelper.Tau * player.RPM / 60); }

            wallSpawnTime -= deltaTime;
            while(wallSpawnTime < 0)
            {
                TryAddWall();
                wallSpawnTime += 1 / wallsPerSecond;
            }

            foreach(Wall wall in walls)
            {
                wall.Distance -= wall.Speed * deltaTime;
                if (PlayerHitBy(wall)) {
                    GameOver();
                }
            }

            walls.RemoveAll(w => w.Distance < centerSquareHalfWidth);

            score += deltaTime;
            gameRotation += (float)Math.Tau * deltaTime * gameRPM / 60;

            base.Update(gameTime);
        }

        private void TryAddWall()
        {
            Vector2[] directions = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
            int i = random.Next(4);
            Vector2 direction = directions[i];

            if (walls.Any(w => w.Rotation.Equals(directions[0 + (i <= 0 ? 1 : 0)]) && wallSpawnDistance - w.Distance < player.Radius*10) &&
                walls.Any(w => w.Rotation.Equals(directions[1 + (i <= 1 ? 1 : 0)]) && wallSpawnDistance - w.Distance < player.Radius * 10) &&
                walls.Any(w => w.Rotation.Equals(directions[2 + (i <= 2 ? 1 : 0)]) && wallSpawnDistance - w.Distance < player.Radius * 10)) { return; }
            walls.Add(new Wall(direction, wallSpawnDistance, wallSpeed));
        }
        private bool PlayerHitBy(Wall wall)
        {
            Vector2[] relativePoints = wall.Points();
            Vector2[] wallPoints = relativePoints.Select(x => x + center).ToArray();

            Vector2 playerPoint = center + player.Rotation * player.Distance;
            double distance = MinimumDistance(wallPoints[0], wallPoints[1],playerPoint);

            return distance < player.Radius;
        }

        private void GameOver()
        {
            player.IsDead = true;
            if (score > highScore) { highScore = score; }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Lerp(gameColor,Color.Black,0.8f));
            Matrix centerAndRotate = Matrix.CreateTranslation(center.X, center.Y, 0) * Matrix.CreateTranslation(-center.X, -center.Y, 0) * Matrix.CreateRotationZ(gameRotation) * Matrix.CreateTranslation(center.X, center.Y, 0);
            
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,null,null,null,null,centerAndRotate);
            _spriteBatch.DrawPolygon(Vector2.Zero,new Polygon(new Vector2[]{ Vector2.Zero, new Vector2(1000,1000), new Vector2(1000,-1000)}),Color.Lerp(gameColor, Color.Black, 0.75f),1000, 1);
            foreach ( Wall wall in walls)
            {
                DrawWall(wall);
            }
            DrawPlayer();
            DrawCenterSquare();
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            DrawScore();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawScore()
        {
            _spriteBatch.DrawString(font, "Score: " + Math.Round(score,2), new Vector2(10, 10), Color.Lerp(gameColor, Color.Black, 0.1F));
            _spriteBatch.DrawString(font, "Highscore: " + Math.Round(highScore, 2), new Vector2(10, 30), Color.Lerp(gameColor, Color.Black, 0.1F));
        }

        private void DrawWall(Wall wall)
        {
            Vector2[] ps = wall.Points();
            Color c = Color.Lerp(gameColor, Color.Black, 0.3F);

            _spriteBatch.DrawLine( ps[0],  ps[1],c,5,0f);
        }
        private void DrawPlayer()
        {
            _spriteBatch.Draw(circle,  player.Distance*player.Rotation, null, gameColor, 0f, new Vector2(circle.Width / 2, circle.Height / 2), 2 * player.Radius / circle.Width, SpriteEffects.None, 0f); ;
        }

        /*private void DrawCenterCircle()
        {
            Color c = Color.Lerp(gameColor, Color.Black, 0.3F);
            _spriteBatch.Draw(circle, center, null, c, 0f, new Vector2(circle.Width / 2, circle.Height / 2), 2* centerSquareHalfWidth / circle.Width, SpriteEffects.None, 0f); ;
        }*/
        private void DrawCenterSquare()
        {
            Color c = Color.Lerp(gameColor, Color.Black, 0.3F);
            float thickness = 3f;
            _spriteBatch.DrawLine( new Vector2(centerSquareHalfWidth, centerSquareHalfWidth),  new Vector2(centerSquareHalfWidth, -centerSquareHalfWidth), c, thickness, 0);
            _spriteBatch.DrawLine( new Vector2(centerSquareHalfWidth, -centerSquareHalfWidth),  new Vector2(-centerSquareHalfWidth, -centerSquareHalfWidth), c, thickness, 0);
            _spriteBatch.DrawLine( new Vector2(-centerSquareHalfWidth, -centerSquareHalfWidth),  new Vector2(-centerSquareHalfWidth, centerSquareHalfWidth), c, thickness, 0);
            _spriteBatch.DrawLine( new Vector2(-centerSquareHalfWidth, centerSquareHalfWidth),  new Vector2(centerSquareHalfWidth, centerSquareHalfWidth), c, thickness, 0);
        }

        static private float MinimumDistance(Vector2 v, Vector2 w, Vector2 p)
        {
            // Return minimum distance between line segment vw and point p
            double l2 = (w - v).Length() * (w - v).Length();    // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0) return (p - v).Length();   // v == w case
                                                      // Consider the line extending the segment, parameterized as v + t (w - v).
                                                      // We find projection of point p onto the line. 
                                                      // It falls where t = [(p-v) . (w-v)] / |w-v|^2
                                                      // We clamp t from [0,1] to handle points outside the segment vw.
            float t = (float)Math.Max(0, Math.Min(1, (p - v).Dot(w - v) / l2));
            Vector2 projection = v + t * (w - v);  // Projection falls on the segment
            return (p - projection).Length();
        }
    }
}