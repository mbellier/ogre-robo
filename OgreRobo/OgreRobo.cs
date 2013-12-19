
using Mogre;
using Mogre.TutorialFramework; // provides a simple 'BaseApplication' class
using System;

namespace OgreRobo
{
    class OgreRobo : BaseApplication
    {
        public int initNbAgents = 30;
        Rect mapDomain = new Rect (-2000, 2000, 2000, -2000);

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

            // ground
            Plane plane = new Plane(Vector3.UNIT_Y, 0);
            MeshManager.Singleton.CreatePlane("ground",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, plane,
                 System.Math.Abs(mapDomain.right - mapDomain.left), System.Math.Abs(mapDomain.top - mapDomain.bottom), 20, 20, true, 1, 5, 5, Vector3.UNIT_Z);
            Entity groundEnt = mSceneMgr.CreateEntity("GroundEntity", "ground");
            mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(groundEnt);
            groundEnt.SetMaterialName("Examples/Rockwall");
            groundEnt.CastShadows = false;
 

            // Agents //////////////////
            agentEnvironment = new AgentEnvironment (mapDomain);
            agentFactory = new AgentFactory(mSceneMgr, agentEnvironment);
            agentFactory.spawnRobot();
            agentFactory.spawnOgre();
            agentFactory.spawnRobotsAndOgres(initNbAgents);
        }



        bool FrameRenderingQueued(FrameEvent evt)
        {
            agentEnvironment.Update(evt.timeSinceLastFrame);
            
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