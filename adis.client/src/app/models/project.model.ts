export enum ProjectStatus {
  Designing = 'Designing',
  ContractorSearch = 'ContractorSearch',
  InExecution = 'InExecution',
  Completed = 'Completed'
}

export interface Geometry {
  type: string;
  coordinates: number[];
}

export interface LocationDto {
  idLocation: number;
  geometry: Geometry;
}

export interface GetProjectDto {
  idProject: number;
  name: string;
  startDate: string;
  endDate: string;
  status: ProjectStatus;
  nameWorkObject: string;
  progress: number;
  responsiblePerson: string;
  idUser: number;
  idLocation: number;
  location: LocationDto;
}