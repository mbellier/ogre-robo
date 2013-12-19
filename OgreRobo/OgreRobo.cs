
using Mogre;
using Mogre.TutorialFramework; // provides a simple 'BaseApplication' class
using System;

namespace OgreRobo
{
    class OgreRobo : BaseApplication
    {
        Rect mapDomain = new Rect (-2000, 2000, 2000, -2000);

        AgentEnvironment agentEnvironment;

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
            mSceneMgr.SetSkyDome(true, "Examples/SpaceSkyPlane", 5, 2); // CloudySky
            

            // ground

            mSceneMgr.SetWorldGeometry("terrain.cfg");
            int terrainSize = 25000;
            mSceneMgr.GetSceneNode("Terrain").SetPosition(-terrainSize / 2, -1, -terrainSize/2);
    

            
            Plane plane = new Plane(Vector3.UNIT_Y, 0);
            MeshManager.Singleton.CreatePlane("ground",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, plane,
                10000, 10000, 20, 20, true, 1, 5, 5, Vector3.UNIT_Z);
            Entity groundEnt = mSceneMgr.CreateEntity("GroundEntity", "ground");
            mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(groundEnt);
            groundEnt.SetMaterialName("Custom/Dirt");
            groundEnt.CastShadows = false;
            
 
            // fog
            //ColourValue fadeColour = new ColourValue(0.09f, 0.09f, 0.09f);
            //fadeColour = new ColourValue(0.9f, 0.9f, 0.9f);
            //mWindow.GetViewport(0).BackgroundColour = fadeColour;
            
            //mSceneMgr.SetFog(FogMode.FOG_EXP2, fadeColour, 0.00025f);

            //fog test
            
            //fadeColour = new ColourValue(0.1f, 0.1f, 0.1f);
            //mWindow.GetViewport(0).BackgroundColour = fadeColour;
            //mSceneMgr.SetFog(FogMode.FOG_LINEAR, fadeColour, 0, 10, 150);
            //mSceneMgr.SetFog(FogMode.FOG_EXP2, fadeColour, 0.00025f);
        
            

            //Plane plane;
            //plane.d = 10000;
            //plane.normal = Vector3.NEGATIVE_UNIT_Y;

            // light

            Light pointLight = mSceneMgr.CreateLight("pointLight");
            pointLight.Type = Light.LightTypes.LT_POINT;
            pointLight.Position = new Vector3(0, 2000, 0);


            // fire
            /*
            ParticleSystem fireParticle = mSceneMgr.CreateParticleSystem("Fire", "Particles/Fire");
            SceneNode particleNode = mSceneMgr.RootSceneNode.CreateChildSceneNode("Particle");
            particleNode.AttachObject(fireParticle);

             * 
             */
             
            // Agents //////////////////
            agentEnvironment = new AgentEnvironment(mapDomain, mSceneMgr);
            agentEnvironment.newRound();
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
            mCameraMan.FastMove = true;
        }

        protected override void CreateViewports()
        {
            Viewport viewport = mWindow.AddViewport(mCamera);
            viewport.BackgroundColour = ColourValue.Black;
            mCamera.AspectRatio = (float)viewport.ActualWidth / viewport.ActualHeight;
        }
    }
}