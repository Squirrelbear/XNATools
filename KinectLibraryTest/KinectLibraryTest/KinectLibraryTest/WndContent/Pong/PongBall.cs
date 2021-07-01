using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class PongBall : WndComponent
    {
        protected Vector2 speed;
        protected Vector2 position;
        protected PongWnd parent;
        protected Random gen;

        public PongBall(Rectangle dest, PongWnd parent)
            : base(dest)
        {
            Texture2D blank = new Texture2D(parent.getParent().getAppRef().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            setTexture(blank);
            this.parent = parent;
            position = new Vector2(dest.X, dest.Y);
            gen = new Random();
            int dirX = (gen.Next(2)==0) ? -1 : 1;
            int dirY = (gen.Next(2)==0) ? -1 : 1;
            speed = new Vector2(gen.Next(150, 230) * dirX, gen.Next(150, 230) * dirY);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            position += speed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (position.Y < 0)
            {
                position.Y = 0;
                speed.Y = -speed.Y;
            }
            else if (position.Y > parent.getRect().Bottom - getRect().Height)
            {
                position.Y = parent.getRect().Bottom - getRect().Height;
                speed.Y = -speed.Y;
            }
            else if (parent.getPaddle(true).getRect().Intersects(new Rectangle((int)position.X, (int)position.Y, getRect().Width, getRect().Height)))
            {
                position.X = parent.getPaddle(true).getRect().Right;
                speed.X = -speed.X * 1.2f;
            }
            else if (parent.getPaddle(false).getRect().Intersects(new Rectangle((int)position.X, (int)position.Y, getRect().Width, getRect().Height)))
            {
                position.X = parent.getPaddle(false).getRect().Left - getRect().Width;
                speed.X = -speed.X * 1.2f;
            }
            else if (!parent.getRect().Contains(new Point((int)position.X, (int)position.Y)))
            {
                parent.increaseScore(!(position.X < 0));
                position.X = parent.getRect().Center.X - getRect().Width / 2;
                position.Y = parent.getRect().Center.Y - getRect().Height / 2;
                int dirX = (gen.Next(2) == 0) ? -1 : 1;
                int dirY = (gen.Next(2) == 0) ? -1 : 1;
                speed = new Vector2(gen.Next(150, 230) * dirX, gen.Next(150, 230) * dirY);
            }

            setLocation(position);
        }
    }

}
