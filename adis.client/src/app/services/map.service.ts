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

  hasMarker(projectId: number): boolean {
    return this.markersMap.has(projectId);
  }

  addMarker(project: GetProjectDto): void {
    if (this.markersMap.has(project.idProject)) return;
    ymaps.ready().then(() => {
      const geometry = project.location?.geometry;
      if (!geometry) return;
  
      let ymapsGeometry: any;
      
      ymapsGeometry = new ymaps.GeoObject({
        geometry: geometry
      });
  
      const props = {
        balloonContent: this.getBalloonContent(project),
        hintContent: project.name
      };
  
      const marker = new ymaps.GeoObject(
        { geometry: ymapsGeometry.geometry },
        {
          preset: 'islands#geoObject',
          balloonContentLayout: ymaps.templateLayoutFactory.createClass(
            props.balloonContent
          ),
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
      script.src = `https://api-maps.yandex.ru/2.1/?apikey=${environment.yandexMapsApiKey}&lang=ru_RU`;
      script.async = true;
      script.defer = true;
      script.onload = () => {
        this.scriptLoaded = true;
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

  private getBalloonContent(project: GetProjectDto): string {
    return `
      <div class="map-balloon">
        <h3>${project.name}</h3>
        <p>Статус: ${this.getStatusLabel(project.status)}</p>
        <p>Объект: ${project.nameWorkObject}</p>
        <p>Прогресс: ${project.progress}%</p>
        ${this.getGeometryType(project.location?.geometry)}
      </div>
    `;
  }

  private getStatusLabel(status: string): string {
    const statusMap: {[key: string]: string} = {
      'Designing': 'Проектирование',
      'ContractorSearch': 'Поиск подрядчика',
      'InExecution': 'В работе',
      'Completed': 'Завершен'
    };
    return statusMap[status] || 'Неизвестный статус';
  }

  private getGeometryType(geometry?: any): string {
    if (!geometry) return '';
    const typeMap: {[key: string]: string} = {
      'Point': 'Точка',
      'LineString': 'Линия',
      'Polygon': 'Полигон'
    };
    return `<p>Тип объекта: ${typeMap[geometry.type] || geometry.type}</p>`;
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
}
