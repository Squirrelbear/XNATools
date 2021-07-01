using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectLibraryTest
{
    public class AICell : AnimatedObject
    {
        private int playerNumber;
        private int cellX, cellY;
        private float cellControl;
        private float cellStrength;
        private AIGridWarWnd game;
        private AICell left, right, up, down;
        private List<AICell> attackSequence;
        private int sequenceID;
        private int timeToNext;

        public AICell(Texture2D white, Rectangle rect, int cellX, int cellY, AIGridWarWnd game)
            : base(white, 1, 1, rect)
        {
            this.game = game;
            this.playerNumber = -1;
            this.cellControl = 0;
            this.cellStrength = 1;
            this.cellX = cellX;
            this.cellY = cellY;
            
            sequenceID = 0;
        }

        public void setupNeighbours()
        {
            left = game.getCell(cellX - 1, cellY);
            right = game.getCell(cellX + 1, cellY);
            up = game.getCell(cellX, cellY - 1);
            down = game.getCell(cellX, cellY + 1);

            AICell[] tempArr = new AICell[] { left, left, left, left, right, right, right, right, 
                                    up, up, up, up, down, down, down, down};
            attackSequence = tempArr.ToList();
            Random rng = game.getSharedRandom();
            Shuffle(attackSequence);
            timeToNext = 0;
        }

        public void seedCell(int playerNumber, float cellControl)
        {
            this.playerNumber = playerNumber;
            this.cellControl = cellControl;
            this.cellStrength = 1;
            setColor(game.getPlayerColor(playerNumber));
            setOpacity(cellControl / 255);
            timeToNext = game.getSharedRandom().Next(3, 25);
        }

        public void doTurn()
        {
            if (this.playerNumber == -1) return;

            if (timeToNext > 0)
            {
                timeToNext--;
                return;
            }

            if (cellStrength < 255)
                cellStrength += 0.03f;

            if (cellControl < 255)
                cellControl += 0.5f * game.getSharedRandom().Next(1, 90);

            for (int round = 0; round < 5; round++)
            {
                if (cellControl > 30)
                {
                    attack(attackSequence[sequenceID]);
                }
                sequenceID++;
                if (sequenceID >= attackSequence.Count())
                    sequenceID = 0;
            }

            setOpacity(cellControl / 255);
            timeToNext = game.getSharedRandom().Next(5, 50);
        }

        public void attack(AICell target)
        {
            if (target == null || playerNumber == target.getPlayerNumber()) return;

            if (target.playerNumber == -1)
            {
                target.seedCell(this.playerNumber, cellControl / 2);
                this.cellControl = cellControl / 2;
            }
            else
            {
                int fightCount = game.getSharedRandom().Next(3, 10);
                for (int fightNumber = 0; fightNumber < fightCount; fightNumber++)
                {
                    int fightOutcome = game.getSharedRandom().Next(-50, 90); /*- (int)(25 * (target.getCellStrength() / 255)), 
                                                                    30 + (int)(45 * (getCellStrength() / 255)));*/
                    if (fightOutcome == 0) continue;
                    if (fightOutcome < 0)
                    {
                        this.cellControl += fightOutcome;
                        if (cellControl < 30) return;
                    }
                    else
                    {
                        target.setCellControl(target.getCellControl() - fightOutcome);
                        if (target.getCellControl() == 0)
                        {
                            target.seedCell(this.playerNumber, cellControl / 2);
                            this.cellControl = cellControl / 2;
                            return;
                        }
                    }
                }
            }
        }

        public void setCellControl(float cellControl)
        {
            if (cellControl > 255)
                this.cellControl = 255;
            else if(cellControl < 0)
                this.cellControl = 0;
            else
                this.cellControl = cellControl;
        }

        public float getCellControl()
        {
            return cellControl;
        }

        public void setCellStrength(float cellStrength)
        {
            if (cellStrength > 255)
                this.cellStrength = 255;
            else if (cellStrength < 0)
                this.cellStrength = 0;
            else
                this.cellStrength = cellStrength;
        }

        public float getCellStrength()
        {
            return cellStrength;
        }

        public int getPlayerNumber()
        {
            return playerNumber;
        }

        private void Shuffle<T>(IList<T> list)
        {
            Random rng = game.getSharedRandom();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
