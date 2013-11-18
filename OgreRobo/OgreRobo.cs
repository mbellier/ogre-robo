
using Mogre;
using Mogre.TutorialFramework; // provides a simple 'BaseApplication' class
using System;

namespace OgreRobo
{
    class OgreRobo : BaseApplication
    {
        Entity ninja;
        AnimationState anim;

        public int xmin = -1000, xmax = 1000, zmin = -1000, zmax = 1000;

        AgentEnvironment agentEnvironment;
        AgentFactory agentFactory;

        public static void Main()
        {
            new OgreRobo().Go();
        }

        protected override void CreateScene()
        {
            mRoot.FrameRenderingQueued += new FrameListener.FrameRenderingQueuedHandler(FrameRenderingQueued);

            // scene configuration //////////
    
            // setting lighting
            mSceneMgr.AmbientLight = ColourValue.White;
            mSceneMgr.ShadowTechnique = ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE;

            // setting sky box
            mSceneMgr.SetSkyDome(true, "Examples/CloudySky", 5, 4);

            // managing entities ///////////

            // adding new entity
            ninja = mSceneMgr.CreateEntity("ninja", "robot.mesh");
            ninja.CastShadows = true;
            mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(ninja);

            anim = ninja.GetAnimationState("Walk");
            anim.Loop = true;
            anim.Enabled = true;

            // ground
            Plane plane = new Plane(Vector3.UNIT_Y, 0);
            MeshManager.Singleton.CreatePlane("ground",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, plane,
                xmax-xmin, zmax-zmin, 20, 20, true, 1, 5, 5, Vector3.UNIT_Z);
            Entity groundEnt = mSceneMgr.CreateEntity("GroundEntity", "ground");
            mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(groundEnt);
            groundEnt.SetMaterialName("Examples/Rockwall");
            groundEnt.CastShadows = false;
 

            // Agents //////////////////
            agentEnvironment = new AgentEnvironment (-1000,1000,-1000,1000);
            agentFactory = new AgentFactory(mSceneMgr, agentEnvironment);
            agentFactory.spawnRobot();
            agentFactory.spawnOgre();
            agentFactory.spawnRobotsAndOgres(4);
        }



        bool FrameRenderingQueued(FrameEvent evt)
        {
            agentEnvironment.Update(evt.timeSinceLastFrame);
            anim.AddTime(evt.timeSinceLastFrame);

           
            this.ninja.ParentNode.Position += new Vector3(.1f,0,0);

            return true;
        }

        protected override void ChooseSceneManager()
        {
            mSceneMgr = mRoot.CreateSceneManager(SceneType.ST_EXTERIOR_CLOSE);
        }

        protected override void CreateCamera()
        {
            mCamera = mSceneMgr.CreateCamera("PlayerCam");
            mCamera.Position = new Vector3(0, 750, 2000);
            mCamera.LookAt(Vector3.ZERO);
            mCamera.NearClipDistance = 5;
            mCameraMan = new CameraMan(mCamera);
        }

        protected override void CreateViewports()
        {
            Viewport viewport = mWindow.AddViewport(mCamera);
            viewport.BackgroundColour = ColourValue.Black;
            mCamera.AspectRatio = (float)viewport.ActualWidth / viewport.ActualHeight;
        }
    }
}