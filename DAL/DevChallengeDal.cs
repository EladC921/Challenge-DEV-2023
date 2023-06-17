using System;
using Challenge_DEV_2023.Controllers;

namespace Challenge_DEV_2023.Models
{
	public class DAL
	{
		public DAL()
		{
            string[] Check(string[] blocks, string token)
			{
				string[] sortedArray = blocks;

				return sortedArray;
			}

			void Swap(string[] array, int index1, int index2)
			{
				string tmp = array[index1];
				array[index1] = array[index2];
				array[index2] = tmp;
			}

			bool isSequent(string block1, string block2, string token)
			{
				BlocksController bc = new BlocksController();

				return false;
			}
		}
	}
}

