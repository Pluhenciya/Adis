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
  plannedEndDate: Date;         
  actualEndDate?: Date;         
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
  plannedEndExecutionDate?: Date;  
  actualEndExecutionDate?: Date;  
  progress: number;
}

export interface PostProjectDto {
  idProject?: number;
  name: string;
  startDate: Date;       
  plannedEndDate: Date;       
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
  plannedEndExecutionDate?: Date;    
}

export interface GetProjectWithTasksDto {
  idProject: number;
  name: string;
  startDate: Date;       
  plannedEndDate: Date;         
  actualEndDate?: Date;      
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
  plannedEndExecutionDate?: Date;  
  actualEndExecutionDate?: Date;  
  progress: number;
  tasks: TaskDto[];
  executionTasks: ExecutionTaskDto[]
}