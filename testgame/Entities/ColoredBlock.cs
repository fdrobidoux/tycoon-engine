﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoTycoon.Core.Graphics.Primitives;
using MonoTycoon.Core.Physics;
using Pong.Mechanics;
using System;
using System.Collections.Generic;

namespace Pong.Entities
{
    public class ColoredBlock : DrawableGameComponent
    {
        public Transform2 Transform { get; set; }

        public float velocity;
        private Vector2 direction;

        public Texture2D colorTexture;

        public ColoredBlock(Game game) : base(game)
        {
            Transform = new Transform2(1f);
        }

        public override void Initialize()
        {
            base.Initialize();

            velocity = 50;
            var direction = new Vector2(-20, 50);
            direction.Normalize();

            Transform.Size = new Size2(20, 20);
            Transform.Location = (Game.GraphicsDevice.Viewport.Bounds.Location - (Transform.Size / 2)).ToVector2();
        }

        private void Match_MatchStateChanges(object sender, MatchState previous)
        {
            if (!(sender is IMatch match))
                return;

            Enabled = Visible = match.State.Equals(MatchState.NotStarted);
        }

        private void whenRoundStateChanges(object sender, Core.ValueChangedEvent<RoundState> e)
        {
            if (!(sender is IMatch match))
                return;

            Enabled = Visible = match.State.Equals(MatchState.NotStarted);
        }

        protected override void LoadContent()
        {
            colorTexture = new RectangleTexture(Color.DarkRed).Create();
        }

        public override void Update(GameTime gameTime)
        {
            //MathHelper.SmoothStep()
        }

        public override void Draw(GameTime gameTime)
        {
            
        }
    }
}
