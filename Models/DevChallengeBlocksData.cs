using System;
namespace Challenge_DEV_2023.Models
{
	public class BlocksData
	{
        private string[] data;
        private int chunkSize;
        private int length;

        public string[] Data { get => data; set => data = value; }
        public int ChunkSize { get => chunkSize; set => chunkSize = value; }
        public int Length { get => length; set => length = value; }

        public BlocksData(string[] data, int chunkSize, int length)
        {
            Data = data;
            ChunkSize = chunkSize;
            Length = length;
        }
    }
}

