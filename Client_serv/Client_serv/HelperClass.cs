using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client_serv
{
    /// <summary>
    /// Определение интерфейса:
    /// набор методов и свойств без реализации. 
    /// Затем этот функционал реализуют классы,
    /// которые наследуются от интерфейса.
    /// Всё описанное в интерфейса может быть только public,
    /// по умолчанию модификатор доступа - public
    /// </summary 

    // Этот интерфейс нужен для унификации страниц.
    // Каждая страница наследуется от этого интерфейса
    // Таким образом любую страницу можно привести к этому интерфейсу
    // И обращаться к методам, описанным в данном интерфейсе

    // P.S. Если класс наследуется от интерфейса, то в классе обязательно 
    // должны быть реализованы методы и свойства описанные в интерфейсе

    public interface Ipage
    {
        void UpdateGrid();
    }

    // Вспомогающий класс
    // Статья про модификатор static - https://metanit.com/sharp/tutorial/3.6.php
    public static class HelperClass
    {
       static Queue<ListSortDirection?> SortQueueColumns;
       static SortDescriptionCollection SortList;

        public static void SaveSortDataGrid(DataGrid dg)
        {
             SortQueueColumns = new Queue<ListSortDirection?>
                        (
                            // Здесь происходит инициализация новой коллекции
                            // dg.Columns - это коллекция колонок, что-то вроде динамического массива
                            // Если вызвать метод Select, то можно привести все объекты коллекции к другому виду
                            // tmp - это любой объект коллекции. Это название может быть любым другим (Не может повторять названия уже объвяленных переменных)
                            // После знака '=>' (Лямбда-оператор) написанно к какому виду привести все объекты коллекции
                            // В данном случае select вернёт новую коллекцию содержащую направление каждой колонки
                            dg.Columns.Select(tmp => tmp.SortDirection)
                        );
            // Объявление коллекции, которая в будущем будет хранить сортировку каждой колонки до обновления таблицы данных
            SortList = new SortDescriptionCollection();

            // Добавляет в коллекцию SortList сортировку каждой колонки до обновления таблицы данных
            foreach (var j in dg.Items.SortDescriptions) SortList.Add(j);
        }
        public static void LoadSortDataGrid(DataGrid dg)
        {
            // Присвоение значений сортировки обратно
            for (int j = 0; j < SortList.Count; j++) dg.Items.SortDescriptions.Add(SortList[j]);
            if(SortQueueColumns.Count>0)
            for (int j = 0; j < dg.Columns.Count; j++) dg.Columns[j].SortDirection = SortQueueColumns.Dequeue();
        }
    }
}
