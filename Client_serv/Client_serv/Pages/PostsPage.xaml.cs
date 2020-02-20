using Client_serv.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
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

namespace Client_serv
{

    public partial class PostsPage : Page,Ipage
    {
        public DataGrid PageDataGrid => dg;
        MainWindow OwnerPage;
        public PostsPage()
        {
            InitializeComponent();
        }
        public PostsPage(MainWindow _OwnerPage) :this()
        {
            updateGrid();
            OwnerPage = _OwnerPage;
        }


        public void updateGrid()
        {
            using (HOSTELEntities h = new HOSTELEntities())
            {
                h.Posts.Load();
                var postDB = from i in h.Posts.Local
                             select new {
                                            id = i.PostID,
                                            post = i.Post 
                                        };
                dg.SelectedValuePath = "id";
                dg.ItemsSource = postDB;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            PostsDialog dialog = new PostsDialog(mode.Add,this);
            dialog.ShowDialog();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                PostsDialog dialog = new PostsDialog(mode.Copy, this,(int)dg.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedIndex > -1)
            {
                PostsDialog dialog = new PostsDialog(mode.Update, this, (int)dg.SelectedValue);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(dg.SelectedIndex > -1)
            {
                if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены, что хотите удалить запись?","Удаление записи",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    using (HOSTELEntities DB = new HOSTELEntities())
                    {
                        DB.Posts.Remove(DB.Posts.Find(dg.SelectedValue));
                        DB.SaveChanges();
                    }
                    updateGrid();
                }
            }
            else
            {
                MessageBox.Show("Не выбрано поле!");
            }
        }

        private void BtnClosePage_Click(object sender, RoutedEventArgs e)
        {
                OwnerPage.pages.Items.Remove(OwnerPage.pages.SelectedItem);

        }
    }
}
