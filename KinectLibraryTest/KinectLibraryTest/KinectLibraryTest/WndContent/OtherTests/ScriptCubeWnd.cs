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
    public class ScriptCubeWnd : WndHandle
    {
        private List<ScriptCube> cubes;
        private Random gen;

        public ScriptCubeWnd(Rectangle dest, WndGroup parent)
            : base(3162, dest, parent)
        {
            ImageTools imgTools = ImageTools.getSingleton(parent.getAppRef());
            cubes = new List<ScriptCube>();
            gen = new Random();
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            foreach (ScriptCube cube in cubes)
                cube.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
            foreach (ScriptCube cube in cubes)
                cube.draw(spriteBatch);
        }

        public void spawnCube()
        {
            cubes.Add(new ScriptCube(gen, this));
        }

        public void removeCube(int id)
        {
            if (id >= 0 && id < cubes.Count)
                cubes.RemoveAt(id);
        }

        public ScriptCube getCube(int id)
        {
            if (id >= 0 && id < cubes.Count)
                return cubes[id];
            else
                return null;
        }

        public void setScript(int cubeID, string script)
        {
            if (cubeID >= 0 && cubeID < cubes.Count)
                cubes[cubeID].setScript(script);
        }

        public string getScript(int cubeID)
        {
            if (cubeID >= 0 && cubeID < cubes.Count)
                return cubes[cubeID].getScript();
            return "Error.... does not exist.";
        }
    }
}
