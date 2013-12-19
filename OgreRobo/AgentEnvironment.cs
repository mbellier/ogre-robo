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

        public int initNbAgents = 30;
        public List<Agent> agentList { get; set; }
        public int  nbNinja = 0;
        public int nbRobot = 0;
        public int round = 0;
        
        public Rect mapDomain;
        public SceneManager scene;
        AgentFactory agentFactory;

        public AgentEnvironment(Rect mapDomain, SceneManager scene)
        {
            rnd = new Random();
            agentList = new List<Agent>();
            this.mapDomain = mapDomain;
            this.scene = scene;

            agentFactory = new AgentFactory(scene, this);
        }

        public void newRound()
        {
            if (round == 0)
            {
                agentFactory.spawnRobotsAndOgres(initNbAgents);
            }
            else
            {
                Agent bestRobot = getBestAgent(0);
                Agent bestNinja = getBestAgent(1);

                foreach (Agent a in agentList)
                {
                    scene.RootSceneNode.RemoveChild(a.node);
                }
                agentList.Clear();

                agentFactory.spawnRobotsAndOgres(initNbAgents);   
            }
            round++;
        }

        public Agent getBestAgent(int team)
        {
            Agent best = null;
            foreach (Agent a in agentList)
            {
                if (best == null)
                {
                    if (a.team == team)
                    {
                        best = a;
                    }
                }
                else if (a.frags > best.frags && a.team == team)
                {
                    best = a;
                }
            }

            return best;
        }

        public bool IsValidPosition(Vector3 pos)
        {
            return pos.x > mapDomain.left && pos.x < mapDomain.right && pos.y > mapDomain.bottom && pos.y < mapDomain.top;
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
                if (!a.dead)
                {
                    a.Update(dt);
                }
            }

            if (nbRobot <= 0 || nbNinja <= 0)
            {
                newRound();
            }
        }
    }
}
