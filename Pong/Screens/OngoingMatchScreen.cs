using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoTycoon.Screens;
using MonoTycoon.States;
using Pong.Entities;
using Pong.Entities.GUI;
using Pong.Mechanics;
using Pong.Mechanics.Serve;

namespace Pong.Screens
{
    public class OngoingMatchScreen : Screen, IMatchStateSensitive, IRoundStateSensitive
    {
#if DEBUG
        SpriteFont debugFont;
#endif
        // Game Components
        Paddle PlayerPaddle { get; set; }
        Paddle AiPaddle { get; set; }
        Ball Ball { get; set; }
        public ScoreDisplay ScoreDisplay { get; private set; }

        IMatch _match;
        IRound _round => _match.CurrentRound;

        public ServeBallHandler ServeBallHandler { get; private set; }
        public FirstServerFinder FirstServerFinder { get; private set; }

        Song music;

        public OngoingMatchScreen(Game game) : base(game)
        {
            Translucent = false;

            Game.Components.Add(Ball = new Ball(game));
            Game.Components.Add(AiPaddle = new Paddle(game, Team.Red));
            Game.Components.Add(PlayerPaddle = new Paddle(game, Team.Blue));
            Game.Components.Add(ScoreDisplay = new ScoreDisplay(game));
            Game.Components.Add(FirstServerFinder = new FirstServerFinder(Game, Ball));
            Game.Components.Add(ServeBallHandler = new ServeBallHandler(Game));
			
			//updateActions.Add((x) => {
			//	if (!(x is IMatchStateSensitive sensitiveToMatchStateComp))
			//		return;
			//});
		}

        public override void Initialize()
        {
            _match = Game.Services.GetService<IMatch>();
			_match.StateChanges += StateChanged;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
#if DEBUG
            debugFont = Game.Content.Load<SpriteFont>("fonts/Arial");
#endif
            music = Game.Content.Load<Song>("music/music");
		}
        
        /// <summary>
		/// MATCH EVENTS
		/// </summary>
        public void StateChanged(IMachineStateComponent<MatchState> component, MatchState previous)
        {
            if (_match.State == MatchState.InstanciatedRound)
            {
                _match.CurrentRound.StateChanges += StateChanged;
            }
            else if (_match.State == MatchState.FindingFirstServer)
            {
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(music);
            }
        }

        /// <summary>
        /// ROUND EVENTS
        /// </summary>
        public void StateChanged(IMachineStateComponent<RoundState> component, RoundState previous)
        {
            if (_round.State.Equals(RoundState.WaitingForBallServe))
            {
                Paddle servingPaddle = Components.OfType<Paddle>().Where((x) => x.Team == _round.ServingTeam).Single();
                ServeBallHandler.AssignRequiredEntities(Ball, servingPaddle);
            }
            else if (previous.Equals(RoundState.WaitingForBallServe))
            {
                ServeBallHandler.FreeRequiredEntities();
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