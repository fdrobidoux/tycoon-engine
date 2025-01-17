﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoTycoon;
using System;
using System.Collections.Generic;
using Pong;
using Pong.Entities;
using MonoTycoon.States;

namespace Pong.Mechanics.Serve
{
    public class ServeBallHandler : GameComponent, IMatchStateSensitive, IRoundStateSensitive
    {
        private const int SECONDS_TIL_CAN_SERVE = 3;

        private bool canServe;

        public Paddle ServingPaddle { get; private set; }
        public Ball TheBall { get; private set; }

        #region "Timers"
        private TimerTask timer_AllowServing;
        #endregion

        public ServeBallHandler(Game game) : base(game)
        {
            timer_AllowServing = new TimerTask(allowServing, TimeSpan.FromSeconds(SECONDS_TIL_CAN_SERVE).TotalMilliseconds, false);
        }

        public override void Initialize()
        {
            Enabled = false;

            IMatch match = Game.Services.GetService<IMatch>();
            match.StateChanges += StateChanged;

            timer_AllowServing.Reset();
            timer_AllowServing.Enabled = true;

            canServe = false;
        }

        public void StateChanged(IMachineStateComponent<MatchState> component, MatchState previousState)
        {
            if (!(component is IMatch match)) return;

            if (match.State == MatchState.InstanciatedRound)
                match.CurrentRound.StateChanges += StateChanged;
        }

        public void StateChanged(IMachineStateComponent<RoundState> component, RoundState previous)
        {
			//if (!(sender is IRound round)) return;
            var round = component;

			Enabled = round.State.Equals(RoundState.WaitingForBallServe);
        }

        public override void Update(GameTime gt)
        {
            timer_AllowServing.Update(gt);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Enabled = false;

                var blah = Vector2.Normalize(new Vector2(-1f, 1f));
                if (ServingPaddle.Team == Team.Blue)
                    blah *= -1;

                // Set velocity to Ball.
                TheBall.Direction = blah;

                // Set round state to `InProgress`.
                Game.Services.GetService<IMatch>().CurrentRound.State = RoundState.InProgress;
            }
            else
            {
                TheBall.Transform.Location = new Vector2(
                    x: TheBall.Transform.Location.X, 
                    y: ServingPaddle.Transform.Location.Y + (ServingPaddle.Transform.Size.Height / 2));
            }
        }

        public void AssignRequiredEntities(Ball ball, Paddle servingPaddle)
        {
            TheBall = ball;
            ServingPaddle = servingPaddle;
        }

        public void FreeRequiredEntities()
        {
            TheBall = null;
            ServingPaddle = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                FreeRequiredEntities();
        }

        private void allowServing()
        {
            canServe = true;
        }
    }
}
