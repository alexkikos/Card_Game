using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Card
    {

        int my_card;
        string type_card;

        public Card(int my_card, string type_card)
        {
            this.my_card = my_card;
            this.type_card = type_card ?? throw new ArgumentNullException(nameof(type_card));
        }
        public Card()
        {
            this.my_card = -1;
            this.type_card = "";
        }
        /// <summary>
        /// используем ивент для отображение карты
        /// </summary>
        public void ShowCurrentCard()
        {
            Console.WriteLine("Масть карты: " + type_card+ "; значение карты: " + my_card);
            
        }

        public string Type_card
        {
            get
            {
              
                return type_card;
            }
            set
            {
  
                type_card = value;
            }
        }


        public int My_card { get => my_card; set => my_card = value; }
    }
}
