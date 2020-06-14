/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

 */

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
        public override string Version => "0.2.2"; //Version

        Transform player;
        Transform fittan;

        FsmString playerCurrentVehicleFsmString;

        const float checkinterval = 2;
        private float timerlastcheck;
        private float timer;

        private float lastdistance;

        private bool warningfirst;
        private bool warningsecond;

        static Settings smaxdistance = new Settings("MaxDistance", "Max distance for detecting your cousin", 500f);
        static Settings sfirstwarningdistance = new Settings("FirstWarningDistance", "First warning distance", 300f);
        static Settings ssecondwarningdistance = new Settings("SecondWarningDistance", "Second warning distance", 150f);
        static Settings smustdrivecar = new Settings("MustDriveCar", "Player must be driving a vehicle to be warned", true);

        public override void OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded

            timerlastcheck = Time.deltaTime;

            warningfirst = false;
            warningsecond = false;

            // We're storing the player and Fittan transforms on load.
            // Looking for objects every frame is very expensive.
            player = GameObject.Find("PLAYER").transform;
            fittan = GameObject.Find("TRAFFIC").transform.Find("VehiclesDirtRoad/Rally/FITTAN").transform;

            playerCurrentVehicleFsmString = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle");

            ModConsole.Print("<color=yellow>Cousin Tracker v" + Version + " loaded</color>");
        }

        private float DistanceToFITTAN()
        {
            return Vector3.Distance(player.position, fittan.position);
        }

        private void SendMessage(string message)
        {
            PlayMakerGlobals.Instance.Variables.FindFsmString("GUIsubtitle").Value = message;
        }

        private bool PlayerInCar()
        {
            return !string.IsNullOrEmpty(playerCurrentVehicleFsmString.Value);
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

        // Update is called once per frames
        public override void Update()
        {
            timer += Time.deltaTime;

            if ((timer - timerlastcheck) > checkinterval)
            {
                if ((bool.Parse(smustdrivecar.GetValue().ToString()) && !PlayerInCar()))
                    return;

                float distance = DistanceToFITTAN();

                if (distance > float.Parse(smaxdistance.GetValue().ToString()))
                {
                    warningfirst = false;
                    warningsecond = false;
                    return;
                }

                if (!warningfirst && distance < float.Parse(sfirstwarningdistance.GetValue().ToString()) && distance < lastdistance)
                {
                    SendMessage("You think you see a cloud of dust up ahead..");
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
