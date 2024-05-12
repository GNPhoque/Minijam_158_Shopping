using System.Linq;
using UnityEngine;

public enum MusicLoopTracks
{
	None,
	PreIntro,
	PreIntroWBacking,
	Intro,
	Part1WOSnare,
	Part1WSnare,
	Part2,
	Ending,
	Outro
}

public class AudioManager : MonoBehaviour
{
	[Header("Music")]
	[SerializeField] AudioSource[] musicLoops;
	[SerializeField] AudioSource sfx;
	int currentPlaying = 0;

	public static AudioManager instance;

	public bool isMenu = false;

	public bool cardShown;
	public bool firstBuy;
	public bool firstCriteriaReached;
	public bool secondCriteriaReached;
	public bool thirdCriteriaReached;

	public bool endingPlayed = false;

	float looptime = 0f;
	public MusicLoopTracks nextMuteChangeMethod = MusicLoopTracks.None;

	[Header("Sound Effects")]
    [SerializeField] AudioClip[] clickClips;
    [SerializeField] AudioClip[] buyClips;
	[SerializeField] AudioClip caughtShoppingSFX;

    public void PlayClick(int index = 0, bool random = false)
    {
		if (random) PlayRandomSfx(clickClips);
		else PlaySfx(clickClips[index]);
    }

	public void PlayRandomClick()
	{
		PlayClick(random: true);
	}
	
	public void PlayBuy(int index = 0, bool random = false)
	{
        if (random) PlayRandomSfx(buyClips);
        else PlaySfx(buyClips[index]);
    }

	public void PlayRandomBuy()
	{
		PlayBuy(random: true);
	}

	public void PlayCaughtShopping()
	{
		PlaySfx(caughtShoppingSFX);
	}

	public void PlayRandomSfx(AudioClip[] soundEffects)
	{
        int index = UnityEngine.Random.Range(0, soundEffects.Length - 1);
		PlaySfx(soundEffects[index]);
    }

    private void Awake()
	{
		if (instance) Destroy(gameObject);
		instance = this;
		//DontDestroyOnLoad(gameObject);
	}

    private void Update()
	{
		if (nextMuteChangeMethod == MusicLoopTracks.None) return;

		float newLoopTime = musicLoops[currentPlaying].time;
		if(newLoopTime < looptime) //looped this frame
		{
			PlayMusicLoop(nextMuteChangeMethod);
		}
		looptime = newLoopTime;
	}

	public void ResetLoop()
	{
		cardShown = false;
		firstBuy = false;
		firstCriteriaReached = false;
		secondCriteriaReached = false;
		thirdCriteriaReached = false;

		for (int i = 0; i < musicLoops.Length; i++)
		{
			musicLoops[i].Play();
		}
	}

	public void PlayMusicLoop(MusicLoopTracks tracksToMute)
	{
		if (isMenu) return;
		print($"Changing Play Loop to {nextMuteChangeMethod}");
		for (int i = 0; i < musicLoops.Length; i++)
		{
			musicLoops[i].mute = tracksToMute != (MusicLoopTracks)i + 1; //mute all tracks except selected one
		}

		MusicLoopTracks wantToSwitchTo = MusicLoopTracks.None;

		if (nextMuteChangeMethod == MusicLoopTracks.Intro) wantToSwitchTo = MusicLoopTracks.Part1WOSnare;

		else if (nextMuteChangeMethod == MusicLoopTracks.Ending) wantToSwitchTo = MusicLoopTracks.Outro;

		else if (nextMuteChangeMethod == MusicLoopTracks.Outro)
		{
			AudioSource lastSource = musicLoops[(int)MusicLoopTracks.Outro - 1];
            lastSource.loop = false;
			lastSource.Play();
			nextMuteChangeMethod = MusicLoopTracks.None;
		}

		if (wantToSwitchTo == MusicLoopTracks.None) nextMuteChangeMethod = MusicLoopTracks.None;
		else nextMuteChangeMethod = wantToSwitchTo;
	}

	public void StopAllLoops()
	{
		for (int i = 0; i < musicLoops.Length; i++)
		{
			musicLoops[i].Stop();
		}
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
		endingPlayed = true;
	}

	public void PlayLoopOutro()
	{
		nextMuteChangeMethod = MusicLoopTracks.Outro;
	}
}
