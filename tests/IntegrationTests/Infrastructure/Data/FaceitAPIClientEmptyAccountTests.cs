using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Infrastructure.Data
{
    public class FaceitAPIClientEmptyAccountTests
    {
        private readonly ITestOutputHelper _output;
        private FaceitAPIClient _faceitAPIClient;
        private readonly string testUser = "Ignat";

        public FaceitAPIClientEmptyAccountTests(ITestOutputHelper output)
        {
            _output = output;
            _faceitAPIClient = new FaceitAPIClient(APIKeys.FaceitAPIKey, APIKeys.UserAPIKey);
        }

        #region Empty account
        [Fact]
        public async void FetchPlayerProfileAsyncIgnat()
        {
            var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);

            Assert.Equal("Ignat", playerProfile.Nickname);
        }

        [Fact]
        public async void FetchPlayerProfileByIdAsyncIgnat()
        {
            var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
            var playerProfileById = await _faceitAPIClient.FetchPlayerProfileByIdAsync(playerProfile.Id);

            Assert.Equal(testUser, playerProfileById.Nickname);
        }

        [Fact]
        public async void FetchMatchesAsyncIgnatLastMatch()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                var matches = await _faceitAPIClient.FetchMatchesAsync(playerProfile.Id, 1);
                Assert.True(matches.Count == 0);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchPlayerStatsAsyncIgnat()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                Task fetch() => _faceitAPIClient.FetchPlayerStatsAsync(playerProfile.Id);
                await Assert.ThrowsAsync<Exception>(fetch);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchOngoingMatchIdAsyncIgnat()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                string ongoingMatchId = await _faceitAPIClient.FetchOngoingMatchIdAsync(playerProfile.Id);
                Assert.Null(ongoingMatchId);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }
        #endregion
    }
}
