import { CommentDto } from "./comment.model";
import { DocumentDto } from "./document.model";
import { UserDto } from "./user.model";

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
    createdAt: Date;
    updatedAt?: Date;
    textResult?: string;
    performers: UserDto[];
    checkers: UserDto[];
    documents: DocumentDto[];
    comments: CommentDto[];
  }

  export interface PostTaskDto {
    name: string;
    description: string;
    idPerformers: number[];
    idCheckers: number[];
    idProject: number;
  }

  export interface PutTaskDto {
    idTask: number;
    name: string;
    description: string;
    idPerformers: number[];
    idCheckers: number[];
  }