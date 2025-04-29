export interface Project {
    idProject: number;
    name: string;
    status: 'draft' | 'inProgress' | 'completed' | 'overdue';
    description: string;
    budget: number;
    startDate: string; 
    endDate: string;   
    createdAt: string;
    idUser?: number;
  }