using FaceitStats.Core.Models;
using FaceitStats.Infrastructure.Data;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Infrastructure.Data
{
    public class FaceitAPIClientTests
    {
        private readonly ITestOutputHelper _output;
        private FaceitAPIClient _faceitAPIClient;
        private readonly string testUser = "100ch";

        public FaceitAPIClientTests(ITestOutputHelper output)
        {
            _output = output;
            _faceitAPIClient = new FaceitAPIClient(APIKeys.FaceitAPIKey, APIKeys.UserAPIKey);
        }

        #region Common account
        [Fact]
        public async void FetchPlayerProfileAsync100ch()
        {
            var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);

            Assert.Equal(testUser, playerProfile.Nickname);
        }

        [Fact]
        public async void FetchPlayerProfileByIdAsync100ch()
        {
            var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
            var playerProfileById = await _faceitAPIClient.FetchPlayerProfileByIdAsync(playerProfile.Id);

            Assert.Equal(testUser, playerProfileById.Nickname);
        }

        [Fact]
        public async void FetchMatchesAsync100chLastMatch()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                var matches = await _faceitAPIClient.FetchMatchesAsync(playerProfile.Id, 1);
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchMatchStatsAsync100chLastMatch()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                List<Match> matches = await _faceitAPIClient.FetchMatchesAsync(playerProfile.Id, 1);
                var matchStats = await _faceitAPIClient.FetchMatchStatsAsync(matches[0].Id);
                foreach (var player in matchStats[0].TeamA.Players)
                {
                    _output.WriteLine($"{player.Nickname}: {player.Kills} / {player.Deaths}");
                }
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchMatchInfoAsync100chLastMatch()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                List<Match> matches = await _faceitAPIClient.FetchMatchesAsync(playerProfile.Id, 1);
                var matchInfo = await _faceitAPIClient.FetchMatchInfoAsync(matches[0].Id);
                matchInfo.FillPartiesIndices();
                foreach (var player in matchInfo.TeamA.Players)
                {
                    _output.WriteLine($"{player.Nickname}'s party: {player.PartyIndex}");
                }
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchPlayerStatsAsync100ch()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                var playerStats = await _faceitAPIClient.FetchPlayerStatsAsync(playerProfile.Id);
                foreach (var mapStats in playerStats.MapOverallStats)
                {
                    _output.WriteLine($"{mapStats.Map}: {mapStats.Matches} matches");
                }
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchOngoingMatchIdAsync100ch()
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

        [Fact]
        public async void FetchMatchStatsAsyncBO3()
        {
            try
            {
                var matchStats = await _faceitAPIClient.FetchMatchStatsAsync("1-e9fb6ba4-b350-407e-8694-3e4f3d26d1dd");
                foreach (var match in matchStats)
                {
                    _output.WriteLine($"{match.RoundStats.RoundNumber} match");
                    foreach (var player in match.TeamA.Players)
                    {
                        _output.WriteLine($"{player.Nickname}: {player.Kills} / {player.Deaths}");
                    }
                }
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchMatchInfoAsyncBO3()
        {
            try
            {
                var matchStats = await _faceitAPIClient.FetchMatchInfoAsync("1-e9fb6ba4-b350-407e-8694-3e4f3d26d1dd");
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchOngoingMatchIdAsyncNull()
        {
            try
            {
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(testUser);
                var ongoingMatchId = await _faceitAPIClient.FetchOngoingMatchIdAsync(playerProfile.Id);
                Assert.Null(ongoingMatchId);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchOngoingMatchIdAsyncNotNull()
        {
            try
            {
                string playingUser = "M8Kdr-";
                var playerProfile = await _faceitAPIClient.FetchPlayerProfileAsync(playingUser);
                var ongoingMatchId = await _faceitAPIClient.FetchOngoingMatchIdAsync(playerProfile.Id);
                Assert.NotNull(ongoingMatchId);
                _output.WriteLine($"Match Id: {ongoingMatchId}");
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchMatchInfoAsync1v1()
        {
            try
            {
                var matchStats = await _faceitAPIClient.FetchMatchInfoAsync("1-7d3478a8-8b01-44a2-bb5f-3b656a5579aa");
                Assert.True(true);
                _output.WriteLine($"{matchStats.TeamA.Players[0].Nickname} vs {matchStats.TeamB.Players[0].Nickname}");
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchMatchStatsAsync1v1()
        {
            try
            {
                var matchStats = await _faceitAPIClient.FetchMatchStatsAsync("1-7d3478a8-8b01-44a2-bb5f-3b656a5579aa");
                foreach (var match in matchStats)
                {
                    _output.WriteLine($"{match.RoundStats.RoundNumber} match");
                    foreach (var player in match.TeamA.Players)
                    {
                        _output.WriteLine($"{player.Nickname}: {player.Kills} / {player.Deaths}");
                    }
                    foreach (var player in match.TeamB.Players)
                    {
                        _output.WriteLine($"{player.Nickname}: {player.Kills} / {player.Deaths}");
                    }
                }
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchMatchInfoAsync2v2()
        {
            try
            {
                var matchStats = await _faceitAPIClient.FetchMatchInfoAsync("1-253cffd3-9c8c-431b-9612-818458346a5c");
                Assert.True(true);
                _output.WriteLine($"{matchStats.TeamA.Players[0].Nickname} vs {matchStats.TeamB.Players[0].Nickname}");
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async void FetchMatchStatsAsync2v2()
        {
            try
            {
                var matchStats = await _faceitAPIClient.FetchMatchStatsAsync("1-253cffd3-9c8c-431b-9612-818458346a5c");
                foreach (var match in matchStats)
                {
                    _output.WriteLine($"{match.RoundStats.RoundNumber} match");
                    foreach (var player in match.TeamA.Players)
                    {
                        _output.WriteLine($"{player.Nickname}: {player.Kills} / {player.Deaths}");
                    }
                    foreach (var player in match.TeamB.Players)
                    {
                        _output.WriteLine($"{player.Nickname}: {player.Kills} / {player.Deaths}");
                    }
                }
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }
        #endregion
    }
}
