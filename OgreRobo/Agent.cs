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

            AnimationState anim = a.entity.GetAnimationState("Walk");
            anim.Loop = true;
            anim.Enabled = true;

            environment.agentList.Add(a);

            a.RandomPosition();

            return a;
        }

        public Agent spawnRobot(int team = 0)
        {
           Agent a = spawnAgent(team, sceneMgr.CreateEntity( "robot.mesh" ));
           a.node.SetScale(new Vector3(1f, 1f, 1f) * 2.2f);
           return a;
        }

        public Agent spawnOgre(int team = 1)
        {
            Agent a = spawnAgent(team, sceneMgr.CreateEntity( "ninja.mesh" ));
            a.meshOrientation = new Quaternion(-Mogre.Math.PI / 2, new Vector3(0, 1, 0));
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
        public Vector3 destination;
        public Agent myTarget;
        public List<Agent> attackers;

        public int health { get; set; }
        public int maxHealth { get; set; }
        public int damages { get; set; }

        public Agent()
        {
            this.maxHealth = 100;
            this.health = maxHealth;
            this.damages = 10;
            this.speed = 150f;
            this.positionTolerance = 10;
            this.targetRadius = 100;
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


        public Quaternion GetOrientation()
        {
            return entity.ParentNode.Orientation;
        }

        public void SetOrientation(Quaternion orientation)
        {
            entity.ParentNode.Orientation = orientation;
        }



        public void GoTo(Vector3 destination)
        {
			//TODO
            //Rectangle r = environment.mapDomain;
            

            //if (new Vector2(destination.x, destination.y).


            this.destination = destination;
        }

        public void LookAt(Quaternion orientation)
        {
            entity.ParentNode.Rotate(orientation);
           
                entity.ParentNode.Rotate(meshOrientation);
        }

        public bool Move(float dt)
        {
            Vector3 pos = GetPosition();
            if (!pos.PositionEquals(destination, positionTolerance ))
            {
                //move
                
                //position
                Vector3 movement;
                movement = (destination - pos);
                movement = movement / movement.Normalise();
                pos += movement * dt * speed;
                SetPosition(pos);

                //orientation
                Vector3 src = GetOrientation() * Vector3.UNIT_X;
                src /= movement.Normalise();
                Quaternion quat = src.GetRotationTo(movement);
                LookAt(quat);

                return true;
            }
            else
            {
                // didn't move
                SetPosition(destination);
                return false;
            }
           
        }

        public void target(Agent target)
        {
            myTarget = target;
            myTarget.attackers.Add(this);
        }

        public void die()
        {
            if (myTarget != null)
            {
                myTarget.attackers.Remove(this);
                myTarget = null;
            }

            foreach (Agent a in attackers)
            {
                a.myTarget = null;
            }
            attackers.Clear();

            RandomPosition();
            environment.deadList.Add(this);
            scene.RootSceneNode.RemoveChild(node);
        }

        public void Update(float dt)
        {
            // Behaviour
            if (myTarget != null)
            {
                GoTo(myTarget.GetPosition());
            }
            else
            {
                List<Agent> list = new List<Agent>();

                foreach (Agent a in environment.agentList)
                {
                    if (GetPosition().PositionEquals(a.GetPosition(), 100) && a.team != this.team)
                    {
                        list.Add(a);
                    }
                }

                if (list.Count > 0)
                {
                    target(list.ElementAt(environment.rnd.Next(list.Count)));
                }
            }

            // update
            AnimationState anim = entity.GetAnimationState("Walk");
            anim.AddTime(dt);
            
            if (!Move(dt))
            {
                if (myTarget != null)
                {
                    myTarget.die();
                }
                GoTo(environment.GetRandomPosition());
            }
        }
    }
}
 