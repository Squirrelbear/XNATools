using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XNATools.WndCore;
using XNATools.WndExtra;

namespace KinectLibraryTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class KinectGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        WndGroup wndCollection;
        bool _exitGame = false;
        Type[] wndTypes = { typeof(PongWnd), typeof(GripWndSample), typeof(NumberGameWnd), 
                              typeof(LightsaberWnd), typeof(DrawingGameWnd), typeof(TilePuzzleWnd) };

        bool numberSwapEnabled = false;

        public KinectGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Content.RootDirectory = "Content";

#if !DEBUG
            try
            {
                graphics.ToggleFullScreen();
            }
            catch (Exception) {}
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            int wndHeight = GraphicsDevice.Viewport.Height;
            int wndWidth = GraphicsDevice.Viewport.Width;
            Rectangle displayRect = new Rectangle(0, 0, wndWidth, wndHeight);

            wndCollection = new WndGroup(-1, displayRect, null, this);
            wndCollection.addWnd(WndFactory.createWnd<DrawingGameWnd>(displayRect, wndCollection));
#if DEBUG
            InGameConsole console = new InGameConsole(wndCollection);
            console.openConsole();
#endif
            //wndCollection.addWnd(WndFactory.createWnd<PongWnd>(displayRect, wndCollection));
            //WndFactory.createWnd(WndFactory.WndCodes.PongWnd, displayRect, wndCollection));

            //kinectInputManager = new KinectInputManager(this);
            //oldLeft = KinectInputManager.GripState.Unknown;
            //oldRight = KinectInputManager.GripState.Unknown;

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /*public void setWnd<T>() where T : WndHandle
        {
            WndHandle handle = wndCollection.getWndAt(0);
            if (handle == null || handle.GetType() != typeof(T))
            {
                wndCollection.removeAllWnd();
                int wndHeight = GraphicsDevice.Viewport.Height;
                int wndWidth = GraphicsDevice.Viewport.Width;
                Rectangle displayRect = new Rectangle(0, 0, wndWidth, wndHeight);
                wndCollection.addWnd(WndFactory.createWnd<T>(displayRect, wndCollection));
            }
        }*/

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_exitGame)
            {
                wndCollection.onWndClosing();
                this.Exit();
                return;
            }
            
            wndCollection.update(gameTime);

            // Allows the game to exit
            if (wndCollection.getInputManager().isKeyPressed(Keys.Escape))
                exitGame();

            if (wndCollection.getInputManager().isKeyPressed(Keys.Home))
                numberSwapEnabled = !numberSwapEnabled;

            if (wndCollection.getInputManager().isKeyPressed(Keys.End))
            {
                traverseWndHandleTree(wndCollection);
            }

            if (numberSwapEnabled)
            {
                int num = (int)Keys.D1;
                for (int i = 0; i < wndTypes.GetLength(0); i++)
                {
                    if (wndCollection.getInputManager().isKeyPressed((Keys)(num + i)))
                        wndCollection.setWnd(wndTypes[i]);
                }
            }

            base.Update(gameTime);
        }

        public void traverseWndHandleTree(WndHandle startPoint)
        {
            WndHandle target = startPoint;
            while (target.getParent() != null) target = target.getParent();
            traverseWndHandleNodes(target);
        }

        private void writeWithIndent(string text, int indent)
        {
            for (int i = 0; i < indent; i++)
                Console.Write("\t");
            Console.WriteLine(text);
        }

        private void traverseWndHandleNodes(WndHandle startPoint, int indent = 0)
        {
            writeWithIndent(startPoint.ToString(), indent);
            if (IsWndGroupType(startPoint.GetType()))
            {
                WndGroup groupObj = (WndGroup)startPoint;
                foreach (WndHandle nextHandle in groupObj.getAllWnd())
                    traverseWndHandleNodes(nextHandle, indent+1);
            }

            traverseWndComponentNodes(startPoint, indent +1);
        }

        bool IsWndGroupType(Type t)
        {
            if (t == typeof(WndGroup)) return true;
            if (t == typeof(object)) return false;
            return IsWndGroupType(t.BaseType);
        }
        bool IsPanelType(Type t)
        {
            if (t == typeof(Panel)) return true;
            if (t == typeof(object)) return false;
            return IsPanelType(t.BaseType);
        }

        private void traverseWndComponentNodes(WndHandle handle, int indent)
        {
            foreach (WndComponent c in handle.getComponents())
            {
                writeWithIndent(c.ToString(), indent);
                if (IsPanelType(c.GetType()))
                    traverseWndComponentNodes((Panel)c, indent + 1);
            }
        }

        private void traverseWndComponentNodes(Panel panel, int indent)
        {
            foreach (WndComponent c in panel.getComponents())
            {
                writeWithIndent(c.ToString(), indent);
                if (IsPanelType(c.GetType()))
                    traverseWndComponentNodes((Panel)c, indent + 1);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            wndCollection.draw(spriteBatch);

            base.Draw(gameTime);
        }

        public void exitGame()
        {
            _exitGame = true;
        }
    }
}
