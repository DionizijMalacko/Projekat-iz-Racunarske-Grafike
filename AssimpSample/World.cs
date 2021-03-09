// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 5f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation =0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 5000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;


        //teksutre 
        private enum TextureObjects { Asphalt = 0, Grass };
        private readonly int m_textureCount = 2;
        private string[] m_textureFiles = { //ucitaj slike sa teksutrama
            ".//Textures//beton.jpg",
            ".//Textures//trava.jpg"
        };
        private uint[] m_textures = null;

        //slajderi
        private float scale = 1, animataionSpeed = 1, airstripSize = 4000;

        //animacija
        private bool animation = false;
        private DispatcherTimer timer;//tajmer
        private float planeY = 0f;//visina aviona
        private float planeX = 0f;//pozicija na pisti
        private float rotation = 0f;//rotacija aviona kad polece

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { if(value>=5&&value<=85) m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set {if(value>350) m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        //brzina animacije
        public float AnimationSpeed {
            get { return animataionSpeed; }
            set { animataionSpeed = value; }
        }

        //velicina piste
        public float AirstripSize {
            get { return airstripSize; }
            set { airstripSize = value; }
        }

        //velicina piste
        public float Scale {
            get { return scale; }
            set { scale = value; }
        }


        //da li je animaijua u toku
        public bool Animation {
            set { animation = value; }
            get { return animation; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            //gl.Color(1f, 0f, 0f);
            gl.Enable(OpenGL.GL_CULL_FACE); //sakrivanje nevidljivih pofvrsina
            gl.Enable(OpenGL.GL_DEPTH_TEST); //testiranje dubine 
            m_scene.LoadScene();
            m_scene.Initialize();


            //color tracking
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            InitializeTextures(gl);
            InitializeLighting(gl);
        }

        private void InitializeLighting(OpenGL gl) {//za svetla
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);//stacionarno (belo)
            gl.Enable(OpenGL.GL_LIGHT1);//reflektor (plavo)

            float[] white = new float[] { 1f, 1f, 1f, 1.0f }; //bela boja
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);//tackassti izvor (180 stepeni)
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, white);//sve komeponente su bele
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, white);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, white);
            float[] blue = new float[] { 0f, 0f, 1f, 1.0f }; //plava boja

            //reflektor
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30F);//cutoff: 30
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, blue);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, blue);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, blue);

        }
        private void InitializeTextures(OpenGL gl) {
            m_textures = new uint[m_textureCount];
            gl.Enable(OpenGL.GL_TEXTURE_2D); //omoguci uotrebu tekstura
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_REPLACE); //stapanje : GL_REPLACE
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i) {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                System.Drawing.Imaging.BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA8, imageData.Width, imageData.Height, 0,
                            OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST); //filtriranje: najblizi sused
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT); //wrap repeat
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                image.UnlockBits(imageData);
                image.Dispose();
            }

        }
        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();
            gl.LoadIdentity();

            DrawText(gl); //tekst  

            float[] pos = { 3000, 2000, 0, 1.0f };//poziciaja svetla 
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pos);//pre rotacije se definise, jr rotacije ne treba da uticu an njega
            gl.LookAt(-m_sceneDistance, 0f, 0, 0f, 0f, 0f, 0f, 1f, 0f);

            gl.Rotate(m_xRotation, 0.0f, 0.0f, 1.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, new float[] { 0.0f, 0.0f, -1.0f });//baca vetlo u negativnom smeru z-ose
            float[] pos2 = { -3000, 200f, 800, 1.0f };//desno od aviona
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, pos2);//REFLEKTOR, POZICIJA
            //podloga
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]); 
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(30,30, 30);//Skaliranje teksuire
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0, 1, 0);
            gl.Color(0.15f, 0.5f, 0.45f);
            gl.TexCoord(0f, 0f);
            gl.Vertex(-4000f, 0f, 2000f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(4000f, 0f, 2000f);
            gl.TexCoord(1f, 1f);
            gl.Vertex(4000f, 0f, -2000f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(-4000f, 0f, -2000f);
            gl.End();
            //----------------
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Scale(18, 18, 18);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //pista            

            gl.Color(0.75f, 0.75f, 0.75f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0, 1, 0);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.TexCoord(0f, 0f);
            gl.Vertex(-4000f, 5f, 500f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(airstripSize, 5f,500f);
            gl.TexCoord(1f, 1f);
            gl.Vertex(airstripSize, 5f, -500f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(-4000f, 5f, -500f);
            gl.End();
            //----------------
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //svetlosni znakovi

            lightPost(gl, -2000, 550);
            lightPost(gl, -2000, -550);
            lightPost(gl, 2000, 550);
            lightPost(gl, 2000, -550);
            lightPost(gl, 0, 550);
            lightPost(gl, 0, -550);

            gl.PushMatrix();// 
            gl.Translate(-3000+planeX, planeY, 0);
            gl.Rotate(0, 90, 0);

            gl.Scale(150*scale, 150*scale, 150*scale);
            gl.PushMatrix();
            gl.Rotate(-rotation, 0, 0);
            //POSTO SE KOD REPLACE REZIMA ZA TEKSTURE NE KORISTI SVETLO ONDA ZA AVION POSTAVLJAM REZIM NA MODULATE DA BI SE VIDEO REFLEKTOR LEPO...
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            m_scene.Draw();
            //ZA OSTALO IDE NA REPLACE JER TAKO SPECIFIKACIJA KAZE
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_REPLACE);
            gl.PopMatrix();
            gl.PopMatrix();

            gl.PopMatrix();

            gl.Flush();
        }
        public void startAnimation() {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(MovePlane);
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Start();
            animation = true;
            MainWindow mw=(MainWindow)System.Windows.Application.Current.MainWindow;
            mw.scale.IsEnabled = mw.animation.IsEnabled = mw.airstrip.IsEnabled =mw.c1.IsEnabled=mw.c2.IsEnabled= false;
        }

        private void MovePlane(object sender, EventArgs e) {
           if (planeX < 6200) {
                planeX += animataionSpeed * 50; //povecavam brzinu aviona
                if (planeX > 2200) { //kada dodje do 2200 uzlece
                    planeY += 15 * animataionSpeed;
                    if(rotation<25)
                       rotation += 0.5f * animataionSpeed;
                    planeX += animataionSpeed * 10;
                }
            }
            else {
                timer.Stop();
                animation = false;
                planeX = planeY = rotation = 0;
                MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;
                mw.scale.IsEnabled = mw.animation.IsEnabled = mw.airstrip.IsEnabled = mw.c1.IsEnabled = mw.c2.IsEnabled = true;

            }

        }

        private void lightPost(OpenGL gl, int v1, int v2) {
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_EMISSION, new float[] { 1f, 1f, 0f }); //ukljucivanje emisione komponente za svetla na pisti
            gl.PushMatrix();
            gl.Translate(v1, 0, v2);
            gl.PushMatrix();
            gl.Color(1.0f,1.0f, 0.0f);
            Cylinder bar = new Cylinder();
            bar.TopRadius = 10;
            bar.BaseRadius = 10;
            bar.Height = 80;
            bar.CreateInContext(gl);
            gl.Rotate(-90f, 0f, 0f);
            bar.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0, 100, 0);
            gl.Scale(50, 50f,50f);
            Sphere sp = new Sphere();
            sp.NormalOrientation=Orientation.Outside;
            sp.CreateInContext(gl);
            sp.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
            gl.PopMatrix();
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_EMISSION, new float[] { 0f, 0f, 0f });

        }

        private void DrawText(OpenGL gl) {
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Color(1f, 0f, 0f);//boja teksta
            String[] lines = new String[] { "Predmet: Racunarska grafika", "Sk. god: 2020/21" , "Ime: Dionizij", "Prezime: Malacko", "Sifra zad: 5.1" };
            String[] underlines = new String[] { "_______________________", "______________", "__________", "______________", "___________" };

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Viewport(2*m_width / 3, 0, m_width / 3, m_height / 3); //viewport se podesi na donji desni ugao

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();
            gl.Translate(-1, 0, 0);//pomeri se ceo tekst malo levo da bi se sve videlo
            gl.Scale(0.15, 0.15, 0.15);//smanji se posto bude ogroman
            for (int i=0; i < lines.Length; i++) {//ispisuje se red op red i podvlaci
                gl.PushMatrix();
                gl.Translate(0, -i-1, 0);
                gl.DrawText3D("Verdana", 10f, 1f, 0.1f,lines[i]);
                gl.PopMatrix();
                gl.PushMatrix();
                gl.Translate(0, -1-i, 0);
                gl.DrawText3D("Verdana", 10f, 1f, 0.1f, underlines[i]);
                gl.PopMatrix();
            }
            gl.PopMatrix();


            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.Viewport(0, 0, m_width, m_height);//vrati se viewport na ceo ekran da bi se ostale stvari iscrtavale na celom ekranu

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.FrontFace(OpenGL.GL_CCW);//3d tekst ovo promeni,pa vratimo na staro
            gl.Enable(OpenGL.GL_LIGHTING);

        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, 0, m_width, m_height);//viewport preko celog ekrana
            gl.Perspective(45f, (double)width / height, 0.5f, 100000f); //projekcija u perspektivi
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
