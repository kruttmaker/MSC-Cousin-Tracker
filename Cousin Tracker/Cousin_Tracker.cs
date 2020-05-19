using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Cousin_Tracker
{
    public class Cousin_Tracker : Mod
    {
        public override string ID => "Cousin_Tracker"; //Your mod ID (unique)
        public override string Name => "Cousin Tracker"; //You mod name
        public override string Author => "kruttmaker"; //Your Username
        public override string Version => "0.2.1"; //Version
       

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => false;

        private float checkinterval;
        private float timerlastcheck;
        private float timer;

        private float lastdistance;

        private bool warningfirst;
        private bool warningsecond;

        static Settings smaxdistance = new Settings("MaxDistance", "Max distance for detecting your cousin", 500f);
        static Settings sfirstwarningdistance = new Settings("FirstWarningDistance", "First warning distance", 300f);
        static Settings ssecondwarningdistance = new Settings("SecondWarningDistance", "Second warning distance", 150f);
        static Settings smustdrivecar = new Settings("MustDriveCar", "Player must be driving a vehicle to be warned", true);

        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded 

            checkinterval = 2f;
            timerlastcheck = Time.deltaTime;

            warningfirst = false;
            warningsecond = false;

            ModConsole.Print("<color=yellow>Cousin Tracker v"+ Version +" loaded</color>");
        }

        private float DistanceToFITTAN()
        {
            Vector3 playerv = GameObject.Find("PLAYER").transform.position;
            Vector3 thedickv = GameObject.Find("FITTAN").transform.position;

            return Vector3.Distance(playerv, thedickv);        
        }

        private void SendMessage(string message)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmString("GUIsubtitle").Value = message;
        }

        private bool PlayerInCar ()
        {
            return !FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").ToString().Equals(""); ;
        }

        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
            Settings.AddSlider(this, smaxdistance, 550f, 900f);

            Settings.AddSlider(this, sfirstwarningdistance, 200f, 500f);

            Settings.AddSlider(this, ssecondwarningdistance, 100f, 200f);

            Settings.AddCheckBox(this, smustdrivecar);
        }

        public override void OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
        }

        public override void OnGUI()
        {
            // Draw unity OnGUI() here
        }

        // Update is called once per frames
        public override void Update()
        {
            timer += Time.deltaTime;

            if ((timer - timerlastcheck) > checkinterval)
            {
                if ( (bool.Parse(smustdrivecar.GetValue().ToString()) && !PlayerInCar()))
                    return;

                float distance = DistanceToFITTAN();

                if (distance > float.Parse(smaxdistance.GetValue().ToString()) )
                {
                    warningfirst = false;
                    warningsecond = false;
                    return;
                }
            
                if (!warningfirst && distance < float.Parse(sfirstwarningdistance.GetValue().ToString()) && distance < lastdistance)
                {
                    SendMessage("You Think you see a cloud of dust up ahead..");
                    warningfirst = true;
                }
                if (!warningsecond && distance < float.Parse(ssecondwarningdistance.GetValue().ToString()) && distance < lastdistance)
                {
                    SendMessage("Watch out, it is your cousin!");
                    warningsecond = true;
                }
               
                lastdistance = distance;
                timerlastcheck = timer;
            }
        }
    }
}
