import { ExecutionTaskDto, TaskDto } from "./task.model";

export enum ProjectStatus {
  Designing = 'Designing',
  ContractorSearch = 'ContractorSearch',
  InExecution = 'InExecution',
  Completed = 'Completed'
}

export interface GetProjectDto {
  idProject: number;
  name: string;
  startDate: Date;       
  endDate: Date;         
  status: ProjectStatus;
  idUser: number;
  responsiblePerson: string;
  workObject: {
    name: string;
    geometry: {
      type: string;
      coordinates: number[];
    }
  };
  contractorName?: string;
  startExecutionDate?: Date;  
  endExecutionDate?: Date;  
  progress: number;
}

export interface PostProjectDto {
  idProject?: number;
  name: string;
  startDate: Date;       
  endDate: Date;         
  status: ProjectStatus;
  idUser: number;
  workObject: {
    name: string;
    geometry: {
      type: string;
      coordinates: number[];
    }
  };
  contractorName?: string;
  startExecutionDate?: Date;  
  endExecutionDate?: Date;    
}

export interface GetProjectWithTasksDto {
  idProject: number;
  name: string;
  startDate: Date;       
  endDate: Date;         
  status: ProjectStatus;
  idUser: number;
  responsiblePerson: string;
  workObject: {
    name: string;
    geometry: {
      type: string;
      coordinates: number[];
    }
  };
  contractorName?: string;
  startExecutionDate?: Date;  
  endExecutionDate?: Date;  
  progress: number;
  tasks: TaskDto[];
  executionTasks: ExecutionTaskDto[]
}