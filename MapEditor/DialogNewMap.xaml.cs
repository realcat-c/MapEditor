/*****************************************************************************
* Project: MapEditor
* Date   : 03.03.2022
* Author : xxxxxxxxxxx (JA)
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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for DialogNewMap.xaml
    /// </summary>
    public partial class DialogNewMap : Window
    {
        readonly MainWindow mWindow;
        public DialogNewMap(MainWindow _mWindow)
        {
            InitializeComponent();
            mWindow = _mWindow;
        }

        void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
                int result = int.Parse(MapSizeTxt.Text);

                if (result <= 0)
                    return;
                if(result>=30)
                {
                    MessageBox.Show("Zu groß.");
                    return;
                }

                mWindow.size = result;
                DialogResult = true;
                return;

        }
        void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
