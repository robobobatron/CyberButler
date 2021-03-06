﻿using CyberButler.DatabaseRecords;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;

namespace CyberButler.Commands
{
    [Group("customcommand", CanInvokeWithoutSubcommand = false), Aliases("cc")]
    internal class CustomCommand
    {
        [Command("add"), 
            Aliases("create"), 
            Description("Example: !customcommand add test This is a test command.")]   
        public async Task Add(CommandContext _ctx,
            [Description("The command name")]String _command,
            [Description("The text to display")][RemainingText]String _text)
        {
            var record = new CommandRecord();
            var server = _ctx.Guild.Id.ToString();
            var existingCustom = record.SelectOne(server, _command);
            var existingDelivered = _ctx.Client.GetCommandsNext().RegisteredCommands.ContainsKey(_command);

            if (existingCustom == null && !existingDelivered)
            {
                record.Server = server;
                record.Command = _command;
                record.Text = _text;

                record.Insert();

                await _ctx.RespondAsync($"Added \"{record.Command}\".");
            }
            else
            {
                await _ctx.RespondAsync($"\"{_command}\" already exists on {_ctx.Guild.Name}.");
            }
        }

        [Command("get"), 
            Aliases("list", "read"), 
            Description("Example: !customcommand get")]
        public async Task Get(CommandContext _ctx)
        {
            var server = _ctx.Guild.Id.ToString();
            var results = new CommandRecord().SelectAll(server);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{_ctx.Guild.Name} Custom Commands",
                Color = DiscordColor.Azure
            };

            foreach (var record in results)
            {
                if (embed.Fields.Count < 25)
                {
                    embed.AddField(record.Command,
                        record.Text.Length <= 1024 ? record.Text : record.Text.Substring(0, 1024));
                }
                else
                {
                    await _ctx.RespondAsync("", embed: embed);

                    embed = new DiscordEmbedBuilder
                    {
                        Title = $"{_ctx.Guild.Name} Custom Commands",
                        Color = DiscordColor.Azure
                    };
                }
            }

            if (embed.Fields.Count > 0)
            {
                await _ctx.RespondAsync("", embed: embed);
            }
        }

        [Command("modify"), 
            Aliases("update"), 
            Description("Example: !customcommand modify test New text")]
        public async Task Modify(CommandContext _ctx, 
            [Description("The command name")]String _command, 
            [Description("Updated text")][RemainingText]String _text)
        {
            var record = new CommandRecord();
            var server = _ctx.Guild.Id.ToString();

            if (record.SelectOne(server, _command) != null)
            {
                await _ctx.RespondAsync($"Are you sure you want to update {_command}? (Yes/No)");
                var interactivity = _ctx.Client.GetInteractivityModule();
                var msg = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == _ctx.User.Id, 
                    TimeSpan.FromMinutes(1));

                if (msg.Message.Content.ToLower() == "yes")
                {
                    record.Update(server, _command, _text);
                    await _ctx.RespondAsync($"\"{_command}\" updated.");
                }
                else
                {
                    await _ctx.RespondAsync($"Update canceled.");
                }
            }
            else
            {
                await _ctx.RespondAsync($"\"{_command}\" does not exist on {_ctx.Guild.Name}.");
            }
        }

        [Command("delete"), 
            Aliases("remove"), 
            Description("Example: !customcommand delete test")]
        public async Task Delete(CommandContext _ctx, 
            [Description("The command name to delete")]String _command)
        {
            var record = new CommandRecord();
            var server = _ctx.Guild.Id.ToString();

            if (record.SelectOne(server, _command) != null)
            {
                await _ctx.RespondAsync($"Are you sure you want to delete {_command}? (Yes/No)");
                var interactivity = _ctx.Client.GetInteractivityModule();
                var msg = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == _ctx.User.Id, 
                    TimeSpan.FromMinutes(1));

                if (msg.Message.Content.ToLower() == "yes")
                {
                    record.Delete(server, _command);
                    await _ctx.RespondAsync($"\"{_command}\" deleted.");
                }
                else
                {
                    await _ctx.RespondAsync($"Delete canceled.");
                }
            }
            else
            {
                await _ctx.RespondAsync($"\"{_command}\" does not exist on {_ctx.Guild.Name}.");
            }
        }
    }
}