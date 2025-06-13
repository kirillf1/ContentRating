// Volume settings storage key
const VOLUME_STORAGE_KEY = 'hlsPlayerVolume';
const MUTED_STORAGE_KEY = 'hlsPlayerMuted';

// Функции для работы с громкостью
const saveVolumeSettings = (volume, muted) => {
    try {
        localStorage.setItem(VOLUME_STORAGE_KEY, volume.toString());
        localStorage.setItem(MUTED_STORAGE_KEY, muted.toString());
    } catch (e) {
        console.warn('Could not save volume settings:', e);
    }
};

const loadVolumeSettings = () => {
    try {
        const savedVolume = localStorage.getItem(VOLUME_STORAGE_KEY);
        const savedMuted = localStorage.getItem(MUTED_STORAGE_KEY);
        
        return {
            volume: savedVolume ? parseFloat(savedVolume) : 1.0,
            muted: savedMuted ? savedMuted === 'true' : false
        };
    } catch (e) {
        console.warn('Could not load volume settings:', e);
        return { volume: 1.0, muted: false };
    }
};

// HLS Player initialization with volume persistence
window.initializeHlsPlayerWithVolume = (videoId, videoUrl) => {
    const video = document.getElementById(videoId);
    if (!video) {
        console.error('Video element not found:', videoId);
        return;
    }

    // Загружаем сохраненные настройки громкости
    const volumeSettings = loadVolumeSettings();
    video.volume = volumeSettings.volume;
    video.muted = volumeSettings.muted;

    // Добавляем обработчики событий для сохранения громкости
    video.addEventListener('volumechange', () => {
        saveVolumeSettings(video.volume, video.muted);
    });

    // Проверяем поддержку HLS.js
    if (window.Hls && Hls.isSupported()) {
        const hls = new Hls({
            debug: false,
            enableWorker: true,
            lowLatencyMode: true,
            backBufferLength: 90
        });

        hls.loadSource(videoUrl);
        hls.attachMedia(video);

        hls.on(Hls.Events.MANIFEST_PARSED, () => {
            console.log('HLS manifest loaded successfully');
            // Применяем сохраненные настройки громкости после загрузки
            video.volume = volumeSettings.volume;
            video.muted = volumeSettings.muted;
        });

        hls.on(Hls.Events.ERROR, (event, data) => {
            console.error('HLS error:', data);
            if (data.fatal) {
                switch (data.type) {
                    case Hls.ErrorTypes.NETWORK_ERROR:
                        console.log('Fatal network error encountered, try to recover');
                        hls.startLoad();
                        break;
                    case Hls.ErrorTypes.MEDIA_ERROR:
                        console.log('Fatal media error encountered, try to recover');
                        hls.recoverMediaError();
                        break;
                    default:
                        console.log('Fatal error, cannot recover');
                        hls.destroy();
                        break;
                }
            }
        });

        // Сохраняем ссылку на hls для возможной очистки
        video.hlsPlayer = hls;
    }
    // Fallback на нативную поддержку HLS (Safari и другие)
    else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        video.src = videoUrl;
        console.log('Using native HLS support');
        
        // Применяем настройки громкости для нативного воспроизведения
        video.addEventListener('loadedmetadata', () => {
            video.volume = volumeSettings.volume;
            video.muted = volumeSettings.muted;
        });
    }
    else {
        console.error('HLS is not supported in this browser');
    }
};

// Обратная совместимость со старой функцией
window.initializeHlsPlayer = (videoId, videoUrl) => {
    window.initializeHlsPlayerWithVolume(videoId, videoUrl);
};

// Функция для очистки HLS плеера
window.destroyHlsPlayer = (videoId) => {
    const video = document.getElementById(videoId);
    if (video && video.hlsPlayer) {
        video.hlsPlayer.destroy();
        video.hlsPlayer = null;
    }
};

// Audio Player initialization with volume persistence
window.initializeAudioPlayerWithVolume = (audioId, audioUrl) => {
    const audio = document.getElementById(audioId);
    if (!audio) {
        console.error('Audio element not found:', audioId);
        return;
    }

    // Загружаем сохраненные настройки громкости
    const volumeSettings = loadVolumeSettings();
    console.log('Loading audio with saved volume settings:', volumeSettings);

    // Функция для применения настроек громкости
    const applyVolumeSettings = () => {
        try {
            audio.volume = volumeSettings.volume;
            audio.muted = volumeSettings.muted;
            console.log('Applied volume settings - Volume:', audio.volume, 'Muted:', audio.muted);
        } catch (e) {
            console.warn('Could not apply volume settings:', e);
        }
    };

    // Добавляем обработчики событий для сохранения громкости (без рекурсии)
    let isApplyingSettings = false;
    audio.addEventListener('volumechange', () => {
        if (!isApplyingSettings) {
            saveVolumeSettings(audio.volume, audio.muted);
            console.log('Saved new volume settings - Volume:', audio.volume, 'Muted:', audio.muted);
        }
    });

    // Применяем настройки сразу после установки src
    audio.src = audioUrl;
    
    // Пытаемся применить настройки сразу
    isApplyingSettings = true;
    applyVolumeSettings();
    isApplyingSettings = false;

    // Применяем настройки громкости после различных событий загрузки
    audio.addEventListener('loadstart', () => {
        console.log('Audio loadstart event');
        isApplyingSettings = true;
        applyVolumeSettings();
        isApplyingSettings = false;
    });

    audio.addEventListener('loadedmetadata', () => {
        console.log('Audio loadedmetadata event');
        isApplyingSettings = true;
        applyVolumeSettings();
        isApplyingSettings = false;
    });

    audio.addEventListener('loadeddata', () => {
        console.log('Audio loadeddata event');
        isApplyingSettings = true;
        applyVolumeSettings();
        isApplyingSettings = false;
    });

    audio.addEventListener('canplay', () => {
        console.log('Audio canplay event');
        isApplyingSettings = true;
        applyVolumeSettings();
        isApplyingSettings = false;
    });

    // Обработка ошибок загрузки
    audio.addEventListener('error', (e) => {
        console.error('Audio loading error:', e);
    });

    // Предзагрузка метаданных
    audio.preload = 'metadata';

    // Дополнительная попытка применить настройки через небольшой таймаут
    setTimeout(() => {
        isApplyingSettings = true;
        applyVolumeSettings();
        isApplyingSettings = false;
    }, 100);
};

// Функция для очистки аудиоплеера
window.destroyAudioPlayer = (audioId) => {
    const audio = document.getElementById(audioId);
    if (audio) {
        // Останавливаем воспроизведение и очищаем источники
        audio.pause();
        audio.src = '';
        audio.load();
        console.log('Audio player destroyed:', audioId);
    }
}; 