using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZoomNet.Models;

namespace ZoomNet.IntegrationTests.Tests
{
	public class Chat : IIntegrationTest
	{
		public async Task RunAsync(string userId, IZoomClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** ACCOUNT CHAT CHANNELS *****\n").ConfigureAwait(false);

			// GET THE CHANNELS FOR THIS USER
			var paginatedChannels = await client.Chat.GetAccountChannelsForUserAsync(userId, 100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {paginatedChannels.TotalRecords} account channels for user {userId}").ConfigureAwait(false);

			// CREATE A NEW CHANNEL
			var channel = await client.Chat.CreateAccountChannelAsync(userId, "INTEGRATION TESTING: new channel", ChatChannelType.Public, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Account channel \"{channel.Name}\" created (Id={channel.Id}").ConfigureAwait(false);

			// UPDATE THE CHANNEL
			await client.Chat.UpdateAccountChannelAsync(userId, channel.Id, "INTEGRATION TESTING: updated channel", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Account channel \"{channel.Id}\" updated").ConfigureAwait(false);

			// RETRIEVE THE CHANNEL
			channel = await client.Chat.GetAccountChannelAsync(userId, channel.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Account channel \"{channel.Id}\" retrieved").ConfigureAwait(false);

			// RETRIEVE THE CHANNEL MEMBERS
			var paginatedMembers = await client.Chat.GetAccountChannelMembersAsync(userId, channel.Id, 10, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Account channel \"{channel.Id}\" has {paginatedMembers.TotalRecords} members").ConfigureAwait(false);

			// SEND A MESSAGE TO THE CHANNEL
			var messageId = await client.Chat.SendMessageToChannelAsync(channel.Id, "This is a test from integration test", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Message \"{messageId}\" sent").ConfigureAwait(false);

			// UPDATE THE MESSAGE
			await client.Chat.UpdateMessageToChannelAsync(messageId, channel.Id, "This is an updated message from integration testing", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Message \"{messageId}\" updated").ConfigureAwait(false);

			// RETRIEVE LIST OF MESSAGES
			var paginatedMessages = await client.Chat.GetMessagesToChannelAsync(channel.Id, 100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {paginatedMessages.TotalRecords ?? paginatedMessages.Records.Length} messages in channel \"{channel.Id}\"").ConfigureAwait(false);

			// DELETE THE MESSAGE
			await client.Chat.DeleteMessageToChannelAsync(messageId, channel.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Message \"{messageId}\" deleted").ConfigureAwait(false);

			// DELETE THE CHANNEL
			await client.Chat.DeleteAccountChannelAsync(userId, channel.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Account channel \"{channel.Id}\" deleted").ConfigureAwait(false);
		}
	}
}
