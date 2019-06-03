using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoTycoon.Core.Physics;

namespace MonoTycoon.Core
{
    public static class SpriteBatchExtensions
    {
        //public static void Draw(this SpriteBatch sb, Texture2D texture2D)
        public static void Draw(this SpriteBatch sb, Texture2D texture, Transform2 transform)
        {
            sb.Draw(texture, transform.Location, sourceRectangle: null, Color.White, transform.Rotation.Value, Vector2.Zero, transform.Scale, SpriteEffects.None, transform.ZIndex);
        }
    }
}