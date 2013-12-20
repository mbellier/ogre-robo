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
        public Timer roundStart { get; set; }
        
        public Rect mapDomain;
        public SceneManager scene;
        AgentFactory agentFactory;
        TextAreaOverlayElement text;

        public AgentEnvironment(Rect mapDomain, SceneManager scene, TextAreaOverlayElement text)
        {
            rnd = new Random();
            agentList = new List<Agent>();
            roundStart = new Timer();
            this.mapDomain = mapDomain;
            this.scene = scene;
            this.text = text;

            agentFactory = new AgentFactory(scene, this);
        }

        public void updateText(Agent bestRobot, Agent bestNinja)
        {
            // Console

            Console.Out.Write("\n\nGénération : ");
            Console.Out.Write(round + 1);
            Console.Out.Write(" (");
            Console.Out.Write(roundStart.Milliseconds / 1000);
            Console.Out.Write(" s)\nGagnant : ");
            Console.Out.Write((nbRobot <= 0) ? "Ninja (" : "Robot (");
            Console.Out.Write(nbNinja + nbRobot);
            Console.Out.Write(")\n");
            Console.Out.Write("\nBest Robot :\n");
            Console.Out.Write("   - Max Health   : ");
            Console.Out.Write(bestRobot.maxHealth);
            Console.Out.Write("\n");
            Console.Out.Write("   - Max Damages  : ");
            Console.Out.Write(bestRobot.damages);
            Console.Out.Write("\n");
            Console.Out.Write("   - Durée de vie : ");
            Console.Out.Write(bestRobot.lifespan / 1000);
            Console.Out.Write(" s\n");
            Console.Out.Write("Best Ninja :\n");
            Console.Out.Write("   - Max Health   : ");
            Console.Out.Write(bestNinja.maxHealth);
            Console.Out.Write("\n");
            Console.Out.Write("   - Max Damages  : ");
            Console.Out.Write(bestNinja.damages);
            Console.Out.Write("\n");
            Console.Out.Write("   - Durée de vie : ");
            Console.Out.Write(bestNinja.lifespan / 1000);
            Console.Out.Write(" s\n");

            // Overlay
            text.Caption = "GENERATION " + (round+1)
            + "\n\n[Previous generation]\n"
            + "Duration: " + roundStart.Milliseconds / 1000 +  "s\n"
            +"Winning team:\n" 
            + ((nbRobot <= 0) ? "Ninjas (" : "Robots (")
            + (nbNinja + nbRobot)
            + ")\n"
            + "\n[Best Robot]\n"
            + "Health: "
            + bestRobot.maxHealth.ToString("0.0")
            + "\n"
            + "Damages: "
            + bestRobot.damages.ToString("0.0")
            + "\n"
            + "Survived "
            + (bestRobot.lifespan / 1000).ToString("0")
            + "s\n" + bestRobot.frags + " kills\n"
            + "\n[Best Ninja]\n"
            + "Health: "
            + bestNinja.maxHealth.ToString("0.0")
            + "\n"
            + "Damages: "
            + bestNinja.damages.ToString("0.0")
            + "\n"
            + "Survived "
            + (bestNinja.lifespan / 1000).ToString("0")
            + "s\n" + bestNinja.frags + " kills\n";
        }

        public void newRound()
        {
            if (round == 0)
            {
                agentFactory.spawnRobotsAndOgres(initNbAgents);
                text.Caption = "GENERATION 1";
            }
            else
            {
                foreach (Agent a in agentList)
                {
                    scene.RootSceneNode.RemoveChild(a.node);

                    if (a.lifespan == 0)
                    {
                        a.lifespan = roundStart.Milliseconds;
                    }
                }
                Agent bestRobot = getBestAgent(0);
                Agent bestNinja = getBestAgent(1);
                agentList.Clear();

                updateText(bestRobot, bestNinja);

                nbRobot = 0;
                nbNinja = 0;
                agentFactory.spawnRobotsAndOgres(initNbAgents);
                
                foreach (Agent a in agentList)
                {
                    float rand = (2 * (float) rnd.NextDouble() - 1);
                    

                    if (a.team == 0)
                    {
                        a.maxHealth = bestRobot.maxHealth + rand * 5;
                        a.damages = bestRobot.damages + rand * -1;
                        a.health = a.maxHealth;
                    }
                    else
                    {
                        a.maxHealth = bestNinja.maxHealth + rand * 5;
                        a.damages = bestNinja.damages +  rand * -1;
                        a.health = a.maxHealth;
                    }
                }
            }

            roundStart.Reset();
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
                // the best agent is the one with the most frags and the longest lifespan
                else if ((a.team == team) && ((a.frags > best.frags) || (a.frags == best.frags && a.lifespan > best.lifespan)) )
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
