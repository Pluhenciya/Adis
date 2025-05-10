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
  }