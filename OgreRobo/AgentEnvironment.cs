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

        public float xmin, xmax, zmin, zmax;

        public AgentEnvironment(float xmin, float xmax, float zmin, float zmax)
        {
            rnd = new Random();
            agentList = new List<Agent>();
            this.xmin = xmin;
            this.xmax = xmax;
            this.zmin = zmin;
            this.zmax = zmax;
        }

        public Vector3 GetRandomPosition()
        {
            return new Vector3(
                (float)rnd.NextDouble() * (xmax - xmin) + xmin,
                0,
                (float)rnd.NextDouble() * (zmax - zmin) + zmin);
        }

        public void Update(float dt)
        {
            Console.WriteLine(agentList.ElementAt(0).GetPosition());
            foreach (Agent a in agentList)
            {
                a.Update(dt);
            }
        }
    }
}
