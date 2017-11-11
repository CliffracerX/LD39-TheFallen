using UnityEngine;
using System.Collections;


[System.Serializable]
public class DiscordJoinEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordSpectateEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordJoinRequestEvent : UnityEngine.Events.UnityEvent<DiscordRpc.JoinRequest> { }

public class DiscordHandler : MonoBehaviour
{
	public static DiscordHandler instance;
	public DiscordRpc.RichPresence presence;
	public int callbackCalls;

	public UnityEngine.Events.UnityEvent onConnect;
	public UnityEngine.Events.UnityEvent onDisconnect;
	public DiscordJoinEvent onJoin;
	public DiscordJoinEvent onSpectate;
	public DiscordJoinRequestEvent onJoinRequest;
	public DiscordRpc.EventHandlers handlers;
	public string[] characterIcos,characterNames,difficultyIcos,difficultyNames,modeNames;

	void Start()
	{
		if(!instance)
		{
			instance=this;
			InitDiscord();
		}
		UpdateState(-1, -1, 0, 0, 0, 0, 0, 0, 0, -1);
	}

	public static void UpdateState(int character, int difficulty, int health, int maxHealth, int mags, int maxMags, int ammo, int maxAmmo, int progress, int mode)
	{
		instance.UpdState(character, difficulty, health, maxHealth, mags, maxMags, ammo, maxAmmo, progress, mode);
	}

	void UpdState(int character, int difficulty, int health, int maxHealth, int mags, int maxMags, int ammo, int maxAmmo, int progress, int mode)
	{
		if(character!=-1 && difficulty!=-1)
		{
			presence.details = "In-game: "+modeNames[mode];
			presence.state = ""+health+"/"+maxHealth+" HP, "+mags+"/"+maxMags+" mags, "+ammo+"/"+maxAmmo+" ammo";
			presence.smallImageKey = characterIcos[character];
			presence.smallImageText = "Playing "+characterNames[character];
			presence.largeImageKey = difficultyIcos[difficulty];
			presence.largeImageText = "On "+difficultyNames[difficulty]+" difficulty";
		}
		else
		{
			presence.details = "In menus.";
			presence.state = "Doing things?";
			presence.smallImageKey = "";
			presence.smallImageText = "";
			presence.largeImageKey = "";
			presence.largeImageText = "";
		}
		DiscordRpc.UpdatePresence(ref presence);
	}

	void Update()
	{
		DiscordRpc.RunCallbacks();
	}

	public void OnQuit()
	{
		DiscordRpc.Shutdown();
		Application.Quit();
	}

	void OnApplicationQuit()
	{
		OnQuit();
	}

	void InitDiscord()
	{
		handlers = new DiscordRpc.EventHandlers();
		handlers.readyCallback = ReadyCall;
		handlers.errorCallback = ErrorCall;
		handlers.disconnectedCallback = DisconnectCall;
		handlers.joinCallback = JoinCall;
		handlers.spectateCallback = SpectateCall;
		handlers.requestCallback = RequestCall;
		DiscordRpc.Initialize("378971031093772288", ref handlers, true, "");
	}

	public void ReadyCall()
	{
		++callbackCalls;
		Debug.Log("DISCORD.READY");
		onConnect.Invoke();
	}

	public void ErrorCall(int errorCode, string message)
	{
		++callbackCalls;
		Debug.Log(string.Format("DISCORD.ERROR {0}: {1}", errorCode, message));
	}

	public void DisconnectCall(int errorCode, string message)
	{
		++callbackCalls;
		Debug.Log(string.Format("DISCORD.DISCONNECT {0}: {1}", errorCode, message));
		onDisconnect.Invoke();
	}

	public void JoinCall(string secret)
	{
		++callbackCalls;
		Debug.Log(string.Format("DISCORD.JOIN ({0})", secret));
		onJoin.Invoke(secret);
	}

	public void SpectateCall(string secret)
	{
		++callbackCalls;
		Debug.Log(string.Format("DISCORD.SPECTATE ({0})", secret));
		onSpectate.Invoke(secret);
	}

	public void RequestCall(DiscordRpc.JoinRequest request)
	{
		++callbackCalls;
		Debug.Log(string.Format("DISCORD.REQUEST {0}: {1}", request.username, request.userId));
		onJoinRequest.Invoke(request);
	}
}