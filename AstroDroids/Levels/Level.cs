using AstroDroids.Managers;
using AstroDroids.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Levels
{
    public class Level
    {
        protected Scene Scene { get { return SceneManager.GetScene(); } }

        public virtual void StartLevel()
        {

        }

        public virtual IEnumerator LevelScript()
        {
            yield break;
        }
    }
}
