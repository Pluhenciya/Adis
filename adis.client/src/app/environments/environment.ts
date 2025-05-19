export const environment = {
    production: false,
    apiUrl: window.location.hostname === 'localhost' ? '/api' : '/docker',
    yandexMapsApiKey: (window as any)['APP_CONFIG']?.yandexMapsApiKey || ''
  };
