using Serilog;
using Server.Entities;

namespace Server.Core
{
    class PlayerHandler
    {
        public Player[] Players;
        public PlayerHandler(int maxPlayers)
        {
            Players = new Player[maxPlayers];
        }

        public void AddPlayer(Player player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] != null) continue;

                Players[i] = player;
                Players[i].Id = i;
                Log.Information($"Client has connected and was given the ID: {Players[i].Id}");
                return;
            }
        }

        /// <summary>
        /// Read all the data required to update the client at hand
        /// </summary>
        public void Update()
        {
            /* Fetch Data For Each User So They Can Build Their Packets */
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == null) continue;

                /* Fetch Data */
                Players[i].Update();
            }

            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == null) continue;

                var players = Players.ToList().Where(x => x != null && x.Id != Players[i].Id).ToArray();
                Players[i].Broadcast(players);
            }

        }
    }
}