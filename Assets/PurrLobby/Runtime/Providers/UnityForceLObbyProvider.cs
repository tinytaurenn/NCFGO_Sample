using PurrNet.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.Events;

using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace PurrLobby.Providers
{
    public class UnityForceLObbyProvider : MonoBehaviour, ILobbyProvider
    {
        public event UnityAction<string> OnLobbyJoinFailed;
        public event UnityAction OnLobbyLeft;
        public event UnityAction<Lobby> OnLobbyUpdated;
        public event UnityAction<List<LobbyUser>> OnLobbyPlayerListUpdated;
        public event UnityAction<List<FriendUser>> OnFriendListPulled;
        public event UnityAction<string> OnError;

        public enum LobbyType
        {
            Private,
            Public,
        }

        public bool IsUnityServiceAvailable
        {
            get { return UnityServices.State != ServicesInitializationState.Uninitialized && AuthenticationService.Instance.IsSignedIn; }
        }

        public Task<Lobby> CreateLobbyAsync(int maxPlayers, Dictionary<string, string> lobbyProperties = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<FriendUser>> GetFriendsAsync(LobbyManager.FriendFilter filter)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetLobbyDataAsync(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<LobbyUser>> GetLobbyMembersAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetLocalUserIdAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task InviteFriendAsync(FriendUser user)
        {
            return Task.CompletedTask;
        }

        public Task<Lobby> JoinLobbyAsync(string lobbyId)
        {
            throw new System.NotImplementedException();
        }

        public Task LeaveLobbyAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task LeaveLobbyAsync(string lobbyId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Lobby>> SearchLobbiesAsync(int maxRoomsToFind = 10, Dictionary<string, string> filters = null)
        {
            throw new System.NotImplementedException();
        }

        public Task SetAllReadyAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task SetIsReadyAsync(string userId, bool isReady)
        {
            throw new System.NotImplementedException();
        }

        public Task SetLobbyDataAsync(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public Task SetLobbyStartedAsync()
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }
    }
}
