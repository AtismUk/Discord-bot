using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_bot.Model
{
    public class ManageVote
    {
        private readonly SocketSlashCommandDataOption _command;
        private readonly DiscordSocketClient _client;

        public ManageVote(SocketSlashCommandDataOption command, DiscordSocketClient client)
        {
            _command = command;
            _client = client;
        }

        public EmbedBuilder CreateVote()
        {
        }

    }
}
