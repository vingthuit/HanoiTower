using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.vs;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        private int diskCount = 3;

        Button solveButton;

        private GamePlay gamePlay;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            KeyDown += new KeyEventHandler(OnButtonKeyDown);
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            Animator.SetSpeed(e);
        }

    private void TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            var text = TextBox.Text;
            try
            {
                diskCount = int.Parse(text);
                //if (diskCount < 1 || diskCount > 17) throw new FormatException();

                Canvas.Children.Clear();
                CenterWindowOnScreen();
                DrawHanoi();
            }
            catch (FormatException)
            {
                MessageBox.Show("Вы ввели недопустимое значение");
            }
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            Left = screenWidth/2 - Width * 1.3;
            Top = screenHeight / 2 - Height;
        }

        private void DrawHanoi()
        {
            SetWindowSize();
            var hanoi = new HanoiTower(diskCount, Canvas);
            gamePlay = new GamePlay(hanoi);
            hanoi.GamePlay = gamePlay;

            CreateSolveButton();
            hanoi.DrawStacks();
            hanoi.DrawDisks();
        }
      
        private void SetWindowSize()
        {
            var offset = 40 * diskCount + 20 * diskCount;

            Application.Current.MainWindow.Height = 130 + diskCount * 20;
            Application.Current.MainWindow.Width = 250 + offset;
        }

        private void ShowDescription(object sender, RoutedEventArgs e)
        {
            Description descriptionWindow = new Description();
            descriptionWindow.ShowDialog();
        }

        internal void CreateSolveButton()
        {
            solveButton = new Button
            {
                Content = "Решить",
            };
            solveButton.Click += gamePlay.Button_Click;
            Canvas.Children.Add(solveButton);
            Canvas.SetTop(solveButton, -25);
            Canvas.SetLeft(solveButton, -20);
        }
    }
}
