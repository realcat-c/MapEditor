/*****************************************************************************
* Project: MapEditor
* Date   : 03.03.2022
* Author : xxxxxxxxxxxx (JA)
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

//////////////////////////
///////Tileset in /Tileset/ von https://opengameart.org/content/tileset-1bit-color
///////////////////////////
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MapEditor.General;
using System.IO;

namespace MapEditor
{
    enum PaintMode { Brush, Erase, Fill}
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DIALOG_FILTER = "Data Files|*.tmap";
        public bool isUnsavedProgress { set; private get; }
        bool isExitAlready = false;
        GridManager gridManager;
        public int size { get; set; }
        public TileSetData[] tileSet { get; set; }
        public TileSetData defaultImg { get; set; }
        public TileSetData eraseImg { get; set; }
        EditorStateManager editorStateManager;
        public MainWindow()
        {
            size = 0;
            InitializeComponent();
            CreateTileSet();
            NewMap();
            Width = 200;
            Height = 100;
            editorStateManager = new EditorStateManager(this);
        }
        void CreateTileSet()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.IndexOf("MapEditor")) + @"MapEditor\MapEditor\TileSet\";
            string[] files = Directory.GetFiles(path);
            tileSet = new TileSetData[files.Length];
            int index = 0;
            foreach(string file in files)
            {
                tileSet[index] = new TileSetData { ImageData = LoadImage(file), TagData = index };
                index++;
            }
            ImgSelect.ItemsSource = tileSet;
            defaultImg = tileSet[0];
            eraseImg = tileSet[tileSet.Length - 1];
        }

        public BitmapImage LoadImage(string filename)
        {
            return new BitmapImage(new Uri(filename, UriKind.Absolute));
        }
        void NewMap()
        {
            gridManager = new GridManager(CvsGrid, this);
            gridManager.NewMap(size);
            isUnsavedProgress = true;
        }

        private void OnNewMapExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new DialogNewMap(this);
            bool? result = dialog.ShowDialog();
            if (result == true) NewMap();
            else if (result == false) return;
            else MessageBox.Show("Abbruch wegen Eingabefehler.");

        }
        private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            gridManager.Save();
        }
        private void OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            gridManager.Load();
        }
        private void OnExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (isUnsavedProgress)
            {
                bool? result = UnsavedProgress();
                if (result == true)
                {
                    gridManager.Save(); 
                }

                if (result == null)
                {
                    return;
                }
            }
            isExitAlready = true;
            Application.Current.Shutdown();
        }
        private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void BrushClick(object sender, EventArgs e)
        {
            gridManager.pMode = PaintMode.Brush;
        }
        void EraseClick(object sender, EventArgs e)
        {
            gridManager.pMode = PaintMode.Erase;
        }
        void FillClick(object sender, EventArgs e)
        {
            gridManager.pMode = PaintMode.Fill;
        }

        bool? UnsavedProgress()
        {
            MessageBoxResult result = MessageBox.Show("Es gibt nicht gespeicherten Fortschritt. Wollen sie jetzt Speichern?", "Potzblitz!", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                return true;
            if (result == MessageBoxResult.No)
                return false;
            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isUnsavedProgress && !isExitAlready)
            {
                bool? result = UnsavedProgress();
                if (result == true)
                {
                    gridManager.Save();
                }

                if (result == null)
                {
                    e.Cancel = true;
                }
                //Application.Current.Shutdown();
            }
            editorStateManager.WriteProgramState();
        }
    }

    public class TileSetData
        {
        //private string _Title;
        //public string Title
        //{
        //    get { return this._Title; }
        //    set { this._Title = value; }
        //}
        private object _TagData;
        public object TagData
        {
            get { return this._TagData; }
            set { this._TagData = value; }
        }

        private BitmapImage _ImageData;
        public BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }
    }
    //<TextBlock Text="{Binding Title}" HorizontalAlignment="Stretch" VerticalAlignment="Bottom" />

                    //    <ListView.ItemTemplate>
                    //    <DataTemplate>
                    //        <StackPanel Orientation = "Vertical" Width="100" Height="100">
                    //            <Image Source = "{Binding ImageData}" VerticalAlignment="Top" Stretch="Uniform" />
                    //            <TextBlock Text = "{Binding Title}"  VerticalAlignment="Bottom" FontStretch="Expanded"/>
                    //        </StackPanel>
                    //    </DataTemplate>
                    //</ListView.ItemTemplate>
}
