using System;
using System.Net.Http;

namespace Challenge_DEV_2023.Models
{
    public class DevChallengeBlocksData
    {
        public string[] Data { get; set; }
        public int ChunkSize { get; set; }
        public int Length { get; set; }

        public DevChallengeBlocksData(string[] data, int chunkSize, int length)
        {
            Data = data;
            ChunkSize = chunkSize;
            Length = length;
        }
    }
}

