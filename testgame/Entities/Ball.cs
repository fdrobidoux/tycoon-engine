using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoTycoon.Core;
using MonoTycoon.Core.Physics;
using testgame.Core;
using testgame.Mechanics;

namespace testgame.Entities
{
    public class Ball : DrawableGameComponent
    {
        private const double STARTING_VELOCITY = 150; // Pixels per second.

        public Transform2 Transform { get; set; }

        public double Velocity = STARTING_VELOCITY;
        public Vector2 Direction = Vector2.UnitX;

        public Texture2D Sprite;

        public Ball(Game game) : base(game)
        {
            Transform = Transform2.Zero;
        }

        public override void Initialize()
        {
            base.Initialize();

            Transform = new Transform2(Vector2.Zero, new Size2(50, 50), 1f);

            IMatch _match = Game.Services.GetService<IMatch>();

            _match.MatchStateChanges += OnMatchStateChanges;

            Velocity = STARTING_VELOCITY;
            //Direction = Vector2.UnitY;
            Direction = new Vector2(0.35f, 0.65f);

            Enabled = false;
            Visible = false;
        }

        private void OnMatchStateChanges(object sender, ValueChangedEvent<MatchState> e)
        {
            if (sender is IMatch match)
            {
                Visible = e.Current.Any(MatchState.InstanciatedRound, MatchState.InProgress);

                if (e.Current == MatchState.InstanciatedRound)
                    SetRoundEvents(match.CurrentRound);
            }
        }

        private void OnRoundStateChanges(object sender, ValueChangedEvent<RoundState> e)
        {
            Visible = !e.Current.Equals(RoundState.NotStarted);
            Enabled = e.Current.Equals(RoundState.InProgress);

            if (e.Current.Equals(RoundState.WaitingForBallServe))
            {
                Transform.Scale = 1.5f;
                Visible = true;
            }
        }

        private void SetRoundEvents(IRound round) => round.RoundStateChanges += OnRoundStateChanges;
        private void UnsetRoundEvents(IRound round) => round.RoundStateChanges -= OnRoundStateChanges;

        public void Reset()
        {
            // TODO: Implement reset method.
            throw new NotImplementedException();

            /*Position = (GraphicsDevice.Viewport.Bounds.Center.ToVector2() / 2f) -
                       ((_baseSize / 2) * Transform.Scale);*/
        }

        protected override void LoadContent()
        {
            Sprite = Game.Content.Load<Texture2D>("ball");
            //DebugFont = Game.Content.Load<SpriteFont>("arial");
        }

        public override void Update(GameTime gt)
        {
            Transform.Location += Direction * (float)(Velocity * gt.ElapsedGameTime.TotalSeconds);

            Bounce(gt, GraphicsDevice.Viewport.Bounds);

            //ConstrainWithinBounds(GraphicsDevice.Viewport.Bounds);
#if DEBUG
            Console.WriteLine($"Transform {Transform.ToString()}");
#endif
        }

        private void Bounce(GameTime gt, Rectangle bounds)
        {
            Transform.DeconstructScaledF(out Vector2 locationF, 
                                         out Vector2 sizeF);
            sizeF /= 2f;
            var minLocationF = locationF - sizeF;

            float diffX, diffY;

            if ((diffX = minLocationF.X) <= 0f)
            {
                Direction *= new Vector2(-1f, 1f);
                Transform -= new Vector2(diffX, 0f);
            }
            else if ((diffX = bounds.Right - (locationF + sizeF).X) <= 0f)
            {
                Direction *= new Vector2(-1f, 1f);
                Transform += new Vector2(diffX, 0f);
            }
            if ((diffY = minLocationF.Y) <= 0f)
            {
                Direction *= new Vector2(1f, -1f);
                Transform -= new Vector2(0f, diffY);
            }
            else if ((diffY = bounds.Bottom - (locationF + sizeF).Y) <= 0f)
            {
                Direction *= new Vector2(1f, -1f);
                Transform += new Vector2(0f, diffY);
            }
        }

        public void ConstrainWithinBounds(Rectangle viewBounds)
        {
            //Vector2 boundOpposite = (Size + Position);

            Transform.Location = new Vector2(
                Math.Clamp(Transform.Location.X, viewBounds.Left, viewBounds.Right - Transform.Size.Width),
                Math.Clamp(Transform.Location.Y, viewBounds.Top, viewBounds.Bottom - Transform.Size.Height)
            );
        }

        private Rectangle centerDivide()
        {
            Rectangle rect = Transform.ToRectangle();
            rect.Location -= rect.Size.DivideBy(2);
            return rect;
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GetSpriteBatch().Draw(Sprite, centerDivide(), Color.White);
#if DEBUG
            var str = $"Transform {Transform.ToRectangle().ToString()}";
            //var position = new Vector2(x: Game.GraphicsDevice.Viewport.Width, y: (Game.GraphicsDevice.Viewport.Height - DebugFont.MeasureString(str).Y));
            //Game.GetSpriteBatch().DrawString(DebugFont, str, position, Color.Azure, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Console.WriteLine("^ DRAWN ^");
#endif 
        }
    }
}