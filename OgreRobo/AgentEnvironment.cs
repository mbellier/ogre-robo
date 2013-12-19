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
        public List<Agent> deadList { get; set; }
        
        public Rect mapDomain;
        public AgentEnvironment(Rect mapDomain)
        {
            rnd = new Random();
            agentList = new List<Agent>();
            deadList = new List<Agent>();
            this.mapDomain = mapDomain;
        }

        public Vector3 GetRandomPosition()
        {
            return new Vector3(
                (float)rnd.NextDouble() * System.Math.Abs(mapDomain.right - mapDomain.left) + mapDomain.left, 
                0,
                (float)rnd.NextDouble() * System.Math.Abs(mapDomain.top - mapDomain.bottom) + mapDomain.bottom);
        }

        public void Update(float dt)
        {
            
            foreach (Agent a in agentList)
            {
                a.Update(dt);
            }
            foreach (Agent a in deadList)
            {
                agentList.Remove(a);
            }
            deadList.Clear();
        }
    }
}
