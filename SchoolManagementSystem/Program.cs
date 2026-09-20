using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Services;
using SchoolManagementSystem.UI;


namespace SchoolManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
          var menu = new Menu();
            menu.ShowMainMenu();
        }
    }
}
