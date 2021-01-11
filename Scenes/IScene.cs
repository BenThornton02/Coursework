using OpenTK;

namespace OpenGL_Game.Scenes
{
    public enum SceneTypes
    {
        SCENE_NONE,
        SCENE_MAIN_MENU,
        SCENE_GAME,
        SCENE_GAME_OVER
    }

    interface IScene
    {
        void Render(FrameEventArgs e);
        void Update(FrameEventArgs e);
        void Close();
    }
}
