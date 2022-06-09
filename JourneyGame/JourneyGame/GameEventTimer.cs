using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace JourneyGame
{
    public partial class MainWindow : Window
    {

        // This is What the Dispathcher Timer repears on
        public void LoadingText(object? sender, EventArgs e)
        {

            // Check if User wants to stop time
            StopTime();

            // If User hasn't stopped time
            if (ContinueTime)
            {
                // Show that a month has passed
                LogBox.Text += "---------- A month passes! \n";

                // Load The Action of the Players + AI
                LoadAction();

                // Show that a month has passed
                CurrentLand.month++;

                // Change the Display so that the player can get the information
                ChangeDisplay();
                DisplayNonPlayerKingdoms();

                CheckIfDead();
            }


        }

        // if player presses Z to stop time
        public void StopTime()
        {
            if (Keyboard.IsKeyDown(Key.Z))
            {
                TimeContinueDisplay.Text = "Time Has Stopped! Press Z to continue time again!";
                ContinueTime = !ContinueTime;
            }
        }

        public void LoadAction()
        {

            // Get Action of Each Kingdom
            foreach (Kingdom kingdoms in CurrentLand.TotalKingdoms)
            {

                // If the Kingdom is not the players, then run it like an "Ai", otherwise run it like a "Player
                if (kingdoms.name != CurrentLand.PlayerKingdom.name)
                {
                    kingdoms.LoadText("Ai", LogBox, FoodFocus, HealthFocus, MoneyFocus, SecurityFocus, WarFocus, CurrentLand);
                }
                else
                {
                    kingdoms.LoadText("Player", LogBox, FoodFocus, HealthFocus, MoneyFocus, SecurityFocus, WarFocus, CurrentLand);
                }
            }
        }

        public void CheckIfDead()
        {

            List<Kingdom> KingdomRemove = new();

            foreach (Kingdom kingdoms in CurrentLand.TotalKingdoms)
            {
                if (kingdoms.sustainability <= 0)
                {
                    KingdomRemove.Add(kingdoms);
                }
            }

            foreach (Kingdom kingdoms in KingdomRemove)
            {
                CurrentLand.TotalKingdoms.Remove(kingdoms);
            }


        }

    }

}
