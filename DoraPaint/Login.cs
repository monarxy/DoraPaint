using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyApp
{
    public class LoginValidation
    {
        public bool LoginVal(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Логин не может быть пустым!");
                return false;
            }
            else return true;
        }
    }
}
