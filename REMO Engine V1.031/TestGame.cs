using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using REMO_Engine_V1._031;

namespace TestGame
{
    public static class GameOverScene
    {
        public static GfxStr RestartButton = new GfxStr("restart", Point.Zero);
        public static Scene scn = new Scene(() => {
        },
            () => {
                RestartButton.Pos = new Point(SquareScene.sqr.Pos.X + 25, SquareScene.sqr.Pos.Y + 30);
                if (Cursor.JustLeftClicked(RestartButton))
                    Projectors.Projector.SwapTo(SquareScene.scn);
                if (User.JustPressed(Keys.Space))
                    Projectors.Projector.SwapTo(SquareScene.scn);
            },
            () => {
                StandAlone.DrawString("GAME OVER", new Point(SquareScene.sqr.Pos.X, SquareScene.sqr.Pos.Y), Color.White);
                StandAlone.DrawString("SCORE : " + SquareScene.Score, new Point(SquareScene.sqr.Pos.X, SquareScene.sqr.Pos.Y-30), Color.White);
                RestartButton.Draw(Color.White);
                if (RestartButton.ContainsCursor())
                    RestartButton.Draw(Color.Red);
                Cursor.Draw(Color.White);
                //StandAlone.DrawString("restart", new Point(SquareScene.sqr.Pos.X+25, SquareScene.sqr.Pos.Y+30), Color.White);
            }
        );
    }


    public static class SquareScene
    {
        public static int DamageTimer = 0;

        public static Gfx2D sqr = new Gfx2D(new Rectangle(375, 400, 30, 30));
        public static List<Gfx> Bullets = new List<Gfx>();
        public static Dictionary<Gfx, int> Power = new Dictionary<Gfx, int>();
        public static int PlayerHps = 300;
        public static Gfx2D HpBar = new Gfx2D(new Rectangle(0, 17, PlayerHps, 10));
        public static List<Gfx> PowerItems = new List<Gfx>();
        public static bool isGameOver = false;
        public static Vector2 v = new Vector2(0, 1);
        public static Vector2 g = new Vector2(0, 2);
        public static void MoveSqr() => sqr.Pos += v.ToPoint();
        public static List<Gfx> Enemies = new List<Gfx>();
        public static Dictionary<Gfx, double> Hps = new Dictionary<Gfx, double>();
        public static void AddEnemy(int hp, Gfx g)
        {
            Enemies.Add(g);
            Hps.Add(g, hp);
        }
        public static void RemoveEnemy(Gfx g)
        {
            Enemies.Remove(g);
            Hps.Remove(g);
        }

        //노랑이

        public static List<Gfx> Enemies2 = new List<Gfx>();
        public static Dictionary<Gfx, double> Hps2 = new Dictionary<Gfx, double>();
        public static void AddEnemy2(int hp2, Gfx g2)
        {
            Enemies2.Add(g2);
            Hps2.Add(g2, hp2);
        }
        public static void RemoveEnemy2(Gfx g2)
        {
            Hps2.Remove(g2);
            Enemies2.Remove(g2);
        }



        public static double speed = 12;
        public static int Score = 0;
        public static int GameOverTimer = 0;




        public static double ATK = 0.1;






        public static Scene scn = new Scene(
            () =>
            {
                scn.bgm = "TestWav";
                StandAlone.FullScreen = new Rectangle(0, 0, 1000, 700);
                isGameOver = false;
                GameOverTimer = 0;
                PlayerHps = 300;
                Bullets.Clear();
                Hps.Clear();
                Enemies.Clear();
                Hps2.Clear();
                Enemies2.Clear();
                PowerItems.Clear();
                Score = 0;
                ATK = 0.1;
                DamageTimer = 0;
            },
            () =>
            {
                User.ArrowKeyPAct((p) => { sqr.MoveByVector(p, speed); });
                if (User.Pressing(Keys.LeftShift))
                    speed = 3;
                else
                    speed = 12;
                if (sqr.Pos.X < 0)
                    sqr.Pos = new Point(0, sqr.Pos.Y);
                if (sqr.Pos.X > 1000 - sqr.Bound.Width)
                    sqr.Pos = new Point(1000 - sqr.Bound.Width, sqr.Pos.Y);
                if (sqr.Pos.Y < 0)
                    sqr.Pos = new Point(sqr.Pos.X, 0);
                if (sqr.Pos.Y > 700 - sqr.Bound.Width)
                    sqr.Pos = new Point(sqr.Pos.X, 700 - sqr.Bound.Width);

                HpBar.Bound = new Rectangle(HpBar.Pos, new Point(PlayerHps, 10));

                for (int i =0; i < PowerItems.Count; i++)
                {
                    PowerItems[i].MoveByVector(new Point(0, 2), 5 + 0.1 * (StandAlone.FrameTimer / 1000));
                    if (Rectangle.Intersect(PowerItems[i].Bound, sqr.Bound) != Rectangle.Empty)
                    {
                        ATK +=0.01;
                        Score += 1000;
                        PowerItems.RemoveAt(i);
                        i--;
                    }
                }


                for (int i = 0; i < Enemies.Count; i++)
                {
                    Enemies[i].MoveByVector(new Point(0, 2), 5 + 0.1 * (StandAlone.FrameTimer / 1000));
                    if (Rectangle.Intersect(Enemies[i].Bound, sqr.Bound) != Rectangle.Empty) 
                    {
                        PlayerHps-=5;
                        DamageTimer = 30;
                    }
                    
                    
                    if (Enemies[i].Pos.Y > 1040)
                    {
                        RemoveEnemy(Enemies[i]);
                        Score += 10;
                        i--;
                        continue;
                    }
                    for (int j = 0; j < Bullets.Count; j++)
                    {
                        if (Rectangle.Intersect(Enemies[i].Bound, Bullets[j].Bound) != Rectangle.Empty)
                        {
                            Hps[Enemies[i]]-=ATK;// Enemies[i].Hp--;
                            Point tmp = Enemies[i].Center;
                            Enemies[i].Bound = new Rectangle(0, 0, (int)(Enemies[i].Bound.Width * 0.8), (int)(Enemies[i].Bound.Width * 0.8));
                            Enemies[i].Center = tmp - new Point(0, 10);
                            if (Hps[Enemies[i]] <= 0)
                            {
                                RemoveEnemy(Enemies[i]);
                                Score += 100;
                                i--;
                            }
                            Bullets.RemoveAt(j);
                            j--;
                            break;
                        }
                    }

                }
                DamageTimer -= 1;

                //노랑이
                
                for (int i = 0; i < Enemies2.Count; i++)
                {
                    Enemies2[i].MoveByVector(new Point(0, 1), 5 + 0.1 * (StandAlone.FrameTimer / 1000));
                    if (Rectangle.Intersect(Enemies2[i].Bound, sqr.Bound) != Rectangle.Empty)
                    {
                        PlayerHps -= 5;
                        DamageTimer = 30;
                    }
                    
                    
                    if (Enemies2[i].Center.Y > 1040)
                    {
                        RemoveEnemy2(Enemies2[i]);
                        Score += 20;
                        i--;
                        continue;
                    }

                    for (int j = 0; j < Bullets.Count; j++)
                    {
                        if (Rectangle.Intersect(Enemies2[i].Bound, Bullets[j].Bound) != Rectangle.Empty)
                        {                            
                            Point tmp = Enemies2[i].Center;
                            Enemies2[i].Bound = new Rectangle(0, 0, (int)(Enemies2[i].Bound.Width * 1.12), (int)(Enemies2[i].Bound.Width * 1.1));
                            Enemies2[i].Center = tmp - new Point(0, -15);
                            Fader.Add(new Gfx2D(Enemies2[i].Bound), 15, Color.Yellow);//Call by Value

                            Bullets.RemoveAt(j);
                            j--;
                            break;
                        }
                    }

                }
                
                if (PlayerHps <= 0)
                {
                    Projectors.Projector.SwapTo(GameOverScene.scn);
                    return;
                }

                if (StandAlone.FrameTimer % 7 == 0)
                    AddEnemy(2, new Gfx2D(new Rectangle(StandAlone.Random(880, 0), -120, 120, 120)));
              

                REMOTimer.Update(StandAlone.Random(50, 100), () =>
                 {
                     PowerItems.Add(new Gfx2D(new Rectangle(StandAlone.Random(980, 0), 0, 20, 20)));
                 });
               
                REMOTimer.Update(StandAlone.Random(10, 30), () =>
                {
                    AddEnemy2(4, new Gfx2D(new Rectangle(StandAlone.Random(990, 0), 0, 10, 10)));
                });
               

                for (int i = 0; i < Bullets.Count; i++)
                {
                    Bullets[i].MoveByVector(new Point(0, -2), 20 + 0.1 * (StandAlone.FrameTimer / 1000));
                    if (Bullets[i].Pos.Y < -40)
                    {
                        Bullets.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
                /*if (User.Pressing(Keys.Space))
                {
                    if (ATK <= 0.15)
                    {
                        Gfx g = new Gfx2D(new Rectangle((sqr.Pos.X + 20), (sqr.Pos.Y), 10 + (int)((ATK - 0.1) * 300), 30));
                        Bullets.Add(g);
                        g.Center = sqr.Center;
                    }
                    if (ATK > 0.15) 
                    {
                        Gfx j = new Gfx2D(new Rectangle((sqr.Pos.X - 30), (sqr.Pos.Y), 10 + (int)((ATK - 0.1) * 300), 30));
                        Bullets.Add(j);

                        Gfx h = new Gfx2D(new Rectangle((sqr.Pos.X + 50), (sqr.Pos.Y), 10 + (int)((ATK - 0.1) * 300), 30));
                        Bullets.Add(h);

                    }
                } */

                if (User.Pressing(Keys.Space))
                {
                    if (ATK <= 0.15)
                    {
                        Gfx g = new Gfx2D(new Rectangle((sqr.Pos.X + 20), (sqr.Pos.Y), 10 + (int)((ATK - 0.1) * 300), 30));
                        Bullets.Add(g);
                        g.Center = sqr.Center;
                    }
                    if (ATK > 0.15)
                    {
                        Gfx j = new Gfx2D(new Rectangle((sqr.Pos.X - 40), (sqr.Pos.Y), 10 + (int)((ATK - 0.1) * 300), 30));
                        Bullets.Add(j);
                        

                        Gfx h = new Gfx2D(new Rectangle((sqr.Pos.X + 30), (sqr.Pos.Y), 10 + (int)((ATK - 0.1) * 300), 30));
                        Bullets.Add(h);

                    }
                    if (ATK > 0.2)
                    {
                        Gfx k = new Gfx2D(new Rectangle((sqr.Pos.X - 30), (sqr.Pos.Y), 30, 30));
                        Bullets.Add(k);

                        Gfx l = new Gfx2D(new Rectangle((sqr.Pos.X + 45), (sqr.Pos.Y), 30, 30));
                        Bullets.Add(l);

                    }
                }

            },
            () =>
            {
                for (int i = 0; i < Enemies.Count; i++)
                    Enemies[i].Draw(Color.White);
                for (int i = 0; i < Enemies2.Count; i++)
                    Enemies2[i].Draw(Color.Yellow,Color.OrangeRed*Fader.Flicker(5));
                for (int i = 0; i < Bullets.Count; i++)
                    Bullets[i].Draw(Color.Red);
                for (int i = 0; i < PowerItems.Count; i++)
                    PowerItems[i].Draw(Color.Green);
                sqr.Draw(Color.White);
                Color FadeColor = Color.White * 0.2f;
                Fader.Add(new Gfx2D(sqr.Bound), 15, FadeColor);
                Fader.DrawAll();
                foreach(Gfx2D g in Fader.FadeAnimations[FadeColor].Keys)
                {
                    g.MoveByVector(new Point(0, 1),10);
                }
                if (User.Pressing(Keys.LeftShift))
                {
                    sqr.Draw(Color.White, Color.Blue * Fader.Flicker(10));
                }
                if (DamageTimer > 0)
                {
                    sqr.Draw(Color.White, Color.Red * Fader.Flicker(10));
                }
               
                HpBar.Draw(Color.White);
                StandAlone.DrawString("SCORE : " + Score, new Point(0, 0), Color.White);
               // StandAlone.DrawString(ATK.ToString(), new Point(100, 0), Color.White, Color.Black);

            });
    }

    public static class REMOTimer
    {
        private static Dictionary<Action,int> timers=new Dictionary<Action, int>();
        public static void Update(int ResetTime, Action TargetAction)
        {
            if(!timers.ContainsKey(TargetAction))
            {
                timers.Add(TargetAction, ResetTime);
            }
            else
            {
                if (timers[TargetAction] > 0)
                    timers[TargetAction]--;
                else
                {
                    timers[TargetAction] = ResetTime;
                    TargetAction();
                }

            }
        }
       

    }
}
