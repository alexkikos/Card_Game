using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;



namespace Core
{
	public sealed class Games
	{
		//delegate void Swaps(Card a, Card b);
		/// <summary>
		/// набор подготовительных методов
		/// формирование карт, перемешка карт, перемешка игроков, раздача карт
		/// </summary>
		delegate void CardsRules();
		/// <summary>
		/// методы для игры, анализ карт, сбор карт со стола итп
		/// </summary>
		delegate void GameBeginFunc();
		/// <summary>
		/// событие дебагера, выводит всю информацию о картах Player в логи
		/// </summary>
		public event EventHandler DebaggerCards;
		public event EventHandler LeaveFromMatch;
		/// <summary>
		/// собираю в делегат все методы управления картами
		/// </summary>
		CardsRules cardsRules;
		/// <summary>
		/// собираю все методы управления игрой
		/// </summary>
		GameBeginFunc gameBeginFunc;
		/// <summary>
		/// массив игроков
		/// </summary>
		List<Player> all_players;
		/// <summary>
		/// коллода карт
		/// </summary>
		LinkedList<Card> cards;
		static Random rand;

		/// <summary>
		/// первичное формирование колоды карт
		/// </summary>
		public void DealCards()
		{
			if (cards.Count > 0) cards.Clear();

			for (int i = 6; i < 15; i++)
			{
				cards.AddLast(new Card(i, "Черва"));
				cards.AddLast(new Card(i, "Пика"));
				cards.AddLast(new Card(i, "Бубна"));
				cards.AddLast(new Card(i, "Крести"));
			}
		}

		public void ShuffleCards()
		{
			int first;
			int second;
			//Swaps swap = delegate (Card a, Card b)
			//{
			//	Card temp = b;
			//	
			//	b = a;
			//	a = temp;
			//};
			for (int i = 0; i < rand.Next(20, 50); i++)//15-25 раз тусую карты, тусование карт происходит в 4 рандом способа
			{
				do
				{
					first = rand.Next(0, 36);//выбираю рандом карту
					second = rand.Next(1, 35);//выбираю рандомно позицию вставки карты			
				} while (first == second);
				Card a = cards.ElementAt(first);///нашел карту рандом для вырезания
				cards.Remove(a);
				switch (rand.Next(1, 5))
				{
					case 1: cards.AddLast(a); break;
					case 2: cards.AddFirst(a); break;
					case 3: cards.AddBefore(cards.Find(cards.ElementAt(second)), a); break;
					case 4: cards.AddAfter(cards.Find(cards.ElementAt(second)), a); break;
				}
			}
		}

		/// <summary>
		/// change player position on new game
		/// </summary>
		void ChangePlayerPosition()
		{
			int first;
			int second;
			for (int i = 0; i < all_players.Count + rand.Next(5,10); i++)//рандомное количество раз прохода. метод меняет игроков местами
			{
				do
				{
					first = rand.Next(0, all_players.Count);//выбираю рандом слот
					second = rand.Next(0, all_players.Count);//выбираю рандомно позицию вставки в слот			
				} while (first == second);
				Player a = all_players.ElementAt(first);///нашел карту рандом для вырезания
				all_players.Remove(a);
				all_players.Insert(second, a);
			}
		}

		/// <summary>
		/// по очереди раздаю каждому игроку по одной карте, количество проходов зависит от количества игроков
		/// </summary>
		public void DistributionCards()
		{
			while (cards.Count > 0)//пока в раздаточной колоде есть карты, раздаем
			{
				for (int i = 0; i < all_players.Count; i++)
				{
					if (cards.Count == 0) return;
					all_players[i].List_card.Add(cards.ElementAt(0));
					cards.RemoveFirst();
				}
			}
		}





		void GameProcess()
		{
			int max, index;
			max = index = 0;

			List<Card> card_from_label = new List<Card>();//помещаю первые карты игроков в новый массив, который хранит все первые карты и игрок победивший добавит к себе карты из 
			//этого массива
			for (int i = 0; i < all_players.Count; i++)
			{
				//если дебаг включен то активируем евент
				if (DebaggerCards != null) DebaggerCards(all_players[i], null);
				///нахожу максимальную карту за столом
				if (all_players[i].List_card.Count > 1)
				{
					if (all_players[i].List_card[0].My_card > max)
					{
						max = all_players[i].List_card[0].My_card;
						index = i;
					}
					card_from_label.Add(all_players[i].List_card[0]);           ///собираю выиграш в массив, и затем данный масив добавляю к нашему игроку
					all_players[i].List_card.Remove(all_players[i].List_card[0]);//удаляю у этого игрока первую карту
				}
				else
				{
					if (LeaveFromMatch != null) LeaveFromMatch(all_players[i], null);//евент на удаление игрока
					all_players.Remove(all_players[i]);//если у игрока нет больше карт, удаляем его
				}
			}
			all_players[index].List_card.AddRange(card_from_label);//в индекс записали нашу победную карту - игрока, к нему и добавляю массив собранных первых карт со стола		
			Console.WriteLine("Winner is : " + all_players[index].Player_name);
			Console.WriteLine("Total players in game : " + all_players.Count);
		}

		void PushKeyCOntinue()
		{
			Console.Write("Push key for continue(x - new game): ");
			string s = Console.ReadLine();
			if (s == "x") StartGame();
			Console.Clear();
		}

		void ShowFirstCards()
		{
			Console.WriteLine("==================================GAME LOBBI===========================================");
			foreach (var item in all_players)
			{
				Console.ForegroundColor = (ConsoleColor)rand.Next(1, 16);
				item.ShowFirst();
				Console.WriteLine("\n\n\n");
			}
			Console.WriteLine("====================================END LOBBI===========================================");
			Console.ResetColor();
		}


		public void StartGame()
		{
			PlayerIni();
			cardsRules();
			do
			{
				gameBeginFunc();

			} while (all_players.Count != 1);
		}

		void PlayerIni()
		{
			Console.Clear();
			if (all_players.Count > 0)
			{
				all_players.Clear();
				GC.Collect();
			}
			Console.Write("Enter name: ");
			string my_name = Console.ReadLine();
			Console.Write("Enter total players: ");
			if (int.TryParse(Console.ReadLine(), out int total_players))
			{
				if (total_players < 2) total_players = 2;
				else if (total_players > 36) total_players = 36;
				for (int i = 0; i < total_players; i++)
				{
					if (i == 0)
					{
						Player a2 = new Player(my_name);
						all_players.Add(a2);
					}
					else
					{
						Player a1 = new Player("Player " + (i + 1).ToString());
						all_players.Add(a1);
					}

				}
			}
			else
			{
				Console.WriteLine("Wrong input");
				PlayerIni();
			}

		}




		public List<Player> All_players { get => all_players; private set => all_players = value; }

		public Games()
		{
			all_players = new List<Player>();
			cards = new LinkedList<Card>();
			rand = new Random();
			///добавляю к делегату методы для игры
			cardsRules += DealCards;
			cardsRules += ShuffleCards;
			cardsRules += ChangePlayerPosition;
			cardsRules += DistributionCards;

			gameBeginFunc += ShowFirstCards;
			gameBeginFunc += GameProcess;
			gameBeginFunc += PushKeyCOntinue;
		}
	}
}
