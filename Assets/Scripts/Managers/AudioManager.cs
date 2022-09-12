using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public enum AudioChannel { Master, SFX, Music }

    public float MasterVolumePercent { get; private set; }
    public float SfxVolumePercent { get; private set; }
    public float MusicVolumePercent { get; private set; }

   [SerializeField] private AudioSource musicPlayer;
   [SerializeField] private AudioSource sfxPlayer;

   [SerializeField] private AudioClip[] music;
   [SerializeField] private AudioClip[] sfx;

    public void Play2DMusic(MusicType musicType) {
        musicPlayer.PlayOneShot(music[(int)musicType]);
    }

    public void Play2DSFX(SFXType sfxType) {
        sfxPlayer.PlayOneShot(sfx[(int)sfxType]);
    }

    public void StopAllMusic() {
        musicPlayer.Stop();
    }

    public void SetVolume(float volumePercent, AudioChannel audioChannel) {
        
        switch (audioChannel) {
            case AudioChannel.Master:
                MasterVolumePercent = volumePercent;
                break;
            case AudioChannel.SFX:
                SfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                MusicVolumePercent = volumePercent;
                break;
            default:
                throw new System.ArgumentOutOfRangeException("invalid Audio Channel");
        }

        musicPlayer.volume = MasterVolumePercent * MusicVolumePercent;
        sfxPlayer.volume = MasterVolumePercent * SfxVolumePercent;
    }

    public enum MusicType {
        Battle_Theme,
        Diamond_Capital,
        First_Village,
        Forest_Thene
    }

    public enum SFXType {
        Random_Encounter,
        Battle_Entrance,
        Select_Character_Icon,
        Character_Icon_Select_Option,
        Swap_Character,
        Close_Button,
        Modify_Party_Button,
    }
}
