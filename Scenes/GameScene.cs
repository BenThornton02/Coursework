using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using System.Drawing;
using System;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        public static float dt = 0;
        EntityManager entityManager;
        SystemManager systemManager;

        public Camera camera;

        public static GameScene gameInstance;

        bool[] keyPressed = new bool[255];

        Vector3 sourcePosition;
        int audioBuffer;
        int audioSource;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();

            // Set the title of the window
            sceneManager.Title = "Game";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            // Set Keyboard events to go to a method in this class
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;
            sceneManager.keyboardUpDelegate += Keyboard_KeyUp;

            // Enable Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace); //Disable me to test for dodgy models
            GL.CullFace(CullFaceMode.Back);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Set Camera
            camera = new Camera(new Vector3(0, 4, 7), new Vector3(0, 0, 0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);


            // Audio
            audioBuffer = ResourceManager.LoadAudio("Audio/buzz.wav");
            audioSource = AL.GenSource();
            AL.Source(audioSource, ALSourcei.Buffer, audioBuffer);
            AL.Source(audioSource, ALSourceb.Looping, true);
            sourcePosition = new Vector3(10.0f, 0.0f, 0.0f);
            AL.Source(audioSource, ALSource3f.Position, ref sourcePosition);
            AL.SourcePlay(audioSource);


            CreateEntities();
            CreateSystems();

            // TODO: Add your initialization logic here

        }

        private void CreateEntities()
        {
            Entity newEntity;

            newEntity = new Entity("Moon");
            newEntity.AddComponent(new ComponentPosition(-2.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentVelocity(0.0f, +0.2f, +0.2f));
            //newEntity.AddComponent(new ComponentTexture("Textures/Moon/moon_diffuse.jpg"));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Moon/moon.obj"));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Wraith_Raider_Starship");
            newEntity.AddComponent(new ComponentPosition(2.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentVelocity(0.0f, 0.0f, +1.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Wraith_Raider_Starship/Wraith_Raider_Starship.obj"));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Intergalactic_Spaceship");
            newEntity.AddComponent(new ComponentPosition(2.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentVelocity(0.0f, +0.2f, +0.2f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Intergalactic_Spaceship/Intergalactic_Spaceship.obj"));
            entityManager.AddEntity(newEntity);

            newEntity = new Entity("Map");
            newEntity.AddComponent(new ComponentPosition(0.0f, 0.0f, 0.0f));
            newEntity.AddComponent(new ComponentGeometry("Geometry/Map/New Map.obj"));
            entityManager.AddEntity(newEntity);
        }

        // Needs to be moved to resource managers eventually
        /*
        public static int LoadAudio(string filename)
        {
            int audioBuffer;

            audioBuffer = AL.GenBuffer();

            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = LoadWave(
                File.Open(filename, FileMode.Open),
                out channels,
                out bits_per_sample,
                out sample_rate);
            ALFormat sound_format =
                channels == 1 && bits_per_sample == 8 ? ALFormat.Mono8 :
                channels == 1 && bits_per_sample == 16 ? ALFormat.Mono16 :
                channels == 2 && bits_per_sample == 8 ? ALFormat.Stereo8 :
                channels == 2 && bits_per_sample == 16 ? ALFormat.Stereo16 :
                (ALFormat)0;
            AL.BufferData(audioBuffer, sound_format, sound_data, sound_data.Length, sample_rate);
            if (AL.GetError() != ALError.NoError)
            {

            }
            return audioBuffer;
        }

        private static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunk_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified stream is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }*/

        private void CreateSystems()
        {
            ISystem newSystem;

            newSystem = new SystemRender();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemPhysics();
            systemManager.AddSystem(newSystem);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>


        public override void Update(FrameEventArgs e)
        {
            dt = (float)e.Time;
            System.Console.WriteLine("fps=" + (int)(1.0/dt));

            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();

            AL.Listener(ALListener3f.Position, ref camera.cameraPosition);
            AL.Listener(ALListenerfv.Orientation, ref camera.cameraDirection, ref camera.cameraUp);

            // TODO: Add your update logic here


            if (keyPressed[(char)Key.Up])
            {
                camera.MoveForward(0.1f);
            }
            if (keyPressed[(char)Key.Down])
            {
                camera.MoveForward(-0.1f);
            }
            if (keyPressed[(char)Key.Left])
            {
                camera.RotateY(-0.01f);
            }
            if (keyPressed[(char)Key.Right])
            {
                camera.RotateY(0.01f);
            }
            if (keyPressed[(char)Key.M])
            {
                sceneManager.ChangeScene(SceneTypes.SCENE_GAME_OVER);
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Action ALL systems
            systemManager.ActionSystems(entityManager);

            // Render score
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.clearColour = Color.Transparent;
            GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), "Score: 000", 18, StringAlignment.Near, Color.White);
            GUI.Render();
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;
        }

        public void Keyboard_KeyUp(KeyboardKeyEventArgs e)
        {
            keyPressed[(char)e.Key] = false;
        }

        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            keyPressed[(char)e.Key] = true;
        }
    }
}
