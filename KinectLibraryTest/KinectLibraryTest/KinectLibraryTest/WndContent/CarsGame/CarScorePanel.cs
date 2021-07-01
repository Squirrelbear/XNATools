using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools.WndCore;
using Microsoft.Xna.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KinectLibraryTest
{
    public class CarScorePanel : Panel
    {
        private List<KeyValuePair<int, string>> scores;
        private CarGameWnd mp;

        public CarScorePanel(Rectangle rect, CarGameWnd mp)
            : base(rect)
        {
            this.mp = mp;
            if (File.Exists("highscores.dat"))
                scores = DeserializeFromString(File.ReadAllText("highscores.dat"));
            else
                scores = new List<KeyValuePair<int, string>>();
        }

        public bool isHighScore(int score)
        {
            return scores.Count == 0 || scores.Min(a => a.Key) < score;
        }

        public void addHighScore(int score, string name)
        {
            scores.Add(new KeyValuePair<int, string>(score, name));
            scores.Sort((x, y) => x.Key.CompareTo(y.Key));
            if(scores.Count > 10)  
                scores.RemoveRange(10, scores.Count - 10);
            File.WriteAllText("highscores.dat", SerializeToString(scores));
        }

        private List<KeyValuePair<int, string>> DeserializeFromString(string settings)
        {
            byte[] b = Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (List<KeyValuePair<int, string>>)formatter.Deserialize(stream);
            }
        }

        private string SerializeToString(List<KeyValuePair<int, string>> settings)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
