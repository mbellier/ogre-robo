using System;
using Mogre;
using Mogre.TutorialFramework; // provides a simple 'BaseApplication' class


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
            mSceneMgr.SetSkyDome(true, "Examples/SpaceSkyPlane",10, 4); 
            

            // ground

            mSceneMgr.SetWorldGeometry("terrain.cfg");
            int terrainSize = 25000;
            mSceneMgr.GetSceneNode("Terrain").SetPosition(-terrainSize / 2, -5, -terrainSize/2);
    

            
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



            // light

            /*
            Light pointLight = mSceneMgr.CreateLight("pointLight");
            pointLight.Type = Light.LightTypes.LT_POINT;
            pointLight.Position = new Vector3(0, 2000, 0);
            */

            // fire

            float x = 0, y = 0;
            ParticleSystem fireParticle = mSceneMgr.CreateParticleSystem("Fire", "CustomSmoke");//"TRPlayer/Torch");
            SceneNode particleNode = mSceneMgr.RootSceneNode.CreateChildSceneNode("Particle");
            particleNode.SetPosition(x, 100, y);
            particleNode.AttachObject(fireParticle);
            Light pointLight2 = mSceneMgr.CreateLight("test");
            pointLight2.Type = Light.LightTypes.LT_POINT;
            pointLight2.Position = new Vector3(x, 500, y);
            //pointLight2.DiffuseColour = ColourValue.White;
            //pointLight2.SetAttenuation(5000,1, 1, 1);
            //pointLight2.SetAttenuation(200000, 1, 0, 0);

            Entity e = mSceneMgr.CreateEntity("Cylinder.mesh");
            e.SetMaterialName("WoodPallet");
            SceneNode n = mSceneMgr.RootSceneNode.CreateChildSceneNode();           
            n.AttachObject(e);
            n.SetPosition(x, 45, y);
            n.Scale(90, 10, 10);
            n.Rotate(new Vector3(0, 0, 1), Mogre.Math.PI / 2);


            // text
            TextAreaOverlayElement text = createTextOverlay();
            text.Caption = "test";
             
            // Agents //////////////////
            agentEnvironment = new AgentEnvironment(mapDomain, mSceneMgr, text);
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
            mCamera.Position = new Vector3(0, 780, 3500);
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

        public TextAreaOverlayElement createTextOverlay()
        {
            // Name the font we want to use
            string fontFileName = @"bluehigh.ttf";

            // Create the font resource
            ResourceGroupManager.Singleton.AddResourceLocation("Media/fonts", "FileSystem");
            ResourcePtr font = FontManager.Singleton.Create("MyFont", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            font.SetParameter("type", "truetype");
            font.SetParameter("source", fontFileName);
            font.SetParameter("size", "30");
            font.SetParameter("resolution", "96");
            font.Load();

            // Create the overlay panel
            OverlayContainer panel = OverlayManager.Singleton.CreateOverlayElement("Panel", "OverlayPanelName") as OverlayContainer;
            panel.MetricsMode = GuiMetricsMode.GMM_PIXELS;
            panel.SetPosition(10, 10);
            panel.SetDimensions(300, 300);

            // Create the text area
            TextAreaOverlayElement textArea = OverlayManager.Singleton.CreateOverlayElement("TextArea", "TextAreaName") as TextAreaOverlayElement;
            textArea.MetricsMode = GuiMetricsMode.GMM_PIXELS;
            textArea.SetPosition(0, 0);
            textArea.SetDimensions(300, 120);
            textArea.CharHeight = 30;
            textArea.FontName = "MyFont";
            textArea.Caption = "";
            textArea.ColourBottom = ColourValue.White;
            textArea.ColourTop = ColourValue.White;
            // Create the Overlay to link all
            Overlay textOverlay = OverlayManager.Singleton.Create("The Text Overlay");
            textOverlay.Add2D(panel);
            panel.AddChild(textArea);

            textOverlay.Show();

            return textArea;
        }
    }
}