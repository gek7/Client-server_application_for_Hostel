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
using Client_serv;

namespace Client_serv.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для PostsDialog.xaml
    /// </summary>
    public partial class PostsDialog : Window
    {
        Ipage CurPage;
        mode CurMode;
        int ID;
        public PostsDialog()
        {
            InitializeComponent();
        }

        public PostsDialog(mode dialogMode,Ipage page, int fieldID = -1) : this()
        {
            CurMode = dialogMode;
            ID = fieldID;
            CurPage = page;
            FillFields();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            if(Check() && Save())
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
            if (TbPost.Text == "")
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
                using(HOSTELEntities db = new HOSTELEntities())
                {
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addPost;

                        case mode.Copy:
                            addPost:
                            db.Posts.Add(new Posts() { Post = TbPost.Text });
                            break;

                        case mode.Update:
                            db.Posts.Find(ID).Post = TbPost.Text;
                            break;
                    }
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show($"Ошибка при сохранении \n {e.Message}");
                return false;
            }
            CurPage.updateGrid();
            return true;
        }

        private void FillFields()
        {
            using(HOSTELEntities DB = new HOSTELEntities())
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
                        TbPost.Text = DB.Posts.Find(ID).Post;
                        break;
                }
            }
        }
    }
}
