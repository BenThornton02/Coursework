using OpenTK.Audio.OpenAL;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Components
{
    class ComponentAudio : IComponent
    {
        int audio;

        public ComponentAudio(string audioName)
        {
            audio = ResourceManager.LoadAudio(audioName);
        }

        public int Audio
        {
            get { return audio; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }
    }
}