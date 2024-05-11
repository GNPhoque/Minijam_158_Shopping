using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MusicLoopTracks
{
	None,
	PreIntro,
	PreIntroWBacking,
	Intro,
	Part1WSnare,
	Part1WOSnare,
	Part2,
	Ending,
	Outro
}

public class AudioManager : MonoBehaviour
{
	[SerializeField] AudioSource[] musicLoops;
	[SerializeField] AudioSource sfx;

	public static AudioManager instance;

	public bool cardShown;
	public bool firstBuy;
	public bool oneKReached;
	public bool twoKReached;
	public bool fiveKReached;

	float looptime = 0f;
	[SerializeField] MusicLoopTracks nextMuteChangeMethod = MusicLoopTracks.None;

	private void Awake()
	{
		if (instance)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (nextMuteChangeMethod == MusicLoopTracks.None) return;

		float newLoopTime = musicLoops[0].time;
		if(newLoopTime < looptime) //looped this frame
		{
			PlayMusicLoop(nextMuteChangeMethod);
		}
		looptime = newLoopTime;
	}

	public void ResetTriggers()
	{
		cardShown = false;
		firstBuy = false;
		oneKReached = false;
		twoKReached = false;
		fiveKReached = false;
	}

	public void PlayMusicLoop(MusicLoopTracks tracksToMute)
	{
		print($"Changing Play Loop to {nextMuteChangeMethod}");
		for (int i = 0; i < musicLoops.Length; i++)
		{
			musicLoops[i].mute = tracksToMute != (MusicLoopTracks)i + 1; //mute all tracks except selected one
		}

		if (nextMuteChangeMethod == MusicLoopTracks.Ending) nextMuteChangeMethod = MusicLoopTracks.Outro;
		else if (nextMuteChangeMethod == MusicLoopTracks.Outro)
		{
			musicLoops.Last().loop = false;
			nextMuteChangeMethod = MusicLoopTracks.None;
		}
		else nextMuteChangeMethod = MusicLoopTracks.None;
	}

	public void ForcePlayMusicLoop()
	{
		looptime = float.MaxValue;
	}

	public void PlaySfx(AudioClip clip)
	{
		sfx.PlayOneShot(clip);
	}

	public void PlayLoopPreIntro()
	{
		nextMuteChangeMethod = MusicLoopTracks.PreIntro;
	}

	public void PlayLoopPreIntroWithBass()
	{
		nextMuteChangeMethod = MusicLoopTracks.PreIntroWBacking;
	}

	public void PlayLoopIntro()
	{
		nextMuteChangeMethod = MusicLoopTracks.Intro;
	}

	public void PlayLoopPart1WOSnare()
	{
		nextMuteChangeMethod = MusicLoopTracks.Part1WOSnare;
	}

	public void PlayLoopPart1WSnare()
	{
		nextMuteChangeMethod = MusicLoopTracks.Part1WSnare;
	}

	public void PlayLoopPart2()
	{
		nextMuteChangeMethod = MusicLoopTracks.Part2;
	}

	public void PlayLoopEnding()
	{
		nextMuteChangeMethod = MusicLoopTracks.Ending;
	}

	public void PlayLoopOutro()
	{
		nextMuteChangeMethod = MusicLoopTracks.Outro;
	}
}
