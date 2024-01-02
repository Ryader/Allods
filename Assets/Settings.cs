using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _graphicsDropdown;
    [SerializeField] private Toggle _fullscreenToggle;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        _resolutionDropdown.ClearOptions();
        List<string> options = new();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = FormatResolution(resolutions[i]);
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        _graphicsDropdown.value = QualitySettings.GetQualityLevel();
        _graphicsDropdown.RefreshShownValue();

        _fullscreenToggle.isOn = Screen.fullScreen;
    }

    private string FormatResolution(Resolution resolution)
    {
        StringBuilder sb = new();
        sb.Append(resolution.width);
        sb.Append(" x ");
        sb.Append(resolution.height);
        return sb.ToString();
    }

    public void SetVolume(float volume)
    {
        _audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        _graphicsDropdown.value = qualityIndex;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        _resolutionDropdown.value = resolutionIndex;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        _fullscreenToggle.isOn = isFullscreen;
    }
}