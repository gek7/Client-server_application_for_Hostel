using Client_serv.Pages;
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
using System.Windows.Shapes;

namespace Client_serv.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для RoomTypesDialog.xaml
    /// </summary>
    public partial class RoomTypesDialog : Window
    {
        RoomTypesPage CurPage;
        mode CurMode;
        int ID;
        public RoomTypesDialog()
        {
            InitializeComponent();
        }

        public RoomTypesDialog(mode dialogMode, RoomTypesPage page, int fieldID = -1) : this()
        {
            CurMode = dialogMode;
            ID = fieldID;
            CurPage = page;
            FillFields();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            if (Check() && Save())
            {
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        bool Check()
        {
            if (TbRoomTypes.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return false;
            }
            return true;
        }

        bool Save()
        {
            try
            {
                using (HOSTELEntities db = new HOSTELEntities())
                {
                    RoomTypes r = new RoomTypes();
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addPost;

                        case mode.Copy:
                        addPost:
                            r.RoomType = TbRoomTypes.Text;
                            db.RoomTypes.Add(r);
                            break;

                        case mode.Update:
                            r = db.RoomTypes.Find(ID);
                            r.RoomType = TbRoomTypes.Text;
                            break;
                    }
                    db.SaveChanges();
                    CurPage.UpdateGrid(r.RtyID);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при сохранении \n {e.Message}");
                return false;
            }
            return true;
        }

        private void FillFields()
        {
            using (HOSTELEntities DB = new HOSTELEntities())
            {
                switch (CurMode)
                {
                    case mode.Add:
                        Title = "Добавление";
                        break;

                    case mode.Copy:
                        Title = "Добавление на основе существующего";
                        goto fill;

                    case mode.Update:
                        Title = "Изменение";
                    fill:
                        TbRoomTypes.Text = DB.RoomTypes.Find(ID).RoomType;
                        break;
                }
            }
        }
    }
}
