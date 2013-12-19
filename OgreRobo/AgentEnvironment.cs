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

        public float initHealth = 100;
        public float initDamages = 10;
        
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

                Console.Out.Write("\n\nGénération : ");
                Console.Out.Write(round);
                Console.Out.Write(", Gagnant : ");
                Console.Out.Write((nbRobot <= 0) ? "Ninja" : "Robot" );
                Console.Out.Write("\nBest Robot :\n");
                Console.Out.Write("   - Max Health : ");
                Console.Out.Write(bestRobot.maxHealth);
                Console.Out.Write("\n   - Max Damages : ");
                Console.Out.Write(bestRobot.damages);
                Console.Out.Write("\nBest Ninja :\n");
                Console.Out.Write("   - Max Health : ");
                Console.Out.Write(bestNinja.maxHealth);
                Console.Out.Write("\n   - Max Damages : ");
                Console.Out.Write(bestNinja.damages);

                foreach (Agent a in agentList)
                {
                    scene.RootSceneNode.RemoveChild(a.node);
                }
                agentList.Clear();

                nbRobot = 0;
                nbNinja = 0;
                agentFactory.spawnRobotsAndOgres(initNbAgents);


                float factorRobot = bestRobot.maxHealth / initHealth;
                float factorNinja = bestNinja.maxHealth / initHealth;

                foreach (Agent a in agentList)
                {
                    float rand = (2 * (float) rnd.NextDouble() - 1);
                    

                    if (a.team == 0)
                    {
                        a.maxHealth = a.maxHealth * factorRobot + rand * 5;
                        a.damages = a.damages * factorRobot + rand * -1;
                        a.health = a.maxHealth;
                    }
                    else
                    {
                        a.maxHealth = a.maxHealth * factorNinja + rand * 5;
                        a.damages = a.damages * factorNinja +  rand * -1;
                        a.health = a.maxHealth;
                    }
                }
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
