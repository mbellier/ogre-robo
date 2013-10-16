
using Mogre;
using Mogre.TutorialFramework; // provides a simple 'BaseApplication' class
using System;

namespace OgreRobo
{
    class OgreRobo : BaseApplication
    {
        public static void Main()
        {
            new OgreRobo().Go();
        }

        protected override void CreateScene()
        {
            // scene configuration //////////

            // setting terrain
            mSceneMgr.SetWorldGeometry("terrain.cfg");

            // setting lighting
            mSceneMgr.AmbientLight = ColourValue.Black;
            mSceneMgr.ShadowTechnique = ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE;

            // setting sky box
            mSceneMgr.SetSkyDome(true, "Examples/CloudySky", 5, 4);


            // managing entities ///////////

            // adding new entity
            Entity ent = mSceneMgr.CreateEntity("Head", "ogrehead.mesh"); // entity with unique name
            SceneNode node = mSceneMgr.RootSceneNode.CreateChildSceneNode("HeadNode"); // node with unique name, as a child of the root node
            node.AttachObject(ent); // attach the entity to the node

            // adding new entity
            Entity ent2 = mSceneMgr.CreateEntity("Head2", "ogrehead.mesh");
            SceneNode node2 = mSceneMgr.RootSceneNode.CreateChildSceneNode("HeadNode2", new Vector3(100, 0, 0)); // translation of x=100
            node2.AttachObject(ent2);


            // adding new entity
            Entity ent3 = mSceneMgr.CreateEntity("ninja", "ninja.mesh");
            ent3.CastShadows = true;
            mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(ent3);

            // adding new plane
            Plane plane = new Plane(Vector3.UNIT_Y, 0);

            MeshManager.Singleton.CreatePlane("ground",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, plane,
                1500, 1500, 20, 20, true, 1, 5, 5, Vector3.UNIT_Z);

            Entity groundEnt = mSceneMgr.CreateEntity("GroundEntity", "ground");
            mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(groundEnt);

            groundEnt.SetMaterialName("Examples/Rockwall");
            groundEnt.CastShadows = false;


            // managing lights /////////////

            // adding new light
            Light pointLight = mSceneMgr.CreateLight("pointLight");
            pointLight.Type = Light.LightTypes.LT_POINT;
            pointLight.Position = new Vector3(0, 150, 250);
            pointLight.DiffuseColour = ColourValue.Red;
            pointLight.SpecularColour = ColourValue.Red;

            // adding new directional light
            Light directionalLight = mSceneMgr.CreateLight("directionalLight");
            directionalLight.Type = Light.LightTypes.LT_DIRECTIONAL;
            directionalLight.DiffuseColour = new ColourValue(.25f, .25f, 0);
            directionalLight.SpecularColour = new ColourValue(.25f, .25f, 0);
            directionalLight.Direction = new Vector3(0, -1, 1);

            // adding new spot light
            Light spotLight = mSceneMgr.CreateLight("spotLight");
            spotLight.Type = Light.LightTypes.LT_SPOTLIGHT;
            spotLight.DiffuseColour = ColourValue.Blue;
            spotLight.SpecularColour = ColourValue.Blue;
            spotLight.Direction = new Vector3(-1, -1, 0);
            spotLight.Position = new Vector3(300, 300, 0);
            spotLight.SetSpotlightRange(new Degree(35), new Degree(50));
        }

        protected override void ChooseSceneManager()
        {
            mSceneMgr = mRoot.CreateSceneManager(SceneType.ST_EXTERIOR_CLOSE);
        }

        protected override void CreateCamera()
        {
            mCamera = mSceneMgr.CreateCamera("PlayerCam");
            mCamera.Position = new Vector3(0, 10, 500);
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