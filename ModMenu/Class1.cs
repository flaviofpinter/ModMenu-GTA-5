using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;

namespace ModMenu
{
    public class Class1 : Script
    {
        MenuPool modMenuPool;
        UIMenu mainMenu;
        UIMenu playerMenu;
        UIMenu weaponsMenu;
        UIMenu vehicleMenu;
        UIMenuItem resetWantedLevel;

        public Class1()
        {
            Setup();
            Tick += onTick;
            KeyDown += onKeyDown;
        }
        void onTick(object sender, EventArgs e)
        {
            if(modMenuPool != null)
            {
                modMenuPool.ProcessMenus();
            }
        }

        void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10 && !modMenuPool.IsAnyMenuOpen())
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }

        void Setup()
        {
            modMenuPool = new MenuPool();
            mainMenu = new UIMenu("Mod Menu", "SELECT AN OPTION");
            modMenuPool.Add(mainMenu);

            playerMenu = modMenuPool.AddSubMenu(mainMenu, "Player");
            weaponsMenu = modMenuPool.AddSubMenu(mainMenu, "Weapons");
            vehicleMenu = modMenuPool.AddSubMenu(mainMenu, "Vehicles");

            SetupPlayerFunctions();
            SetupWeaponsFunctions();
            SetupVehicleFunctions();
        }
        
        void SetupPlayerFunctions()
        {
            ResetWantedLevel();
        }

        void SetupWeaponsFunctions()
        {
            WeaponSelectorMenu();
        }

        void SetupVehicleFunctions()
        {
            VehicleSelectorMenu();
        }

        void VehicleSelectorMenu()
        {
            UIMenu submenu = modMenuPool.AddSubMenu(vehicleMenu, "Vehicle Selector");
            List<dynamic> listOfVehicles = new List<dynamic>();
            VehicleHash[] allVehicleHashes = (VehicleHash[])Enum.GetValues(typeof(VehicleHash));
            for(int i = 0; i < allVehicleHashes.Length; i++)
            {
                listOfVehicles.Add(allVehicleHashes[i]);
            }

            UIMenuListItem list = new UIMenuListItem("Vehicle:", listOfVehicles, 0);
            submenu.AddItem(list);

            UIMenuItem getVehicle = new UIMenuItem("Get Vehicle");
            submenu.AddItem(getVehicle);

            submenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == getVehicle)
                {
                    int listIndex = list.Index;
                    VehicleHash vehicle = allVehicleHashes[listIndex];

                    Ped gamePed = Game.Player.Character;
                    Vehicle v = World.CreateVehicle(vehicle, gamePed.Position, gamePed.Heading);
                    v.PlaceOnGround();
                    gamePed.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                }
            };
        }

        void ResetWantedLevel()
        {
            resetWantedLevel = new UIMenuItem("Reset Wanted Level");
            playerMenu.AddItem(resetWantedLevel);

            playerMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == resetWantedLevel)
                {
                    if (Game.Player.WantedLevel == 0)
                    {
                        UI.ShowSubtitle("Your wanted level is already at 0!");
                    }
                    else
                    {
                        Game.Player.WantedLevel = 0;
                    }
                }
            };
        }

        void WeaponSelectorMenu()
        {
            UIMenu submenu = modMenuPool.AddSubMenu(weaponsMenu, "Weapon Selector Menu");
            List<dynamic> listOfWeapons = new List<dynamic>();
            WeaponHash[] allWeaponsHashes = (WeaponHash[])Enum.GetValues(typeof(WeaponHash));
            for (int i = 0; i < allWeaponsHashes.Length; i++)
            {
                listOfWeapons.Add(allWeaponsHashes[i]);
            }

            UIMenuListItem list = new UIMenuListItem("Weapons: ", listOfWeapons, 0);
            submenu.AddItem(list);

            UIMenuItem getWeapon = new UIMenuItem("Get Weapon");
            submenu.AddItem(getWeapon);

            submenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == getWeapon)
                {
                    int listIndex = list.Index;
                    WeaponHash currentHash = allWeaponsHashes[listIndex];
                    Game.Player.Character.Weapons.Give(currentHash, 9999, true, true);
                }
            };
        }
    }
}
