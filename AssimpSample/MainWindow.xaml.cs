using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow() {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try {
                m_world = new World(".\\3D Models", "uploads_files_800322_Hawk_T2.obj", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e) {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args) {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args) {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args) {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        //handleri za slajdere
        private void airstrip_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.AirstripSize = (float)e.NewValue;

        }

        private void animation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.AnimationSpeed = (float)e.NewValue;
        }

        private void scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (m_world != null)
                m_world.Scale = (float)e.NewValue;
        }
        //Kraj slajdera

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (!m_world.Animation)
                switch (e.Key) {
                    case Key.F2: this.Close(); break;
                    case Key.E: m_world.RotationX -= 5.0f; break;
                    case Key.D: m_world.RotationX += 5.0f; break;
                    case Key.S: m_world.RotationY -= 5.0f; break;
                    case Key.F: m_world.RotationY += 5.0f; break;
                    case Key.Add: m_world.SceneDistance -= 700.0f; break;
                    case Key.Subtract: m_world.SceneDistance += 700.0f; break;
                    case Key.V: m_world.startAnimation(); break;
                }
        }

        //Checkbox za svetla
        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            CheckBox c = (CheckBox)sender;
            if (this.openGLControl == null)
                return;
            if (c.IsChecked == true)
                this.openGLControl.OpenGL.Enable(OpenGL.GL_LIGHT0);
            else
                this.openGLControl.OpenGL.Disable(OpenGL.GL_LIGHT0);

        }
        private void CheckBox_Checked2(object sender, RoutedEventArgs e) {
            CheckBox c = (CheckBox)sender;
            if (this.openGLControl == null)
                return;
            if (c.IsChecked == true)
                this.openGLControl.OpenGL.Enable(OpenGL.GL_LIGHT1);
            else
                this.openGLControl.OpenGL.Disable(OpenGL.GL_LIGHT1);
        }
    }
}
