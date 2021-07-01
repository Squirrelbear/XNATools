using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework;
using XNATools;
using Microsoft.Xna.Framework.Graphics;

namespace KinectLibraryTest
{
    public class AIGridWarWnd : WndHandle
    {
        private Random sharedRand;
        private AICell[,] grid;
        private Texture2D white;
        private int cellCountX, cellCountY;

        public AIGridWarWnd(Rectangle rect, WndGroup parent)
            : base(8887, rect, parent)
        {
            ImageTools imgTools = ImageTools.getSingleton(parent.getAppRef());
            white = imgTools.createColorTexture(Color.White);

            sharedRand = new Random();
            int cellWidth = 16;
            int cellHeight = 16;
            cellCountX = rect.Width / cellWidth;
            cellCountY = rect.Height / cellHeight;

            grid = new AICell[cellCountX, cellCountY];
            for (int x = 0; x < cellCountX; x++)
            {
                for (int y = 0; y < cellCountY; y++)
                {
                    grid[x, y] = new AICell(white, new Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight),
                                            x, y, this);
                }
            }
            for (int x = 0; x < cellCountX; x++)
            {
                for (int y = 0; y < cellCountY; y++)
                {
                    grid[x, y].setupNeighbours();
                }
            }

            for (int player = 1; player < 6; player++)
            {
                AICell startLocation;
                do
                {
                    startLocation = getCell(sharedRand.Next(0, cellCountX), sharedRand.Next(0, cellCountY));
                } while (startLocation.getPlayerNumber() != -1);
                startLocation.seedCell(player, 100);
            }
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            for (int x = 0; x < cellCountX; x++)
            {
                for (int y = 0; y < cellCountY; y++)
                {
                    grid[x, y].doTurn();
                }
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
            for (int x = 0; x < cellCountX; x++)
            {
                for (int y = 0; y < cellCountY; y++)
                {
                    grid[x, y].draw(spriteBatch);
                }
            }
        }

        public AICell getCell(int cellX, int cellY)
        {
            if (cellX < 0 || cellX > cellCountX - 1 || cellY < 0 || cellY > cellCountY - 1)
                return null;
            else
                return grid[cellX, cellY];
        }

        public Random getSharedRandom()
        {
            return sharedRand;
        }

        public Color getPlayerColor(int playerNumber)
        {
            switch (playerNumber)
            {
                case -1: return Color.White;
                case 1: return Color.Blue;
                case 2: return Color.Green;
                case 3: return Color.Red;
                case 4: return Color.Brown;
                case 5: return Color.Black;
                default: return Color.White;
            }
        }
    }
}
