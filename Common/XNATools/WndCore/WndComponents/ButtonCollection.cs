using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNATools.WndCore
{
    public class ButtonCollection : WndComponent
    {
        protected List<Button> buttonList;
        protected SoundEffect clicked, changed;
        protected SoundEffectInstance iChanged, iClicked;
        protected bool clickedEffectSet, changedEffectSet;
        protected int selectedIndex;
        protected bool btnClicked;
        protected bool keepOneSelected;

        public ButtonCollection(Rectangle dest)
            : base(dest)
        {
            clicked = changed = null;
            clickedEffectSet = changedEffectSet = false;
            buttonList = new List<Button>();
            selectedIndex = 0;
            btnClicked = false;
            keepOneSelected = true;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            btnClicked = false;
        }

        public override void draw(SpriteBatch spritebatch)
        {
            base.draw(spritebatch);

            foreach (Button b in buttonList)
            {
                b.draw(spritebatch);
            }
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            base.mouseMoved(oldP, newP);

            int selectedIndex = -1;
            for (int i = 0; i < buttonList.Count; i++)
            {
                Button b = buttonList[i];
                if (b.getRect().Contains(newP) && !b.getSelected())
                {
                    setSelected(i);
                    selectedIndex = i;
                }
                else if (!keepOneSelected && !b.getRect().Contains(newP))
                {
                    b.setSelected(false);
                }
            }

            if (!keepOneSelected && selectedIndex == -1)
            {
                selectedIndex = -1;
            }
        }

        public override void mouseClickedLeft(Point p)
        {
            base.mouseClickedLeft(p);

            for (int i = 0; i < buttonList.Count; i++)
            {
                Button b = buttonList[i];
                b.mouseClickedLeft(p);
                if (b.getIsClicked())
                {
                    btnClicked = true;
                    setSelected(i);
                }
            }
        }

        public bool isBtnClicked()
        {
            return btnClicked;
        }

        public virtual void add(Button button)
        {
            buttonList.Add(button);
        }

        public void remove(int removePos)
        {
            buttonList.RemoveAt(removePos);
        }

        public void setSelected(int id)
        {
            if (id < 0 || id > buttonList.Count - 1)
                return;

            if (changedEffectSet)
                iChanged.Play();

            //reset currently selected
            buttonList[selectedIndex].setSelected(false);

            buttonList[id].setSelected(true);
            selectedIndex = id;
        }

        public override void next()
        {

            //do nothing if list only contains 1, or is empty
            if (buttonList.Count > 1)
            {
                if (changedEffectSet)
                    iChanged.Play();

                for (int i = 0; i < buttonList.Count; i++)
                {

                    //find currently selected
                    if (buttonList[i].getSelected())
                    {

                        //reset currently selected
                        buttonList[i].setSelected(false);

                        //select next in line
                        if (i + 1 < buttonList.Count)
                        {

                            buttonList[i + 1].setSelected(true);
                            selectedIndex = i + 1;
                            return;

                            //if no next in line, start at beginning   
                        }
                        else
                        {

                            buttonList[0].setSelected(true);
                            selectedIndex = 0;
                            return;

                        }
                    }
                }
            }
        }

        public override void previous()
        {
            if (buttonList.Count > 1)
            {
                if(changedEffectSet)
                    iChanged.Play();

                for (int i = 0; i < buttonList.Count; i++)
                {

                    if (buttonList[i].getSelected())
                    {

                        buttonList[i].setSelected(false);

                        if ((i - 1) >= 0)
                        {

                            buttonList[i - 1].setSelected(true);
                            selectedIndex = i - 1;
                            return;

                        }
                        else
                        {

                            buttonList[buttonList.Count - 1].setSelected(true);
                            selectedIndex = buttonList.Count - 1;
                            return;

                        }
                    }
                }
            }
        }

        public Button getSelected()
        {

            foreach (Button b in buttonList)
            {

                if (b.getSelected())

                    return b;
            }

            return null;
        }

        public void playSelectedSound()
        {
            if(clickedEffectSet)
                iClicked.Play();
        }

        public void setChangedSound(SoundEffect changed)
        {
            this.changed = changed;
            iChanged = changed.CreateInstance();
            changedEffectSet = true;
        }

        public void setClickedSound(SoundEffect clicked)
        {
            this.clicked = clicked;
            iClicked = clicked.CreateInstance();
            clickedEffectSet = true;
        }

        public void disableChangedSound()
        {
            changedEffectSet = false;
        }

        public void disableClickedSound()
        {
            clickedEffectSet = false;
        }

        public override void moveByAndChildren(Vector2 translation)
        {
            foreach (WndComponent c in buttonList)
                c.moveByAndChildren(translation);

            base.moveByAndChildren(translation);
        }

        public void setKeepOneSelected(bool keepOneSelected)
        {
            this.keepOneSelected = keepOneSelected;
        }

        public List<Button> getButtons()
        {
            return buttonList;
        }
    }
}
