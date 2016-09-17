using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Wildfire.Utility;
using System.Xml;
using GTA.Math;
using GTA;
using GTA.Native;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Wildfire
{
    public class GTAWildfire : Script
    {
        public static FreeviewCamera FreeviewCamera {  get { return freeviewCamera; } }
        private static FreeviewCamera freeviewCamera = new FreeviewCamera();

        public static GTAFireController FireController {  get {  return fireController;} }
        private static GTAFireController fireController = new GTAFireController();

        public GTAWildfire()
        {
            KeyDown += OnKeyDown;
            Tick += OnTick;
            SetupGTAFireController();
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.KeyCode == System.Windows.Forms.Keys.Z && e.Control)
            {
                if (((e.Modifiers & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift))
                {
                    Dev_RemoveLastNode();
                }

                else
                {
                    Dev_RemoveLastPerimeterLine();
                }
            }

            else if (e.KeyCode == System.Windows.Forms.Keys.T)
            {

                var cbuildingData = MemoryAccess.GetCBuildingInfo();

                var perimeter = dbgPerimeter.ToArray();


                foreach (var cBuilding in cbuildingData)
                {
                    Vector3 position = new Vector3(cBuilding.Position[0], cBuilding.Position[1], cBuilding.Position[2]);

                    var name = cBuilding.Name.Substring(0, cBuilding.Name.Length - 4);

                    if (Helpers.InsidePolygon(position, perimeter) && (
                        cBuilding.Name.StartsWith("prop_bush_lrg_") ||
                        cBuilding.Name.StartsWith("prop_bush_med_") ||
                        cBuilding.Name.StartsWith("prop_w_r_cedar_") ||
                        cBuilding.Name.StartsWith("prop_tree_cedar_") ||
                        cBuilding.Name.StartsWith("test_tree_cedar_") ||
                        cBuilding.Name.StartsWith("prop_tree_oak_") ||
                        cBuilding.Name.StartsWith("prop_logpile_") ||
                        cBuilding.Name.StartsWith("prop_tree_eng_oak_") ||
                         cBuilding.Name.StartsWith("prop_tree_mquite_") ||
                          cBuilding.Name.StartsWith("prop_tree_pine_") ||
                           cBuilding.Name.StartsWith("prop_grass_dry_") ||
                            cBuilding.Name.StartsWith("prop_tree_birch_")))
                    {
                        dbgNodes.Add(new GTARegionNode(position));
                    }
                }

               /* var pos = freeviewCamera.MainCamera.Position;

                if (Helpers.InsidePolygon(pos, dbgPerimeter.ToArray()))
                {
                    dbgNodes.Add(new GTARegionNode(new Vector3(pos.X, pos.Y, World.GetGroundHeight(pos + new Vector3(0, 2.0f, 0)))));

                    UI.Notify("New node: " + pos.ToString());
                }

                else
                {
                    UI.Notify("Not inside the region.");
                }*/
            }

            else if (e.KeyCode == System.Windows.Forms.Keys.J)
            {
                Dev_SaveActiveZone();
            }
        }

        private void Dev_Update()
        {
            if (freeviewCamera.MainCamera.IsActive)
            {
                freeviewCamera.Update();

                #region controls

                if (Game.IsControlJustPressed(0, Control.ScriptRRight))
                {
                    Dev_DisableZoningTool();
                    return;
                }

                if (Game.IsControlJustPressed(0, Control.SelectPrevWeapon))
                {
                    if (dbgNodes.Count > 0)
                    {
                        dbgNodes[dbgNodes.Count - 1].Location += new Vector3(0, 0, 1.0f);

                        dbgNodes[dbgNodes.Count - 1].ResetDraw();

                        UI.ShowSubtitle("++");
                    }
                }

                if (Game.IsControlJustPressed(0, Control.SelectNextWeapon))
                {
                    if (dbgNodes.Count > 0)
                    {
                        dbgNodes[dbgNodes.Count - 1].Location -= new Vector3(0, 0, 1.0f);

                        dbgNodes[dbgNodes.Count - 1].ResetDraw();

                        UI.ShowSubtitle("--");
                    }
                }

                if (Game.IsControlJustPressed(0, Control.ReplayBack))
                {
                    if (dbgNodes.Count > 0)
                    {
                        dbgNodes[dbgNodes.Count - 1].Location += new Vector3(0, 0.4f, 0);

                        dbgNodes[dbgNodes.Count - 1].ResetDraw();
                    }
                }

                if (Game.IsControlJustPressed(0, Control.ReplayAdvance))
                {
                    if (dbgNodes.Count > 0)
                    {
                        dbgNodes[dbgNodes.Count - 1].Location += new Vector3(0, -0.4f, 0);

                        dbgNodes[dbgNodes.Count - 1].ResetDraw();
                    }
                }

                if (Game.IsControlJustPressed(0, Control.ReplayFfwd))
                {
                    if (dbgNodes.Count > 0)
                    {
                        dbgNodes[dbgNodes.Count - 1].Location += new Vector3(0.4f, 0, 0);

                        dbgNodes[dbgNodes.Count - 1].ResetDraw();
                    }
                }

                if (Game.IsControlJustPressed(0, Control.ReplayRewind))
                {
                    if (dbgNodes.Count > 0)
                    {
                        dbgNodes[dbgNodes.Count - 1].Location += new Vector3(-0.4f, 0, 0);

                        dbgNodes[dbgNodes.Count - 1].ResetDraw();
                    }
                }

                #endregion

                foreach (var region in fireController.Regions)
                {
                    if (region.Perimeter.Vertices[0].DistanceTo(freeviewCamera.MainCamera.Position) < 600f)
                    {
                        region.DebugDraw = true;
                    }

                    else region.DebugDraw = false;
                }

                for (int i = 1; i < dbgPerimeter.Count; i++)
                {
                    Vector3 start = dbgPerimeter[i - 1];

                    Vector3 end = dbgPerimeter[i];

                    Function.Call(Hash.DRAW_LINE, start.X, start.Y, start.Z,
                         end.X, end.Y, end.Z, 255, 255, 0, 255);
                }

                for (int i = 0; i < dbgNodes.Count; i++)
                {
                    dbgNodes[i].Draw();
                }

                if (bRemoveLastPerimeter)
                {
                    if (dbgPerimeter.Count > 0)
                    {
                        dbgPerimeter.RemoveAt(dbgPerimeter.Count - 1);

                        UI.Notify("Removed");
                    }

                    else
                    {
                        UI.Notify("Nothing to Remove");
                    }

                    bRemoveLastPerimeter = false;
                }


                if (bRemoveLastNode)
                {
                    if (dbgNodes.Count > 0)
                    {
                        dbgNodes.RemoveAt(dbgNodes.Count - 1);

                        UI.Notify("Removed");
                    }

                    else
                    {
                        UI.Notify("Nothing to Remove");
                    }

                    bRemoveLastNode = false;
                }


                if (Game.IsControlJustPressed(0, Control.ScriptRDown))
                {
                    var pos = freeviewCamera.MainCamera.Position;

                    dbgPerimeter.Add(pos);

                    UI.Notify("Selected point" + pos.ToString());
                }

                string result;

                Dev_UpdateOnscreenKeyboard(out result);

                if (result != null)
                {
                    Dev_WriteXmlFireRegionEntries(result);

                    SetupGTAFireController();

                    Dev_EnableZoningTool();

                    dbgPerimeter.Clear();

                    dbgNodes.Clear();

                    UI.ShowSubtitle("DONE!");
                }
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            Dev_Update();

            fireController.Update();

    
        }

        private static void SetupGTAFireController()
        {
            if (System.IO.File.Exists("scripts\\fireregions.xml"))
            {
                fireController = new GTAFireController(GetXMLFireRegionEntries("scripts\\fireregions.xml"));

                UI.Notify(string.Format("GTAFireController loaded with {0} active zones.", fireController.Regions.Count()));
            }

            else
            {
                UI.Notify("GTAFireController could not load.");
            }
        }

        private static List<GTAFireRegion> GetXMLFireRegionEntries(string filename)
        {
            List<GTAFireRegion> regions = new List<GTAFireRegion>();

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(filename);

            var rootElement = xmlDocument.SelectSingleNode("/fireRegions");

            foreach (XmlNode node in rootElement.ChildNodes)
            {
                List<Vector3> perimeter = new List<Vector3>(), nodes = new List<Vector3>();

                string alias = node.Attributes.GetNamedItem("alias").Value;

                int probability = Convert.ToInt32(node.Attributes.GetNamedItem("probability").Value);

                int density = Convert.ToInt32(node.Attributes.GetNamedItem("density").Value);

                foreach (XmlNode subNode in node.SelectSingleNode("perimeterPoints"))
                {
                    perimeter.Add(new Vector3(Convert.ToSingle(subNode.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture),
                       Convert.ToSingle(subNode.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture),
                        Convert.ToSingle(subNode.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture)));
                }

                foreach (XmlNode subNode in node.SelectSingleNode("nodes"))
                {
                    nodes.Add(new Vector3(Convert.ToSingle(subNode.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture),
                       Convert.ToSingle(subNode.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture),
                        Convert.ToSingle(subNode.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture)));
                }

                FireRegionParams frParameters = new FireRegionParams
                {
                    Alias = alias,
                    Density = density,
                    Probability = probability,
                    Perimeter = new GTARegionPerimeter(perimeter.ToArray()),
                    Nodes = nodes.Select(x => (GTARegionNode)x).ToArray()
                };

                regions.Add(new GTAFireRegion(frParameters));
            }

            return regions;
        }

        public static void Dev_ToggleScript()
        {
            // UI.ShowSubtitle("Started fire @ " + fireController.CreateRandomFire());

            var region = fireController.CreateRandomFire();

            // Game.Player.Character.Position = fireController.Regions.Find(x => x.Alias == "wfRegion5").StartNode.Location;
            // UI.ShowSubtitle("region: " + region);
        }

        private static string dbgAlias = "";
        private static List<Vector3> dbgPerimeter = new List<Vector3>();
        private static List<GTARegionNode> dbgNodes = new List<GTARegionNode>();
        private static bool bRemoveLastPerimeter = false, bRemoveLastNode = false;
        private static bool bUpdatingKeyboard = false;

        public static void Dev_RemoveLastPerimeterLine()
        {
            bRemoveLastPerimeter = true;
        }

        public static void Dev_RemoveLastNode()
        {
            bRemoveLastNode = true;
        }

        public static void Dev_SaveActiveZone()
        {
            if (dbgPerimeter.Count < 3) return;

            dbgPerimeter[0] = dbgPerimeter.Last(); //close the polygon

            if (dbgAlias.Length < 1)
            {
                Function.Call(Hash.DISPLAY_ONSCREEN_KEYBOARD, false, "FMMC_KEY_TIP", "", "", "", "", "", 40);
                bUpdatingKeyboard = true;
            }

            else
            {
                Dev_WriteXmlFireRegionEntries(dbgAlias);

                SetupGTAFireController();

                Dev_EnableZoningTool();

                dbgPerimeter.Clear();

                dbgNodes.Clear();

                UI.ShowSubtitle("DONE!");
            }
        }

        public static void Dev_SetDebugRegion(GTAFireRegion region)
        {
            dbgPerimeter = region.Perimeter.Vertices.ToList();
            dbgNodes = region.Nodes.ToList();
            dbgAlias = region.Alias;
        }

        public static void Dev_EnableZoningTool()
        {
            Function.Call(Hash.DO_SCREEN_FADE_OUT, 1200);
            Wait(1100);

            var pos = dbgPerimeter.Count > 0 ? dbgPerimeter[0] + new Vector3(0, 0, 200.0f) : Game.Player.Character.Position;

            freeviewCamera.EnterCameraView(pos);

            Wait(100);
            Function.Call(Hash.DO_SCREEN_FADE_IN, 800);
        }

        public static void Dev_DisableZoningTool()
        {
            Function.Call(Hash.DO_SCREEN_FADE_OUT, 1200);
            Wait(1100);

            freeviewCamera.ExitCameraView();

            Wait(100);

            Function.Call(Hash.DO_SCREEN_FADE_IN, 800);

            foreach (var region in fireController.Regions)
            {
                region.DebugDraw = false;
            }
        }

        void Dev_UpdateOnscreenKeyboard(out string result)
        {
            if (!bUpdatingKeyboard)
            {
                result = null;
                return;
            }

            if (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) == 2)
            {
                bUpdatingKeyboard = false;
                result = null;
                return;
            }

            while (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) == 0)
                Wait(0);

            if (Function.Call<string>(Hash.GET_ONSCREEN_KEYBOARD_RESULT) == null)
            {
                bUpdatingKeyboard = false;
                result = null;
                return;
            }

            result = Function.Call<string>(Hash.GET_ONSCREEN_KEYBOARD_RESULT);
            bUpdatingKeyboard = false;
        }

        private static void Dev_WriteXmlFireRegionEntries(string zoneAlias)
        {
            XmlDocument xmlDocument = new XmlDocument();

            if (!System.IO.File.Exists("scripts\\fireregions.xml"))
            {
                try
                {
                    string resourceFile = Properties.Resources.fireregions;

                    string[] text = resourceFile.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    using (System.IO.StreamWriter stream = new System.IO.StreamWriter("scripts\\fireregions.xml"))
                    {
                        foreach (string line in text)
                        {
                            stream.WriteLine(line);
                        }
                    }
                }

                catch (UnauthorizedAccessException)
                {
                }

                catch (System.IO.IOException)
                {
                }

                catch (Exception)
                {
                }
            }

            xmlDocument.Load("scripts\\fireregions.xml");

            var rootElement = xmlDocument.SelectSingleNode("/fireRegions");

            var newElement = xmlDocument.CreateElement("fireRegion");

            var attribute = xmlDocument.CreateAttribute("alias");
            attribute.Value = zoneAlias;

            newElement.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("probability");
            attribute.Value = Convert.ToString(5);

            newElement.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("density");
            attribute.Value = Convert.ToString(10);

            newElement.Attributes.Append(attribute);

            UI.Notify(string.Format("Writing {0} perimeter points, {1} tree locations to file", dbgPerimeter.Count, dbgNodes.Count));

            var subElement = xmlDocument.CreateElement("perimeterPoints");

            foreach (var point in dbgPerimeter)
            {
                var elem = xmlDocument.CreateElement("point");

                attribute = xmlDocument.CreateAttribute("x");
                attribute.Value = point.X.ToString();
                elem.Attributes.Append(attribute);

                attribute = xmlDocument.CreateAttribute("y");
                attribute.Value = point.Y.ToString();
                elem.Attributes.Append(attribute);

                attribute = xmlDocument.CreateAttribute("z");
                attribute.Value = point.Z.ToString();
                elem.Attributes.Append(attribute);

                subElement.AppendChild(elem);
            }

            newElement.AppendChild(subElement);

            subElement = xmlDocument.CreateElement("nodes");

            foreach (var point in dbgNodes)
            {
                var elem = xmlDocument.CreateElement("point");

                attribute = xmlDocument.CreateAttribute("x");
                attribute.Value = point.Location.X.ToString();
                elem.Attributes.Append(attribute);

                attribute = xmlDocument.CreateAttribute("y");
                attribute.Value = point.Location.Y.ToString();
                elem.Attributes.Append(attribute);

                attribute = xmlDocument.CreateAttribute("z");
                attribute.Value = point.Location.Z.ToString();
                elem.Attributes.Append(attribute);

                subElement.AppendChild(elem);
            }

            newElement.AppendChild(subElement);

            rootElement.AppendChild(newElement);

            using (XmlTextWriter writer = new XmlTextWriter("scripts\\fireregions.xml", null))
            {
                writer.Formatting = Formatting.Indented;
                xmlDocument.Save(writer);
            }
        }

        protected override void Dispose(bool A_0)
        {
            fireController?.Dispose();

            freeviewCamera?.ExitCameraView();

            LoopedPTFX.Remove(Game.Player.Character.Position, 1000f);

            Function.Call(Hash.DESTROY_ALL_CAMS, true);

            base.Dispose(A_0);
        }
    }
}
