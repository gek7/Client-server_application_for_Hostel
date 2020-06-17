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
using System.Data.SqlClient;

namespace Client_serv.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для PostsDialog.xaml
    /// </summary>
    public partial class PostsDialog : Window
    {
        PostsPage CurPage;
        mode CurMode;
        int ID;
        public PostsDialog()
        {
            InitializeComponent();
        }

        public PostsDialog(mode dialogMode, PostsPage page, int fieldID = -1) : this()
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
                    Posts p = new Posts();
                    switch (CurMode)
                    {
                        case mode.Add:
                            goto addPost;

                        case mode.Copy:
                        addPost:
                            p.Post = TbPost.Text;
                            db.Posts.Add(p);
                            break;

                        case mode.Update:
                            p=db.Posts.Find(ID);
                            p.Post = TbPost.Text;
                            break;
                    }
                    db.SaveChanges();
                    CurPage.UpdateGrid(p.PostID);
                }
            }
            catch(Exception e)
            {
                if (e.HResult == -2146233087)
                {
                    MessageBox.Show($"Такая должность уже существует");
                }
                else
                {
                    MessageBox.Show($"Ошибка при сохранении \n {e.Message}");
                }
                return false;
            }
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
