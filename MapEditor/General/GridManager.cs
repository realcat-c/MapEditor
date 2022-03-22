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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

//unsaved progress
//save load
//---pair imgpath + tagNum
//new file
//edit tilemap
//xml settings

namespace MapEditor
{
    class GridManager
    {
        const string DIALOG_FILTER = "Data Files(*.tmap)|*.tmap";
        Image[,] imgMap;
        readonly Canvas cvsGrid;
        MainWindow mainWindow;
        TileSetData[] tileSet;
        public PaintMode pMode { get; set; }

        public GridManager(Canvas _cvsGrid, MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
            cvsGrid = _cvsGrid;
            pMode = PaintMode.Brush;
        }
        public void NewMap(int _size)
        {
            imgMap = CreateGrid(_size);
            FillGrid(_size);
        }

        Image[,] CreateGrid(int _size)
        {
            if(cvsGrid.Children.Count>0)
                cvsGrid.Children.Clear();
            return new Image[_size, _size];
        }
        void FillGrid(int _size)
        {
            float elementSize = (float)cvsGrid.Width / _size;
            TileSetData img = mainWindow.defaultImg;

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    imgMap[x, y] = new Image()
                    {
                        Source = img.ImageData,
                        Width = elementSize,
                        Height = elementSize,
                        Tag = new ImgData(x, y)
                    };
                    imgMap[x, y].MouseDown += new MouseButtonEventHandler(OnGridClick);
                    imgMap[x, y].MouseEnter += new MouseEventHandler(OnGridClick);

                    Canvas.SetLeft(imgMap[x, y], elementSize * x);
                    Canvas.SetTop(imgMap[x, y], elementSize * y);

                    cvsGrid.Children.Add(imgMap[x, y]);
                }
            }
        }

        public void OnGridClick(object sender, MouseEventArgs e)
        {
            bool isMouseDown = Mouse.LeftButton == MouseButtonState.Pressed;
            if(isMouseDown)
            {
                switch (pMode)
                {
                    case PaintMode.Brush:
                        PaintSingle(sender);
                        break;
                    case PaintMode.Erase:
                        EraseSingle(sender);
                        break;
                    case PaintMode.Fill:
                        PaintFill();
                        break;
                }

                mainWindow.isUnsavedProgress = true;
            }
        }

        public void PaintSingle(object sender)
        {
            object select = mainWindow.ImgSelect.SelectedItem;
            if (select == null) return;

            Image img = (Image)sender;

            TileSetData selImg = (TileSetData)select;
            ImgData imgData = (ImgData)img.Tag;

            img.Source = selImg.ImageData;
            imgData.ImgNum = (int)selImg.TagData;
        }
        public void EraseSingle(object sender)
        {
            object select = mainWindow.eraseImg;
            if (select == null) return;

            Image img = (Image)sender;

            TileSetData selImg = (TileSetData)select;
            ImgData imgData = (ImgData)img.Tag;

            img.Source = selImg.ImageData;
            imgData.ImgNum = (int)selImg.TagData;
        }
        public void PaintFill()
        {
            object select = mainWindow.ImgSelect.SelectedItem;
            if (select == null) return;

            TileSetData selImg = (TileSetData)select;

            foreach (Image element in cvsGrid.Children)
            {

                element.Source = selImg.ImageData;
                ImgData imgData = (ImgData)element.Tag;
                imgData.ImgNum = (int)selImg.TagData;
            }
        }

        public void Save()
        {
            MapData mapData = new MapData();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = DIALOG_FILTER;
            string path = null;
            //folder
            if (sfd.ShowDialog() ?? false)
                path = sfd.FileName;
            else return;

            mapData.mapSize = mainWindow.size;
            mapData.map = new MapElement[mainWindow.size, mainWindow.size];
            for (int x = 0; x < mainWindow.size; x++)
            {
                for (int y = 0; y < mainWindow.size; y++)
                {
                    ImgData imgData = (ImgData)imgMap[x, y].Tag;

                    mapData.map[x, y] = new MapElement(x, y, imgData.ImgNum);
                }
            }
            MapDataSerializer.Save(path, mapData);
            mainWindow.isUnsavedProgress = false;
            //TileSetData[] selData = (TileSetData[])mainWindow.ImgSelect.ItemsSource;

        }
        public void Load()
        {
            MapData mapData = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = DIALOG_FILTER;
            if (ofd.ShowDialog() ?? false)
            {
                string filename = ofd.FileName;
                try
                {
                    mapData = MapDataSerializer.Load(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else return;

            mainWindow.size = mapData.mapSize;
            imgMap = CreateGrid(mainWindow.size);
            float elementSize = (float)cvsGrid.Width / mainWindow.size;

            foreach (MapElement mElement in mapData.map)
            {
                int x = mElement.xCoo, y = mElement.yCoo;
                imgMap[x, y] = new Image()
                {
                    Source = mainWindow.tileSet[mElement.value].ImageData,
                    Width = elementSize,
                    Height = elementSize,
                    Tag = new ImgData(x, y, mElement.value)
                };
                imgMap[x, y].MouseDown += new MouseButtonEventHandler(OnGridClick);
                imgMap[x, y].MouseEnter += new MouseEventHandler(OnGridClick);

                Canvas.SetLeft(imgMap[x, y], elementSize * x);
                Canvas.SetTop(imgMap[x, y], elementSize * y);

                cvsGrid.Children.Add(imgMap[x, y]);
            }
        }
    }
    //class GridElement
    //{
    //    public Image elementImage;
    //}

    class ImgData
    {
        public int XCoo { get; set; }
        public int YCoo { get; set; }
        public int ImgNum { get; set; }
        public ImgData(int _x, int _y, int _imgNum = 0)
        {
            XCoo = _x;
            YCoo = _y;
            ImgNum = _imgNum;
        }
    }
}
