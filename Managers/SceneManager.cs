using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Scenes;

namespace OpenGL_Game.Managers
{
    class SceneManager : GameWindow
    {
        Scene scene;
        public static int width = 1600, height = 900;
        public static int windowXPos = 100, windowYPos = 50;

        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;

        public delegate void KeyboardDelegate(KeyboardKeyEventArgs e);
        public KeyboardDelegate keyboardDownDelegate;
        public KeyboardDelegate keyboardUpDelegate;

        public delegate void MouseDelegate(MouseButtonEventArgs e);
        public MouseDelegate mouseDelegate;


        public SceneManager() : base(width, height, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16))
        {
            this.X = windowXPos;
            this.Y = windowYPos;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape) Exit();
            if (keyboardDownDelegate != null) keyboardDownDelegate.Invoke(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (keyboardUpDelegate != null) keyboardUpDelegate.Invoke(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if(mouseDelegate != null) mouseDelegate.Invoke(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            //Load the GUI
            GUI.SetUpGUI(width, height);

            ChangeScene(SceneTypes.SCENE_MAIN_MENU);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            updater(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            renderer(e);

            GL.Flush();
            SwapBuffers();
        }

        /*
        public void StartNewGame()
        {
            if(scene != null) scene.Close();
            scene = new GameScene(this);
        }

        public void StartMenu()
        {
            if (scene != null) scene.Close();
            scene = new MainMenuScene(this);
        }*/

        public void ChangeScene(SceneTypes sceneType)
        {
            if (scene != null) scene.Close();
            switch(sceneType)
            {
                case SceneTypes.SCENE_GAME:
                    scene = new GameScene(this);
                    break;

                case SceneTypes.SCENE_MAIN_MENU:
                    scene = new MainMenuScene(this);
                    break;

                case SceneTypes.SCENE_GAME_OVER:
                    scene = new GameOverScene(this);
                    break;
            }
        }

        public static int WindowWidth
        {
            get { return width; }
        }

        public static int WindowHeight
        {
            get { return height; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            SceneManager.width = Width;
            SceneManager.height = Height;

            //Load the GUI
            GUI.SetUpGUI(Width, Height);
        }
    }

}

