export const environment = {
    production: false,
    apiUrl: window.location.hostname === 'localhost' ? '/api' : '/docker',
    yandexMapsApiKey: ''
  };
