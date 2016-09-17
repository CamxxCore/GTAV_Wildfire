using System;
using System.Linq;
using GTA;
using NativeUI;

namespace Wildfire
{
    public sealed class MainMenu : Script
    {
        private readonly MenuPool mainPool = new MenuPool();

        private readonly UIMenu mainMenu, scriptMenu, devMenu, regionList;  

        public MainMenu()
        {
            Tick += MainMenu_Tick;
            KeyDown += MainMenu_KeyDown;

            regionList = new UIMenu("Edit Regions", string.Empty);

            regionList.OnItemSelect += OnItemSelect;

            mainPool.Add(regionList);

            scriptMenu = new UIMenu("Script Menu", string.Empty);

            scriptMenu.OnItemSelect += OnItemSelect;

            mainPool.Add(scriptMenu);

            var menuItem = new UIMenuItem("Toggle Script");
            menuItem.Activated += (s, e) => GTAWildfire.Dev_ToggleScript();
            scriptMenu.AddItem(menuItem);

            devMenu = new UIMenu("Dev Options", string.Empty);

            devMenu.OnItemSelect += OnItemSelect;

            mainPool.Add(devMenu);

            menuItem = new UIMenuItem("New Region");
            menuItem.Activated += (s, e) => { GTAWildfire.Dev_EnableZoningTool(); s.Visible = false; };
            devMenu.AddItem(menuItem);

            menuItem = new UIMenuItem("Edit Existing Regions");

            devMenu.BindMenuToItem(regionList, menuItem);
            devMenu.AddItem(menuItem);

            mainMenu = new UIMenu("Wilfire pre-alpha", "");

            mainMenu.OnItemSelect += OnItemSelect;

            mainPool.Add(mainMenu);

            menuItem = new UIMenuItem("Script");
            mainMenu.BindMenuToItem(scriptMenu, menuItem);
            mainMenu.AddItem(menuItem);       

            menuItem = new UIMenuItem("Dev Options");
            mainMenu.BindMenuToItem(devMenu, menuItem);
            mainMenu.AddItem(menuItem);
        }

        private void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == devMenu)
            {
                switch (index)
                {
                    case 1: SetupRegionListMenu(); break;
                    default: break;
                }
            }
        }

        private void SetupRegionListMenu()
        {
            if (ReferenceEquals(regionList, null)) return;

            regionList.MenuItems.Clear();

            var regions = GTAWildfire.FireController.Regions.OrderBy(x => x.Alias);

            foreach (var region in regions)
            {
                UIMenuItem item = new UIMenuItem(region.Alias);

                item.Activated += (s, e) =>
                {
                    SetupEditRegion(e.Text);

                    mainPool.CloseAllMenus();

                    GTAWildfire.Dev_EnableZoningTool();   
                };

                regionList.AddItem(item);
            }

            regionList.RefreshIndex();
        }

        private void SetupEditRegion(string regionName)
        {
            var region = GTAWildfire.FireController.Regions.Where(x => x.Alias == regionName).FirstOrDefault();

            if (!ReferenceEquals(region, null))
            {
                GTAWildfire.Dev_SetDebugRegion(region);               
            }
        }

        private void MainMenu_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.K)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }

        protected override void Dispose(bool A_0)
        {
            //var pos = Game.Player.Character.Position;
            //Function.Call(Hash.STOP_FIRE_IN_RANGE, pos.X, pos.Y, pos.Z, 1000.0f);
            base.Dispose(A_0);
        }

        private void MainMenu_Tick(object sender, EventArgs e)
        {
            mainPool.ProcessMenus();
        }
    }
}
