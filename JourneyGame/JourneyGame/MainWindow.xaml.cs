using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;

namespace JourneyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        // Gloval Variables

        // This is the land where the kingdoms are
        public LandScape CurrentLand = new("Land of Grass", 0);

        // This is lets time pass
        public DispatcherTimer TimePasser = new();

        // Pass or Stop Time
        public bool ContinueTime = true;

        public MainWindow()
        {
            InitializeComponent();

            // Show the New Display
            ChangeDisplay();

            // Display all the kingdoms that are not the players
            DisplayNonPlayerKingdoms();

            // Load the Scenarios
            LoadScenarios();

            // Set current Player Kingdom to This
            CurrentLand.PlayerKingdom.name = "Kingdom of Nothing";

            // Start timer
            TimePasser.Interval = TimeSpan.FromMilliseconds(2000); // running the timer every (text_speed) miliseconds
            TimePasser.Tick += new EventHandler(LoadingText); // linking the timer event
            TimePasser.Start();
        }

        public void DisplayNonPlayerKingdoms()
        {

            OtherKingdomsDisplay.Text = "";

            // Display All The Non Player Kingdoms
            foreach (Kingdom kingdoms in CurrentLand.TotalKingdoms)
            {
                if (kingdoms.name != CurrentLand.PlayerKingdom.name)
                {
                    OtherKingdomsDisplay.Text += $"{kingdoms.name} | Stability: {kingdoms.sustainability} \n";
                }
            }
        }

        public void ChangeDisplay()
        {
            // Display Name of Land
            LandNameDisplay.Text = CurrentLand.PlayerKingdom.name;

            // Display Current Month
            SeasonNameDisplay.Text = CurrentLand.month.ToString();

            // Display Name of Kingdom
            DisplayKingdomName.Text = CurrentLand.PlayerKingdom.name;

            // Stats Display
            StatsDisplay.Text = $"Age: {CurrentLand.PlayerKingdom.age} ; Stability: {CurrentLand.PlayerKingdom.sustainability} ; Health: {CurrentLand.PlayerKingdom.health} ; Wealth: {CurrentLand.PlayerKingdom.wealth} ; Food: {CurrentLand.PlayerKingdom.food} ; Population: {CurrentLand.PlayerKingdom.population} ; War: {CurrentLand.PlayerKingdom.war} ; Rebellion: {CurrentLand.PlayerKingdom.rebellion}";

            // Infanstructure Display
            InfrastructureDisplay.Text = $"Hospitals: {CurrentLand.PlayerKingdom.hospitals} ; Banks: {CurrentLand.PlayerKingdom.banks} ; Farms: {CurrentLand.PlayerKingdom.farms} ; Homes: {CurrentLand.PlayerKingdom.homes} ; Jails: {CurrentLand.PlayerKingdom.jails}";

            // Show the Relationships
            OtherRelationshipDisplay.Text = "";
            for (int i = 0; i < CurrentLand.TotalRelationships.Count; i++)
            {
                OtherRelationshipDisplay.Text += $"{CurrentLand.TotalRelationships[i].Kingdom1.name} \n";
                OtherRelationshipDisplay.Text += $"{CurrentLand.TotalRelationships[i].kingdom2.name} \n";
                OtherRelationshipDisplay.Text += $"Relationship: {CurrentLand.TotalRelationships[i].relationship} \n";
                OtherRelationshipDisplay.Text += $"--------------- \n";
            }

            // Display all the player rulers
            for (int i = 0; i < CurrentLand.PlayerKingdom.Rulers.Count; i++)
            {

                switch (CurrentLand.PlayerKingdom.Rulers[i].type)
                {

                    case "LEADER":
                        LEADER.Text = $"Leader: {CurrentLand.PlayerKingdom.Rulers[i].name}";
                        break;

                    case "COMMANDER":
                        COMMANDER.Text = $"Commander: {CurrentLand.PlayerKingdom.Rulers[i].name}";
                        break;

                    case "FINANCE_MINISTER":
                        FINANCE_MINISTER.Text = $"Finance Minister: {CurrentLand.PlayerKingdom.Rulers[i].name}";
                        break;

                    case "AMBASSADOR":
                        AMBASSADOR.Text = $"Ambassador: {CurrentLand.PlayerKingdom.Rulers[i].name}";
                        break;

                    case "PUBLIC_MINISTER":
                        PUBLIC_MINISTER.Text = $"Public Minister: {CurrentLand.PlayerKingdom.Rulers[i].name}";
                        break;

                    default:
                        break;
                }
            }

            // Show Time is paused or not
            TimeContinueDisplay.Text = "Time is continuing! Press Z to stop time!";

            // Scroll Down
            ScrollBar.ScrollToEnd();
        }

        public void LoadScenarios()
        {
            List<ChangeThis> Success = new();
            List<ChangeThis> Failure = new();

            string[] Choices = { "1", "2" };
            // Choice 1: 50% of +50 Health
            Success.Add(new(50, 0, 0, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            // Choice 2: 100% of -10 health
            Success.Add(new(0, 0, 0, -10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // If Fail Choice 1
            Failure.Add(new(0, 0, 0, -50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

            // If Fail Choice 2
            Failure.Add(new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 , 0));

            CurrentLand.Scenarios.Add(new("Event!", $"Your kingdom is aproached by a medic who claims that he can fix your kindom. Your PUBLIC_MINISTER walks up to them and decides that: 1 for Yes, 2 for No", Choices, Success, Failure, "The Medic is welcomes in!", "The Medic was not welcomed!", "PUBLIC_MINISTER"));
        }

        public void SaveGame(object sender, RoutedEventArgs e)
        {
            // Seralzie the data
            var options = new JsonSerializerOptions { IncludeFields = true };
            // I don't exactly know how this works but it says something that it is beyond the depth of 64 and therefore needs to be perseved like this???
            var timerOptions = new JsonSerializerOptions { IncludeFields = true, ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };

            // Save onto string
            string jsonLand = JsonSerializer.Serialize(CurrentLand, options);
            string jsonTimer = JsonSerializer.Serialize(TimePasser, timerOptions);

            string TextHistory = LogBox.Text;

            // The data as a JSON string may be easily saved to a file
            File.WriteAllText(@"data-files/CurrentLand.txt", jsonLand);
            File.WriteAllText(@"data-files/Timer.txt", jsonTimer);
            File.WriteAllText(@"data-files/TextHistory.txt", TextHistory);

        }

        public void LoadGame(object sender, RoutedEventArgs e)
        {
            // Seralzie
            var options = new JsonSerializerOptions { IncludeFields = true };
            string jsonCurrentLand = File.ReadAllText(@"data-files/CurrentLand.txt");
            string jsonTimer = File.ReadAllText(@"data-files/Timer.txt");

            string TempTextHistroy = File.ReadAllText(@"data-files/TextHistory.txt");

            LandScape? TempCurrentLand = JsonSerializer.Deserialize<LandScape>(jsonCurrentLand, options);
            DispatcherTimer? TempTimer = JsonSerializer.Deserialize<DispatcherTimer>(jsonTimer, options);


            if (TempCurrentLand != null && TempTimer != null && TempTextHistroy != null)
            {
                CurrentLand = TempCurrentLand;
                TimePasser = TempTimer;
                LogBox.Text = TempTextHistroy;

                ChangeDisplay();
                DisplayNonPlayerKingdoms();
            }


        }

        // When the 'War' butotn is pressed
        public void PressWarButton(object sender, RoutedEventArgs e)
        {
            CurrentLand.PlayerKingdom.PlayerDeclareWarOn(LogBox, CurrentLand);
        }

        // When the 'Trade' butotn is pressed
        public void PressTradeButton(object sender, RoutedEventArgs e)
        {
            CurrentLand.PlayerKingdom.PlayerDeclareTradeOn(CurrentLand);
        }
    }

    public class LandScape
    {
        // Name of The Land
        public string name = "";

        // Number of Months passing
        public int month;

        // Scenarios that happen in the land
        public List<Scenarios> Scenarios = new List<Scenarios>();

        // The Player Kingdom
        public Kingdom PlayerKingdom = new("None", 100, 1, 100, 1, 100, 1, 1000, 1, 10, 1, 0, 0, 0);

        // Total Kingdoms
        public List<Kingdom> TotalKingdoms = new();

        // Total Relationships
        public List<Relationship> TotalRelationships = new();

        public LandScape(string name, int month)
        {
            this.name = name;
            this.month = month;

            // Generate Random Number of Kingdoms
            Random rand = new Random();

            int randomNum = rand.Next(5) + 1;

            // Add Player Kingdom
            TotalKingdoms.Add(PlayerKingdom);

            // Add Kingdoms based on Random Number
            for (int i = 0; i < randomNum; i++)
            {
                TotalKingdoms.Add(new($"Kingdom of {i}", 100, 1, 100, 1, 100, 1, 1000, 1, 10, 1, 0, 0, 0));
            }

            // Add Rulers to all Kingdoms
            FormAllRulers();

            // Form relationships with all kingdoms
            FormRelationship();
        }

        // Form a relationship with all kingdoms at the beginning of the game
        public void FormRelationship()
        {
            for (int i = 0; i < TotalKingdoms.Count; i++)
            {
                for (int x = 0; x < TotalKingdoms.Count; x++)
                {

                    // Generate a relationship if it doesn't already exist

                    if (NotInList(TotalKingdoms[i], TotalKingdoms[x]))
                    {
                        TotalRelationships.Add(new(TotalKingdoms[i], TotalKingdoms[x]));
                    }

                }
            }
        }

        // Add the Rulers to the Kingdoms at the beginning of the game
        public void FormAllRulers()
        {
            for (int x = 0; x < TotalKingdoms.Count; x++)
            {
                TotalKingdoms[x].Rulers.Add(new("Bobby", "NONE", 10, 20, 1, 100));
                TotalKingdoms[x].Rulers.Add(new("Zimmy", "NONE", 20, 10, 1, 100));
                TotalKingdoms[x].Rulers.Add(new("Gobby", "NONE", 1, 1, 30, 100));
                TotalKingdoms[x].Rulers.Add(new("Bobby", "NONE", 30, 1, 1, 100));
                TotalKingdoms[x].Rulers.Add(new("Gummy", "NONE", 10, 10, 10, 100));
            }

        }

        // If this kingdom is already not in the list
        public bool NotInList(Kingdom checkKingdom1, Kingdom checkKingdom2)
        {
            for (int y = 0; y < TotalRelationships.Count; y++)
            {

                if ((TotalRelationships[y].Kingdom1.name == checkKingdom1.name && TotalRelationships[y].kingdom2.name == checkKingdom2.name) || (TotalRelationships[y].kingdom2.name == checkKingdom1.name && TotalRelationships[y].Kingdom1.name == checkKingdom2.name) || (checkKingdom1.name == checkKingdom2.name))
                {
                    return false;
                }
            }

            return true;
        }

    }

    public class Relationship
    {

        public Kingdom Kingdom1;
        public Kingdom kingdom2;
        public double relationship = 10;
        public string relationShipType = "Nothing";

        public Relationship(Kingdom Kingdom1, Kingdom kingdom2)
        {
            this.Kingdom1 = Kingdom1;
            this.kingdom2 = kingdom2;
        }
    }


    public class Kingdom
    {

        public string name = "";

        public double sustainability;

        public int hospitals;
        public double health;

        public int banks;
        public double wealth;

        public int farms;
        public double food;

        public int homes;
        public double population;

        public int jails;
        public double rebellion;

        public double war;

        public double age = 0;

        public List<Ruler> Rulers = new();

        public double FoodCosts = 30;
        public double HealthCosts = 50;
        public double MoneyCosts = 100;
        public double SecurityCosts = 30;
        public double WarCosts = 10;

        public double inflation = 1;


        public Kingdom(string name, double sustainability, int hospitals, double health, int banks, double wealth, int farms, double food, int homes, double population, int jails, double rebellion, double war, double age)
        {
            this.name = name;
            this.sustainability = sustainability;
            this.hospitals = hospitals;
            this.health = health;
            this.banks = banks;
            this.wealth = wealth;
            this.farms = farms;
            this.food = food;
            this.homes = homes;
            this.population = population;
            this.jails = jails;
            this.rebellion = rebellion;
            this.war = war;
            this.age = age;
        }

        public void LoadText(string Controller, TextBlock LogBox, CheckBox FoodFocus, CheckBox HealthFocus, CheckBox MoneyFocus, CheckBox SecurityFocus, CheckBox WarFocus, LandScape CurrentLand)
        {



            // Players and AI go through slightly different things
            if (Controller == "Player")
            {
                ChangeCharacterRole();
                RandomScenario(CurrentLand, CurrentLand.PlayerKingdom);
                FocusOnThis(LogBox, FoodFocus, HealthFocus, MoneyFocus, SecurityFocus, WarFocus);
                FarmMaterials();
                Rebellion(Controller, LogBox);
                RebellionConsequences();
                CheckWealth(Controller, LogBox);
                PopulationGrowth();
                Starvation();
                Consequences();
            }

            if (Controller == "Ai")
            {
                AiChangeCharacterRole();
                AiDecideToTrade(LogBox, CurrentLand);
                AiDecideToWar(LogBox, CurrentLand);
                SearchForBattle(CurrentLand, LogBox);
                AiDecision();
                FarmMaterials();
                Rebellion(Controller, LogBox);
                RebellionConsequences();
                CheckWealth(Controller, LogBox);
                PopulationGrowth();
                Starvation();
                Consequences();

            }
        }


        public void PlayerDeclareTradeOn(LandScape LandScape)
        {

            // Get What Kingdom the player wants to trade with
            string input = Microsoft.VisualBasic.Interaction.InputBox($"Trade with whom?", "Choice New Title", "");

            // Get the kindom the player wants to trade with
            Kingdom kingdom = ContainKingdom(LandScape, input);

            // Get the relationship the player wnats to trade with
            Relationship relationShip = RelationshipFind(kingdom.name, name, LandScape);

            // If the kingdom's name is equal to the inpur by the player and their relationship isn't a war
            if (kingdom.name == input && relationShip.relationShipType != "War")
            {
                // Get what the player wants to trade with
                string TradeWhat = Microsoft.VisualBasic.Interaction.InputBox($"Trade What?", "Choice New Title", "").ToLower();

                // And how much
                double? TradeHowMany = Convert.ToDouble(Microsoft.VisualBasic.Interaction.InputBox($"How Many Of This?", "Choice New Title", ""));

                // If the trade many is not null and the thing the player wants to trade with exists
                if (TradeHowMany != null && (TradeWhat == "food" || TradeWhat == "health" || TradeWhat == "population" || TradeWhat == "war" || TradeWhat == "wealth"))
                {
                    // For what material the player wants back
                    string ForWhatMaterial = Microsoft.VisualBasic.Interaction.InputBox($"For what?", "Choice New Title", "").ToLower();

                    // For how much they want of this material
                    double? HowManyYouWantBack = Convert.ToDouble(Microsoft.VisualBasic.Interaction.InputBox($"How Many of Ths??", "Choice New Title", ""));

                    // if how much you want exists and the material exists
                    if (HowManyYouWantBack != null && (ForWhatMaterial == "food" || ForWhatMaterial == "health" || ForWhatMaterial == "population" || ForWhatMaterial == "war" || ForWhatMaterial == "wealth"))
                    {

                        // Random
                        Random rand = new();

                        // Find the AMBASSADOR Ruler
                        Ruler FinancePerson = FindRuler(Rulers, "AMBASSADOR");

                        // Chance of this trade working out
                        double chance = Convert.ToDouble(rand.Next(50)) + FinancePerson.charisma;

                        // If this goes through
                        if (chance > 50)
                        {
                            MessageBox.Show("It went through!");

                            // Give and Get Material
                            GiveThisAmount(kingdom, TradeWhat, Convert.ToDouble(TradeHowMany));
                            GiveBackThisAmount(kingdom, ForWhatMaterial, Convert.ToDouble(HowManyYouWantBack));

                            relationShip.relationship += 10;


                        }
                        else
                        {
                            MessageBox.Show("It did not go through!");

                            relationShip.relationship -= 10;
                        }





                    }
                    else
                    {
                        MessageBox.Show("Input Wrong");
                    }
                }
            }
            else
            {
                MessageBox.Show("Inpur Wrong");
            }

        }

        // if player wants to declare war
        public void PlayerDeclareWarOn(TextBlock LogBox, LandScape LandScape)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox($"Declare War On Who? Type in the name of the kingdom!", "Choice New Title", "");

            Kingdom kingdom = ContainKingdom(LandScape, input);

            Relationship relationShip = RelationshipFind(kingdom.name, name, LandScape);

            if (kingdom.name == input && relationShip.relationShipType != "War")
            {
                War(LogBox, LandScape, relationShip, kingdom);
            }


        }

        // if this kingdom has this name
        public Kingdom ContainKingdom(LandScape LandScape, string input)
        {
            for (int i = 0; i < LandScape.TotalKingdoms.Count; i++)
            {
                if (LandScape.TotalKingdoms[i].name == input)
                {
                    return LandScape.TotalKingdoms[i];
                }
            }

            return LandScape.TotalKingdoms[0];

        }

        // Ai Selects the role they want for their rulers
        public void AiChangeCharacterRole()
        {
            List<Ruler> UnkownRole = new();

            // For each ruler that does not have a role
            for (int i = 0; i < Rulers.Count; i++)
            {
                if (Rulers[i].type == "NONE")
                {
                    UnkownRole.Add(Rulers[i]);
                }
            }

            // Give them a random role that doesn't exist
            for (int i = 0; i < UnkownRole.Count; i++)
            {
                Random rand = new();

                int random = rand.Next(100);

                if (random > 0 && !RoleAlreadyExists("LEADER"))
                {
                    UnkownRole[i].type = "LEADER";
                }
                else if (random > 20 && !RoleAlreadyExists("COMMANDER"))
                {
                    UnkownRole[i].type = "COMMANDER";
                }
                else if (random > 40 && !RoleAlreadyExists("FINANCE_MINISTER"))
                {
                    UnkownRole[i].type = "FINANCE_MINISTER";
                }
                else if (random > 60 && !RoleAlreadyExists("AMBASSADOR"))
                {
                    UnkownRole[i].type = "AMBASSADOR";
                }
                else if (random > 80 && !RoleAlreadyExists("PUBLIC_MINISTER"))
                {
                    UnkownRole[i].type = "PUBLIC_MINISTER";
                }
                else
                {
                    SearchForRole(UnkownRole[i]);
                }

            }

        }

        // Searcing through the roles to find one that doesn't exist
        public void SearchForRole(Ruler UnkownRole)
        {
            string[] search = { "LEADER", "COMMANDER", "FINANCE_MINISTER", "AMBASSADOR", "PUBLIC_MINISTER" };

            for (int i = 0; i < search.Length; i++)
            {
                if (!RoleAlreadyExists(search[i]))
                {
                    UnkownRole.type = search[i];

                    break;
                }
            }
        }


        // Player chnages their role
        public void ChangeCharacterRole()
        {

            List<Ruler> UnkownRole = new();

            for (int i = 0; i < Rulers.Count; i++)
            {
                if (Rulers[i].type == "NONE")
                {
                    UnkownRole.Add(Rulers[i]);
                }
            }

            for (int i = 0; i < UnkownRole.Count; i++)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox($"{UnkownRole[i].name} needs a role or get's denied! They have: Reputation: {UnkownRole[i].reputation}, Military: {UnkownRole[i].military}, Morality: {UnkownRole[i].morality}, Charisma: {UnkownRole[i].charisma}. Type 1 for leader, Type 2 for Commander, Type 3 for Finance Minister, Type 4 for Ambassador, Type 5 for Public Minister", "Choice New Title", "");

                switch (input)
                {

                    case "1":

                        if (!RoleAlreadyExists("LEADER"))
                        {
                            UnkownRole[i].type = "LEADER";
                        }
                        else
                        {
                            MessageBox.Show("THIS ROLE ALREADY EXISTS!");
                        }
                        break;

                    case "2":

                        if (!RoleAlreadyExists("COMMANDER"))
                        {
                            UnkownRole[i].type = "COMMANDER";
                        }
                        else
                        {
                            MessageBox.Show("THIS ROLE ALREADY EXISTS!");
                        }
                        break;

                    case "3":

                        if (!RoleAlreadyExists("FINANCE_MINISTER"))
                        {
                            UnkownRole[i].type = "FINANCE_MINISTER";
                        }
                        else
                        {
                            MessageBox.Show("THIS ROLE ALREADY EXISTS!");
                        }

                        break;

                    case "4":
                        if (!RoleAlreadyExists("AMBASSADOR"))
                        {
                            UnkownRole[i].type = "AMBASSADOR";
                        }
                        else
                        {
                            MessageBox.Show("THIS ROLE ALREADY EXISTS!");
                        }

                        break;

                    case "5":
                        if (!RoleAlreadyExists("PUBLIC_MINISTER"))
                        {
                            UnkownRole[i].type = "PUBLIC_MINISTER";
                        }
                        else
                        {
                            MessageBox.Show("THIS ROLE ALREADY EXISTS!");
                        }
                        break;

                    default:
                        MessageBox.Show("This role doesn't exist! The leader is angry that they got something wrong!");
                        UnkownRole[i].morality -= 5;
                        UnkownRole[i].type = "NONE";
                        break;
                }
            }
        }

        // If this role already exists in the Rulers
        public bool RoleAlreadyExists(string Role)
        {
            for (int i = 0; i < Rulers.Count; i++)
            {
                if (Rulers[i].type == Role)
                {
                    return true;
                }
            }

            return false;

        }

        // Find a ruler based on their role
        public Ruler FindRuler(List<Ruler> role, string Role)
        {
            for (int i = 0; i < Rulers.Count; i++)
            {
                if (role[i].type == Role)
                {
                    return Rulers[i];
                }
            }

            return Rulers[0];
        }

        // If the Ai decides to go to war
        public void AiDecideToWar(TextBlock LogBox, LandScape LandScape)
        {


            // Select Random Kingdom
            Random rand = new Random();

            int randomKingdomToAttack = rand.Next(LandScape.TotalKingdoms.Count);

            // Can't declare war on themselves
            if (LandScape.TotalKingdoms[randomKingdomToAttack].name != name)
            {

                // Find Relationship
                Relationship relationShip = RelationshipFind(LandScape.TotalKingdoms[randomKingdomToAttack].name, name, LandScape);

                // If the relationship is bad
                if (relationShip.relationship <= 0 && relationShip.relationShipType != "War")
                {
                    War(LogBox, LandScape, relationShip, LandScape.TotalKingdoms[randomKingdomToAttack]);
                }
            }

        }

        // When going to war
        public void War(TextBlock LogBox, LandScape LandScape, Relationship relationShip, Kingdom kingdom)
        {

            // Declares War!
            LogBox.Text += $"{name} declares war on {kingdom.name}!!! \n";

            relationShip.relationShipType = "War";

            // Relationship drops by 100!
            RelationshipChange(kingdom.name, name, -100, LandScape);
        }

        // Searching for battle between countries
        public void SearchForBattle(LandScape LandScape, TextBlock LogBox)
        {

            // search through relationships
            for (int i = 0; i < LandScape.TotalRelationships.Count; i++)
            {
                Relationship relation = LandScape.TotalRelationships[i];

                // if the relationships is war
                if (relation.relationShipType == "War")
                {
                    Battle(relation.Kingdom1, relation.kingdom2, LandScape, LogBox);
                }
            }
        }


        // Battle between countries
        public void Battle(Kingdom AttackingKingdom, Kingdom DefendingKingdom, LandScape land, TextBlock LogBox)
        {

            // Display Attack
            LogBox.Text += $"{AttackingKingdom.name} decides to attack {DefendingKingdom.name}! \n";

            // Calculate attack and defense
            double AttacKStrength = ((AttackingKingdom.war * 1000) + AttackingKingdom.wealth) * 1.5;
            double DefendStrength = ((DefendingKingdom.war * 1000) + DefendingKingdom.wealth + DefendingKingdom.population) * 1.2;

            // Get Commanders
            Ruler AttackingRuler = FindRuler(AttackingKingdom.Rulers, "COMMANDER");
            Ruler DefendingRuler = FindRuler(DefendingKingdom.Rulers, "COMMANDER");

            // Add Commander Bonuses
            AttacKStrength *= AttackingRuler.military;
            DefendStrength *= DefendingRuler.military;

            // Display attack and defense power
            LogBox.Text += $"{AttackingKingdom.name} has Attack Power {AttacKStrength} and {DefendingKingdom.name} has Defend Power {DefendStrength}! \n";


            // Decide who wins or loses
            if (AttacKStrength > DefendStrength)
            {
                DefendingKingdom.health -= 20;
                DefendingKingdom.sustainability -= 2;

                LogBox.Text += $"Therefore {AttackingKingdom.name} won! \n";
            }
            else if (AttacKStrength < DefendStrength)
            {
                AttackingKingdom.health -= 10;
                AttackingKingdom.sustainability -= 2;

                LogBox.Text += $"Therefore {DefendingKingdom.name} won! \n";
            }
            else
            {

                LogBox.Text += $"It is a tie and nobody won! \n";

                DefendingKingdom.health -= 50;
                AttackingKingdom.health -= 50;
            }

            // Check if kingdom will surrender
            Surrender(AttackingKingdom, DefendingKingdom, land, LogBox);
            Surrender(DefendingKingdom, AttackingKingdom, land, LogBox);


        }

        // If kingdom will surrender
        public void Surrender(Kingdom kingdom, Kingdom kingdom2, LandScape land, TextBlock LogBox)
        {
            if (kingdom.health < 0 || kingdom.sustainability < 20)
            {
                LogBox.Text += $"{kingdom.name} surrenders to {kingdom2.name}! \n";

                Relationship relation = RelationshipFind(kingdom.name, kingdom2.name, land);

                double wealth = kingdom.wealth / 2;

                kingdom.wealth /= 2;
                kingdom2.wealth += wealth;

                relation.relationship = 10;
                relation.relationShipType = "Nothing";
            }
        }


        // if the ai decides to trade
        public void AiDecideToTrade(TextBlock LogBox, LandScape LandScape)
        {
            Random rand = new Random();

            int randomNum = rand.Next(100);

            // If the empire wants to trade or not
            if (randomNum > 90)
            {
                int randomKingdom = rand.Next(LandScape.TotalKingdoms.Count);

                double relationShip = RelationshipFind(LandScape.TotalKingdoms[randomKingdom].name, name, LandScape).relationship;

                // and which empire they wish to trade with
                if (LandScape.TotalKingdoms[randomKingdom].name != name && LandScape.TotalKingdoms[randomKingdom].name != LandScape.PlayerKingdom.name && relationShip > 0)
                {
                    LogBox.Text += $"{name} wanted to trade with {LandScape.TotalKingdoms[randomKingdom].name}! \n";

                    // How much they will be willing to give
                    string IWillGiveYou = RandomMaterial();
                    double forThisAmount = AmountOfMaterial(IWillGiveYou);

                    // How much they want back
                    string wantedMaterial = RandomMaterial();
                    double wantedThisAmount = AmountOfMaterial(wantedMaterial);

                    // what do they want to trade and for how much
                    LogBox.Text += $"They want to trade {forThisAmount} of {IWillGiveYou} for {wantedThisAmount} of {wantedMaterial}! \n";

                    // How much the finance minister is good
                    Ruler FinancePerson = FindRuler(Rulers, "AMBASSADOR");

                    double acceptOrNot = Convert.ToDouble(rand.Next(50)) + FinancePerson.charisma;

                    // Will they accept?
                    if (acceptOrNot > 50)
                    {

                        // If accept, then change the stats
                        LogBox.Text += $"The deal went through! \n";
                        GiveThisAmount(LandScape.TotalKingdoms[randomKingdom], IWillGiveYou, forThisAmount);
                        GiveBackThisAmount(LandScape.TotalKingdoms[randomKingdom], wantedMaterial, wantedThisAmount);

                        RelationshipChange(LandScape.TotalKingdoms[randomKingdom].name, name, 10, LandScape);
                    }
                    else
                    {
                        LogBox.Text += $"The deal went not went through! \n";

                        RelationshipChange(LandScape.TotalKingdoms[randomKingdom].name, name, -10, LandScape);

                    }


                }
                else if (LandScape.TotalKingdoms[randomKingdom].name == LandScape.PlayerKingdom.name) // If they want to give
                {

                    // How much they will give you
                    string IWillGiveYou = RandomMaterial();
                    double forThisAmount = AmountOfMaterial(IWillGiveYou);

                    // How much they want back
                    string wantedMaterial = RandomMaterial();
                    double wantedThisAmount = AmountOfMaterial(wantedMaterial);

                    // Configure message box
                    string caption = $"{name} wants to trade with you! \n";
                    string message = $"{name} want to trade {forThisAmount} of {IWillGiveYou} for {wantedThisAmount} of {wantedMaterial}!";
                    MessageBoxButton buttons = MessageBoxButton.OKCancel;

                    // Will you accept?
                    if (MessageBox.Show(message, caption, buttons, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                    {

                        // I accept, the change the stats
                        GiveThisAmount(LandScape.TotalKingdoms[randomKingdom], IWillGiveYou, forThisAmount);
                        GiveBackThisAmount(LandScape.TotalKingdoms[randomKingdom], wantedMaterial, wantedThisAmount);

                        RelationshipChange(LandScape.TotalKingdoms[randomKingdom].name, name, 10, LandScape);
                    }
                    else
                    {
                        RelationshipChange(LandScape.TotalKingdoms[randomKingdom].name, name, -10, LandScape);
                    }

                }
            }
        }

        // Give kingdom your material
        public void GiveThisAmount(Kingdom thisKingdom, string thisMaterialGiven, double thisAmount)
        {

            if (thisMaterialGiven == "health")
            {
                thisKingdom.health += thisAmount;
                health -= thisAmount;
            }
            else if (thisMaterialGiven == "wealth")
            {
                thisKingdom.wealth += thisAmount;
                wealth -= thisAmount;
            }
            else if (thisMaterialGiven == "food")
            {
                thisKingdom.food += thisAmount;
                food -= thisAmount;
            }
            else if (thisMaterialGiven == "population")
            {
                thisKingdom.population += thisAmount;
                population -= thisAmount;
            }
            else if (thisMaterialGiven == "war")
            {
                thisKingdom.war += thisAmount;
                war -= thisAmount;
            }
        }

        // Kingdom gives you back material
        public void GiveBackThisAmount(Kingdom thisKingdom, string thisMaterialGiven, double thisAmount)
        {
            if (thisMaterialGiven == "health")
            {
                thisKingdom.health -= thisAmount;
                health += thisAmount;
            }
            else if (thisMaterialGiven == "wealth")
            {
                thisKingdom.wealth -= thisAmount;
                wealth += thisAmount;
            }
            else if (thisMaterialGiven == "food")
            {
                thisKingdom.food -= thisAmount;
                food += thisAmount;
            }
            else if (thisMaterialGiven == "population")
            {
                thisKingdom.population -= thisAmount;
                population += thisAmount;
            }
            else if (thisMaterialGiven == "war")
            {
                thisKingdom.war -= thisAmount;
                war += thisAmount;
            }
        }

        // How much of material given (only for AI)
        public double AmountOfMaterial(string thisMaterial)
        {

            Random rand = new Random();

            if (thisMaterial == "health")
            {
                return Math.Abs(rand.NextDouble() * health);
            }
            else if (thisMaterial == "wealth")
            {
                return Math.Abs(rand.NextDouble() * wealth);
            }
            else if (thisMaterial == "food")
            {
                return Math.Abs(rand.NextDouble() * food);
            }
            else if (thisMaterial == "population")
            {
                return Math.Abs(rand.NextDouble() * population);
            }
            else if (thisMaterial == "war")
            {
                return Math.Abs(rand.NextDouble() * war);
            }

            return 0;
        }

        // Random material the Ai choses
        public string RandomMaterial()
        {

            Random rand = new Random();

            int whichMaterial = rand.Next(4);

            if (whichMaterial == 0)
            {
                return "health";
            }
            else if (whichMaterial == 1)
            {
                return "wealth";
            }
            else if (whichMaterial == 2)
            {
                return "food";
            }
            else if (whichMaterial == 3)
            {
                return "population";
            }
            else if (whichMaterial == 4)
            {
                return "war";
            }

            return "nothing";

        }

        // Find relationshup between countries
        public Relationship RelationshipFind(string name1, string name2, LandScape LandScape)
        {
            for (int i = 0; i < LandScape.TotalRelationships.Count; i++)
            {
                if ((LandScape.TotalRelationships[i].Kingdom1.name == name1 && LandScape.TotalRelationships[i].kingdom2.name == name2) || (LandScape.TotalRelationships[i].Kingdom1.name == name2 && LandScape.TotalRelationships[i].kingdom2.name == name1))
                {
                    return LandScape.TotalRelationships[i];
                }
            }

            return LandScape.TotalRelationships[0];
        }


        // Chnage relationship
        public void RelationshipChange(string name1, string name2, double changeBy, LandScape LandScape)
        {
            for (int i = 0; i < LandScape.TotalRelationships.Count; i++)
            {
                if ((LandScape.TotalRelationships[i].Kingdom1.name == name1 && LandScape.TotalRelationships[i].kingdom2.name == name2) || (LandScape.TotalRelationships[i].Kingdom1.name == name2 && LandScape.TotalRelationships[i].kingdom2.name == name1))
                {
                    LandScape.TotalRelationships[i].relationship += changeBy;

                    break;
                }
            }
        }

        // If the player focuses on this
        public void FocusOnThis(TextBlock LogBox, CheckBox FoodFocus, CheckBox HealthFocus, CheckBox MoneyFocus, CheckBox SecurityFocus, CheckBox WarFocus)
        {

            double cost = 0;

            if (FoodFocus.IsChecked == true)
            {
                cost = BuyFarm(cost);
            }

            if (HealthFocus.IsChecked == true)
            {
                cost = BuyHealth(cost);
            }

            if (MoneyFocus.IsChecked == true)
            {
                cost = BuyMoney(cost);
            }

            if (SecurityFocus.IsChecked == true)
            {
                cost = BuySecurity(cost);
            }

            if (WarFocus.IsChecked == true)
            {
                cost = BuyWar(cost);
            }

            if (cost > 0)
            {
                LogBox.Text += $"You payed {cost} money to build this month! \n";
            }

            wealth -= cost;

        }

        // If buy farm
        public double BuyFarm(double cost)
        {
            farms++;

            cost += FoodCosts * inflation;

            inflation += 0.1;

            return cost;
        }

        // if buy health
        public double BuyHealth(double cost)
        {
            hospitals++;
            homes++;

            cost += HealthCosts * inflation;

            inflation += 0.1;

            return cost;
        }

        // if but money
        public double BuyMoney(double cost)
        {
            banks++;

            cost += MoneyCosts * inflation;

            inflation += 0.1;

            return cost;
        }

        // if buy security
        public double BuySecurity(double cost)
        {
            jails++;

            cost += SecurityCosts * inflation;

            inflation += 0.1;

            return cost;
        }

        // if buy war
        public double BuyWar(double cost)
        {
            war++;

            cost += WarCosts * inflation;

            inflation += 0.1;

            return cost;
        }

        // Get Food and Wealth based on banks + population
        public void FarmMaterials()
        {
            food += ((farms * 10) + (population * 1.5));
            wealth += ((population / 100) + (banks * 10));
        }

        // Check for stats and rebel it not enough of these stats
        public void Rebellion(string Controller, TextBlock LogBox)
        {
            if (food < 300)
            {
                rebellion += 2;

                if (Controller == "Player")
                {
                    LogBox.Text += "The citizens don't like the lack of food! \n";
                }

            }

            if (food < 100)
            {
                rebellion++;

                if (Controller == "Player")
                {
                    LogBox.Text += "Starvation Pains! \n";
                }

            }

            if (war > 10)
            {
                rebellion++;

                if (Controller == "Player")
                {
                    LogBox.Text += "The citizens feel that you are war mongering! \n";
                }

            }

            if (jails > 10)
            {
                rebellion--;
            }

            if (jails * 100 < population)
            {
                rebellion += 2;

                if (Controller == "Player")
                {
                    LogBox.Text += "Crime is rising from lack of jails! \n";
                }
            }

        }

        // If rebellion is above a certain threshold
        public void RebellionConsequences()
        {
            if (rebellion > 10)
            {
                food -= 10;
                wealth -= 10;
                sustainability--;
            }
            else if (rebellion > 50)
            {
                food -= 10;
                wealth -= 2;
                population--;
                sustainability -= 2;
            }
        }

        // Population growth based on food
        public void PopulationGrowth()
        {

            if (food > 100)
            {
                population += 1 + homes;

            }
            else if (food > 500)
            {
                population += 2 + homes;

            }
            else if (food > 1000)
            {
                population += 1 + homes;
            }
            else
            {
                population -= 1;
            }


        }

        // Check wealth
        public void CheckWealth(string Controller, TextBlock LogBox)
        {
            if (wealth < 100 && Controller == "Player")
            {
                LogBox.Text += "You are low on cash! Be warned! \n";


            }
            else if (wealth > 500)
            {

                if (Controller == "Player")
                {
                    LogBox.Text += "You are high on cash! Citizens are happy! \n";
                }

                population++;
                food += 10;
            }
        }

        // Remove food based on population
        public void Starvation()
        {
            food -= ((population * 1.5) - (homes * 2));
        }

        // Consequences if not enough of certain stats
        public void Consequences()
        {
            if (food < 0)
            {
                sustainability -= 2;
            }

            if (wealth < 0)
            {
                sustainability -= 2;
            }

            if (health < 0)
            {
                sustainability -= 2;
            }
        }

        // Ai makes their decision
        public void AiDecision()
        {
            Random rand = new Random();

            int randomNum = rand.Next(100);

            double cost = 0;


            if (randomNum > 50)
            {
                if (0 < (wealth - (FoodCosts * inflation)))
                {
                    cost = BuyFarm(cost);
                }

            }
            else if (randomNum > 60)
            {
                if (0 < (wealth - (HealthCosts * inflation)))
                {
                    cost = BuyHealth(cost);
                }

            }
            else if (randomNum > 70)
            {
                if (0 < (wealth - (MoneyCosts * inflation)))
                {
                    cost = BuyMoney(cost);
                }

            }
            else if (randomNum > 80)
            {
                if (0 < (wealth - (SecurityCosts * inflation)))
                {
                    cost = BuySecurity(cost);
                }

            }
            else if (randomNum > 90)
            {
                if (0 < (wealth - (WarCosts * inflation)))
                {
                    cost = BuyWar(cost);
                }
            }


            if (food < 100)
            {
                cost = BuyFarm(cost);
            }

            if (rebellion > 0)
            {
                cost = BuySecurity(cost);
            }

            wealth -= cost;

        }

        // Generate a random scenario
        public void RandomScenario(LandScape LandScape, Kingdom kingdom)
        {
            Random rand = new();

            // Chance of Scenario Happening
            if (rand.Next(100) > 95)
            {
                // Select Random Scenario from the landscape
                int random = rand.Next(LandScape.Scenarios.Count);

                // Load Scenario
                LandScape.Scenarios[random].ShowScenario(kingdom, FindRuler);
            }

        }
    }

    public class Ruler
    {
        public string name = "";
        public string type = "";

        public double charisma;
        public double morality;
        public double military;
        public double reputation;

        public Ruler(string name, string type, double charisma, double morality, double military, double reputation)
        {
            this.name = name;
            this.type = type;
            this.charisma = charisma;
            this.morality = morality;
            this.military = military;
            this.reputation = reputation;
        }
    }


    public class Scenarios
    {

        public string name = "";
        public string description = "";
        public string[] choices;

        public string RulerNeeded = "";

        public string successResult = "";
        public string failResult = "";

        public List<ChangeThis> Success = new();
        public List<ChangeThis> Failure = new();

        public Scenarios(string name, string description, string[] choices, List<ChangeThis> Success, List<ChangeThis> Failure, string successResult, string failResult, string RulerNeeded)
        {
            this.name = name;
            this.description = description;
            this.choices = choices;

            this.Success = Success;
            this.Failure = Failure;

            this.successResult = successResult;
            this.failResult = failResult;
            this.RulerNeeded = RulerNeeded;
        }


        // Just the loop that won't stop until the right choice is chosen
        public void ShowScenario(Kingdom ThisKingdom, Func<List<Ruler>, string, Ruler> FindRuler)
        {
            // Set up Loop
            bool correctInput = false;

            while (!correctInput)
            {
                correctInput = ChoicesEffect(ThisKingdom, FindRuler);
            }


        }

        // Function designed to show the effect of a choice
        public bool ChoicesEffect(Kingdom ThisKingdom, Func<List<Ruler>, string, Ruler> FindRuler)
        {
            // Searc through the choice list
            for (int i = 0; i < choices.Length; i++)
            {

                // Get Input
                string input = Microsoft.VisualBasic.Interaction.InputBox(description, name, "");

                // If the Input is one of the Choices in the array
                if (input == $"{choices[i]}")
                {

                    // Random
                    Random rand = new();

                    // Find the Role needed for the current decision
                    Ruler Role = FindRuler(ThisKingdom.Rulers, RulerNeeded);

                    // If the random is above the % chance
                    if (Success[i].chance < (rand.Next(100) + Role.charisma + Role.reputation))
                    {
                        // Success Text
                        MessageBox.Show(successResult);
                        // Changes Stats
                        Success[i].ChangeStats(ThisKingdom);
                    }
                    else
                    {
                        // Fail Text
                        MessageBox.Show(failResult);
                        // Chnage Stats
                        Failure[i].ChangeStats(ThisKingdom);

                    }

                    return true;

                }
            }

            return false;
        }
    }

    public class ChangeThis
    {

        public int chance;

        public double sustainability;
        public int hospitals;
        public double health;
        public int banks;
        public double wealth;
        public int farms;
        public double food;
        public int homes;
        public double population;
        public int jails;
        public double rebellion;
        public double war;

        public double FoodCosts;
        public double HealthCosts;
        public double MoneyCosts;
        public double SecurityCosts;
        public double WarCosts;

        public double inflation;


        public ChangeThis(int chance, double sustainability, int hospitals, double health, int banks, double wealth, int farms, double food, int homes, double population, int jails, double rebellion, double war, double FoodCosts, double HealthCosts, double MoneyCosts, double SecurityCosts, double WarCosts, double inflation)
        {
            this.chance = chance;
            this.sustainability = sustainability;
            this.hospitals = hospitals;
            this.health = health;
            this.banks = banks;
            this.wealth = wealth;
            this.farms = farms;
            this.food = food;
            this.homes = homes;
            this.population = population;
            this.jails = jails;
            this.rebellion = rebellion;
            this.war = war;
            this.FoodCosts = FoodCosts;
            this.HealthCosts = HealthCosts;
            this.MoneyCosts = MoneyCosts;
            this.SecurityCosts = SecurityCosts;
            this.WarCosts = WarCosts;
            this.inflation = inflation;
        }

        // Change Stats based on choices
        public void ChangeStats(Kingdom kingdom)
        {
            kingdom.sustainability += sustainability;
            kingdom.hospitals += hospitals;
            kingdom.health += health;
            kingdom.banks += banks;
            kingdom.wealth += wealth;
            kingdom.farms += farms;
            kingdom.food += food;
            kingdom.homes += homes;
            kingdom.population += population;
            kingdom.jails += jails;
            kingdom.rebellion += rebellion;
            kingdom.war += war;
            kingdom.FoodCosts += FoodCosts;
            kingdom.HealthCosts += HealthCosts;
            kingdom.MoneyCosts += MoneyCosts;
            kingdom.SecurityCosts += SecurityCosts;
            kingdom.WarCosts += WarCosts;
            kingdom.inflation += inflation;
        }

    }











    // Not used but it is cool
    public class TextLoad
    {

        public string text;
        public int words_loaded = 0;
        public int max_word_count;
        public DispatcherTimer dispatcherTimer = new();
        public TextBlock textblock = new();

        public TextLoad(int text_speed, string text, double x, double y, Canvas ThisCanvas)
        {

            this.text = text;
            max_word_count = text.Length;

            Canvas.SetLeft(textblock, x);
            Canvas.SetTop(textblock, y);

            ThisCanvas.Children.Add(textblock);

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(text_speed); // running the timer every (text_speed) miliseconds
            dispatcherTimer.Tick += new EventHandler(LoadingText); // linking the timer event
            dispatcherTimer.Start(); // starting the timer
        }

        public void LoadingText(object? sender, EventArgs e)
        {
            if (words_loaded < max_word_count)
            {
                textblock.Text += text[words_loaded];

                words_loaded++;
            }
            else
            {
                dispatcherTimer.Stop();
            }

        }

    }
}
