using OpenGL_Game.Managers;

namespace OpenGL_Game.Components
{
    class ComponentTexture : IComponent
    {
        int texture;

        public ComponentTexture(string textureName)
        {
            texture = ResourceManager.LoadTexture(textureName);
        }

        public int Texture
        {
            get { return texture; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TEXTURE; }
        }
    }
}
