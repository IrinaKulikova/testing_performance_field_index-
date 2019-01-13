using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace indexes
{
    /**
        Домашнее задание:
        Создать таблицу содержащую как минимум одно текстовым полем.
        Заполнить таблицу не менее 10000 случайными, но повторяющимися записями (можно воспользоваться, например, сервисом https://www.mockaroo.com)
        Сделать выборку для поиска количества повторений одной из записи - замерять общее время выполнения операции.
        Добавить индекс к текстовому полю, выполнить повторую выборку и сравнить время запроса с ранее полученным.
        Сделать вывод об эффективности или не эффективности использования индекстов
     */
    class Program
    {
        static void Main(string[] args)
        {
            //You shold just start this programm

            TesterDB tester = new TesterDB("TSQL_HW_3_Kulikova", "Users", @".\Users.sql");
            TesterDB testerIndexFirstName = new TesterDB("TSQL_HW_3_Kulikova_indexing_first_name", "Users", @".\Users_indexing_first_name.sql");

            tester.TestMethod("Lurette");
            testerIndexFirstName.TestMethod("Lurette");

            Console.Read();
        }
    }
}
