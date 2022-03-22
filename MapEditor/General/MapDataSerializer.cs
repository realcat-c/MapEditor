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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MapEditor
{
    public class MapDataSerializer
    {
        public static void Save(string path, MapData mData)
        {
			try
			{
				using (FileStream fs = File.Create(path))
				{
					BinaryFormatter bf = new BinaryFormatter();
					bf.Serialize(fs, mData);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static MapData Load(string path)
        {
			try
			{
				using (FileStream fs = File.OpenRead(path))
				{
					BinaryFormatter bf = new BinaryFormatter();
					return bf.Deserialize(fs) as MapData;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
    }

	[Serializable]
	public class MapData
    {
		public int mapSize { get; set; }
		public MapElement[,] map { get; set; }
		public string[] tSetPaths { get; set; }
    }
	[Serializable]
	public class MapElement
	{
		public int xCoo { get; private set; }
		public int yCoo { get; private set; }
		public int value { get; set; }
		

		public MapElement(int _x, int _y, int _value = 0)
        {
			xCoo = _x;
			yCoo = _y;
			value = _value;
        }
	}
}
