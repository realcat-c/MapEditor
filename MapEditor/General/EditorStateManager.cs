/*****************************************************************************
* Project: MapEditor
* Date   : 03.03.2022
* Author : realcat-c (JA)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*   03.03.2022	JA	Created
******************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MapEditor.General
{
    class EditorStateManager
    {
        MainWindow mainWindow;
        const string PATH = "config1.xml";
        Dictionary<string, string[]> settingsDict = new Dictionary<string, string[]>
        {
            {"windowPosition", new string[]{ "windowPosX", "windowPosY", "windowSizeX", "windowSizeY" } }
            //,{"key2", new string[]{ "2" } }
        };

        public WindowState windowState { get; set; }
        public EditorStateManager(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
            ReadProgramState();
        }
        void InitProgrammState()
        {
            XmlDocument xmlSave = new XmlDocument();
            XmlNode rootNode = xmlSave.CreateElement("root");
            xmlSave.AppendChild(rootNode);
            foreach(KeyValuePair<string, string[]> pair in settingsDict)
            {
                XmlNode childNode = xmlSave.CreateElement(pair.Key);
                foreach(string entry in pair.Value)
                {
                    XmlAttribute entryAttr = xmlSave.CreateAttribute(entry);
                    childNode.Attributes.Append(entryAttr);
                }
                rootNode.AppendChild(childNode);
            }
            xmlSave.Save(PATH);
        }

        void ReadProgramState()
        {
            if (!File.Exists(PATH) || !CheckProgramState())
            {
                WriteProgramState();
                return;
            }

            using (XmlReader xmlReader = XmlReader.Create(PATH))
            {

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name == settingsDict.ElementAt(0).Key)
                        {
                            settingsDict.TryGetValue(settingsDict.ElementAt(0).Key, out string[] value);
                            //if(xmlReader.HasAttributes)
                            windowState = new WindowState
                            {
                                windowPosX = double.Parse(xmlReader.GetAttribute(value[0])),
                                windowPosY = double.Parse(xmlReader.GetAttribute(value[1])),
                                windowSizeX = double.Parse(xmlReader.GetAttribute(value[2])),
                                windowSizeY = double.Parse(xmlReader.GetAttribute(value[3]))
                            };
                        }
                    }
                }
            }
            SetWindowState();
        }

        public void WriteProgramState()
        {
            windowState = UpdateWindowState();

            if (!File.Exists(PATH) || !CheckProgramState()) InitProgrammState();
            XmlDocument xmlSave = new XmlDocument();
            xmlSave.Load(PATH) ;

            XmlNode xmlNode = xmlSave.SelectSingleNode("root/windowPosition");
            xmlNode.Attributes[0].Value = windowState.windowPosX.ToString();
            xmlNode.Attributes[1].Value = windowState.windowPosY.ToString();
            xmlNode.Attributes[2].Value = windowState.windowSizeX.ToString();
            xmlNode.Attributes[3].Value = windowState.windowSizeY.ToString();

            xmlSave.Save(PATH);
            //xmlNode = null;
            //xmlSave = null;
            //GC.Collect();
        }

        bool CheckProgramState()
        {
            bool isValid = true;
            int index = 0;
            using (XmlReader xmlReader = XmlReader.Create(PATH))
            {

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name == settingsDict.ElementAt(index).Key)
                        {
                            settingsDict.TryGetValue(settingsDict.ElementAt(index)
                                .Key, out string[] value);
                            if (xmlReader.AttributeCount != value.Length)
                            {
                                isValid = false;
                                break;
                            }
                            index++;
                        }
                    }
                }
            }
            if (index != settingsDict.Count)
                isValid = false;
            return isValid;
        }

        WindowState UpdateWindowState()
        {
            return new WindowState
            {
                windowPosX = mainWindow.Left,
                windowPosY = mainWindow.Top,
                windowSizeX = mainWindow.Width,
                windowSizeY = mainWindow.Height
            };
        }
        void CheckNaN()
        {
            if (Double.IsNaN(windowState.windowPosX)) windowState.windowPosX = 300;
            if (Double.IsNaN(windowState.windowPosY)) windowState.windowPosY = 300;
            if (Double.IsNaN(windowState.windowSizeX)) windowState.windowSizeX = 300;
            if (Double.IsNaN(windowState.windowSizeY)) windowState.windowSizeY = 300;
        }

        void SetWindowState()
        {
            mainWindow.Left = windowState.windowPosX;
            mainWindow.Top = windowState.windowPosY;
            mainWindow.Width = windowState.windowSizeX;
            mainWindow.Height = windowState.windowSizeY;
        }
    }

    class WindowState
    {
        public double windowPosX { get; set; }
        public double windowPosY { get; set; }
        public double windowSizeX { get; set; }
        public double windowSizeY { get; set; }
    }
}
