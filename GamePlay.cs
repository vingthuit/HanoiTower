using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace WpfApp2
{
    internal class GamePlay
    {
        private readonly int diskCount;
        private readonly int rodLength;

        private Vector mousePos;
        private Point mouseDownPos;
        private Stack<Rectangle>[] allDisks;

        private Rectangle[] stacks;
        private bool stackCollided;
        private int fromStack;
        private int toStack;

        private readonly System.Drawing.Point[] solvedMoves;
        private int movesIndex;

        private Animator animator;
        private readonly HanoiTower hanoi;

        internal Canvas Canvas;
        internal Rectangle[] Stacks { set => stacks = value; }
        internal Stack<Rectangle>[] AllDisks { set => allDisks = value; }
        internal Rectangle DraggedDisk { get; set; }

        internal GamePlay(HanoiTower hanoi)
        {
            this.hanoi = hanoi;
            rodLength = hanoi.RodLength;
            Canvas = hanoi.Canvas;
            diskCount = hanoi.DiskCount;

            var size = (int)Math.Pow(2, diskCount) - 1;
            solvedMoves = new System.Drawing.Point[size];
        }

        #region Mouse actions
        internal void MouseDown(object sender, MouseEventArgs e)
        {
            DraggedDisk = (Rectangle)sender;
            if (!allDisks.Any(t => t.TryPeek(out var disk) && disk.ActualWidth == DraggedDisk.ActualWidth)) return;
            mousePos = (Vector)e.GetPosition(DraggedDisk);
            DraggedDisk.MouseMove += OnDragMove;
            DraggedDisk.LostMouseCapture += OnLostCapture;
            DraggedDisk.MouseUp += OnMouseUp;
            Mouse.Capture(DraggedDisk);

            mouseDownPos = e.GetPosition(Canvas) - mousePos;
            fromStack = CheckCollision();
        }

        private void OnDragMove(object sender, MouseEventArgs e)
        {
            var mouseMovePos = e.GetPosition(Canvas) - mousePos;
            UpdatePosition(mouseMovePos);
            toStack = CheckCollision();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FinishDrag(e);
            Mouse.Capture(null);
        }

        private void OnLostCapture(object sender, MouseEventArgs e)
        {
            FinishDrag(e);
        }

        private void FinishDrag(MouseEventArgs e)
        {
            DraggedDisk.MouseMove -= OnDragMove;
            DraggedDisk.LostMouseCapture -= OnLostCapture;
            DraggedDisk.MouseUp -= OnMouseUp;

            if (UndoPosition())
            {
                return;
            }

            var newPos = GetPositionOnRod(toStack);
            UpdatePosition(newPos);

            allDisks[fromStack].Pop();
            allDisks[toStack].Push(DraggedDisk);
        }

        internal Point GetPositionOnRod(int rod)
        {
            return new Point
            {
                X = Canvas.GetLeft(stacks[rod]) - DraggedDisk.ActualWidth / 2 + stacks[rod].ActualWidth / 2,
                Y = rodLength - DraggedDisk.ActualHeight * (allDisks[rod].Count + 1)
            };
        }

        private bool UndoPosition()
        {
            if (!stackCollided)
            {
                UpdatePosition(mouseDownPos);
                return true;
            }

            allDisks[toStack].TryPeek(out var nextStackDisk);
            Trace.WriteLine(DraggedDisk.ActualWidth);
            Trace.WriteLine(nextStackDisk?.ActualWidth);
            if (fromStack != toStack && !(DraggedDisk.ActualWidth > nextStackDisk?.ActualWidth)) return false;
            UpdatePosition(mouseDownPos);
            return true;
        }

        private void UpdatePosition(Point newPos)
        {
            Canvas.SetLeft(DraggedDisk, newPos.X);
            Canvas.SetTop(DraggedDisk, newPos.Y);
        }

        private int CheckCollision()
        {
            System.Drawing.RectangleF rect1 = new System.Drawing.Rectangle
            {
                X = (int)Canvas.GetLeft(DraggedDisk),
                Y = (int)Canvas.GetTop(DraggedDisk),
                Width = (int)DraggedDisk.ActualWidth,
                Height = (int)DraggedDisk.ActualHeight
            };

            for (var i = 0; i < stacks.Length; i++)
            {
                System.Drawing.RectangleF rect2 = new System.Drawing.Rectangle
                {
                    X = (int)Canvas.GetLeft(stacks[i]),
                    Y = (int)Canvas.GetTop(stacks[i]),
                    Width = (int)stacks[i].ActualWidth,
                    Height = (int)stacks[i].ActualHeight
                };

                stackCollided = rect1.IntersectsWith(rect2);
                if (stackCollided)
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        #region Solver
        internal void Solve(System.Drawing.Point move)
        {
            fromStack = move.X;
            toStack = move.Y;

            DraggedDisk = allDisks[fromStack].Pop();

            var newPos = GetPositionOnRod(toStack);
            animator.StartAnimation(newPos);

            allDisks[toStack].Push(DraggedDisk);
        }

        private void HanoiTowers(int quantity, int from, int to, int buffer)
        {
            while (true)
            {
                if (quantity == 0) return;
                HanoiTowers(quantity - 1, from, buffer, to);

                solvedMoves[movesIndex] = new System.Drawing.Point(from, to);
                movesIndex += 1;

                var from1 = from;
                quantity -= 1;
                from = buffer;
                buffer = from1;
            }
        }

        internal void Button_Click(object sender, RoutedEventArgs e)
        {           
            hanoi.StartNewGame();
            movesIndex = 0;
            HanoiTowers(diskCount, 0, 2, 1);
            animator = new Animator(this, solvedMoves);
            Solve(solvedMoves[0]);


        }
        #endregion
    }
}
