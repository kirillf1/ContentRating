// Service Worker для ContentRating PWA
const CACHE_NAME = 'contentrating-cache-v1';
const urlsToCache = [
  '/',
  '/index.html',
  '/css/app.css',
  '/js/theme.js',
  '/js/hls-player.js',
  '/_framework/blazor.webassembly.js',
  '/_content/MudBlazor/MudBlazor.min.css',
  '/_content/MudBlazor/MudBlazor.min.js'
];

// Установка service worker
self.addEventListener('install', function(event) {
  console.log('Service Worker: Installing...');
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(function(cache) {
        console.log('Service Worker: Caching app shell');
        return cache.addAll(urlsToCache);
      })
      .then(function() {
        console.log('Service Worker: Skip waiting on install');
        return self.skipWaiting();
      })
  );
});

// Активация service worker
self.addEventListener('activate', function(event) {
  console.log('Service Worker: Activating...');
  event.waitUntil(
    caches.keys().then(function(cacheNames) {
      return Promise.all(
        cacheNames.map(function(cacheName) {
          if (cacheName !== CACHE_NAME) {
            console.log('Service Worker: Deleting old cache:', cacheName);
            return caches.delete(cacheName);
          }
        })
      );
    }).then(function() {
      console.log('Service Worker: Claiming clients');
      return self.clients.claim();
    })
  );
});

// Обработка fetch запросов
self.addEventListener('fetch', function(event) {
  event.respondWith(
    caches.match(event.request)
      .then(function(response) {
        // Возвращаем кэшированную версию или запрашиваем из сети
        return response || fetch(event.request);
      }
    )
  );
});

// Обработка сообщений от клиента
self.addEventListener('message', function(event) {
  if (event.data && event.data.type === 'SKIP_WAITING') {
    console.log('Service Worker: Skip waiting message received');
    self.skipWaiting();
  }
}); 