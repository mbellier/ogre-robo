using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace OgreRobo
{
    class AgentFactory
    {
        public SceneManager sceneMgr { get; set; }
        public AgentEnvironment environment { get; set; }
        public string[] ninjaAttack = new string[] { "Attack1", "Attack2", "Attack3", "SideKick", "Kick" };
        public string[] ninjaDeath = new string[] { "Death1", "Death2" };


        public AgentFactory(SceneManager sceneMgr, AgentEnvironment e)
        {
            this.sceneMgr = sceneMgr;
            this.environment = e;
        }

        public Agent spawnAgent(int team, Entity entity)
        {
            Agent a = new Agent();

            a.team = team;
            a.environment = environment;
            a.destination = environment.GetRandomPosition();
            
            a.entity = entity;
            a.entity.CastShadows = true;
            a.scene = sceneMgr;
            a.node = sceneMgr.RootSceneNode.CreateChildSceneNode();
            a.node.AttachObject(a.entity);

            a.walk = a.entity.GetAnimationState("Walk");
            a.walk.Loop = true;
            a.walk.Enabled = true;
            a.attack = a.entity.GetAnimationState(getAttack(team));
            a.attack.Loop = true;
            a.death = a.entity.GetAnimationState(getDeath(team));
            a.death.Loop = false;

            environment.agentList.Add(a);

            a.RandomPosition();

            return a;
        }

        public Agent spawnRobot(int team = 0)
        {
           Agent a = spawnAgent(team, sceneMgr.CreateEntity( "robot.mesh" ));
           a.node.SetScale(new Vector3(1f, 1f, 1f) * 2.2f);
           environment.nbRobot++;
           return a;
        }

        public Agent spawnOgre(int team = 1)
        {
            Entity e1 = sceneMgr.CreateEntity("ninja.mesh");
            Agent a = spawnAgent(team, e1);

           // Entity e2 = sceneMgr.CreateEntity("ogrehead.mesh");
           // a.node.CreateChildSceneNode(new Vector3(0, 180, -20), new Quaternion(Mogre.Math.PI, new Vector3(0, 1, 0))).AttachObject(e2);

            a.meshOrientation = new Quaternion(-Mogre.Math.PI / 2, new Vector3(0, 1, 0));
            environment.nbNinja++;
            return a;
        }

        public void spawnRobotsAndOgres(int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                spawnOgre();
                spawnRobot();
            }
        }



        public string getAttack(int team)
        {
            if (team == 0)
            {
                return "Shoot";
            }
            else
            {
                return ninjaAttack[environment.rnd.Next() % ninjaAttack.Length];
            }
        }

        public string getDeath(int team)
        {
            if (team == 0)
            {
                return "Die";
            }
            else
            {
                return ninjaDeath[environment.rnd.Next() % ninjaDeath.Length];
            }
        }
    }


    class Agent
    {
       
        public int team {get; set;}
        public AgentEnvironment environment { get; set; }
        public Entity entity { get; set; }
        public SceneManager scene { get; set; }
        public SceneNode node { get; set; }
        public Quaternion meshOrientation { get; set; }

        public float positionTolerance { get; set; }
        public float targetRadius { get; set; }
        public float speed { get; set; }
        public Vector3 destination { get; set; }
        public Agent myTarget { get; set; }
        public List<Agent> attackers { get; set; }

        public AnimationState walk { get; set; }
        public AnimationState attack { get; set; }
        public AnimationState death { get; set; }

        public float health { get; set; }
        public float maxHealth { get; set; }
        public float damages { get; set; }
        public uint lifespan { get; set; }
        public bool dying { get; set; }
        public bool dead { get; set; }
        public int frags { get; set; }

        public Agent()
        {
            this.maxHealth = 100;
            this.damages = 10;
            this.health = this.maxHealth;
            this.speed = 150f;
            this.positionTolerance = 100;
            this.targetRadius = 100;
            this.dying = false;
            this.dead = false;
            this.lifespan = 0;
            this.frags = 0;
            meshOrientation = new Quaternion(0, new Vector3(0, 0, 0));
            attackers = new List<Agent>();
        }

        public Vector3 GetPosition () {
            return entity.ParentNode.Position;
        }
        public void SetPosition(float x, float y, float z)
        {
            entity.ParentNode.SetPosition(x, y, z);
        }
        public void SetPosition (Vector3 position) {
            entity.ParentNode.Position = position;
        }

        public void RandomPosition()
        {
            SetPosition(environment.GetRandomPosition());
        }

        public void GoTo(Vector3 destination)
        {
            if (environment.IsValidPosition(destination))
                this.destination = destination;
        }

        public Quaternion GetOrientation()
        {
            return entity.ParentNode.Orientation;
        }

        public void SetOrientation(Quaternion orientation)
        {
            entity.ParentNode.Orientation = orientation;
        }

        public void LookAt(Quaternion orientation)
        {
            entity.ParentNode.Rotate(orientation);
            entity.ParentNode.Rotate(meshOrientation);
        }

        public bool Move(float dt)
        {
            Vector3 pos = GetPosition();
            if (!pos.PositionEquals(destination, positionTolerance))
            {
                //position
                Vector3 movement;
                movement = (destination - pos);
                movement.Normalise();
                pos += movement * dt * speed;
                SetPosition(pos);

                //orientation
                Vector3 src = GetOrientation() * Vector3.UNIT_X;
                src /= movement.Normalise();
                Quaternion quat = src.GetRotationTo(movement);
                LookAt(quat);

                //animation
                attack.Enabled = false;
                walk.Enabled = true;
                walk.AddTime(dt);
                return true;
            }
            else
            {
                walk.Enabled = false;
                attack.Enabled = true;
                attack.AddTime(dt);
                return false;
            }

        }

        public void target(Agent target)
        {
            untarget();
            myTarget = target;
            myTarget.attackers.Add(this);
            GoTo(myTarget.GetPosition());
        }

        public void untarget()
        {
            if (myTarget != null)
            {
                myTarget.attackers.Remove(this);
                myTarget = null;
            }
        }

        public void die()
        {
            dying = true;

            untarget();
            foreach (Agent a in attackers)
            {
                a.myTarget = null;
            }
            attackers.Clear();

            walk.Enabled = false;
            attack.Enabled = false;
            death.Enabled = true;

            lifespan = environment.roundStart.Milliseconds;
        }

        public void takeDamage(float damage)
        {
            health -= damage;
            if (health < 0)
            {
                die();
            }
        }

        public void hit(Agent target,  float dt)
        {
            target.takeDamage(damages * dt);
            target.target(this);
        }

        public void Update(float dt)
        {if (dying)
            {
                death.AddTime(dt);
                if (death.HasEnded)
                {
                    dead = true;

                    if (team == 0)
                    {
                        environment.nbRobot--;
                    }
                    else
                    {
                        environment.nbNinja--;
                    }
                }
                return;
            }

            // Behaviour
            if (myTarget != null)
            {
                GoTo(myTarget.GetPosition());
                bool move = Move(dt);

                if (!move)
                {
                    hit(myTarget, dt);
                }

                if (myTarget == null)
                {
                    frags++;
                }
                else
                {
                    return;   
                }
            }
            else
            {
                Agent possibleTarget = null;
                float squareDist = 225 * positionTolerance * positionTolerance;
                foreach (Agent a in environment.agentList)
                {
                    float tmpDist = (GetPosition() - a.GetPosition()).SquaredLength;
                    if (tmpDist < squareDist && a.team != this.team && a.dying == false)
                    {
                        possibleTarget = a;
                        squareDist = tmpDist;
                    }
                }

                if (possibleTarget != null)
                {
                    target(possibleTarget);
                }

                bool move = Move(dt);
                if (!move)
                {
                    GoTo(environment.GetRandomPosition());
                }
            }
        }
    }
}

 