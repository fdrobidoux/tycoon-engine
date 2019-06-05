using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoTycoon.Core.Screens;
using testgame.Core;
using testgame.Entities;
using testgame.Entities.GUI;
using testgame.Mechanics;
using testgame.Mechanics.Serve;

namespace testgame.Screens
{
    public class OngoingMatchScreen : Screen
    {
#if DEBUG
        private SpriteFont debugFont;
#endif
        // Game Components
        private Paddle PlayerPaddle { get; set; }
        private Paddle AiPaddle { get; set; }
        private Ball Ball { get; set; }
        private ScoreDisplay ScoreDisplay { get; set; }

        private IMatch _match;
        private IRound _round => _match.CurrentRound;

        public ServeBallHandler ServeBallHandler { get; private set; }
        private FirstServerFinder FirstServerFinder { get; set; }

        private Song music;

        public OngoingMatchScreen(Game game) : base(game)
        {
            Translucent = false;

            Components.Add(Ball = new Ball(game));
            Components.Add(AiPaddle = new Paddle(game, Team.Red));
            Components.Add(PlayerPaddle = new Paddle(game, Team.Blue));
            Components.Add(ScoreDisplay = new ScoreDisplay(game));
            Components.Add(FirstServerFinder = new FirstServerFinder(Game, Ball));
            Components.Add(ServeBallHandler = new ServeBallHandler(Game));
        }

        public override void Initialize()
        {
            _match = Game.Services.GetService<IMatch>();
            _match.MatchStateChanges += onMatchStateChanges;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            debugFont = Game.Content.Load<SpriteFont>("Arial");
            music = Game.Content.Load<Song>("music");
        }

        /// <summary>
        /// MATCH EVENTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onMatchStateChanges(object sender, ValueChangedEvent<MatchState> e)
        {
            if (!(sender is IMatch match))
                return;

            if (e.Current == MatchState.InstanciatedRound)
            {
                match.CurrentRound.RoundStateChanges += onRoundStateChanges;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(music);
            }
        }

        /// <summary>
        /// ROUND EVENTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onRoundStateChanges(object sender, ValueChangedEvent<RoundState> e)
        {
            if (!(sender is IRound round))
                return;

            if (e.Current.Equals(RoundState.WaitingForBallServe))
            {
                Paddle servingPaddle = Components.OfType<Paddle>().Where((x) => x.Team == round.ServingTeam).Single();
                ServeBallHandler.AssignEntitiesNecessaryForServing(Ball, servingPaddle);
            }
            else if (e.Previous.Equals(RoundState.WaitingForBallServe))
            {
                ServeBallHandler.UnassignEntitiesNecessaryForServing();
            }
        }

        public override void Draw(GameTime gt)
        {
            base.Draw(gt);
#if DEBUG
            string matchState = Enum.GetName(typeof(MatchState), _match.State);
            Game.GetSpriteBatch().DrawString(debugFont, $"MatchState: {matchState}", Vector2.Zero,
                Color.Salmon, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            string roundState = (this._round != null) ? Enum.GetName(typeof(RoundState), this._round.State) : "NULL";
            Game.GetSpriteBatch().DrawString(debugFont, $"RoundState: {roundState}", new Vector2(0f, 20f),
                Color.YellowGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Ball?.Dispose();
                PlayerPaddle?.Dispose();
                AiPaddle?.Dispose();
                ServeBallHandler?.Dispose();
            }
        }
    }
}