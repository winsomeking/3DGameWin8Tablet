using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Display;

using CommonDX;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SharpDX_Windows_8_Abstraction
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SwapChainBackgroundPanel
    {
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Used to set the layout overlayed over the SharpDX game screen.
        /// </summary>
        //public void SetLayout(UIElement element)
        //{
        //    Children.Clear();
        //    Children.Add(element);
        //}

        // this method is called every second by the timer. Count down the remaining
        // time that the game has to run.
        public void setTimeText(String time)
        {
            // display the new time in a text field.
            countDownText.Text = "Time Left: " + time;
        }

        // this method is called when the score changes. It displays the new score on
        // the screen.
        public void updateScore(int newScore)
        {
            scoreText.Text = "Score: " + newScore.ToString();
        }
    }
}
