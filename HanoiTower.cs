using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp2
{
    internal class HanoiTower
    {
        private readonly int diskThickness;
        private readonly int minDiskSize;
        private readonly Stack<Rectangle>[] allDisks;
        private SolidColorBrush[] colors;

        private readonly int rodThickness;
        private readonly int stacksOffset;
        private readonly int standLength;
        private readonly Rectangle[] stacks;

        internal GamePlay GamePlay { get; set; }

        internal Canvas Canvas { get; }
        internal int DiskCount { get; }
        internal int RodLength { get; }

        internal HanoiTower(int diskCount, Canvas canvas)
        {
            Canvas = canvas;
            DiskCount = diskCount;

            minDiskSize = 40;
            diskThickness = 20;
            allDisks = new Stack<Rectangle>[3];
            for (var i = 0; i < allDisks.Length; i++)
            {
                allDisks[i] = new Stack<Rectangle>();
            }

            stacks = new Rectangle[allDisks.Length];
            stacksOffset = 30;
            standLength = minDiskSize + diskThickness * diskCount;

            rodThickness = 4;
            RodLength = diskThickness * diskCount + 10;

            SetDiskColors();
        }

        private static SolidColorBrush SetColor(byte r, byte g, byte b)
        {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void SetDiskColors()
        {
            colors = new[]
            {
                SetColor(127, 0, 255),
                SetColor(0, 255, 255),
                SetColor(0, 255, 127),

                SetColor(0, 127, 0),
                SetColor(127, 255, 0),
                SetColor(255, 255, 0),
                
                SetColor(255, 127, 0),
                SetColor(255, 0, 0),
                SetColor(255, 0, 127),

                SetColor(127, 0, 127),
                SetColor(255, 0, 255),
                SetColor(255, 127, 255),                
            };
        }

        internal void DrawStacks()
        {
            var stackColor = Brushes.Black;
            const int roundRadius = 3;
            for (var i = 0; i < stacks.Length; i++)
            {
                var stand = new Rectangle
                {
                    Fill = stackColor,
                    Width = standLength,
                    Height = rodThickness,
                    RadiusX = roundRadius,
                    RadiusY = roundRadius,
                };
                var rod = new Rectangle
                {
                    Fill = stackColor,
                    Width = rodThickness,
                    Height = RodLength + rodThickness,
                    RadiusX = roundRadius,
                    RadiusY = roundRadius,
                };
                Canvas.Children.Add(stand);
                Canvas.Children.Add(rod);

                var standOffset = i * standLength + i * stacksOffset;
                Canvas.SetLeft(stand, standOffset);
                Canvas.SetTop(stand, RodLength);

                var rodOffset = standOffset + standLength / 2 - rodThickness / 2;
                Canvas.SetLeft(rod, rodOffset);
                Canvas.SetTop(rod, 0);

                stacks[i] = rod;
            }
            GamePlay.Stacks = stacks;
        }

        internal void DrawDisks()
        {
            var random = new Random().Next(colors.Length + 1);
            for (var i = 1; i <= DiskCount; i++)
            {
                var increment = (DiskCount - i) * 10;
                var diskLength = minDiskSize + increment * 2;
                const int roundRadius = 10;
                var disk = new Rectangle
                {
                    Fill = colors[(i + random) % colors.Length],
                    Width = diskLength,
                    Height = diskThickness,
                    RadiusX = roundRadius,
                    RadiusY = roundRadius,
                };
                var offsetX = standLength / 2 - minDiskSize / 2 - increment;
                var offsetY = RodLength - i * diskThickness;
                Canvas.Children.Add(disk);
                Canvas.SetLeft(disk, offsetX);
                Canvas.SetTop(disk, offsetY);

                allDisks[0].Push(disk);
                disk.MouseLeftButtonDown += GamePlay.MouseDown;
            }
            GamePlay.AllDisks = allDisks;
        }

        internal void StartNewGame()
        {
            var button = (Button)Canvas.Children[0];
            button.IsEnabled = false;

            foreach (var stack in allDisks)
            {
                stack.Clear();
            }

            var lastStackIndex = stacks.Length * 2;
            for (var i = 1; i <= DiskCount; i++)
            {
                var disk = (Rectangle)Canvas.Children[i + lastStackIndex];
                GamePlay.DraggedDisk = disk;
                var point = new Point
                {
                    X = Canvas.GetLeft(stacks[0]) - disk.ActualWidth / 2 + stacks[0].ActualWidth / 2,
                    Y = RodLength - disk.ActualHeight * i
                };
                Canvas.SetLeft(disk, point.X);
                Canvas.SetTop(disk, point.Y);
                allDisks[0].Push(disk);
            }
            GamePlay.AllDisks = allDisks;
        }
    }
}
