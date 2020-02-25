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
    /// Логика взаимодействия для MultiBuildingsDialog.xaml
    /// </summary>
    public partial class MultiBuildingsDialog
    {
        MultiPage CurPage;
        mode CurMode;
        int ID;
        public MultiBuildingsDialog()
        {
            InitializeComponent();
        }
        public MultiBuildingsDialog(mode dialogMode, MultiPage page, int fieldID = -1) : this()
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
            if (TbName.Text == "" || TbAddress.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return false;
            }
            return true;
        }

        bool Save()
        {
            int BuildID;
            try
            {
                using (HOSTELEntities db = new HOSTELEntities())
                {
                    Buildings b = new Buildings();
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addPost;

                        case mode.Copy:
                        addPost:
                            FillObject(b);
                            db.Buildings.Add(b);
                            break;

                        case mode.Update:
                            b = db.Buildings.Find(ID);
                            FillObject(b);
                            break;
                    }
                    db.SaveChanges();
                    BuildID = b.BuildingID;
                }
                CurPage.UpdateGrid(BuildID, -1);
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
            using (HOSTELEntities db = new HOSTELEntities())
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
                        Buildings b = db.Buildings.Find(ID);
                        TbName.Text = b.Name;
                        TbAddress.Text = b.Address;
                        break;
                }
            }
        }

        private void FillObject(Buildings b)
        {
            b.Name = TbName.Text;
            b.Address = TbAddress.Text;
        }

    }
}
