import { CommentDto } from "./comment.model";
import { DocumentDto } from "./document.model";
import { UserDto } from "./user.model";
import { WorkObjectSectionDto } from "./work-object-section.model";

export enum TaskStatus {
    ToDo = 'ToDo',
    Doing = 'Doing',
    Checking = 'Checking',
    Completed = 'Completed'
  }

export interface TaskDto {
    idTask: number;
    name: string;
    description: string;
    status: TaskStatus;
    performers: UserDto[];
    checkers: UserDto[];
  }

  export interface TaskDetailsDto {
    idTask: number;
    name: string;
    description: string;
    status: TaskStatus;
    textResult?: string;
    performers: UserDto[];
    checkers: UserDto[];
    documents: DocumentDto[];
    comments: CommentDto[];
    plannedEndDate: Date;
    actualEndDate?: Date;
  }

  export interface PostTaskDto {
    name: string;
    description: string;
    idPerformers: number[];
    idCheckers: number[];
    idProject: number;
    plannedEndDate: string;
    status?: TaskStatus;
  }

  export interface PutTaskDto {
    idTask: number;
    name: string;
    description: string;
    idPerformers: number[];
    idCheckers: number[];
    plannedEndDate: string;
    status?: TaskStatus;
  }

  export interface ExecutionTaskDto {
    idExecutionTask: number;
    name: string;
    isCompleted: boolean; 
    documents: DocumentDto[];
    workObjectSection: WorkObjectSectionDto
  }