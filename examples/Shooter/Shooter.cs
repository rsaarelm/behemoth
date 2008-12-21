using System;
using Tao.OpenGl;
using Tao.FreeGlut;

namespace Shooter
{
  public class Shooter
  {
    const byte KEY_ESC = 27;


    static void Reshape(int w, int h)
    {
      Console.WriteLine("Window size is now: " + w + ", " + h);
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluPerspective(45.0, (double)w / (double)h, 0.1, 100.0);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
    }

   
    static void KeyboardCallback(byte ch, int mouseX, int mouseY)
    {
      if (ch == KEY_ESC)
      {
        Quit();
      }
    }


    static void Quit()
    {
      Glut.glutLeaveMainLoop();
    }


    static void IdleCallback()
    {
    }


    static void Display()
    {
      Gl.glClearColor(0f, 0.1f, 0.1f, 1f);
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
      Gl.glLoadIdentity();

      Glu.gluLookAt(0, 1.0, 5.0, 0, 0, 0, 0, 1.0, 0);
      Glut.glutWireTeapot(1.0);

      Glut.glutSwapBuffers();
    }


    public static void Main(string[] args)
    {
      Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
      Glut.glutInit();
      Glut.glutInitWindowSize(800, 600);
      Glut.glutCreateWindow("Behemoth Shooter");

      Glut.glutDisplayFunc(new Glut.DisplayCallback(Display));
      Glut.glutReshapeFunc(new Glut.ReshapeCallback(Reshape));
      Glut.glutKeyboardFunc(new Glut.KeyboardCallback(KeyboardCallback));
      Glut.glutIdleFunc(new Glut.IdleCallback(IdleCallback));

      Glut.glutMainLoop();
    }
  }
}