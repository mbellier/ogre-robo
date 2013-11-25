using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;


namespace OgreRobo
{


    class AgentEnvironment
    {
        public Random rnd;

        public List<Agent> agentList { get; set; }

        public Rect mapDomain;

        public AgentEnvironment(Rect mapDomain)
        {
            rnd = new Random();
            agentList = new List<Agent>();
            this.mapDomain = mapDomain;
        }

        public Vector3 GetRandomPosition()
        {
            return new Vector3(
                (float)rnd.NextDouble() * System.Math.Abs(mapDomain.Width) + mapDomain.left,
                0,
                (float)rnd.NextDouble() * System.Math.Abs(mapDomain.Height) + mapDomain.bottom);
        }

        public void Update(float dt)
        {
            
            foreach (Agent a in agentList)
            {
                a.Update(dt);
            }
        }
    }
}
