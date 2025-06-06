import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { Observable, Subject } from 'rxjs';
import { GetProjectDto, ProjectStatus } from '../models/project.model';

declare const ymaps: any;

@Injectable({
  providedIn: 'root'
})
export class MapService {
  private map: any;
  private markers: Map<number, any> = new Map();
  private scriptLoaded = false;
  private markerClickSubject = new Subject<GetProjectDto>();
  private markersMap = new Map<number, any>();
  private drawingManager: any;
  private resolveInitPromise!: () => void;
  private initPromise = new Promise<void>(resolve => this.resolveInitPromise = resolve);
  private currentGeoObject: any;
  private editorEventHandlers: any[] = [];

  async initMapAsync(container: HTMLElement, center: [number, number], zoom: number) {
    await this.loadYmapsApi();
    return new Promise<void>((resolve) => {
      ymaps.ready(() => {
        this.map = new ymaps.Map(container, {
          center: center,
          zoom: zoom,
          controls: ['zoomControl']
        });
        resolve();
      });
    });
  }

  constructor() {
    this.loadYmapsApi();
  }

  onMarkerClick(): Observable<GetProjectDto> {
    return this.markerClickSubject.asObservable();
  }

  initMap(container: HTMLElement, center: [number, number], zoom: number) {
    // Инициализация Яндекс Карты
    this.map = new ymaps.Map(container, {
      center: center,
      zoom: zoom,
      controls: ['zoomControl']
    });
  }

  async drawGeometry(type: string): Promise<{ type: string; coordinates: any }> {
    await this.initPromise;
    this.clearMarkers();

    if (!this.map) {
      throw new Error('Карта не инициализирована');
    }

    return new Promise((resolve, reject) => {
      try {
        this.drawingManager = new ymaps.drawing.Manager(this.map, {
          drawingMode: type === 'Polygon' ? 'polygon' : type.toLowerCase(),
          drawingCursor: 'crosshair',
          editorOptions: {
            menuOptions: {
              items: ['editor.drawing.delete']
            }
          }
        });

        const finishDrawing = (event: any) => {
          const geoObject = event.get('geoObject');
          const geometry = geoObject.geometry;

          this.drawingManager.stopDrawing();
          resolve({
            type: geometry.getType(),
            coordinates: this.normalizeCoordinates(geometry)
          });
        };

        this.drawingManager.events.add(['drawend'], finishDrawing);
        this.drawingManager.events.add(['drawstop'], reject);

        // Активируем режим рисования
        this.drawingManager.startDrawing();

      } catch (error) {
        reject(error);
      }
    });
  }

  private normalizeCoordinates(geometry: any): any {
    const type = geometry.getType();
    const coords = geometry.getCoordinates();

    // Дополнительная валидация
    if (!coords || coords.length === 0) {
      throw new Error('Нет данных о координатах');
    }

    if (type === 'LineString' && coords.length < 2) {
      throw new Error('Линия должна содержать минимум 2 точки');
    }

    if (type === 'Polygon') {
      return coords[0].slice(0, -1); // Удаляем последнюю точку
    }

    return coords;
  }

  addGeometry(geometry: { type: string; coordinates: any }) {
    this.clearMarkers();
  
    if (!geometry?.coordinates || geometry.coordinates.length === 0) {
      console.error('Invalid geometry data:', geometry);
      return;
    }
  
    let ymapsGeometry: any;

    try {
      switch (geometry.type) {
        case 'Point':
          ymapsGeometry = new ymaps.Placemark(geometry.coordinates);
          break;
        case 'LineString':
          ymapsGeometry = new ymaps.Polyline(geometry.coordinates);
          break;
        case 'Polygon':
          ymapsGeometry = new ymaps.Polygon(geometry.coordinates);
          break;
        default:
          throw new Error('Unsupported geometry type');
      }
  
      ymapsGeometry.options.set({
        strokeColor: '#3f51b5',
        strokeWidth: 2,
        fillColor: '#3f51b550'
      });
  
      this.map.geoObjects.add(ymapsGeometry);
      
      const bounds = ymapsGeometry.geometry.getBounds();
      if (bounds) {
        this.map.setBounds(bounds, {
          checkZoomRange: true,
          zoomMargin: 30
        });
      }
    } catch (error) {
      console.error('Error adding geometry:', error);
    }
  }

  hasMarker(projectId: number): boolean {
    return this.markersMap.has(projectId);
  }

  addMarker(project: GetProjectDto): void {
    if (!project.workObject?.geometry?.coordinates) return;
    ymaps.ready().then(() => {
      const geometry = project.workObject.geometry;
      if (!geometry) return;

      let ymapsGeometry: any;

      ymapsGeometry = new ymaps.GeoObject({
        geometry: geometry
      });

      const marker = new ymaps.GeoObject(
        { geometry: ymapsGeometry.geometry },
        {
          preset: 'islands#geoObject',
          ...this.getStyleForType(geometry.type, project.status)
        }
      );

      marker.events.add('click', () => {
        this.markerClickSubject.next(project);
      });

      this.map.geoObjects.add(marker);
      this.markersMap.set(project.idProject, marker);
    });
  }

  private getStyleForType(type: string, status: ProjectStatus) {
    const baseStyles = {
      strokeColor: '#000',
      strokeWidth: 2,
      fillColor: this.getStatusColor(status),
      fillOpacity: 0.4
    };

    switch (type) {
      case 'Point':
        return {
          preset: 'islands#circleIcon',
          iconColor: this.getStatusColor(status)
        };
      case 'LineString':
        return {
          strokeColor: this.getStatusColor(status),
          strokeWidth: 4
        };
      case 'Polygon':
        return baseStyles;
      default:
        return {};
    }
  }

  setCenter(coords: [number, number], zoom: number): void {
    this.map.setCenter(coords, zoom);
  }

  private loadYmapsApi(): void {
    if (!this.scriptLoaded && !document.querySelector('#yandex-maps-script')) {
      const script = document.createElement('script');
      script.id = 'yandex-maps-script';
      script.src = `https://api-maps.yandex.ru/2.1/?apikey=${environment.yandexMapsApiKey}&lang=ru_RU&load=package.full`;
      script.async = true;
      script.defer = true;
      script.onload = () => {
        ymaps.ready(() => {
          this.scriptLoaded = true;
          console.log('Yandex Maps API loaded');
        });
      };
      document.head.appendChild(script);
    }
  }

  private getStatusColor(status: ProjectStatus): string {
    const colors = {
      [ProjectStatus.Designing]: '#3DAFC9',
      [ProjectStatus.ContractorSearch]: '#AD3DC9',
      [ProjectStatus.InExecution]: '#E3CE32',
      [ProjectStatus.Completed]: '#52CD2C'
    };
    return colors[status] || '#2D3038';
  }

  destroyMap() {
    if (this.map) {
      this.map.destroy();
      this.map = null;
      this.markers.clear();
    }
  }

  highlightMarker(projectId: number): void {
    this.markersMap.forEach((marker, id) => {
      if (id === projectId) {
        marker.options.set('preset', 'islands#redIcon');
      } else {
        marker.options.set('preset', 'islands#icon');
      }
    });
  }

  // Добавим метод для получения координат проекта
  getProjectCoords(projectId: number): [number, number] | null {
    const marker = this.markersMap.get(projectId);
    return marker?.geometry.getCoordinates() || null;
  }

  updateMapSize() {
    if (this.map) {
      this.map.container.fitToViewport();
    }
  }

  mapExists(): boolean {
    return !!this.map;
  }

  clearMarkers(): void {
    // Удаляем все объекты с карты
    this.map.geoObjects.removeAll();

    // Очищаем внутренние хранилища
    this.markersMap.clear();
    this.markers.clear();

    // Сбрасываем состояние
    if (this.map) {
      this.map.setCenter(this.map.getCenter(), this.map.getZoom());
    }
  }

  updateMap(): void {
    if (this.map) {
      this.map.container.fitToViewport();
      const bounds = this.map.geoObjects.getBounds();
      if (bounds) {
        this.map.setBounds(bounds, { checkZoomRange: true });
      }
    }
  }

  async startDrawing(geometryType: 'Point' | 'LineString' | 'Polygon'): Promise<any> {
    await this.loadYmapsApi();
    this.clearPreviousDrawing();
    
    return new Promise((resolve, reject) => {
      try {
        if (geometryType === 'Point') {
          const clickHandler = (e: any) => {
            const coords = e.get('coords');
            const point = new ymaps.Placemark(coords);
            this.map.geoObjects.add(point);
            this.safeRemoveEvent('click', clickHandler);
            resolve({ type: 'Point', coordinates: coords });
          };
          this.map.events.add('click', clickHandler);
          return;
        }
  
        this.initGeoObject(geometryType);
        const editor = this.currentGeoObject.editor;
  
        editor.options.set({
          drawingCursor: 'crosshair',
          dblclickFinish: true
        });
  
        this.addEventHandlers(editor, resolve, reject);
        this.map.geoObjects.add(this.currentGeoObject);
        editor.startDrawing();
      } catch (error) {
        this.clearPreviousDrawing();
        reject(error);
      }
    });
  }
  
  private initGeoObject(geometryType: string) {
    this.currentGeoObject = geometryType === 'Polygon'
      ? new ymaps.Polygon([], {
        strokeColor: '#FF0000',
        strokeWidth: 4,
        fillColor: '#FFFFFF',
        fillOpacity: 0.5
      })
      : new ymaps.Polyline([], {
        strokeColor: '#FF0000',
        strokeWidth: 4
      });
  }

  private addEventHandlers(editor: any, resolve: Function, reject: Function) {
    const finishHandler = () => {
      try {
        const geometry = this.currentGeoObject.geometry;
        const coords = geometry.getCoordinates();
        const type = geometry.getType();
  
        this.validateCoordinates(type, coords);
        
        // Сохраняем стили после завершения рисования
        this.currentGeoObject.options.set({
          strokeColor: '#FF0000',
          strokeWidth: 2,
          fillColor: '#FFFFFF',
          fillOpacity: 0.4
        });
  
        resolve({
          type: type,
          coordinates: coords
        });
      } catch (error) {
        reject(error);
      }
    };
  
    this.editorEventHandlers = [
      editor.events.add('drawingstop', finishHandler)
    ];
  }

  private validateCoordinates(type: string, coords: any) {
    if (type === 'LineString' && coords.length < 2) {
      throw new Error('Минимум 2 точки для линии');
    }
    if (type === 'Polygon' && coords[0].length < 3) {
      throw new Error('Минимум 3 точки для полигона');
    }
  }


  private safeRemoveEvent(eventType: string, handler: Function) {
    try {
      this.map.events.remove(eventType, handler);
    } catch (e) {
      console.warn(`Ошибка удаления события ${eventType}:`, e);
    }
  }

  private clearPreviousDrawing() {
    // Удаляем все геообъекты
    this.map.geoObjects.removeAll();
    
    // Очищаем текущий объект и обработчики
    this.currentGeoObject = null;
  }
}
