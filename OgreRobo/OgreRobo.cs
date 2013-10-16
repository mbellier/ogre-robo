
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
            mSceneMgr.AmbientLight = new ColourValue(1, 1, 1);

            Entity ent = mSceneMgr.CreateEntity("Head", "ogrehead.mesh");

            SceneNode node = mSceneMgr.RootSceneNode.CreateChildSceneNode("HeadNode");

            node.AttachObject(ent);
        }

    }
}