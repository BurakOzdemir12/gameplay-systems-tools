using System;
using _Project.Systems._Core.EventBus;
using _Project.Systems.EnvironmentSystem.Time;
using _Project.Systems.EnvironmentSystem.Weather.Enums;
using _Project.Systems.EnvironmentSystem.Weather.Events;
using _Project.Systems.EnvironmentSystem.Weather.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Project.Systems.EnvironmentSystem
{
    public class VisualEnvManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private WeathersConfigSo weatherData;
        [SerializeField] private Camera mainCamera;

        [Header("Lights")] [SerializeField] private Light sun;
        [SerializeField] private Light moon;

        [Header("Light Settings")] [SerializeField]
        private AnimationCurve lightIntensityCurve;

        [SerializeField] private float maxSunIntensity;
        [SerializeField] private float maxMoonIntensity;

        [SerializeField] private Color dayAmbientLight;
        [SerializeField] private Color nightAmbientLight;
        [SerializeField] private Volume volume;
        [SerializeField] private ColorAdjustments colorAdjustments;

        [Space(10)] [SerializeField] private GameObject particlesContainer;

        [Header("Snow Weather Settings")] [SerializeField]
        private ParticleSystem snowParticles;

        private static readonly int SNOW_AMOUNT_ID = Shader.PropertyToID("_SnowAmount");
        [SerializeField] private float snowAccumulationSpeed = 0.1f;
        [SerializeField] private float snowMeltSpeed = 0.1f;
        private float currentSnowAmount = 0f;

        [Header("Rainy Weather Settings")] [SerializeField]
        private ParticleSystem rainParticles;
        //TODO create Wetness property id

        private EventBinding<WeatherChangedEvent> weatherChangedBinding;
        [Header("Current Weather")] private WeatherType currentWeatherType;

        [Space(2)] [Header("Skybox Settings")] [Tooltip("Material used for Skybox Blending")] [SerializeField]
        private Material skyboxMaterial;

        #region Shader Properties

        // Sun Settings
        private static readonly int SUN_SIZE_ID = Shader.PropertyToID("_SunSize");

        private static readonly int SUN_HAZE_ID = Shader.PropertyToID("_SunHaze");

        // Atmosphere & Colors
        private static readonly int ATMOSPHERE_THICKNESS_ID = Shader.PropertyToID("_AtmosphereThickness");
        private static readonly int ZENITH_COLOR_ID = Shader.PropertyToID("_ZenithColor");
        private static readonly int HORIZON_COLOR_ID = Shader.PropertyToID("_HorizonColor");
        private static readonly int GROUND_COLOR_ID = Shader.PropertyToID("_GroundColor");
        private static readonly int SKY_EXPOSURE_ID = Shader.PropertyToID("_SkyExposure");

        private static readonly int SKY_SATURATION_ID = Shader.PropertyToID("_SkySaturation");

        // Horizon Settings
        private static readonly int HORIZON_HEIGHT_ID = Shader.PropertyToID("_HorizonHeight");

        private static readonly int HORIZON_SHARPNESS_ID = Shader.PropertyToID("_HorizonSharpness");

        // Moon Settings
        private static readonly int ENABLE_MOON_ID = Shader.PropertyToID("_EnableMoon");
        private static readonly int MOON_TEX_ID = Shader.PropertyToID("_MoonTex");

        private static readonly int MOON_SIZE_ID = Shader.PropertyToID("_MoonSize");

        // Stars Settings
        private static readonly int ENABLE_STARS_ID = Shader.PropertyToID("_EnableStars");
        private static readonly int STAR_SEED_ID = Shader.PropertyToID("_StarSeed");
        private static readonly int STAR_DENSITY_ID = Shader.PropertyToID("_StarDensity");

        private static readonly int STAR_INTENSITY_ID = Shader.PropertyToID("_StarIntensity");

        // Cloud Settings
        private static readonly int CLOUD_TINT_ID = Shader.PropertyToID("_CloudTint");
        private static readonly int CLOUD_SEED_ID = Shader.PropertyToID("_CloudSeed");
        private static readonly int CLOUD_ROTATION_Y_ID = Shader.PropertyToID("_CloudRotationY");
        private static readonly int CLOUD_COVERAGE_ID = Shader.PropertyToID("_CloudCoverage");
        private static readonly int CLOUD_SOFTNESS_ID = Shader.PropertyToID("_CloudSoftness");
        private static readonly int CLOUD_SCALE_ID = Shader.PropertyToID("_CloudScale");
        private static readonly int CLOUD_BASE_HEIGHT_ID = Shader.PropertyToID("_CloudBaseHeight");
        private static readonly int CLOUD_WIND_DIRECTION_ID = Shader.PropertyToID("_CloudWindDirection");
        private static readonly int CLOUD_SPEED_ID = Shader.PropertyToID("_CloudSpeed");

        [Header("Day/Night Sky Colors")]
        [SerializeField] private Color daySkyColor;
        [SerializeField] private Color nightSkyColor;
        [SerializeField] private float dayAtmosphere = 0.5f;
        [SerializeField] private float nightAtmosphere = 1;
        
        #endregion

        //Time Service
        private TimeService timeService;

        private void Awake()
        {
            if (weatherData.rainParticlePrefab != null)
            {
                rainParticles = Instantiate(weatherData.rainParticlePrefab, particlesContainer.transform);
                rainParticles.transform.position = particlesContainer.transform.position;
                rainParticles.Stop();
            }

            if (weatherData.snowParticlePrefab != null)
            {
                snowParticles = Instantiate(weatherData.snowParticlePrefab, particlesContainer.transform);
                snowParticles.transform.position = particlesContainer.transform.position;
                snowParticles.Stop();
            }
        }

        private void OnEnable()
        {
            weatherChangedBinding = new EventBinding<WeatherChangedEvent>(HandleWeatherChangedEvent);
            EventBus<WeatherChangedEvent>.Subscribe(weatherChangedBinding);
        }


        private void OnDisable()
        {
            EventBus<WeatherChangedEvent>.Unsubscribe(weatherChangedBinding);
        }

        private void Start()
        {
            // _timeService = ServiceLocator.Get<TimeService>();
            if (TimeManager.Instance != null)
            {
                timeService = TimeManager.Instance.TimeService;
            }

            timeText.text = timeService.CurrentTime.ToString("hh:mm");

            volume.profile.TryGet(out colorAdjustments);

            mainCamera = Camera.main;
            currentWeatherType = WeatherType.Clear;
            currentSnowAmount = 0;
        }

        private void Update()
        {
            RotateSun();
            UpdateLightSetting();
            UpdateUI();
            HandleParticlePosition();
            HandleWeatherShaderValues();
            HandleSkyBoxBlend();
            // Debug.Log($"Global Snow Amount: {currentSnowAmount}");
        }


        private void HandleWeatherShaderValues()
        {
            float targetSpeed = (currentWeatherType == WeatherType.Snowy)
                ? snowAccumulationSpeed
                : snowMeltSpeed;
            float targetSnowAmount = (currentWeatherType == WeatherType.Snowy) ? 1f : 0f;

            currentSnowAmount = Mathf.MoveTowards(currentSnowAmount, targetSnowAmount,
                targetSpeed * UnityEngine.Time.deltaTime);
            Shader.SetGlobalFloat(SNOW_AMOUNT_ID, currentSnowAmount);
        }


        private void HandleWeatherChangedEvent(WeatherChangedEvent evt)
        {
            UpdateWeather(evt.CurrentWeatherType);
        }

        private void RotateSun()
        {
            float rotation = timeService.CalculateSunAngle();
            sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
            moon.transform.rotation = Quaternion.AngleAxis(rotation - 180, Vector3.right);
        }

        private void UpdateLightSetting()
        {
            float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
            sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
            moon.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(dotProduct));

            if (colorAdjustments == null) return;

            colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight,
                lightIntensityCurve.Evaluate(dotProduct));
        }

        private void UpdateUI()
        {
            timeText.text = timeService.CurrentTime.ToString("hh:mm");
        }

        private void HandleParticlePosition()
        {
            if (!mainCamera) return;
            Vector3 targetPos = mainCamera.transform.position;
            targetPos.y += 10;
            particlesContainer.transform.position = targetPos;
        }

        private void HandleSkyBoxBlend()
        {
            if (!skyboxMaterial) return;
            float targetAtmosphereThickness = Mathf.Lerp();
            skyboxMaterial.SetFloat(SUN_SIZE_ID, targetAtmosphereThickness);
        }

        private void UpdateWeather(WeatherType weatherType)
        {
            currentWeatherType = weatherType;
            switch (weatherType)
            {
                case WeatherType.Rainy:
                    if (rainParticles != null) rainParticles.Play();
                    if (snowParticles.isPlaying) snowParticles.Stop();
                    break;
                case WeatherType.Snowy:
                    if (snowParticles != null) snowParticles.Play();
                    if (rainParticles.isPlaying) rainParticles.Stop();
                    break;
                case WeatherType.Clear:
                    if (rainParticles != null && rainParticles.isPlaying) rainParticles.Stop();
                    if (snowParticles != null && snowParticles.isPlaying) snowParticles.Stop();
                    break;
            }
        }
    }
}