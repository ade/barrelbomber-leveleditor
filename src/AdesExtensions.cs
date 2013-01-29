using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Xna.Framework;
using System.IO;
using System.Diagnostics;

namespace GLEED2D.src
{
    class AdesExtensions
    {

        /* CHANGES MADE TO ORIGINAL CODE
         * menu items in main form (file->export)
         * contextmenu item added to LayerContextMenu with call to exportPrefabDialog
         * LevelsPath string property in Constants
         *
         * the rest: look for references to methods
         * 
         */

        public const Microsoft.Xna.Framework.Input.Keys KEY_ROTATE = Microsoft.Xna.Framework.Input.Keys.R;


        //Level properties
        private const string EXPORTPATH_KEY = "exportPath";
        private const string SAVEPATH_KEY = "savePath";
        private const string PROPERTY_WIDTH = "width";
        private const string PROPERTY_HEIGHT = "height";
        private const string PROPERTY_BACKDROP = "backdrop";
        private const string PROPERTY_BOUNDED_LEFT = "bounded_left";
        private const string PROPERTY_BOUNDED_RIGHT = "bounded_right";
        private const string PROPERTY_BOUNDED_TOP = "bounded_top";
        private const string PROPERTY_BOUNDED_GROUND = "bounded_ground";
        private const string PROPERTY_CINEMATIC_BORDERS = "cinematic_borders";
        private const string PROPERTY_BALLBOMB_AMMO = "ammo_ballbomb";
        private const string PROPERTY_AIRSTRIKE_AMMO = "ammo_airstrike";
        private const string PROPERTY_MISSILE_AMMO = "ammo_missile";
        private const string PROPERTY_BAZOOKA_AMMO = "ammo_bazooka";
        private const string PROPERTY_Y_LIMIT = "limit_y_scroll";
        private const string PROPERTY_TITLE = "title";

        //private const string PROPERTY_STAR1 = "star1";
        //private const string PROPERTY_STAR2 = "star2";
        //private const string PROPERTY_STAR3 = "star3";


        //Layer properties
        private const string PROPERTY_INCLUDE = "include";

        //Object
        private const string PROPERTY_TYPE = "type";
        private const string PROPERTY_TYPE_DEFAULT = "breakable";
        private const string PROPERTY_DYNAMIC = "dynamic";
        private const string PROPERTY_PARENT = "parent";

        private const string TYPEFILTER_BARREL_BREAKABLE = "barrel_breakable";
        private const string TYPEFILTER_BARREL_EXPLODE = "barrel_explode";
        private const string TYPEFILTER_BARREL_ROCKET = "barrel_rocket";
        private const string TYPEFILTER_OBJ_FAIL = "obj_fail";
        private const string TYPEFILTER_DECAL = "decal";

        private static string[] typeFilters = new string[] { TYPEFILTER_BARREL_BREAKABLE, TYPEFILTER_BARREL_EXPLODE, TYPEFILTER_BARREL_ROCKET, TYPEFILTER_OBJ_FAIL, TYPEFILTER_DECAL };

        //Joint
        private const string PROPERTY_SHAPE_A = "shape A";
        private const string PROPERTY_SHAPE_B = "shape B";

        //Poly
        private const string PROPERTY_TEXTURE = "texture";
        private const string PROPERTY_MATERIAL = "material";

        private static Dictionary<string, string> fileList;
        
        private static GridItem shapeListener = null;

        private static void setExportFile(Level level, string filename)
        {
            string path = filename;

            if (level.CustomProperties.ContainsKey(EXPORTPATH_KEY))
            {
                level.CustomProperties[EXPORTPATH_KEY].value = path;
            }
            else
            {
                CustomProperty exportPath = new CustomProperty();
                exportPath.type = typeof(string);
                exportPath.name = "exportPath";
                exportPath.value = path;
                level.CustomProperties.Add(EXPORTPATH_KEY, exportPath);
            }
            
        }
        private static String getExportPath(Level level)
        {
            if (level.CustomProperties.ContainsKey(EXPORTPATH_KEY))
            {
                String filename = (string)level.CustomProperties[EXPORTPATH_KEY].value;
                String path = filename.Substring(0, filename.LastIndexOf("\\"));
                if (Directory.Exists(path))
                    return path;
                else
                    return Application.StartupPath;

            }
            else
                return Application.StartupPath;

        }
        public static String getExportFile(Level level)
        {
            if (level.CustomProperties.ContainsKey(EXPORTPATH_KEY))
            {
                String filename = (string)level.CustomProperties[EXPORTPATH_KEY].value;
                return filename;

            }
            else
                return null;

        }

        internal static void setSavePath(string filename)
        {
            string path = filename.Substring(0, filename.LastIndexOf("\\"));
            Constants.Instance.LevelFolder = path;

        }
        internal static String getSavePath()
        {
            if (Directory.Exists(Constants.Instance.LevelFolder))
                return Constants.Instance.LevelFolder;
            else
                return Application.StartupPath;

        }
        public static void exportLevel(String filename, Level level)
        {
            string problems = "";

            setExportFile(level, filename);
            XmlWriter writer = XmlWriter.Create(filename);
            writer.WriteStartDocument();
            writer.WriteStartElement("level");
            writer.WriteAttributeString("name", level.Name);
            writer.WriteAttributeString(PROPERTY_WIDTH, getLevelCustomProperty(PROPERTY_WIDTH, level).ToString());
            writer.WriteAttributeString(PROPERTY_HEIGHT, getLevelCustomProperty(PROPERTY_HEIGHT, level).ToString());
            writer.WriteAttributeString(PROPERTY_BACKDROP, getLevelCustomProperty(PROPERTY_BACKDROP, level).ToString());
            writer.WriteAttributeString(PROPERTY_BOUNDED_GROUND, (bool)getLevelCustomProperty(PROPERTY_BOUNDED_GROUND, level) ? "1" : "0");
            writer.WriteAttributeString(PROPERTY_BOUNDED_TOP, (bool)getLevelCustomProperty(PROPERTY_BOUNDED_TOP, level) ? "1" : "0");
            writer.WriteAttributeString(PROPERTY_BOUNDED_LEFT, (bool)getLevelCustomProperty(PROPERTY_BOUNDED_LEFT, level) ? "1" : "0");
            writer.WriteAttributeString(PROPERTY_BOUNDED_RIGHT, (bool)getLevelCustomProperty(PROPERTY_BOUNDED_RIGHT, level) ? "1" : "0");
            writer.WriteAttributeString(PROPERTY_CINEMATIC_BORDERS, (bool)getLevelCustomProperty(PROPERTY_CINEMATIC_BORDERS, level) ? "1" : "0");
            writer.WriteAttributeString(PROPERTY_Y_LIMIT, (bool)getLevelCustomProperty(PROPERTY_Y_LIMIT, level) ? "1" : "0");
            writer.WriteAttributeString(PROPERTY_BALLBOMB_AMMO, getLevelCustomProperty(PROPERTY_BALLBOMB_AMMO, level).ToString());
            writer.WriteAttributeString(PROPERTY_BAZOOKA_AMMO, getLevelCustomProperty(PROPERTY_BAZOOKA_AMMO, level).ToString());
            writer.WriteAttributeString(PROPERTY_AIRSTRIKE_AMMO, getLevelCustomProperty(PROPERTY_AIRSTRIKE_AMMO, level).ToString());
            writer.WriteAttributeString(PROPERTY_MISSILE_AMMO, getLevelCustomProperty(PROPERTY_MISSILE_AMMO, level).ToString());
            writer.WriteAttributeString(PROPERTY_TITLE, getLevelCustomProperty(PROPERTY_TITLE, level).ToString());


            if (Int32.Parse(getLevelCustomProperty(PROPERTY_BALLBOMB_AMMO, level).ToString()) + Int32.Parse(getLevelCustomProperty(PROPERTY_AIRSTRIKE_AMMO, level).ToString()) + Int32.Parse(getLevelCustomProperty(PROPERTY_MISSILE_AMMO, level).ToString()) + Int32.Parse(getLevelCustomProperty(PROPERTY_BAZOOKA_AMMO, level).ToString()) == 0)
                problems += "There's no ammo in the level.\n";

            if (getLevelCustomProperty(PROPERTY_TITLE, level).ToString() == "untitled")
                problems += "The level is named 'untitled'.\n";

            int layersSkipped = 0;
            

            foreach (Layer layer in level.Layers)
            {
                if (!layer.CustomProperties.ContainsKey(PROPERTY_INCLUDE) || ((bool)layer.CustomProperties[PROPERTY_INCLUDE].value) == true)
                {
                    for (int i = 0; i < layer.Items.Count; i++)
                    {
                        
                        
                        if (layer.Items[i] is TextureItem)
                        {
                            //TextureItem specific properties
                            TextureItem item = (TextureItem)layer.Items[i];
                            Vector2 position = getWorldCenterPosition(item);

                            string template = (string)item.CustomProperties["template"].value;
                            if (template.ToLower().Equals("[weldjoint]"))
                            {
                                String bodyA = getItemCustomPropertyString(PROPERTY_SHAPE_A, item);
                                String bodyB = getItemCustomPropertyString(PROPERTY_SHAPE_B, item);

                                if (bodyA.Length == 0)
                                    problems += "Weld joint '" + item.Name + "' doesn't have linked shapes.";
                                else if(bodyB.Length == 0)
                                    problems += "Weld joint '" + item.Name + "' doesn't have linked shapes.";
                                
                                writer.WriteStartElement("joint");
                                writer.WriteAttributeString("type", "weld");
                                writer.WriteAttributeString("a", bodyA);
                                writer.WriteAttributeString("b", bodyB);
                            } else if (template.ToLower().Equals("[revolutejoint]"))
                            {
                                String bodyA = getItemCustomPropertyString(PROPERTY_SHAPE_A, item);
                                String bodyB = getItemCustomPropertyString(PROPERTY_SHAPE_B, item);

                                if (bodyA.Length == 0)
                                    problems += "Weld joint '" + item.Name + "' doesn't have linked shapes.";
                                else if (bodyB.Length == 0)
                                    problems += "Weld joint '" + item.Name + "' doesn't have linked shapes.";

                                writer.WriteStartElement("joint");
                                writer.WriteAttributeString("type", "revo");
                                writer.WriteAttributeString("a", bodyA);
                                writer.WriteAttributeString("b", bodyB);

                                writer.WriteAttributeString("limit", getItemCustomPropertyBoolString("enableLimit", item));
                                writer.WriteAttributeString("motor", getItemCustomPropertyBoolString("enableMotor", item));
                                writer.WriteAttributeString("lowerAngle", getItemCustomPropertyString("lowerAngle", item));
                                writer.WriteAttributeString("upperAngle", getItemCustomPropertyString("upperAngle", item));
                                writer.WriteAttributeString("maxMotorTorque", getItemCustomPropertyString("maxMotorTorque", item));
                                writer.WriteAttributeString("motorSpeed", getItemCustomPropertyString("motorSpeed", item));

                            }
                            else
                            {
                                writer.WriteStartElement("shape");

                                //Common
                                writer.WriteAttributeString("id", layer.Items[i].Name);
                                writer.WriteAttributeString(PROPERTY_DYNAMIC, getItemCustomPropertyBoolString(PROPERTY_DYNAMIC, item));

                                //Shape
                                writer.WriteAttributeString("template", template);
                                writer.WriteAttributeString("rotation", item.getRotation().ToString(CultureInfo.InvariantCulture) + "");
                                writer.WriteAttributeString(PROPERTY_TYPE, getItemCustomPropertyString(PROPERTY_TYPE, item));

                                if(item.CustomProperties.ContainsKey(PROPERTY_PARENT)) {
                                    writer.WriteAttributeString(PROPERTY_PARENT, getItemCustomPropertyString(PROPERTY_PARENT, item));
                                }
                            }

                            //Both
                            writer.WriteAttributeString("x", position.X.ToString(CultureInfo.InvariantCulture) + "");
                            writer.WriteAttributeString("y", position.Y.ToString(CultureInfo.InvariantCulture) + "");

                            
                        }
                        else if (layer.Items[i] is PathItem)
                        {
                            PathItem item = (PathItem)layer.Items[i];
                            writer.WriteStartElement("polygon");
                            
                            //common attribs
                            writer.WriteAttributeString("id", layer.Items[i].Name);
                            writer.WriteAttributeString(PROPERTY_DYNAMIC, getItemCustomPropertyBoolString(PROPERTY_DYNAMIC, item));

                            //Polygon specific properties
                            writer.WriteAttributeString(PROPERTY_TEXTURE, getItemCustomPropertyString(PROPERTY_TEXTURE, item));
                            writer.WriteAttributeString(PROPERTY_MATERIAL, getItemCustomPropertyString(PROPERTY_MATERIAL, item));

                            //Polygon vertices
                            writer.WriteStartElement("worldpoints");
                            foreach (Vector2 v in item.WorldPoints)
                            {
                                writer.WriteStartElement("worldpoint");
                                writer.WriteAttributeString("x", v.X.ToString(CultureInfo.InvariantCulture));
                                writer.WriteAttributeString("y", v.Y.ToString(CultureInfo.InvariantCulture));
                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();

                        }

                        
                        //End object
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    layersSkipped++;
                }

            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            String result = "Export finished.\n\n";
            if (layersSkipped > 0)
            {
                result += layersSkipped + " layers skipped.";
            }
            if(problems.Length > 0) {
                result += "Problems:\n" + problems;
            }
            MessageBox.Show(result);
        }
        private static string getItemCustomPropertyString(string key, Item i)
        {
            if (i.CustomProperties.ContainsKey(key))
            {
                return i.CustomProperties[key].value.ToString();
            }
            else
            {
                MessageBox.Show("Problem: item '" + i.Name + "' is missing a property: '" + key + "'.");
                return "";
            }
        }
        private static string getItemCustomPropertyBoolString(string key, Item i)
        {
            if (i.CustomProperties.ContainsKey(key))
            {
                return (bool)i.CustomProperties[key].value ? "1" : "0";
            }
            else
            {
                MessageBox.Show("Problem: item '" + i.Name + "' is missing a property: '" + key + "'.");
                return "";
            }
        }
        private static object getLevelCustomProperty(string key, Level l)
        {
            if (l.CustomProperties.ContainsKey(key))
            {
                return l.CustomProperties[key].value;
            }
            else
            {
                MessageBox.Show("Problem: level node is missing a property: '" + key + "'.");
                return null;
            }
        }
        internal static void onAddTextureItem(Item item, string path)
        {
            
            String templateName = getTemplateName(path);
            
            addCustomProperty("template", templateName, "template name of shapedefinition (DO NOT MODIFY!!)", item.CustomProperties);

            if (templateName.ToLower().Equals("[weldjoint]"))
            {
                if (item.Name == null || !item.Name.StartsWith("WELD JOINT"))
                {
                    Random rnd = new Random();
                    item.Name = "WELD JOINT " + rnd.Next(1000, 100000000);
                }
                addWeldJointProperties(item);
            }
            else if (templateName.ToLower().Equals("[revolutejoint]"))
            {
                if(item.Name == null || !item.Name.StartsWith("REVOLUTE JOINT")) {
                    Random rnd = new Random();
                    item.Name = "REVOLUTE JOINT " + rnd.Next(1000, 100000000);
                }
                addRevoluteJointProperties(item);
            }
            else
            {
                string description = "Valid values: " + PROPERTY_TYPE_DEFAULT;
                for (int i = 0; i < typeFilters.Length; i++)
                    description += ", " + typeFilters[i];

                //Check if filename contains one of the type names and set object type accordingly..
                bool match = false;
                for (int i = 0; i < typeFilters.Length; i++)
                {
                    if (templateName.ToLower().StartsWith(typeFilters[i]))
                    {
                        addCustomProperty(PROPERTY_TYPE, typeFilters[i], description, item.CustomProperties);
                        match = true;

                        if(templateName.ToLower().StartsWith(TYPEFILTER_DECAL)) {
                            addCustomProperty(PROPERTY_PARENT, "", "Parent object. Required. Click value text box and then an item to select.", item.CustomProperties);
                        }
                        break;
                    }
                }

                //...or set default type.
                if (!match)
                    addCustomProperty(PROPERTY_TYPE, PROPERTY_TYPE_DEFAULT, description, item.CustomProperties);
                
                //General properties
                onAddItem(item);
            }

            

        }
        

        private static void addWeldJointProperties(Item item)
        {
            addCustomProperty(PROPERTY_SHAPE_A, "", "shape name. click property text box and then a shape to select.", item.CustomProperties);
            addCustomProperty(PROPERTY_SHAPE_B, "", "shape name. click property text box and then a shape to select.", item.CustomProperties);
        }
        private static void addRevoluteJointProperties(Item item)
        {
            addCustomProperty(PROPERTY_SHAPE_A, "", "shape name. click property text box and then a shape to select.", item.CustomProperties);
            addCustomProperty(PROPERTY_SHAPE_B, "", "shape name. click property text box and then a shape to select.", item.CustomProperties);
            addCustomProperty("enableLimit", false, "Enable limiting of turn (maximum and minimum angle need to be specified)", item.CustomProperties);
            addCustomProperty("enableMotor", false, "Enable motor to generate torque? maxMotorTorque and motorSpeed need to be specified.", item.CustomProperties);
            addCustomProperty("lowerAngle", "0", "Lowerbound of angle limit", item.CustomProperties);
            addCustomProperty("upperAngle", "0", "Upperbound of angle limit", item.CustomProperties);
            addCustomProperty("maxMotorTorque", "0", "Max rotational force (torque) motor can produce", item.CustomProperties);
            addCustomProperty("motorSpeed", "0", "Speed of motor", item.CustomProperties);

        }

        
        internal static String exportLevelDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML Files (*.xml)|*.xml";

            string path = getExportPath(Editor.Instance.level);

            if (path != null)
            {
                if(Directory.Exists(path))
                    dialog.InitialDirectory = path;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
                AdesExtensions.exportLevel(dialog.FileName, Editor.Instance.level);

            return dialog.FileName;
        }

        internal static void onSelectGridProperty()
        {
            PropertyGrid propGrid = MainForm.Instance.propertyGrid1;
            Layer layer = Editor.Instance.SelectedLayer;
            GridItem selectedItem = propGrid.SelectedGridItem;
            if (selectedItem.Label.Equals(PROPERTY_SHAPE_A) || selectedItem.Label.Equals(PROPERTY_SHAPE_B) || selectedItem.Label.Equals(PROPERTY_PARENT))
            {
                shapeListener = selectedItem;
                //previousItem = Editor.Instance.SelectedItems.Count == 1 ? Editor.Instance.SelectedItems[0] : null;
            }
        }
        internal static bool allowSelect(Item i)
        {
            //Write name of shape to propertygrid, if listening
            if (i is TextureItem && shapeListener != null)
            {
                DictionaryPropertyDescriptor dpd = (DictionaryPropertyDescriptor)shapeListener.PropertyDescriptor;
                dpd.SetValue(shapeListener, i.Name);
                
                MainForm.Instance.propertyGrid1.Refresh();
                MainForm.Instance.propertyGrid1.SelectedGridItem = shapeListener.Parent;
                shapeListener = null;
                return false;
            }
            else
            {
                shapeListener = null;
                return true;
            }
        }
        
        private static Vector2 getWorldCenterPosition(TextureItem item) {
            return item.Position;
            /*
            //x' = Cos(Theta) * x - Sin(Theta) * y
            //y' = Sin(Theta) * x + Cos(Theta) * y
            Vector2 ret = new Vector2();
            float textureCenterX = item.texture.Width / 2;
            float textureCenterY = item.texture.Height / 2;
            textureCenterY = (float)Math.Cos(-item.Rotation) * textureCenterX - (float)Math.Sin(-item.Rotation) * textureCenterY;
            textureCenterY = (float)Math.Sin(-item.Rotation) * textureCenterX + (float)Math.Cos(-item.Rotation) * textureCenterY;


            float xDiff = textureCenterX - item.Origin.X;
            float yDiff = textureCenterY - item.Origin.Y;

            ret.X = item.Position.X + xDiff;
            ret.Y = item.Position.Y + yDiff;

            return ret;
             */
            
        }




        internal static Vector2 getTextureCenter(Microsoft.Xna.Framework.Graphics.Texture2D texture)
        {
            //This patch enables accurate origin position of textures (original was missing float cast)
            return new Vector2((float)texture.Width / 2, (float)texture.Height / 2);
        }

        internal static bool shouldSnapPosition(Microsoft.Xna.Framework.Input.KeyboardState kstate)
        {
            //This patch enables free positioning (ignore grid) when left control is held 
            return (Constants.Instance.SnapToGrid && !kstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl));
        }

        internal static bool shouldSnapRotation(Microsoft.Xna.Framework.Input.KeyboardState kstate)
        {
            return !kstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl);
        }


        internal static String quickExport()
        {
            String file = getExportFile(Editor.Instance.level);
            if (file == null)
                return exportLevelDialog();
            else
            {
                exportLevel(file, Editor.Instance.level);
                return file;
            }          

        }


        internal static string getTemplateName(string path)
        {
            String templateName = path.Substring(path.LastIndexOf("\\") + 1, path.LastIndexOf(".") - path.LastIndexOf("\\") - 1);
            return templateName;
        }

        internal static void addCustomPropertiesToLevel(Level level)
        {
            bool hasProperties = level.CustomProperties.ContainsKey(PROPERTY_WIDTH);

            addCustomProperty(PROPERTY_WIDTH, "1280", "play area width", level.CustomProperties);
            addCustomProperty(PROPERTY_HEIGHT, "720", "play area height", level.CustomProperties);
            addCustomProperty(PROPERTY_BACKDROP, "day", "backdrop template (day/night/...)", level.CustomProperties);
            addCustomProperty(PROPERTY_BOUNDED_LEFT, false, "add physical boundary to this side", level.CustomProperties);             
            addCustomProperty(PROPERTY_BOUNDED_RIGHT, false, "add physical boundary to this side",level.CustomProperties);                         
            addCustomProperty(PROPERTY_BOUNDED_TOP, false, "add physical boundary to this side",level.CustomProperties);                         
            addCustomProperty(PROPERTY_BOUNDED_GROUND, true, "add physical boundary to this side",level.CustomProperties);
            addCustomProperty(PROPERTY_Y_LIMIT, true, "disable Y-scroll", level.CustomProperties);                         
            addCustomProperty(PROPERTY_CINEMATIC_BORDERS, true, "show black borders when rendering out-of-scene space (if disabled, parallaxbackgound is seen)",level.CustomProperties);             
            addCustomProperty(PROPERTY_BALLBOMB_AMMO, "0", "ammo amount",level.CustomProperties);                         
            addCustomProperty(PROPERTY_AIRSTRIKE_AMMO, "0","ammo amount",level.CustomProperties);                         
            addCustomProperty(PROPERTY_MISSILE_AMMO, "0","ammo amount",level.CustomProperties);                         
            addCustomProperty(PROPERTY_BAZOOKA_AMMO, "0","ammo amount",level.CustomProperties);            
            addCustomProperty(PROPERTY_TITLE, "untitled", "title of the level, as seen when playing", level.CustomProperties);

            if (!hasProperties)
            {
                Layer l1 = new Layer("guides");
                onAddLayer(l1);
                l1.CustomProperties["include"].value = false;
                level.Layers.Add(l1);
                RectangleItem r1 = new RectangleItem(new Rectangle(0, -700, 1280, 700));
                r1.Name = "outline";
                l1.Items.Add(r1);
                Layer l2 = new Layer("structure");
                onAddLayer(l2);
                level.Layers.Add(l2);
                Layer l3 = new Layer("objectives");
                onAddLayer(l3);
                level.Layers.Add(l3);
            }

        }
        private static void addCustomProperty(string key, object value, string desc, SerializableDictionary dict)
        {
            if(!dict.ContainsKey(key))
                dict.Add(key, new CustomProperty(key, value, value.GetType(), desc));
        }

        internal static void dummyReference()
        {
        }

        internal static void onAddPoly(Item pi)
        {
            //Poly specific
            addCustomProperty(PROPERTY_TEXTURE, "", "texture name", pi.CustomProperties);
            addCustomProperty(PROPERTY_MATERIAL, "", "material preset", pi.CustomProperties);

            onAddItem(pi);
        }
        internal static void onAddItem(Item i)
        {
            //General
            addCustomProperty(PROPERTY_DYNAMIC, true, "can object move?", i.CustomProperties);
            
        }
        
        public static Dictionary<string, string> GetFilesRecursive(string b)
        {
            // 1.
            // Store results in the file results list.
            Dictionary<string, string> result = new Dictionary<string, string>();

            // 2.
            // Store a stack of our directories.
            Stack<string> stack = new Stack<string>();

            // 3.
            // Add initial directory.
            stack.Push(b);

            // 4.
            // Continue while there are directories to process
            while (stack.Count > 0)
            {
                // A.
                // Get top directory
                string dir = stack.Pop();

                try
                {
                    // B
                    // Add all files at this directory to the result List.
                    string[] files = Directory.GetFiles(dir, "*.png");
                    for (int i = 0; i < files.Length; i++)
                    {
                        string fname = files[i].Substring(files[i].LastIndexOf("\\") + 1);
                        fname = fname.Substring(0, fname.LastIndexOf("."));
                        result.Add(fname.ToLower(), files[i]);
                    }
                    //result.AddRange(Directory.GetFiles(dir, "*.*"));

                    // C
                    // Add all directories at this directory.
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                    // D
                    // Could not open the directory
                }
            }
            return result;
        }


        internal static void loadContentRoot(string p)
        {
            fileList = GetFilesRecursive(p);
        }
        internal static string xmlLevelFileNameToRealFilePath(string filename)
        {
            string fname = filename.Substring(filename.LastIndexOf("\\") + 1);

            if (fname.StartsWith("["))
                return Application.StartupPath + "\\" + fname;

            fname = fname.Substring(0, fname.LastIndexOf("."));

            
            if(!fileList.ContainsKey(fname.ToLower())) {
                MessageBox.Show("The following object/image was not found: " + fname.ToLower() + "\r\nMake sure it exists somewhere in the content root path. Crashing!");
            }
 
            return fileList[fname.ToLower()];
        }

        internal static void onAddLayer(Layer l)
        {
            addCustomProperty("include", true, "include this layer when exporting to game?", l.CustomProperties);
        }
    }
}
