using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfApp2
{
    internal class Animator
    {
        private readonly GamePlay gamePlay;
        private Rectangle draggedDisk;
        private Point newPos;

        private static double _speed = 0.3;

        private readonly System.Drawing.Point[] solvedMoves;
        private int movesIndex = 1;
        private Storyboard storyboard = new Storyboard();

        internal Animator(GamePlay gamePlay, System.Drawing.Point[] solvedMoves)
        {
            this.gamePlay = gamePlay;
            this.solvedMoves = solvedMoves;
        }

        internal void StartAnimation(Point pos)
        {
            draggedDisk = gamePlay.DraggedDisk;
            newPos = pos;
            UpDiskAnimation();
        }

        internal static void SetSpeed(KeyEventArgs e)
        {
            if (_speed < 100 && e.Key == Key.Right)
            {
                _speed *= 3;
            }
            if (_speed > 0.5 && e.Key == Key.Left)
            {
                _speed =0.3;
            }
            Trace.WriteLine(_speed);
        }
        private static DoubleAnimation MakeAnimation(double from, double to, PropertyPath path)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(Math.Abs(from - to) / _speed)),
            };
            Storyboard.SetTargetProperty(animation, path);
            return animation;
        }

        private void CreateStoryBoard(Timeline animation)
        {
            storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin(draggedDisk);
        }

        private void UpDiskAnimation()
        {
            var from = Canvas.GetTop(draggedDisk);
            var to = -draggedDisk.ActualHeight * 2;
            var path = new PropertyPath("(Canvas.Top)");
            var yAnimation = MakeAnimation(from, to, path);

            yAnimation.Completed += UpAnimationCompleted;
            CreateStoryBoard(yAnimation);
        }

        private void UpAnimationCompleted(object sender, EventArgs e)
        {
            RightAnimate();
        }

        private void RightAnimate()
        {
            var from = Canvas.GetLeft(draggedDisk);
            var to = newPos.X;
            var path = new PropertyPath("(Canvas.Left)");
            var xAnimation = MakeAnimation(from, to, path);

            xAnimation.Completed += RightCompleted;
            CreateStoryBoard(xAnimation);
        }

        private void RightCompleted(object sender, EventArgs e)
        {
            DownAnimation();
        }

        private void DownAnimation()
        {
            var from = Canvas.GetTop(draggedDisk);
            var to = newPos.Y;
            var path = new PropertyPath("(Canvas.Top)");
            var yAnimation = MakeAnimation(from, to, path);

            yAnimation.Completed += AnimationCompleted;
            CreateStoryBoard(yAnimation);
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            if (movesIndex == solvedMoves.Length)
            {
                var button = (Button)gamePlay.Canvas.Children[0];
                button.IsEnabled = true;
            }

            if (movesIndex >= solvedMoves.Length) return;
            gamePlay.Solve(solvedMoves[movesIndex]);
            movesIndex += 1;
        }
    }
}
